namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// 行ごとにパースされた結果を表します。
/// </summary>
public abstract class LineNode(int lineNumber, AbsoluteRange lineRange)
{
    /// <summary>
    /// 行番号(0オリジン)を取得します。
    /// </summary>
    public int LineNumber { get; } = lineNumber;

    /// <summary>
    /// 行の範囲を取得します。
    /// </summary>
    public AbsoluteRange LineRange { get; } = lineRange;

    /// <summary>
    /// 行の長さを取得します。
    /// </summary>
    public int LineLength => LineRange.End - LineRange.Start;
}