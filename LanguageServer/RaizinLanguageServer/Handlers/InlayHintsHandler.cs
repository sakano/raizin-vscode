using System;
using System.Collections.Generic;
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
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// コード上のヒントを提供します。
/// </summary>
public sealed class InlayHintsHandler(IScriptHolder scriptHolder, IDefinitionHolder definitionHolder, StatusRepository statusRepository) : InlayHintsHandlerBase
{
    protected override InlayHintRegistrationOptions CreateRegistrationOptions(InlayHintClientCapabilities capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            ResolveProvider = false,
            WorkDoneProgress = false,
        };

    public override Task<InlayHint> Handle(InlayHint request, CancellationToken cancellationToken) => throw new NotSupportedException();

    /// <summary>
    /// ヒント作成リクエストが来たときに呼び出されます。
    /// </summary>
    public override Task<InlayHintContainer?> Handle(InlayHintParams request, CancellationToken cancellationToken)
    {
        var hints = CreateInlayHints(request);
        if (hints is null) return Task.FromResult<InlayHintContainer?>(null);
        return Task.FromResult<InlayHintContainer?>(new InlayHintContainer(hints));
    }

    /// <summary>
    /// ヒントを作成します。
    /// </summary>
    private List<InlayHint>? CreateInlayHints(InlayHintParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない

        var hints = new List<InlayHint>();

        for (var lineNumber = request.Range.Start.Line; lineNumber <= request.Range.End.Line; lineNumber++)
        {
            if (!script.TryGetLineNode<CommandLineNode>(lineNumber, out var commandLine)) continue; // コマンド行でない
            if (commandLine.Parameters.Count == 0) continue; // パラメータがない
            if (commandLine.Definition is null) continue; // コマンド定義が見つからない

            var paramIndex = -1;
            foreach (var paramNode in commandLine.Parameters)
            {
                paramIndex++;

                if (paramNode.Range.Length == 0) continue; // パラメータ値が空
                if (!commandLine.Definition.TryGetParameterDefinition(paramIndex, out var paramDef)) continue; // パラメータ定義が見つからない

                // パラメータ値の左側に表示するパラメータ定義のヒントを追加
                if (!string.IsNullOrWhiteSpace(paramDef.InlayHint))
                {
                    hints.Add(CreateParameterNameHint(commandLine, paramNode.Range.Start, paramDef.InlayHint));
                }

                CreateParameterInlayHints(hints, script.RawText, commandLine, paramNode, paramDef);
            }
        }

        return hints;
    }

    /// <summary>
    /// パラメータ値の右側に表示するヒントを作成します。
    /// </summary>
    private void CreateParameterInlayHints(List<InlayHint> hints, RawText rawText, LineNode lineNode, ParameterNode paramNode, ParameterDefinition paramDef)
    {
        if (paramNode.Range.Length == 0) return; // パラメータ値が空

        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                CreateParameterInlayHints(hints, rawText, lineNode, indirectNode.IdParameter, paramDef);
                return;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                CreateParameterInlayHints(hints, rawText, lineNode, getMpidNode.RightParameter, paramDef);
                return;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                CreateParameterInlayHints(hints, rawText, lineNode, predicateValueNode.ValueParameter, paramDef);
                return;
            case PredicateParameterNode predicateNode:
                // 判定条件
                CreateParameterInlayHints(hints, rawText, lineNode, predicateNode.LeftParameter, paramDef);
                CreateParameterInlayHints(hints, rawText, lineNode, predicateNode.RightParameter, paramDef);
                return;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var param in multipleNode.Parameters)
                {
                    CreateParameterInlayHints(hints, rawText, lineNode, param, paramDef);
                }
                return;
            case CompareParameterNode compareNode:
                // 比較式
                CreateParameterInlayHints(hints, rawText, lineNode, compareNode.LeftParameter, paramDef);
                CreateParameterInlayHints(hints, rawText, lineNode, compareNode.RightParameter, paramDef);
                return;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                if (referStatusNode.ReferStatus?.TryGetChoiceValue(rawText.Slice(paramNode.Range), out var choiceDef) ?? false)
                {
                    if (!string.IsNullOrEmpty(choiceDef.ShortDescription))
                    {
                        // 参照先ステータスの選択肢に一致する場合はその選択肢のヒントを追加
                        hints.Add(CreateParameterValueHint(lineNode, paramNode.Range.End, choiceDef.ShortDescription));
                    }
                }
                else
                {
                    CreateParameterInlayHints(hints, rawText, lineNode, referStatusNode.ChildParameter, paramDef);
                }

