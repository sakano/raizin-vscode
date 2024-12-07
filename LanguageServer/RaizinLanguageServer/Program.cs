using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using RaizinLanguageServer.Handlers;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Statuses;
using RaizinLanguageServer.Utilities;
using CommandRepository = RaizinLanguageServer.Models.Commands.CommandRepository;

var languageServer = await LanguageServer.From(options => options
    .WithServerInfo(new ServerInfo { Name = IRaizinConfiguration.LanguageServerName, Version = IRaizinConfiguration.LanguageServerVersion })
    .WithInput(Console.OpenStandardInput())
    .WithOutput(Console.OpenStandardOutput())
    .WithHandler<CompletionHandler>()
    .WithHandler<ConfigurationHandler>()
    .WithHandler<DefinitionHandler>()
    .WithHandler<DocumentSymbolHandler>()
    .WithHandler<HoverHandler>()
    .WithHandler<InlayHintsHandler>()
    .WithHandler<ReferencesHandler>()
    .WithHandler<SemanticTokensHandler>()
    .WithHandler<SignatureHelpHandler>()
    .WithHandler<TextDocumentHandler>()
    .WithServices(services =>
    {
        services.AddSingleton<ScriptHolder>();
        services.AddSingleton<IScriptHolder>(static provider => provider.GetRequiredService<ScriptHolder>());

        services.AddSingleton<StatusRepository>();
        services.AddSingleton<RaizinScriptParser>();
        services.AddSingleton<IRaizinConfiguration, RaizinConfiguration>();
        services.AddSingleton<IDefinitionHolder, DefinitionHolder>();
        services.AddSingleton<CommandRepository>();
        services.AddSingleton<DiagnosticRepository>();
        services.AddSingleton<Diagnostics>();

        services.AddSingleton<ILogger>(static provider =>
        {
            var windowLanguageServer = provider.GetRequiredService<IWindowLanguageServer>();
            var clientLanguageServer = provider.GetRequiredService<IClientLanguageServer>();
            var loggerProvider = new LoggerProvider(windowLanguageServer, clientLanguageServer);
            return loggerProvider.CreateLogger("DefaultLogger");
        });
    })
    .OnInitialized((server, request, response, token) =>
    {
        server.Window.LogInfo(IRaizinConfiguration.LanguageServerName + " Initialized");
        return Task.CompletedTask;
    })
).ConfigureAwait(false);

await languageServer.WaitForExit.ConfigureAwait(false);