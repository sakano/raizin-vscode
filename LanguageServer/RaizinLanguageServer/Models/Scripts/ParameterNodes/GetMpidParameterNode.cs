using System.Diagnostics;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// get_mpid コマンドのパラメータを表すノードです。
/// </summary>
public sealed class GetMpidParameterNode : ParameterNode
{
    /// <summary>
    /// 左辺のパラメータ
    /// </summary>
    public StatusParameterNode StatusParameter { get; }

    /// <summary>
    /// 右辺のパラメータ
    /// </summary>
    public ParameterNode RightParameter { get; }

    /// <summary>
    /// 演算子の範囲
    /// </summary>
    public AbsoluteRange OperatorRange { get; }

    /// <summary>
    /// get_mpid コマンドのパラメータを表すノードです。
    /// </summary>
    public GetMpidParameterNode(AbsoluteRange paramRange, ParameterType expectedType, StatusParameterNode statusParameter, ParameterNode rightParameter, AbsoluteRange operatorRange) : base(paramRange, expectedType)
    {
        Debug.Assert(paramRange.InRange(statusParameter.Range.Start));
        Debug.Assert(paramRange.InRange(statusParameter.Range.End));
        Debug.Assert(paramRange.InRange(operatorRange.Start));
        Debug.Assert(paramRange.InRange(operatorRange.End));
        Debug.Assert(paramRange.InRange(rightParameter.Range.Start));
        Debug.Assert(paramRange.InRange(rightParameter.Range.End));

        StatusParameter = statusParameter;
        OperatorRange = operatorRange;
        RightParameter = rightParameter;
    }
}