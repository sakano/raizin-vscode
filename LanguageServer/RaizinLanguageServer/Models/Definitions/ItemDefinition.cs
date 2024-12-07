namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// アイテム定義
/// </summary>
public sealed record ItemDefinition : IDescriptiveValue
{
    public static string DisplayName => "アイテム";

    /// <summary>
    /// アイテム名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// item.csv の行番号
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