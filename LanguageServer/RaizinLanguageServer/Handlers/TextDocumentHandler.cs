using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Scripts;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// ファイルが変更された際に、スクリプトをパースします。
/// </summary>
public sealed class TextDocumentHandler(ScriptHolder scriptHolder, RaizinScriptParser parser, Diagnostics diagnostics) : TextDocumentSyncHandlerBase
{
    public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) => new(uri, IRaizinConfiguration.Language);

    protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            Change = TextDocumentSyncKind.Full,
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            Save = false,
        };

    /// <summary>
    /// テキストが開かれたらパースします。
    /// </summary>
    public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        var script = parser.Parse(request.TextDocument.Uri, new RawText { Text = request.TextDocument.Text });
        scriptHolder.SetScript(request.TextDocument.Uri, script);

        diagnostics.Diagnose(script);

        return Unit.Task;
    }

    /// <summary>
    /// テキストが変更されたら再パースします。
    /// </summary>
    public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        scriptHolder.TryGetScript(request.TextDocument.Uri, out var script);
        foreach (var change in request.ContentChanges)
        {
            script = parser.Parse(request.TextDocument.Uri, new RawText { Text = change.Text });
            scriptHolder.SetScript(request.TextDocument.Uri, script);
        }

        if (script is not null) diagnostics.Diagnose(script);

        return Unit.Task;
    }

    /// <summary>
    /// テキストが閉じられたらパース結果を破棄します。
    /// </summary>
    public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        scriptHolder.RemoveScript(request.TextDocument.Uri);
        diagnostics.Clear(request.TextDocument.Uri);
        return Unit.Task;
    }

    /// <summary>
    /// テキストが保存されても何もしない
    /// </summary>
    public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => throw new NotSupportedException();
}