using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// psonN.csv などの定義情報を提供するインターフェース
/// </summary>
public interface IDefinitionHolder
{
    /// <summary>
    /// ID 定義情報が変更された際に発生するイベント
    /// </summary>
    public event Action<IDefinitionHolder> OnDefinitionChanged;

    /// <summary>
    /// 人物定義リスト
    /// </summary>
    public IReadOnlyList<PersonDefinition> PersonDefinitionList { get; }

    /// <summary>
    /// 指定された人物IDの人物定義を取得します。
    /// </summary>
    public bool TryGetPersonDefinition(ReadOnlySpan<char> personId, [NotNullWhen(true)] out PersonDefinition? personDefinition);

    /// <summary>
    /// 惑星定義リスト
    /// </summary>
    public IReadOnlyList<PlanetDefinition> PlanetDefinitionList { get; }

    /// <summary>
    /// 指定された惑星IDの惑星定義を取得します。
    /// </summary>
    public bool TryGetPlanetDefinition(ReadOnlySpan<char> planetId, [NotNullWhen(true)] out PlanetDefinition? planetDefinition);
    
    /// <summary>
    /// 恒星ID定義リスト
    /// </summary>
    public IReadOnlyList<StarDefinition> StarDefinitionList { get; }

    /// <summary>
    /// 恒星IDから恒星定義を取得します。
    /// </summary>
    bool TryGetStarDefinition(ReadOnlySpan<char> starId, [NotNullWhen(true)] out StarDefinition? starDefinition);

    /// <summary>
    /// アイテム定義リスト
    /// </summary>
    public IReadOnlyList<ItemDefinition> ItemDefinitionList { get; }

    /// <summary>
    /// 指定されたアイテム名のアイテム定義を取得します。
    /// </summary>
    public bool TryGetItemDefinition(ReadOnlySpan<char> itemName, [NotNullWhen(true)] out ItemDefinition? itemDefinition);

    /// <summary>
    /// スキル定義リスト
    /// </summary>
    public IReadOnlyList<SkillDefinition> SkillDefinitionsList { get; }

    /// <summary>
    /// 指定されたスキル名のスキル定義を取得します。
    /// </summary>
    public bool TryGetSkillDefinition(ReadOnlySpan<char> skillName, [NotNullWhen(true)] out SkillDefinition? skillDefinition);

    /// <summary>
    /// 艦種定義リスト
    /// </summary>
    public IReadOnlyList<ShipDefinition> ShipDefinitionsList { get; }

    /// <summary>
    /// 指定された艦種IDの艦種定義を取得します。
    /// </summary>
    public bool TryGetShipDefinition(ReadOnlySpan<char> shipId, [NotNullWhen(true)] out ShipDefinition? shipDefinition);

    /// <summary>
    /// 状態ID定義リスト
    /// </summary>
    public IReadOnlyList<SituationDefinition> SituationDefinitionList { get; }

    /// <summary>
    /// 状態IDから状態定義を取得します。
    /// </summary>
    bool TryGetSituationDefinition(ReadOnlySpan<char> situationId, [NotNullWhen(true)] out SituationDefinition? situationDefinition);

    /// <summary>
    /// 役職ID定義リスト
    /// </summary>
    public IReadOnlyList<JobDefinition> JobDefinitionList { get; }

    /// <summary>
    /// 役職IDから役職定義を取得します。
    /// </summary>
    bool TryGetJobDefinition(ReadOnlySpan<char> jobId, [NotNullWhen(true)] out JobDefinition? jobDefinition);

    /// <summary>
    /// 勲功ID定義リスト
    /// </summary>
    public IReadOnlyList<HonorDefinition> HonorDefinitionList { get; }
    
    /// <summary>
    /// 勲功値から勲功定義を取得します。
    /// </summary>
    public bool TryGetHonorDefinition(ReadOnlySpan<char> honor, [NotNullWhen(true)] out HonorDefinition? honorDefinition);

    /// <summary>
    /// 変数定義リスト
    /// </summary>
    public IReadOnlyList<VariableDefinition> VariableDefinitionList { get; }

    /// <summary>
    /// 変数名から変数定義を取得します。
    /// </summary>
    bool TryGetVariableDefinition(ReadOnlySpan<char> variableName, [NotNullWhen(true)] out VariableDefinition? variableDefinition);
}