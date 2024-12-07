namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 恒星IDを表します
/// </summary>
public sealed class StarDefinition : IDescriptiveValue
{
    public static string DisplayName => "恒星ID";

    /// <summary>
    /// 恒星ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 恒星名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// pnetN.csv での行番号
    /// </summary>
    public required int LineNumber { get; init; }

    /// <inheritdoc/>
    public string ScriptValue => Id;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public string? ShortDescription => Name;

    /// <inheritdoc/>
    public string? Description => null;
}