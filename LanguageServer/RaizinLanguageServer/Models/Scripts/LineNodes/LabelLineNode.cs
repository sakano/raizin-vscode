using System;
using RaizinLanguageServer.Models.Definitions;

namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// ラベル行を表す LineNode です。
/// </summary>
public sealed class LabelLineNode : LineNode, IDescriptiveValue
{
    /// <summary>
    /// 先頭の "*" を含まないラベル名の範囲を取得します。
    /// </summary>
    public AbsoluteRange LabelNameRange => new(LabelRange.Start + 1, LabelRange.End);

    /// <summary>
    /// 先頭の "*" を含むラベルの範囲を取得します。
    /// </summary>
    public AbsoluteRange LabelRange { get; }

    /// <summary>
    /// 先頭の "*" を含むラベルを取得します。
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LabelLineNode(int lineNumber, AbsoluteRange lineRange, AbsoluteRange labelRange, ReadOnlySpan<char> label) : base(lineNumber, lineRange)
    {
        LabelRange = labelRange;
        Label = label.ToString(); // 使う機会が多いので string として保持
    }

    /// <inheritdoc/>
    public string ScriptValue => Label;

    /// <inheritdoc/>
    public string SearchValue => Label;

    /// <inheritdoc/>
    public string? ShortDescription => null;

    /// <inheritdoc/>
    public string? Description => null;

    /// <inheritdoc/>
    public override string ToString() => $"{nameof(LabelLineNode)}(LineNumber = {LineNumber}, LineRange = {LineRange}, LabelNameRange = {LabelNameRange})";
}