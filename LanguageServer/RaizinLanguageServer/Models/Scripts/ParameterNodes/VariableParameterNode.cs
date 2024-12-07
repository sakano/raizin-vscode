using RaizinLanguageServer.Models.Definitions;

namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// 変数が指定されているパラメータを表します。
/// </summary>
public sealed class VariableParameterNode : ParameterNode
{
    /// <summary>
    /// パラメータで指定される変数
    /// </summary>
    public VariableDefinition Variable { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public VariableParameterNode(AbsoluteRange paramRange, ParameterType expectedType, VariableDefinition variable) : base(paramRange, expectedType)
    {
        Variable = variable;
    }
}