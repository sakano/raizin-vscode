namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// パラメータを表します。
/// </summary>
public class ParameterNode(AbsoluteRange paramRange, ParameterType expectedType)
{
    /// <summary>
    /// パラメータの範囲
    /// </summary>
    public AbsoluteRange Range { get; } = paramRange;

    /// <summary>
    /// このパラメータで指定されることが期待される型
    /// </summary>
    public ParameterType ExpectedType { get; } = expectedType;
}