namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 艦種定義
/// </summary>
public sealed class ShipDefinition : IDescriptiveValue
{
    public static string DisplayName => "艦種ID";

    /// <summary>
    /// 艦種ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 艦種名
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 定義ファイルでの行番号
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