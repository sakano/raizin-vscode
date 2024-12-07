using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// 参照を探す機能を提供します。 
/// </summary>
public sealed class ReferencesHandler(ScriptHolder scriptHolder) : ReferencesHandlerBase
{
    protected override ReferenceRegistrationOptions CreateRegistrationOptions(ReferenceCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            WorkDoneProgress = false,
        };

    /// <summary>
    /// 参照を探すリクエストが来た際に呼び出されます。
    /// </summary>
    public override Task<LocationContainer?> Handle(ReferenceParams request, CancellationToken cancellationToken)
    {
        var locations = CreateLocationLinks(request);
        return Task.FromResult(locations is null ? null : LocationContainer.From(locations));
    }

    /// <summary>
    /// 参照位置のリストを作成します。
    /// </summary>
    private IEnumerable<Location>? CreateLocationLinks(ReferenceParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない
        if (!script.TryGetLineNode<LineNode>(request.Position, out var lineNode)) return null; // コマンド行でない

        var absolutePosition = lineNode.LineRange.Start + request.Position.Character;

        if (lineNode is LabelLineNode labelLineNode)
        {
            if (labelLineNode.LabelRange.InRange(absolutePosition))
            {
                var labelName = script.RawText.Slice(labelLineNode.LabelRange);
                return CreateLabelLocationLinks(script, labelName);
            }
        }
        else if (lineNode is CommandLineNode commandLine)
        {
            if (!commandLine.TryGetParameterIndex(request.Position, out var paramIndex)) return null; // パラメータ番号が見つからない
            if (!commandLine.TryGetParameterNode(paramIndex, out var paramNode)) return null; // パラメータノードが見つからない

            if (paramNode.ExpectedType is ParameterType.Label)
            {
                var labelName = script.RawText.Slice(paramNode.Range);
                return CreateLabelLocationLinks(script, labelName);
            }
        }

        return null;
    }

    /// <summary>
    /// 指定されたラベル名の参照を作成します。
    /// </summary>
    private IEnumerable<Location>? CreateLabelLocationLinks(RaizinScript script, ReadOnlySpan<char> labelName)
    {
        if (labelName.Length == 0) return null; // ラベル名が空

        var locations = new List<Location>();
        foreach (var lineNode in script.EnumerateLineNodes())
        {
            if (lineNode is LabelLineNode labelLineNode)
            {
                // ラベル行を参照として追加
                if (script.RawText.Slice(labelLineNode.LabelRange).SequenceEqual(labelName))
                {
                    locations.Add(new Location
                    {
                        Uri = script.Uri,
                        Range = new Range(
                            labelLineNode.LineNumber, labelLineNode.LabelRange.Start - labelLineNode.LineRange.Start,
                            labelLineNode.LineNumber, labelLineNode.LabelRange.End - labelLineNode.LineRange.Start),
                    });
                }
            }
            else if (lineNode is CommandLineNode commandLine)
            {
                if (commandLine.Definition is null) continue;

                // ラベルパラメータを参照として追加
                for (var paramIndex = 0; paramIndex < commandLine.Definition.Parameters.Count; paramIndex++)
                {
                    var paramDef = commandLine.Definition.Parameters[paramIndex];
                    if (paramDef.Type is ParameterDefinitionType.Label && commandLine.TryGetParameterNode(paramIndex, out var paramNode))
                    {
                        if (script.RawText.Slice(paramNode.Range).SequenceEqual(labelName))
                        {
                            locations.Add(new Location
                            {
                                Uri = script.Uri,
                                Range = new Range(
                                    commandLine.LineNumber, paramNode.Range.Start - commandLine.LineRange.Start,
                                    commandLine.LineNumber, paramNode.Range.End - commandLine.LineRange.Start),
                            });
                        }
                    }
                }
            }
        }

        return locations;
    }
}