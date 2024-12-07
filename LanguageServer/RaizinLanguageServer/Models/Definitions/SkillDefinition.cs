namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// スキル定義
/// </summary>
public sealed record SkillDefinition : IDescriptiveValue
{
    public static string DisplayName => "スキル";

    /// <summary>
    /// スキル名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// sk_base.csv の行番号
    /// </summary>
    public required int LineNumber { get; init; }

    /// <inheritdoc/>
    public string ScriptValue => Name;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public string? ShortDescription => null;

    /// <inheritdoc/>
    public string? Description => null;
}