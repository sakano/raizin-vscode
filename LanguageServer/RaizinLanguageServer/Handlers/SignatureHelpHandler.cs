using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// コマンドのシグネチャヘルプを表示する
/// </summary>
public sealed class SignatureHelpHandler(IScriptHolder scriptHolder) : SignatureHelpHandlerBase
{
    protected override SignatureHelpRegistrationOptions CreateRegistrationOptions(SignatureHelpCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            TriggerCharacters = new[] { "," },
            RetriggerCharacters = new[] { "," },
            WorkDoneProgress = false,
        };

    /// <summary>
    /// シグネチャヘルプ作成リクエストが来たときに呼び出されます
    /// </summary>
    public override Task<SignatureHelp?> Handle(SignatureHelpParams request, CancellationToken cancellationToken)
    {
        var help = CreateSignatureHelp(request);
        if (help is null) return Task.FromResult<SignatureHelp?>(null);
        return Task.FromResult<SignatureHelp?>(help);
    }

    /// <summary>
    /// シグネチャヘルプを作成します
    /// </summary>
    private SignatureHelp? CreateSignatureHelp(SignatureHelpParams request)
    {
        if (!scriptHolder.TryGetScript(request.TextDocument.Uri, out var script)) return null; // スクリプトが見つからない
        if (!script.TryGetLineNode<CommandLineNode>(request.Position, out var commandLine)) return null; // コマンド行でない
        if (commandLine.Definition is null) return null; // コマンド定義が見つからない

        if (commandLine.Definition.Parameters.Count == 0)
        {
            // パラメータがない場合
            return new SignatureHelp
            {
                Signatures = new[]
                {
                    new SignatureInformation
                    {
                        Label = commandLine.Definition.GetSignature(),
                        Documentation = new MarkupContent
                        {
                            Kind = MarkupKind.Markdown,
                            Value = commandLine.Definition.Description
                        },
                    },
                },
                ActiveSignature = 0,
            };
        }

        // カーソル位置のパラメータ番号を取得
        if (!commandLine.TryGetParameterIndex(request.Position, out var activeParameter))
        {
            activeParameter = -1;
        }

        return new SignatureHelp
        {
            Signatures = new[]
            {
                new SignatureInformation
                {
                    Label = commandLine.Definition.GetSignature(),
                    Parameters = new(commandLine.Definition.Parameters.Select(static argDef => new ParameterInformation { Label = argDef.Name.ToString() })),
                    Documentation = new MarkupContent
                    {
                        Kind = MarkupKind.Markdown,
                        Value = commandLine.Definition.Description
                    },
                },
            },
            ActiveSignature = 0,
            ActiveParameter = activeParameter,
        };
    }
}