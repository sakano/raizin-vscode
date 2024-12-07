namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// psonN.csv での人物定義
/// </summary>
public sealed class PersonDefinition : IDescriptiveValue
{
    public static string DisplayName => "人物ID";

    /// <summary>
    /// 人物ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 人物名
    /// </summary>
    public required string Name { get; init; } = "";

    /// <summary>
    /// psonN.csv での行番号
    /// </summary>
    public required int LineNumber { get; init; } = 0;

    /// <inheritdoc/>
    public string ScriptValue => Id;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public string ShortDescription => Name;

    /// <inheritdoc/>
    public string? Description => null;
}