using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace RaizinLanguageServer.Utilities;

public sealed class LoggerProvider(IWindowLanguageServer windowLanguageServer, IClientLanguageServer clientLanguageServer) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, static (name, arg) => new Logger(name, arg.windowLanguageServer, arg.clientLanguageServer), (windowLanguageServer, clientLanguageServer));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}