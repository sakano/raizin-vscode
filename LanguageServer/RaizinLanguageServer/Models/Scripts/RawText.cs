using System;

namespace RaizinLanguageServer.Models.Scripts;

/// <summary>
/// スクリプトのテキスト全体を表します。
/// </summary>
public readonly struct RawText
{
    /// <summary>
    /// 指定された範囲のテキストを取得します。
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ReadOnlySpan<char> Slice(AbsoluteRange range) => Text.AsSpan(range.Start, range.End - range.Start);

    public required string Text { get; init; }
}