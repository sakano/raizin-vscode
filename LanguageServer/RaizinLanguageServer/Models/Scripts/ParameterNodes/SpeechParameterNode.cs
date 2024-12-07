using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// 台詞パラメータを表すノードです。
/// </summary>
public sealed class SpeechParameterNode : ParameterNode
{
    /// <summary>
    /// 通常の文字列部分と特殊文字部分を分割したリスト
    /// </summary>
    public IReadOnlyList<Part> Parts { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SpeechParameterNode(AbsoluteRange range, IReadOnlyList<Part> parts) : base(range, ParameterType.Speech)
    {
        Debug.Assert(parts.All(x => range.InRange(x.Range.Start) && range.InRange(x.Range.End)));

        Parts = parts;
    }

    public readonly struct Part(bool isSpecial, AbsoluteRange range)
    {
        /// <summary>
        /// 特殊文字かどうか
        /// </summary>
        public bool IsSpecial { get; } = isSpecial;

        /// <summary>
        /// 範囲
        /// </summary>
        public AbsoluteRange Range { get; } = range;
    }
}