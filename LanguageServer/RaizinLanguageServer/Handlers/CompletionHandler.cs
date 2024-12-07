using System;
using System.Collections.Generic;
using System.Linq;
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
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// コマンド名やパラメータの補完を提供します。
/// </summary>
public sealed class CompletionHandler : CompletionHandlerBase
{
    protected override CompletionRegistrationOptions CreateRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            TriggerCharacters = new[] { ",", ":", "<", ">", "=", "!", "|", " ", "\t" },
            AllCommitCharacters = null,
            ResolveProvider = false,
            WorkDoneProgress = false,
            CompletionItem = new()
            {
                LabelDetailsSupport = false,
            }
        };

    public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken) => throw new NotSupportedException(); // resolveProvider 未使用

    private readonly IScriptHolder _scriptHolder;
    private readonly IDefinitionHolder _definitionHolder;
    private readonly CommandRepository _commandRepository;
    private readonly StatusRepository _statusRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CompletionHandler(IScriptHolder scriptHolder, IDefinitionHolder definitionHolder, IRaizinConfiguration configuration, CommandRepository commandRepository, StatusRepository statusRepository)
    {
        _scriptHolder = scriptHolder;
        _definitionHolder = definitionHolder;
        _commandRepository = commandRepository;
        _statusRepository = statusRepository;

        definitionHolder.OnDefinitionChanged += ClearCache;
    }

    /// <summary>
    /// 補完候補リストをリクエストされた際に呼び出されます。
    /// </summary>
    public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
    {
        var completionList = CreateCompletionList(request);
        return Task.FromResult(completionList ?? EmptyCompletionList);
    }

    /// <summary>
    /// 補完候補リストを生成します。
    /// </summary>
    private CompletionList? CreateCompletionList(CompletionParams request)
    {
        if (!_scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない
        if (!script.TryGetLineNode<CommandLineNode>(request.Position, out var commandLine)) return null; // コマンド行でない

        if (commandLine.InCommandNameRange(request.Position))
        {
            // コマンド名を補完
            return CreateCommandNameCompletionList(commandLine);
        }


        // パラメータ値を補完
        return CreateParameterCompletionList(commandLine, request, script);
    }

    /// <summary>
    /// コマンド名の補完用に CompletionList を取得します。
    /// </summary>
    private CompletionList CreateCommandNameCompletionList(CommandLineNode commandLine)
    {
        if (GetCompletionItemCache("commandName", out var cache))
        {
            foreach (var def in _commandRepository.CommandList)
            {
                if (def.IsDeprecated) continue;
                if (def.CommandNameRegex is not null) continue;
                cache.Add(new CompletionItem
                {
                    Label = def.CommandName,
                    Kind = CompletionItemKind.Function,
                    Documentation = new MarkupContent { Kind = MarkupKind.Markdown, Value = $"__{def.GetSignature()}__\n\n{def.Description}" }
                });
            }
        }

        // コマンド名全体が置換されるように補完
        return CreateCompletionList(cache, commandLine, commandLine.CommandNameRange);
    }

    /// <summary>
    /// パラメータ値の補完用に CompletionList を作成します。
    /// </summary>
    private CompletionList? CreateParameterCompletionList(CommandLineNode commandLine, CompletionParams request, RaizinScript script)
    {
        if (commandLine.Definition is null) return null; // コマンド定義が見つからない
        if (!commandLine.TryGetParameterIndex(request.Position, out var paramIndex)) return null; // パラメータ番号が取得できない
        if (!commandLine.Definition.TryGetParameterDefinition(paramIndex, out var paramDef)) return null; // パラメータ定義が見つからない
        if (!commandLine.TryGetParameterNode(paramIndex, out var paramNode)) return null; // パラメータノードが見つからない


        var compItems = new List<CompletionItem>();
        var absolutePosition = request.Position.Character + commandLine.LineRange.Start;
        return CreateParameterCompletionList(compItems, script, commandLine, paramNode, paramDef, absolutePosition, true, true, true);
    }

    /// <summary>
    /// パラメータ値の補完候補を生成します。
    /// </summary>
    private CompletionList? CreateParameterCompletionList(List<CompletionItem> compItems, RaizinScript script, LineNode lineNode, ParameterNode paramNode, ParameterDefinition paramDef, int absolutePosition, bool variableEnable, bool indirectEnable, bool choiceEnable)
    {
        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                if (indirectNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, indirectNode.StatusParameter, paramDef, absolutePosition, variableEnable, indirectEnable, false);
                if (indirectNode.IdParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, indirectNode.IdParameter, paramDef, absolutePosition, variableEnable, false, false);
                return null;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                if (getMpidNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, getMpidNode.StatusParameter, paramDef, absolutePosition, false, false, false);
                if (getMpidNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, getMpidNode.RightParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                return null;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                if (predicateValueNode.ValueParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, predicateValueNode.ValueParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                return null;
            case PredicateParameterNode predicateNode:
                // 判定条件
                if (predicateNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, predicateNode.LeftParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                if (predicateNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, predicateNode.RightParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                return null;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var param in multipleNode.Parameters)
                {
                    if (param.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, param, paramDef, absolutePosition, false, false, choiceEnable);
                }

                return null;
            case CompareParameterNode compareNode:
                // 比較式
                if (compareNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, compareNode.LeftParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                if (compareNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterCompletionList(compItems, script, lineNode, compareNode.RightParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                return null;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                if (referStatusNode.ChildParameter.Range.InRange(absolutePosition))
                {
                    if (choiceEnable && referStatusNode.ReferStatus is not null)
                    {
                        // 参照先ステータスの選択肢を補完対象に追加
                        compItems.AddRange(CreateCompletionItems("a_", CompletionItemKind.EnumMember, referStatusNode.ReferStatus.Choices));
                    }

                    return CreateParameterCompletionList(compItems, script, lineNode, referStatusNode.ChildParameter, paramDef, absolutePosition, variableEnable, indirectEnable, choiceEnable);
                }

                return null;
        }

        // 一番下のノードの種類に従って補完候補を生成
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PersonId), ParameterType.PersonId, _definitionHolder.PersonDefinitionList.Where(static x => x.Name is not ("予備枠" or "未設定" or "指定なし")), variableEnable, indirectEnable));
                break;
            case ParameterType.PlanetId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PlanetId), ParameterType.PlanetId, ParameterType.PrimaryPlanetId, _definitionHolder.PlanetDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.PrimaryPlanetId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PrimaryPlanetId), ParameterType.PrimaryPlanetId, _definitionHolder.PlanetDefinitionList.Where(static x => x.IsPrimary), variableEnable, indirectEnable));
                break;
            case ParameterType.StarId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.StarId), ParameterType.StarId, _definitionHolder.StarDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.ShipId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.ShipId), ParameterType.ShipId, _definitionHolder.ShipDefinitionsList, variableEnable, indirectEnable));
                break;
            case ParameterType.AnyStatusId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.AnyStatusId), ParameterType.AnyStatusId, _statusRepository.EnumerateAllStatus(), variableEnable, indirectEnable));
                break;
            case ParameterType.PersonStatusId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PersonStatusId), ParameterType.PersonStatusId, _statusRepository.PersonStatusList, variableEnable, indirectEnable));
                break;
            case ParameterType.PowerStatusId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PowerStatusId), ParameterType.PowerStatusId, _statusRepository.PowerStatusList, variableEnable, indirectEnable));
                break;
            case ParameterType.PlanetStatusId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.PlanetStatusId), ParameterType.PlanetStatusId, _statusRepository.PlanetStatusList, variableEnable, indirectEnable));
                break;
            case ParameterType.SituationId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.SituationId), ParameterType.SituationId, _definitionHolder.SituationDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.JobId:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.JobId), ParameterType.JobId, _definitionHolder.JobDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.ItemName:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.ItemName), ParameterType.ItemName, _definitionHolder.ItemDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.SkillName:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.SkillName), ParameterType.SkillName, _definitionHolder.SkillDefinitionsList, variableEnable, indirectEnable));
                break;
            case ParameterType.Honor:
                compItems.AddRange(CreateEnumCompletionItems(nameof(ParameterType.Honor), ParameterType.Honor, _definitionHolder.HonorDefinitionList, variableEnable, indirectEnable));
                break;
            case ParameterType.Age:
                compItems.AddRange(CreateEnumCompletionItems(ParameterType.Age, variableEnable, indirectEnable));
                break;
            case ParameterType.PregnancyMonth:
                compItems.AddRange(CreateEnumCompletionItems(ParameterType.PregnancyMonth, variableEnable, indirectEnable));
                break;
            case ParameterType.Max:
                compItems.AddRange(CreateEnumCompletionItems(ParameterType.Max, ParameterType.Int, variableEnable, indirectEnable));
                break;
            case ParameterType.Int:
                compItems.AddRange(CreateEnumCompletionItems(ParameterType.Int, variableEnable, indirectEnable));
                break;
            case ParameterType.Label:
                compItems.AddRange(CreateCompletionItems("l_", CompletionItemKind.Event, script.EnumerateLabelLineNodes()));
                break;
            case ParameterType.Unknown:
                compItems.AddRange(CreateEnumCompletionItems(ParameterType.Unknown, variableEnable, indirectEnable));
                break;
        }

        // 選択肢があれば全て補完対象に追加
        if (choiceEnable)
        {
            for (var index = 0; index < paramDef.Choices.Count; index++)
            {
                compItems.Add(CreateCompletionItem(CompletionItemKind.EnumMember, "c_", index, paramDef.Choices[index]));
            }
        }

        return CreateCompletionList(compItems, lineNode, paramNode.Range);
    }

    /// <summary>
    /// 指定された型の補完候補を生成します。
    /// </summary>
    private List<CompletionItem> CreateEnumCompletionItems(ParameterType expectedType, bool variableEnable, bool indirectEnable)
    {
        var items = new List<CompletionItem>();
        // 変数を補完対象に追加
        if (variableEnable)
        {
            items.AddRange(CreateVariableCompletionItems("v_", expectedType));
        }

        if (indirectEnable)
        {
            // 間接参照用のステータス名を補完対象に追加
            items.AddRange(CreateStatusCompletionItems("w_", expectedType));
        }

        return items;
    }

    /// <inheritdoc cref="CreateEnumCompletionItems(ParameterType, bool, bool)"/>
    private List<CompletionItem> CreateEnumCompletionItems(ParameterType expectedType, ParameterType expectedType2, bool variableEnable, bool indirectEnable)
    {
        var items = new List<CompletionItem>();
        // 変数を補完対象に追加
        if (variableEnable)
        {
            items.AddRange(CreateVariableCompletionItems("v_", expectedType));
            items.AddRange(CreateVariableCompletionItems("v2_", expectedType2));
        }

        if (indirectEnable)
        {
            // 間接参照用のステータス名を補完対象に追加
            items.AddRange(CreateStatusCompletionItems("w_", expectedType));
            items.AddRange(CreateStatusCompletionItems("w2_", expectedType2));
        }

        return items;
    }

    /// <summary>
    /// 指定された型の補完候補を生成し + list から補完候補を生成します。
    /// </summary>
    private List<CompletionItem> CreateEnumCompletionItems<T>(string key, ParameterType expectedType, IEnumerable<T>? list, bool variableEnable, bool indirectEnable) where T : IDescriptiveValue
    {
        var items = CreateEnumCompletionItems(expectedType, variableEnable, indirectEnable);

        // 固有の補完候補を生成
        if (list is not null)
        {
            items.AddRange(CreateCompletionItems(key,　"z_", CompletionItemKind.EnumMember, list));
        }

        return items;
    }

    /// <inheritdoc cref="CreateEnumCompletionItems{T}(string, ParameterType, IEnumerable{T}?, bool, bool)"/>
    private List<CompletionItem> CreateEnumCompletionItems<T>(string key, ParameterType expectedType, ParameterType expectedType2, IEnumerable<T>? list, bool variableEnable, bool indirectEnable) where T : IDescriptiveValue
    {
        var items = CreateEnumCompletionItems(expectedType, expectedType2, variableEnable, indirectEnable);
        // 固有の補完候補を生成
        if (list is not null)
        {
            items.AddRange(CreateCompletionItems(key,　"z_", CompletionItemKind.EnumMember, list));
        }

        return items;
    }

    /// <summary>
    /// 変数名の補完候補を生成します。
    /// </summary>
    private List<CompletionItem> CreateVariableCompletionItems(string sort, ParameterType expectedType)
    {
        if (GetCompletionItemCache($"var_{expectedType}_{sort}", out var cache))
        {
            for (var index = 0; index < _definitionHolder.VariableDefinitionList.Count; index++)
            {
                var varDef = _definitionHolder.VariableDefinitionList[index];
                if (varDef.IsNameRegex) continue;
                if (expectedType is ParameterType.Unknown || varDef.Type is ParameterType.Unknown || expectedType == varDef.Type)
                {
                    cache.Add(CreateCompletionItem(CompletionItemKind.Variable, sort, index, varDef));
                }
            }
        }

        return cache;
    }

    /// <summary>
    /// 間接参照用のステータス名の補完候補を生成します。
    /// </summary>
    private List<CompletionItem> CreateStatusCompletionItems(string sort, ParameterType expectedType)
    {
        if (GetCompletionItemCache($"stat_{expectedType}_{sort}", out var cache))
        {
            int index = 0;
            foreach (var status in _statusRepository.EnumerateAllStatus())
            {
                if (status.Name == "NON") continue;
                if (expectedType is ParameterType.Unknown || expectedType == status.Type)
                {
                    cache.Add(CreateCompletionItem(CompletionItemKind.Reference, sort, index, status));
                    index++;
                }
            }
        }

        return cache;
    }

    /// <summary>
    /// リストから補完候補を生成します。
    /// </summary>
    private List<CompletionItem> CreateCompletionItems<T>(string key, string sort, CompletionItemKind kind, IEnumerable<T> list) where T : IDescriptiveValue
    {
        if (GetCompletionItemCache($"{key}_{sort}", out var cache))
        {
            var index = 0;
            foreach (var item in list)
            {
                cache.Add(CreateCompletionItem(kind, sort, index, item));
                index++;
            }
        }

        return cache;
    }

    /// <summary>
    /// リストから補完候補を生成します。
    /// </summary>
    private static IEnumerable<CompletionItem> CreateCompletionItems<T>(string sort, CompletionItemKind kind, IEnumerable<T> list) where T : IDescriptiveValue
    {
        var index = 0;
        foreach (var item in list)
        {
            yield return CreateCompletionItem(kind, sort, index, item);
            index++;
        }
    }

    /// <summary>
    /// 補完候補を生成します。
    /// </summary>
    private static CompletionItem CreateCompletionItem<T>(CompletionItemKind kind, string sort, int index, T item) where T : IDescriptiveValue
    {
        var documentation = string.IsNullOrEmpty(item.Description) ? null : new MarkupContent { Kind = MarkupKind.Markdown, Value = item.Description };
        var labelDetails = item.SearchValue == item.ShortDescription ? null : new CompletionItemLabelDetails { Description = item.ShortDescription };

        return new CompletionItem
        {
            Label = item.SearchValue,
            Kind = kind,
            TextEditText = item.ScriptValue,
            SortText = sort + index.ToString("0000"),
            Documentation = documentation,
            LabelDetails = labelDetails,
        };
    }

    /// <summary>
    /// 指定された範囲が置換されるような補完設定を生成します。
    /// </summary>
    private static CompletionList CreateCompletionList(IEnumerable<CompletionItem> items, LineNode lineNode, AbsoluteRange range)
    {
        return new(items)
        {
            ItemDefaults = new()
            {
                EditRange = new(new Range
                {
                    Start = new Position(lineNode.LineNumber, range.Start - lineNode.LineRange.Start),
                    End = new Position(lineNode.LineNumber, range.End - lineNode.LineRange.Start),
                }),
            }
        };
    }

    /// <summary>
    /// 空の補完リスト
    /// </summary>
    private static CompletionList EmptyCompletionList => new();

    /// <summary>
    /// 補完候補のキャッシュ
    /// </summary>
    private readonly Dictionary<string, List<CompletionItem>> _completionItemCache = new();

    /// <summary>
    /// 設定が変更された際にキャッシュをクリアします。
    /// </summary>
    private void ClearCache(IDefinitionHolder _)
    {
        _completionItemCache.Clear();
    }

    /// <summary>
    /// 指定されたキーに対応する補完候補のキャッシュを取得します。
    /// </summary>
    /// <returns>キャッシュが新規作成されたか</returns>
    private bool GetCompletionItemCache(string key, out List<CompletionItem> cache)
    {
        if (_completionItemCache.TryGetValue(key, out cache!)) return false;

        cache = new();
        _completionItemCache.Add(key, cache);
        return true;
    }
}