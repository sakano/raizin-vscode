using System.Diagnostics;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// 間接参照パラメータを表すノードです。
/// 以下の形式で構成されます。
/// ステータス名:ID
/// </summary>
public sealed class IndirectReferenceParameterNode : ParameterNode
{
    /// <summary>
    /// ステータス名を表す範囲
    /// </summary>
    public StatusParameterNode StatusParameter { get; }

    /// <summary>
    /// ID指定の範囲
    /// </summary>
    public ParameterNode IdParameter { get; }

    /// <summary>
    /// 間接参照パラメータを表すノードです。
    /// 以下の形式で構成されます。
    /// ステータス名:ID
    /// </summary>
    public IndirectReferenceParameterNode(AbsoluteRange paramRange, ParameterType expectedType, StatusParameterNode statusParam, ParameterNode idParam) : base(paramRange, expectedType)
    {
        Debug.Assert(paramRange.InRange(statusParam.Range.Start));
        Debug.Assert(paramRange.InRange(statusParam.Range.End));
        Debug.Assert(paramRange.InRange(idParam.Range.Start));
        Debug.Assert(paramRange.InRange(idParam.Range.End));

        StatusParameter = statusParam;
        IdParameter = idParam;
    }
}