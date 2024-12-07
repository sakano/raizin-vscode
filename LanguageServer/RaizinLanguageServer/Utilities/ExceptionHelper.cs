using System;
using System.Diagnostics.CodeAnalysis;

namespace RaizinLanguageServer.Utilities;

public static class ExceptionHelper
{
    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string? paramName, string? message = null) => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    public static void ThrowException(string? message = null) => throw new Exception(message);
}