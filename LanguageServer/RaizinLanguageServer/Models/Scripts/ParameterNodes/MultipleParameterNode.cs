using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// |で区切られた複数指定パラメータを表すノードです。
/// </summary>
public sealed class MultipleParameterNode : ParameterNode
{
    /// <summary>
    /// パラメータ
    /// </summary>
    public IReadOnlyList<ParameterNode> Parameters { get; }

    /// <summary>
    /// |で区切られた複数パラメータを表すノードです。
    /// </summary>
    public MultipleParameterNode(AbsoluteRange paramRange, ParameterType expectedType, IReadOnlyList<ParameterNode> parameters) : base(paramRange, expectedType)
    {
        Debug.Assert(parameters.Count(x => paramRange.InRange(x.Range.Start) && paramRange.InRange(x.Range.End)) == parameters.Count);
        Parameters = parameters;
    }
}