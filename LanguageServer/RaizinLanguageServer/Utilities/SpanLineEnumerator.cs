using System;
using System.Runtime.CompilerServices;

namespace RaizinLanguageServer.Utilities;

public static class SpanLineEnumeratorExtensions
{
    /// <summary>
    /// Enumerates the range of lines of the span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanLineEnumerator EnumerateLineRanges(this ReadOnlySpan<char> source) => new(source);

    /// <summary>
    /// Enumerates the range of lines of the string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanLineEnumerator EnumerateLineRanges(this string source) => new(source);
}

/// <summary>
/// Enumerates the range of lines of a <see cref="ReadOnlySpan{Char}"/>.
/// </summary>
public ref struct SpanLineEnumerator
{
    private ReadOnlySpan<char> _remaining;
    private Range _current;
    private bool _finished;

    private int _start;

    public SpanLineEnumerator(ReadOnlySpan<char> buffer)
    {
        _remaining = buffer;
        _current = default;
        _finished = false;
    }

    /// <summary>
    /// Gets the range of line at the current position of the enumerator.
    /// </summary>
    public Range Current => _current;

    /// <summary>
    /// Returns this instance as an enumerator.
    /// </summary>
    public SpanLineEnumerator GetEnumerator() => this;

    /// <summary>
    /// Advances the enumerator to the next line of the span.
    /// </summary>
    /// <returns>
    /// True if the enumerator successfully advanced to the next line; false if
    /// the enumerator has advanced past the end of the span.
    /// </returns>
    public bool MoveNext()
    {
        if (_finished) return false;

        var remaining = _remaining;


        var idx = remaining.IndexOfAny('\r', '\n');

        if ((uint)idx < (uint)remaining.Length)
        {
            var stride = 1;
            if (remaining[idx] == '\r' && (uint)(idx + 1) < (uint)remaining.Length && remaining[idx + 1] == '\n')
            {
                stride = 2;
            }

            _current = new Range(_start, _start + idx);
            _remaining = remaining.Slice(idx + stride);
            _start += idx + stride;
        }
        else
        {
            // We've reached EOF, but we still need to return 'true' for this final
            // iteration so that the caller can query the Current property once more.

            _current = new Range(_start, _start + remaining.Length);
            _remaining = default;
            _finished = true;
        }

        return true;
    }
}