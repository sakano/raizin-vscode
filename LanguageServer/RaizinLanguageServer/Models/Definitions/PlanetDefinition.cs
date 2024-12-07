namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// pnetN.csv での惑星定義
/// </summary>
public sealed class PlanetDefinition : IDescriptiveValue
{
    public static string DisplayName => "惑星ID";

    /// <summary>
    /// 惑星ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 惑星名
    /// </summary>
    public required string Name { get; init; } = "";

    /// <summary>
    /// pnetN.csv での行番号
    /// </summary>
    public required int LineNumber { get; init; } = 0;

    /// <summary>
    /// 主星かどうか
    /// </summary>
    public required bool IsPrimary { get; init; }

    /// <inheritdoc/>
    public string ScriptValue => Id;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public string? ShortDescription => Name;

    /// <inheritdoc/>
    public string? Description => null;
}