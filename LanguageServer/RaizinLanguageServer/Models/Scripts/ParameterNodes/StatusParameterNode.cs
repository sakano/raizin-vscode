using System.Diagnostics;
using RaizinLanguageServer.Models.Statuses;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// ステータスIDが指定されるパラメータを表します。
/// </summary>
public class StatusParameterNode : ParameterNode
{
    /// <summary>
    /// 指定されているステータス
    /// </summary>
    public Status? Status { get; }

    /// <summary>
    /// コンストラクタ 
    /// </summary>
    public StatusParameterNode(AbsoluteRange paramRange, ParameterType expectedType, Status? status) : base(paramRange, expectedType)
    {
        Debug.Assert(expectedType is ParameterType.PersonStatusId or ParameterType.PowerStatusId or ParameterType.PlanetStatusId or ParameterType.AnyStatusId);
        Status = status;
    }
}