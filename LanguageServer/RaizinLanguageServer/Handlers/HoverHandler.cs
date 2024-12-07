using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using RaizinLanguageServer.Models.Statuses;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// コマンド名やパラメータにホバーした際のドキュメントを提供します。
/// </summary>
public sealed class HoverHandler(IScriptHolder scriptHolder, IDefinitionHolder definitionHolder, StatusRepository statusRepository) : HoverHandlerBase
{
    protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            WorkDoneProgress = false,
        };

    /// <summary>
    /// ホバーのリクエストを受け取った際に呼び出されます。
    /// </summary>
    public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
    {
        var hover = CreateHover(request);
        return Task.FromResult(hover);
    }

    /// <summary>
    /// ホバーを作成します。
    /// </summary>
    private Hover? CreateHover(HoverParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない
        if (!script.TryGetLineNode<CommandLineNode>(request.Position, out var commandLine)) return null; // コマンド行でない
        if (commandLine.Definition is null) return null; // コマンド定義が見つからない

        if (commandLine.InCommandNameRange(request.Position))
        {
            // コマンド名にホバーしたらコマンドの説明を表示
            var description = $"__{commandLine.Definition.GetSignature()}__\n\n{commandLine.Definition.Description}";
            return CreateHover(commandLine, commandLine.CommandNameRange, new MarkupContent { Kind = MarkupKind.Markdown, Value = description });
        }

        // パラメータにホバーしたらパラメータの説明を表示
        if (!commandLine.TryGetParameterIndex(request.Position, out var paramIndex)) return null; // パラメータ番号が見つからない
        if (!commandLine.TryGetParameterNode(paramIndex, out var paramNode)) return null; // パラメータノードが見つからない
        if (!commandLine.Definition.TryGetParameterDefinition(paramIndex, out var paramDef)) return null; // パラメータ定義が見つからない

        var absolutePosition = commandLine.LineRange.Start + request.Position.Character;
        return CreateParameterHover(script.RawText, commandLine, paramNode, paramDef, absolutePosition);
    }

    private Hover? CreateParameterHover(RawText rawText, LineNode lineNode, ParameterNode paramNode, ParameterDefinition paramDef, int absolutePosition)
    {
        if (paramNode.Range.Length == 0) return null; // パラメータ値が空

        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                if (indirectNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, indirectNode.StatusParameter, paramDef, absolutePosition);
                if (indirectNode.IdParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, indirectNode.IdParameter, paramDef, absolutePosition);
                return null;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                if (getMpidNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, getMpidNode.StatusParameter, paramDef, absolutePosition);
                if (getMpidNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, getMpidNode.RightParameter, paramDef, absolutePosition);
                return null;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                if (predicateValueNode.ValueParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, predicateValueNode.ValueParameter, paramDef, absolutePosition);
                return null;
            case PredicateParameterNode predicateNode:
                // 判定条件
                if (predicateNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, predicateNode.LeftParameter, paramDef, absolutePosition);
                if (predicateNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, predicateNode.RightParameter, paramDef, absolutePosition);
                return null;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var childNode in multipleNode.Parameters)
                {
                    if (childNode.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, childNode, paramDef, absolutePosition);
                }

                return null;
            case CompareParameterNode compareNode:
                // 比較式
                if (compareNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, compareNode.LeftParameter, paramDef, absolutePosition);
                if (compareNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, compareNode.RightParameter, paramDef, absolutePosition);
                return null;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                if (referStatusNode.ReferStatus is not null && referStatusNode.ReferStatus.TryGetChoiceValue(rawText.Slice(referStatusNode.Range), out var def))
                {
                    // 参照先ステータスの選択肢のホバー
                    return CreateHover(lineNode, referStatusNode.Range, new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = def.ShortDescription,
                    });
                }

                if (referStatusNode.ChildParameter.Range.InRange(absolutePosition)) return CreateParameterHover(rawText, lineNode, referStatusNode.ChildParameter, paramDef, absolutePosition);
                return null;
            case VariableParameterNode variableNode:
                // 変数
                return CreateHover(lineNode, variableNode.Range, new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = variableNode.Variable.Description,
                });
        }

        // 一番下のノードの種類に従ってホバーを表示
        var paramText = rawText.Slice(paramNode.Range);
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
                return CreateHoverForEnumParameter<PersonDefinition>(lineNode, paramText, paramNode.Range, PersonDefinition.DisplayName, definitionHolder.TryGetPersonDefinition);
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
                return CreateHoverForEnumParameter<PlanetDefinition>(lineNode, paramText, paramNode.Range, PlanetDefinition.DisplayName, definitionHolder.TryGetPlanetDefinition);
            case ParameterType.StarId:
                return CreateHoverForEnumParameter<StarDefinition>(lineNode, paramText, paramNode.Range, StarDefinition.DisplayName, definitionHolder.TryGetStarDefinition);
            case ParameterType.ShipId:
                return CreateHoverForEnumParameter<ShipDefinition>(lineNode, paramText, paramNode.Range, ShipDefinition.DisplayName, definitionHolder.TryGetShipDefinition);
            case ParameterType.PersonStatusId:
                return CreateHoverForEnumParameter<PersonStatus>(lineNode, paramText, paramNode.Range, PersonStatus.DisplayName, statusRepository.TryGetPersonStatus);
            case ParameterType.PlanetStatusId:
                return CreateHoverForEnumParameter<PlanetStatus>(lineNode, paramText, paramNode.Range, PlanetStatus.DisplayName, statusRepository.TryGetPlanetStatus);
            case ParameterType.PowerStatusId:
                return CreateHoverForEnumParameter<PowerStatus>(lineNode, paramText, paramNode.Range, PowerStatus.DisplayName, statusRepository.TryGetPowerStatus);
            case ParameterType.AnyStatusId:
                if (statusRepository.TryGetAnyStatus(paramText, out var anyStatus))
                {
                    switch (anyStatus)
                    {
                        case PersonStatus:
                            return CreateHover(lineNode, paramNode.Range, PersonStatus.DisplayName, anyStatus);
                        case PlanetStatus:
                            return CreateHover(lineNode, paramNode.Range, PlanetStatus.DisplayName, anyStatus);
                        case PowerStatus:
                            return CreateHover(lineNode, paramNode.Range, PowerStatus.DisplayName, anyStatus);
                    }
                }
            
                return null;
            case ParameterType.SituationId:
                return CreateHoverForEnumParameter<SituationDefinition>(lineNode, paramText, paramNode.Range, SituationDefinition.DisplayName, definitionHolder.TryGetSituationDefinition);
            case ParameterType.JobId:
                return CreateHoverForEnumParameter<JobDefinition>(lineNode, paramText, paramNode.Range, JobDefinition.DisplayName, definitionHolder.TryGetJobDefinition);
            case ParameterType.ItemName:
                return CreateHoverForEnumParameter<ItemDefinition>(lineNode, paramText, paramNode.Range, ItemDefinition.DisplayName, definitionHolder.TryGetItemDefinition);
            case ParameterType.SkillName:
                return CreateHoverForEnumParameter<SkillDefinition>(lineNode, paramText, paramNode.Range, SkillDefinition.DisplayName, definitionHolder.TryGetSkillDefinition);
            case ParameterType.Honor:
                return CreateHoverForEnumParameter<HonorDefinition>(lineNode, paramText, paramNode.Range, HonorDefinition.DisplayName, definitionHolder.TryGetHonorDefinition);
        }

        // 最後まで見つからなければ選択肢のホバーを探す
        if (paramDef.TryGetChoiceValue(paramText, out var choiceDef))
        {
            return CreateHover(lineNode, paramNode.Range, new MarkupContent
            {
                Kind = MarkupKind.Markdown,
                Value = choiceDef.ShortDescription,
            });
        }

        return null;
    }

    private delegate bool TryGetDelegate<in TValue, TDefinition>(TValue value, [NotNullWhen(true)] out TDefinition? definition) where TValue : allows ref struct;

    /// <summary>
    /// 文字列指定のみの列挙型パラメータに対するホバーを作成します。
    /// </summary>
    private static Hover? CreateHoverForEnumParameter<T>(LineNode lineNode, ReadOnlySpan<char> text, AbsoluteRange range, string label, TryGetDelegate<ReadOnlySpan<char>, T> tryGet)
        where T : IDescriptiveValue
    {
        if (!tryGet.Invoke(text, out var value)) return null;
        return CreateHover(lineNode, range, label, value);
    }

    /// <summary>
    /// パラメータの値に対するホバーを作成します。
    /// </summary>
    private static Hover CreateHover(LineNode lineNode, AbsoluteRange range, string label, IDescriptiveValue paramValue)
    {
        var description = paramValue.Description;
        if (string.IsNullOrEmpty(description))
        {
            if (paramValue.ShortDescription != paramValue.ScriptValue)
            {
                description = paramValue.ShortDescription;
            }
        }

        var value = string.IsNullOrEmpty(description)
            ? $"{label}: {paramValue.ScriptValue}"
            : $"{label}: {paramValue.ScriptValue}\n\n{description}";

        return CreateHover(lineNode, range, new MarkupContent
        {
            Kind = MarkupKind.Markdown,
            Value = value,
        });
    }

    /// <summary>
    /// 指定された範囲に対するホバーを作成します。
    /// </summary>
    private static Hover CreateHover(LineNode lineNode, AbsoluteRange range, MarkupContent content)
    {
        return new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(content),
            Range = new()
            {
                Start = new(lineNode.LineNumber, range.Start - lineNode.LineRange.Start),
                End = new(lineNode.LineNumber, range.End - lineNode.LineRange.Start),
            },
        };
    }
}