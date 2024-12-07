using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// コード上のシンボルを提供します。
/// </summary>
public sealed class DocumentSymbolHandler(IScriptHolder scriptHolder) : DocumentSymbolHandlerBase
{
    protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            WorkDoneProgress = false,
        };

    /// <summary>
    /// シンボルをリクエストされた際に呼び出されます。
    /// </summary>
    public override Task<SymbolInformationOrDocumentSymbolContainer?> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
    {
        var symbols = CreateDocumentSymbol(request);
        if (symbols is null) return Task.FromResult<SymbolInformationOrDocumentSymbolContainer?>(null);
        return Task.FromResult<SymbolInformationOrDocumentSymbolContainer?>(SymbolInformationOrDocumentSymbolContainer.From(symbols));
    }

    /// <summary>
    /// シンボルを作成します。
    /// </summary>
    private List<SymbolInformationOrDocumentSymbol>? CreateDocumentSymbol(DocumentSymbolParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない

        var symbols = new List<SymbolInformationOrDocumentSymbol>();

        foreach (var lineNode in script.EnumerateLineNodes())
        {
            switch (lineNode)
            {
                case CommandLineNode commandLine:
                {
                    var commandName = script.RawText.Slice(commandLine.CommandNameRange);
                    // eve_start と eve_end をシンボルとして追加
                    if (commandName is "eve_start" or "eve_end")
                    {
                        var range = new Range
                        {
                            Start = new Position(commandLine.LineNumber, commandLine.CommandNameRange.Start - commandLine.LineRange.Start),
                            End = new Position(commandLine.LineNumber, commandLine.CommandNameRange.End - commandLine.LineRange.Start),
                        };
                        symbols.Add(new SymbolInformationOrDocumentSymbol(new DocumentSymbol()
                        {
                            Name = commandName.ToString(),
                            Kind = SymbolKind.Namespace,
                            Range = range,
                            SelectionRange = range,
                        }));
                    }

                    break;
                }
                case LabelLineNode labelLine:
                {
                    // ラベルをシンボルとして追加
                    var range = new Range
                    {
                        Start = new Position(labelLine.LineNumber, labelLine.LabelRange.Start - labelLine.LineRange.Start),
                        End = new Position(labelLine.LineNumber, labelLine.LabelRange.End - labelLine.LineRange.Start),
                    };
                    symbols.Add(new SymbolInformationOrDocumentSymbol(new DocumentSymbol
                    {
                        Name = labelLine.Label,
                        Kind = SymbolKind.Event,
                        Range = range,
                        SelectionRange = range,
                    }));
                    break;
                }
            }
        }

        return symbols;
    }
}