using System.Collections.Generic;

namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 役職IDを表します
/// </summary>
public sealed class JobDefinition : IDescriptiveValue
{
    public static string DisplayName => "役職ID";

    /// <inheritdoc/>
    public string ScriptValue { get; }

    /// <inheritdoc/>
    public string SearchValue => ScriptValue;

    /// <inheritdoc/>
    public string ShortDescription { get; }

    /// <inheritdoc/>
    public string? Description => null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private JobDefinition(string scriptValue, string shortDescription)
    {
        ScriptValue = scriptValue;
        ShortDescription = shortDescription;
    }

    public static IReadOnlyList<JobDefinition> Rai7List { get; } =
    [
        new("2", "艦隊総司令"),
        new("3", "妾"),
        new("4", "特命担当"),
        new("5", "諜報担当"),
        new("7", "艦隊司令"),
        new("9", "技術担当"),
        new("16", "人事担当"),
        new("17", "経済担当"),
        new("18", "防衛担当"),
        new("19", "補給担当"),
        new("0", "NON")
    ];

    public static IReadOnlyList<JobDefinition> Rai8List { get; } =
    [
        new("2", "艦長"),
        new("3", "妾"),
        new("4", "特命担当"),
        new("5", "外交担当"),
        new("9", "開発担当"),
        new("16", "内政担当"),
        new("0", "NON")
    ];
}