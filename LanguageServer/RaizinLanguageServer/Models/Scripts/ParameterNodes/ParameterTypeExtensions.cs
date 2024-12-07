namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

public static class ParameterTypeExtensions
{
    /// <summary>
    /// 変数参照ができないタイプなら true
    /// </summary>
    public static bool IsVariableDisabled(this ParameterType parameterType)
    {
        return parameterType is
            ParameterType.Speech or ParameterType.Label or
            ParameterType.ScriptFile or ParameterType.File;
    }
}