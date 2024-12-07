using System.Diagnostics;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// if_cpsts, chk_cpsts コマンドの比較式パラメータを表すノードです。
/// </summary>
public sealed class CompareParameterNode : ParameterNode
{
    /// <summary>
    /// 左辺のパラメータ
    /// </summary>
    public ParameterNode LeftParameter { get; }

    /// <summary>
    /// 右辺のパラメータ
    /// </summary>
    public ParameterNode RightParameter { get; }

    /// <summary>
    /// 演算子の範囲
    /// </summary>
    public AbsoluteRange OperatorRange { get; }

    /// <summary>
    /// if_cpsts, chk_cpsts コマンドの比較式パラメータを表すノードです。
    /// </summary>
    public CompareParameterNode(AbsoluteRange paramRange, ParameterType expectedType, ParameterNode leftParameter, ParameterNode rightParameter, AbsoluteRange operatorRange) : base(paramRange, expectedType)
    {
        Debug.Assert(paramRange.InRange(leftParameter.Range.Start));
        Debug.Assert(paramRange.InRange(leftParameter.Range.End));
        Debug.Assert(paramRange.InRange(operatorRange.Start));
        Debug.Assert(paramRange.InRange(operatorRange.End));
        Debug.Assert(paramRange.InRange(rightParameter.Range.Start));
        Debug.Assert(paramRange.InRange(rightParameter.Range.End));

        LeftParameter = leftParameter;
        OperatorRange = operatorRange;
        RightParameter = rightParameter;
    }
}