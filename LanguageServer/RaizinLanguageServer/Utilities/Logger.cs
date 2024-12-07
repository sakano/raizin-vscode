using System;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace RaizinLanguageServer.Utilities;

public sealed class Logger(string categoryName, IWindowLanguageServer windowLanguageServer, IClientLanguageServer clientLanguageServer) : ILogger
{
    // ReSharper disable once UnusedMember.Local
    private string CategoryName { get; } = categoryName;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        switch (logLevel)
        {
            case LogLevel.Trace:
                clientLanguageServer.LogTrace(new LogTraceParams
                {
                    Message = formatter(state, exception),
                    Verbose = "trace"
                });
                break;
            case LogLevel.Debug:
                windowLanguageServer.Log($"{formatter(state, exception)}");
                break;
            case LogLevel.Information:
                windowLanguageServer.LogInfo($"{formatter(state, exception)}");
                break;
            case LogLevel.Warning:
                windowLanguageServer.LogWarning($"{formatter(state, exception)}");
                break;
            case LogLevel.Error:
                windowLanguageServer.LogError($"{formatter(state, exception)}");
                break;
            case LogLevel.Critical:
                windowLanguageServer.LogError($"{formatter(state, exception)}");
                break;
            case LogLevel.None:
                break;
            default:
                ExceptionHelper.ThrowArgumentOutOfRangeException(nameof(logLevel));
                break;
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
}