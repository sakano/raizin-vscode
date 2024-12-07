using RaizinLanguageServer.Models.Definitions;

namespace RaizinLanguageServer.Models.Commands;

/// <summary>
/// 特定の値をとるパラメータの選択肢を表します
/// </summary>
public sealed class ChoiceValueDefinition(string scriptValue, string shortDescription) : IDescriptiveValue
{
    /// <inheritdoc/>
    public string ShortDescription { get; } = shortDescription;

    /// <inheritdoc/>
    public string ScriptValue { get; } = scriptValue;

    /// <inheritdoc/>
    public string SearchValue => ScriptValue;

    /// <inheritdoc/>
    public string? Description => null;
}