                return;
            case VariableParameterNode:
                // 変数
                break;
        }

        // 一番下のノードの種類に従ってヒントを表示
        InlayHint? paramHint = null;
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
                paramHint = CreateHintForEnumParameter<PersonDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetPersonDefinition);
                break;
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
                paramHint = CreateHintForEnumParameter<PlanetDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetPlanetDefinition);
                break;
            case ParameterType.StarId:
                paramHint = CreateHintForEnumParameter<StarDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetStarDefinition);
                break;
            case ParameterType.ShipId:
                paramHint = CreateHintForEnumParameter<ShipDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetShipDefinition);
                break;
            case ParameterType.SituationId:
                paramHint = CreateHintForEnumParameter<SituationDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetSituationDefinition);
                break;
            case ParameterType.JobId:
                paramHint = CreateHintForEnumParameter<JobDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetJobDefinition);
                break;
            case ParameterType.Honor:
                paramHint = CreateHintForEnumParameter<HonorDefinition>(rawText, lineNode, paramDef, paramNode.Range, definitionHolder.TryGetHonorDefinition);
                break;
            case ParameterType.Choice:
                paramHint = CreateHintForEnumParameter<IDescriptiveValue>(rawText, lineNode, paramDef, paramNode.Range, null);
                break;
            case ParameterType.PersonStatusId: // 数値指定の場合のみヒントを追加
                var statusText = rawText.Slice(paramNode.Range);
                if (statusText.IsInteger() && statusRepository.TryGetPersonStatus(statusText, out var status))
                {
                    paramHint = CreateParameterValueHint(lineNode, paramNode.Range.End, status.Name);
                }

                break;
        }

        if (paramHint is null)
        {
            // 最後までヒントが見つかっていなければパラメータ定義の選択肢から探す
            if (paramDef.TryGetChoiceValue(rawText.Slice(paramNode.Range), out var choiceDef) && !string.IsNullOrEmpty(choiceDef.ShortDescription))
            {
                paramHint = CreateParameterValueHint(lineNode, paramNode.Range.End, choiceDef.ShortDescription);
            }
        }

        if (paramHint is not null)
        {
            hints.Add(paramHint);
        }
    }

    private delegate bool TryGetDelegate<in TValue, TDefinition>(TValue intId, [NotNullWhen(true)] out TDefinition? definition) where TValue : allows ref struct;

    /// <summary>
    /// 列挙型パラメータに対するヒントを作成します。
    /// </summary>
    private static InlayHint? CreateHintForEnumParameter<T>(RawText rawText, LineNode lineNode, ParameterDefinition paramDef, AbsoluteRange paramRange, TryGetDelegate<ReadOnlySpan<char>, T>? tryGet)
        where T : IDescriptiveValue
    {
        var text = rawText.Slice(paramRange);
        if (text is "0" || tryGet is null || !tryGet(text, out var def)) return null;
        var label = def.ShortDescription;
        if (string.IsNullOrWhiteSpace(label)) return null;
        return CreateParameterValueHint(lineNode, paramRange.End, label);
    }

    /// <summary>
    /// ステータス名のヒントを作成します。
    /// </summary>
    private static InlayHint CreateParameterNameHint(LineNode lineNode, int absolutePosition, string label)
    {
        return new InlayHint
        {
            Position = new Position(lineNode.LineNumber, absolutePosition - lineNode.LineRange.Start),
            Label = new(label),
            Kind = InlayHintKind.Parameter,
            PaddingRight = true,
        };
    }

    /// <summary>
    /// ステータス値に対するヒントを作成します。
    /// </summary>
    private static InlayHint CreateParameterValueHint(LineNode lineNode, int absolutePosition, string label)
    {
        return new InlayHint
        {
            Position = new Position(lineNode.LineNumber, absolutePosition - lineNode.LineRange.Start),
            Label = new(label),
            Kind = InlayHintKind.Parameter,
            PaddingLeft = true,
            PaddingRight = true,
        };
    }
}