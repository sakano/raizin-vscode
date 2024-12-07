using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using RaizinLanguageServer.Models;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// クライアントから設定変更を受け取る
/// </summary>
/// <param name="configuration"></param>
public sealed class ConfigurationHandler(RaizinConfiguration configuration) : DidChangeConfigurationHandlerBase, IOnLanguageServerInitialize
{
    /// <summary>
    /// サーバーが初期化された際に初期設定を受け取る
    /// </summary>
    Task IOnLanguageServerInitialize.OnInitialize(ILanguageServer server, InitializeParams request, CancellationToken cancellationToken)
    {
        configuration.Initialize(request);
        return Task.CompletedTask;
    }

    /// <summary>
    /// クライアントからの設定変更を受け取る
    /// </summary>
    public override Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
    {
        configuration.Update(request);
        return Unit.Task;
    }
}