using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models.Scripts.LineNodes;

namespace RaizinLanguageServer.Models.Scripts;

/// <summary>
/// パースされた Raizin スクリプトを表すクラスです。
/// スクリプトは全ての行が1行ごとに <see cref="LineNode"/> によって表されます。 
/// </summary>
public sealed class RaizinScript(DocumentUri uri, RawText rawText, List<LineNode> lineNodes, List<LabelLineNode> labelLineNodes)
{
    /// <summary>
    /// スクリプトの URI を取得します。
    /// </summary>
    public DocumentUri Uri { get; } = uri;

    /// <summary>
    /// スクリプトのテキストを取得します。
    /// </summary>
    public RawText RawText { get; } = rawText;

    /// <summary>
    /// 全ての行ノードを列挙します。
    /// </summary>
    public IEnumerable<LineNode> EnumerateLineNodes() => lineNodes;

    /// <summary>
    /// 指定された行番号の行ノードを取得します。
    /// </summary>
    public bool TryGetLineNode<T>(int lineNumber, [NotNullWhen(true)] out T? lineNode) where T : LineNode
    {
        if (lineNodes.Count < lineNumber)
        {
            // 指定された行番号の行が存在しない
            lineNode = null;
            return false;
        }

        lineNode = lineNodes[lineNumber] as T;
        return lineNode is not null;
    }

    /// <summary>
    /// 指定された位置の行ノードを取得します。
    /// </summary>
    public bool TryGetLineNode<T>(Position position, [NotNullWhen(true)] out T? lineNode) where T : LineNode
    {
        if (!TryGetLineNode(position.Line, out lineNode))
        {
            return false;
        }

        if (lineNode.LineRange.Length < position.Character)
        {
            // 指定された位置が行の範囲外
            lineNode = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 全てのラベル行ノードを列挙します。
    /// </summary>
    public IEnumerable<LabelLineNode> EnumerateLabelLineNodes() => labelLineNodes;

    /// <summary>
    /// 指定されたラベル名のラベル行ノードを取得します。
    /// </summary>
    public bool TryGetLabelLineNode(ReadOnlySpan<char> labelName, [NotNullWhen(true)] out LabelLineNode? labelLineNode)
    {
        if (labelName.Length > 0)
        {
            foreach (var node in labelLineNodes)
            {
                if (RawText.Slice(node.LabelRange).SequenceEqual(labelName))
                {
                    labelLineNode = node;
                    return true;
                }
            }
        }

        labelLineNode = null;
        return false;
    }
}