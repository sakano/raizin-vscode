using System.Diagnostics;

namespace RaizinLanguageServer.Models.Scripts;

/// <summary>
/// <see cref="RawText"/> の中での絶対範囲を表します。
/// </summary>
public readonly struct AbsoluteRange
{
    /// <summary>
    /// 範囲の開始位置を取得します。
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// 範囲の終了位置を取得します。
    /// </summary>
    public int End { get; }

    /// <summary>
    /// 範囲の長さを取得します。
    /// </summary>
    public int Length => End - Start;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AbsoluteRange(int start, int end)
    {
        Debug.Assert(start <= end);
        Start = start;
        End = end;
    }
}

public static class AbsoluteRangeExtensions
{
    /// <summary>
    /// 指定した位置が範囲内にあるかどうかを判定します。
    /// </summary>
    public static bool InRange(this AbsoluteRange range, int absolutePosition) => range.Start <= absolutePosition && absolutePosition <= range.End;
}