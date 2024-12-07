using System.Diagnostics;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// 判定値パラメータを表すノードです。
/// 以下の形式で構成されます。
/// [&lt;>!=]パラメータ
/// </summary>
public sealed class PredicateValueParameterNode : ParameterNode
{
    /// <summary>
    /// 演算子の範囲
    /// </summary>
    public AbsoluteRange OperatorRange { get; }

    /// <summary>
    /// パラメータ
    /// </summary>
    public ParameterNode ValueParameter { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PredicateValueParameterNode(AbsoluteRange paramRange, ParameterType expectedType, AbsoluteRange operatorRange, ParameterNode valueParameter) : base(paramRange, expectedType)
    {
        Debug.Assert(paramRange.InRange(operatorRange.Start));
        Debug.Assert(paramRange.InRange(operatorRange.End));
        Debug.Assert(paramRange.InRange(valueParameter.Range.Start));
        Debug.Assert(paramRange.InRange(valueParameter.Range.End));

        OperatorRange = operatorRange;
        ValueParameter = valueParameter;
    }
}