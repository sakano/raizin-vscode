using System.Diagnostics;
using RaizinLanguageServer.Models.Statuses;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// 参照先ステータスに対応する値が指定されるパラメータを表します。
/// </summary>
public sealed class ReferStatusParameterNode : ParameterNode
{
    /// <summary>
    /// 参照先ステータス
    /// </summary>
    public Status? ReferStatus { get; }

    /// <summary>
    /// 子ノード
    /// </summary>
    public ParameterNode ChildParameter { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReferStatusParameterNode(AbsoluteRange paramRange, ParameterType expectedType, Status? referStatus, ParameterNode childParameter) : base(paramRange, expectedType)
    {
        Debug.Assert(paramRange.InRange(childParameter.Range.Start));
        Debug.Assert(paramRange.InRange(childParameter.Range.End));
        Debug.Assert(referStatus is null ? expectedType is ParameterType.Unknown : referStatus.Type == expectedType);

        ReferStatus = referStatus;
        ChildParameter = childParameter;
    }
}