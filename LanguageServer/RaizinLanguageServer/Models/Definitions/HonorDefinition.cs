using System.Collections.Generic;

namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 勲功を表します
/// </summary>
public sealed class HonorDefinition : IDescriptiveValue
{
    public static string DisplayName => "勲功";

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
    private HonorDefinition(string id, string shortDescription)
    {
        ScriptValue = id;
        ShortDescription = shortDescription;
    }

    public static IReadOnlyList<HonorDefinition> List { get; } =
    [
        new("13000", "覇王"),
        new("12750", "上皇"),
        new("12700", "太后"),
        new("12600", "婿"),
        new("12300", "王子"),
        new("12200", "妃"),
        new("12100", "姫"),
        new("12080", "王族"),
        new("11000", "将軍"),
        new("10000", "分将"),
        new("9000", "分１位"),
        new("8000", "分２位"),
        new("7000", "分３位"),
        new("6000", "分４位"),
        new("5000", "分５位"),
        new("4000", "分６位"),
        new("3000", "分７位"),
        new("2000", "分８位"),
    ];
}