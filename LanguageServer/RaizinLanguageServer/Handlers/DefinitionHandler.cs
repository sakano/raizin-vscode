using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// 定義位置へのジャンプを提供します。
/// </summary>
public sealed class DefinitionHandler(IScriptHolder scriptHolder, IDefinitionHolder definitionHolder, IRaizinConfiguration configuration) : DefinitionHandlerBase
{
    protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            WorkDoneProgress = false,
        };

    /// <summary>
    /// 定義位置をリクエストされた際に呼び出されます。
    /// </summary>
    public override Task<LocationOrLocationLinks?> Handle(DefinitionParams request, CancellationToken cancellationToken)
    {
        var location = CreateLocationLink(request);
        return Task.FromResult(location is null ? null : new LocationOrLocationLinks(location));
    }

    /// <summary>
    /// 定義位置を作成します。
    /// </summary>
    private LocationLink? CreateLocationLink(DefinitionParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない
        if (!script.TryGetLineNode<CommandLineNode>(request.Position, out var commandLine)) return null; // コマンド行でない
        if (!commandLine.TryGetParameterIndex(request.Position, out var paramIndex)) return null; // パラメータ番号が見つからない
        if (!commandLine.TryGetParameterNode(paramIndex, out var paramNode)) return null; // パラメータノードが見つからない

        var absolutePosition = commandLine.LineRange.Start + request.Position.Character;
        return CreateParameterLocationLink(script, commandLine, paramNode, absolutePosition);
    }

    private LocationLink? CreateParameterLocationLink(RaizinScript script, LineNode lineNode, ParameterNode paramNode, int absolutePosition)
    {
        if (paramNode.Range.Length == 0) return null; // パラメータ値が空

        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                if (indirectNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, indirectNode.StatusParameter, absolutePosition);
                if (indirectNode.IdParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, indirectNode.IdParameter, absolutePosition);
                return null;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                if (getMpidNode.StatusParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, getMpidNode.StatusParameter, absolutePosition);
                if (getMpidNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, getMpidNode.RightParameter, absolutePosition);
                return null;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                if (predicateValueNode.ValueParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, predicateValueNode.ValueParameter, absolutePosition);
                return null;
            case PredicateParameterNode predicateNode:
                // 判定条件
                if (predicateNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, predicateNode.LeftParameter, absolutePosition);
                if (predicateNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, predicateNode.RightParameter, absolutePosition);
                return null;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var childNode in multipleNode.Parameters)
                {
                    if (childNode.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, childNode, absolutePosition);
                }

                return null;
            case CompareParameterNode compareNode:
                // 比較式
                if (compareNode.LeftParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, compareNode.LeftParameter, absolutePosition);
                if (compareNode.RightParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, compareNode.RightParameter, absolutePosition);
                return null;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                if (referStatusNode.ChildParameter.Range.InRange(absolutePosition)) return CreateParameterLocationLink(script, lineNode, referStatusNode.ChildParameter, absolutePosition);
                return null;
            case VariableParameterNode:
                // 変数
                return null;
        }

        // 一番下のノードの種類に従ってジャンプ先を生成
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
                // psonN.txt の該当行へジャンプ
                return CreateLocationLink<PersonDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetPersonDefinition, configuration.TryGetPsonFullPath, static def => def.LineNumber);
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
                // pnetN.txt の該当行へジャンプ
                return CreateLocationLink<PlanetDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetPlanetDefinition, configuration.TryGetPnetFullPath, static def => def.LineNumber);
            case ParameterType.StarId:
                // pnetN.txt の該当行へジャンプ
                return CreateLocationLink<StarDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetStarDefinition, configuration.TryGetPnetFullPath, static def => def.LineNumber);
            case ParameterType.ShipId:
                return CreateLocationLink<ShipDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetShipDefinition, configuration.TryGetShipFullPath, static def => def.LineNumber);
            case ParameterType.ItemName:
                return CreateLocationLink<ItemDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetItemDefinition, configuration.TryGetItemFullPath, static def => def.LineNumber);
            case ParameterType.SkillName:
                return CreateLocationLink<SkillDefinition>(script.RawText, lineNode, paramNode.Range, definitionHolder.TryGetSkillDefinition, configuration.TryGetSkillFullPath, static def => def.LineNumber);
            case ParameterType.Label:
                // ラベル行へジャンプ
                if (!script.TryGetLabelLineNode(script.RawText.Slice(paramNode.Range), out var labelLineNode)) return null;
                return CreateLocationLink(lineNode, paramNode.Range, script.Uri, labelLineNode.LineNumber);
            case ParameterType.ScriptFile:
                // スクリプトファイルへジャンプ
                if (!configuration.TryGetFullPath($"{script.RawText.Slice(paramNode.Range)}.txt", out var fullPath)) return null;
                return CreateLocationLink(lineNode, paramNode.Range, new Uri(fullPath, UriKind.Absolute), 0);
            default:
                return null;
        }
    }

    private delegate bool TryGetDelegate<in TValue, TDefinition>(TValue value, [NotNullWhen(true)] out TDefinition? definition) where TValue : allows ref struct;

    private delegate bool FullPathGetter([NotNullWhen(true)] out string? fullPath);

    /// <summary>
    /// パラメータから定義位置を作成します。
    /// </summary>
    private static LocationLink? CreateLocationLink<T>(RawText rawText, LineNode lindeNode, AbsoluteRange range, TryGetDelegate<ReadOnlySpan<char>, T> tryGet, FullPathGetter pathGet, Func<T, int> getLineNumber)
    {
        var paramText = rawText.Slice(range);
        if (!tryGet.Invoke(paramText, out var personDef)) return null;
        if (!pathGet.Invoke(out var path)) return null;

        var lineNumber = getLineNumber.Invoke(personDef);
        return CreateLocationLink(lindeNode, range, new Uri(path, UriKind.Absolute), lineNumber);
    }

    /// <summary>
    /// LocationLink を作成します。
    /// </summary>
    private static LocationLink CreateLocationLink(LineNode lineNode, AbsoluteRange range, DocumentUri targetUri, int targetLine)
    {
        return new LocationLink
        {
            OriginSelectionRange = new Range
            {
                Start = new Position(lineNode.LineNumber, range.Start - lineNode.LineRange.Start),
                End = new Position(lineNode.LineNumber, range.End - lineNode.LineRange.Start),
            },
            TargetUri = targetUri,
            TargetRange = new Range(targetLine, 0, targetLine, 0),
            TargetSelectionRange = new Range(targetLine, 0, targetLine, 0),
        };
    }
}