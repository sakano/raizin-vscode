using System.Collections.Generic;

namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 人物の状態IDを表します
/// </summary>
public sealed record SituationDefinition : IDescriptiveValue
{
    public static string DisplayName => "状態ID";

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
    private SituationDefinition(string scriptValue, string shortDescription)
    {
        ScriptValue = scriptValue;
        ShortDescription = shortDescription;
    }

    public static IReadOnlyList<SituationDefinition> Rai7List { get; } =
    [
        new("1", "覇王種子"),
        new("2", "仕官中"),
        new("3", "死亡"),
        new("4", "海賊"),
        new("5", "在野・放浪中"),
        new("6", "仮死"),
        new("7", "村娘"),
        new("8", "捕虜"),
        new("9", "修行中"),
        new("10", "アシスタント"),
        new("13", "プレイヤー部下プレー用種子"),
        new("15", "精母"),
        new("16", "歌娘"),
        new("18", "平民"),
        new("19", "大衆大奥"),
        new("99", "データ未設定"),
    ];

    public static IReadOnlyList<SituationDefinition> Rai8List { get; } =
    [
        new("1", "覇王種子"),
        new("2", "仕官中"),
        new("3", "死亡"),
        new("4", "海賊"),
        new("5", "在野・放浪中"),
        new("6", "仮死"),
        new("7", "村娘"),
        new("8", "捕虜"),
        new("9", "修行中"),
        new("10", "アシスタント"),
        new("13", "プレイヤー部下プレー用種子"),
        new("15", "精母"),
        //new("16", "歌娘"),
        new("18", "平民"),
        new("19", "大衆大奥"),
        new("20", "プレイヤーの部下"),
        new("21", "大奥取締役"),
        new("22", "収容所所長"),
        new("26", "商人"),
        new("27", "商人奴隷"),
        new("28", "商人奴隷"),
        new("31", "潜入中諜報員"),
        new("99", "データ未設定"),
    ];
}