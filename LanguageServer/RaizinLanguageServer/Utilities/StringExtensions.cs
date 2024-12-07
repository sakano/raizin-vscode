using System;

namespace RaizinLanguageServer.Utilities;

public static class StringExtensions
{
    public static bool IsInteger(this string value) => value.AsSpan().IsInteger();

    public static bool IsInteger(this ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return false;
        }

        var index = value[0] is '-' or '+' ? 1 : 0;

        if (index == 1 && value.Length == 1)
        {
            return false;
        }

        for (; index < value.Length; index++)
        {
            if (!char.IsAsciiDigit(value[index]))
            {
                return false;
            }
        }

        return true;
    }
}