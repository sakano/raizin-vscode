using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Definitions;

/// <summary>
/// 変数の定義を表します。
/// </summary>
public sealed partial class VariableDefinition : IDescriptiveValue
{
    /// <summary>
    /// 変数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 変数名に一致する正規表現
    /// </summary>
    public Regex? NameRegex { get; private init; }

    /// <summary>
    /// 変数名が正規表現であるか
    /// </summary>
    [MemberNotNullWhen(true, nameof(NameRegex))]
    public bool IsNameRegex => NameRegex is not null;

    /// <summary>
    /// 変数名の中で指定された値を取得する
    /// </summary>
    public ReadOnlySpan<char> GetSpecifiedValue(ReadOnlySpan<char> text) => text.Slice(PrefixLength);

    /// <summary>
    /// 変数に入っている値の型
    /// </summary>
    public ParameterType Type { get; }

    public ParameterType SpecifiedType { get; }

    public int PrefixLength { get; }

    /// <inheritdoc/>
    public string ScriptValue => Name;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public string? ShortDescription => null;

    /// <inheritdoc/>
    public string Description { get; }


    /// <summary>
    /// コンストラクタ
    /// </summary>
    private VariableDefinition(string name, string description, ParameterType type, Regex? nameRegex = null, ParameterType specifiedType = ParameterType.Unknown, int prefixLength = 0)
    {
        Debug.Assert((nameRegex is null && prefixLength == 0) || (nameRegex is not null && prefixLength > 0));

        Name = name;
        Description = description;
        Type = type;
        NameRegex = nameRegex;
        SpecifiedType = specifiedType;
        PrefixLength = prefixLength;
    }

    public static readonly IReadOnlyList<VariableDefinition> List =
    [
        new("player", "プレイヤーの人物ID", ParameterType.PersonId),
        new("pval", "直前に実行したコマンドの実行結果", ParameterType.Unknown),
        new("pid1", "人物ID専用変数1", ParameterType.PersonId),
        new("pid2", "人物ID専用変数2", ParameterType.PersonId),
        new("pid3", "人物ID専用変数3", ParameterType.PersonId),
        new("pid4", "人物ID専用変数4", ParameterType.PersonId),
        new("poid_player", "プレイヤーの覇王の人物ID", ParameterType.PersonId),
        new("poid_pid1", "pid1 の覇王の人物ID", ParameterType.PersonId),
        new("poid_pid2", "pid2 の覇王の人物ID", ParameterType.PersonId),
        new("poid_pid3", "pid3 の覇王の人物ID", ParameterType.PersonId),
        new("poid_pid4", "pid4 の覇王の人物ID", ParameterType.PersonId),
        new("yome_player", "プレイヤーの嫁の人物ID", ParameterType.PersonId),
        new("yome_pid1", "pid1 の嫁の人物ID", ParameterType.PersonId),
        new("yome_pid2", "pid2 の嫁の人物ID", ParameterType.PersonId),
        new("yome_pid3", "pid3 の嫁の人物ID", ParameterType.PersonId),
        new("yome_pid4", "pid4 の嫁の人物ID", ParameterType.PersonId),
        new("poidXXXX", "指定した人物の覇王の人物ID", ParameterType.PersonId, PoidRegex(), ParameterType.PersonId, 4),
        new("yomeXXXX", "指定した人物の嫁の人物ID", ParameterType.PersonId, YomeRegex(), ParameterType.PersonId, 4),
        new("eveflgXXX", "イベントフラグ", ParameterType.Unknown, EveFlgRegex(), ParameterType.EveFlgNo, 6),
        new("soidXX", "指定した惑星を支配する覇王の人物ID", ParameterType.PersonId, SoidRegex(), ParameterType.PlanetId, 4),
        new("cap_player", "プレイヤーが所属する勢力の首都の惑星ID", ParameterType.PlanetId),
        new("cap_pid1", "pid1 が所属する勢力の首都の惑星ID", ParameterType.PlanetId),
        new("cap_pid2", "pid2 が所属する勢力の首都の惑星ID", ParameterType.PlanetId),
        new("cap_pid3", "pid3 が所属する勢力の首都の惑星ID", ParameterType.PlanetId),
        new("cap_pid4", "pid4 が所属する勢力の首都の惑星ID", ParameterType.PlanetId),
        new("capXXXX", "指定した人物が所属する勢力の首都の惑星ID", ParameterType.PlanetId, CapRegex(), ParameterType.PersonId, 3),
        new("OLD", "オルドが可能になる年齢", ParameterType.Age),
        new("TANTO", "担当配属が可能になる年齢", ParameterType.Age),
        new("TANJO", "子供が産まれる月数", ParameterType.PregnancyMonth),
        new("PTANJO", "子供が生まれる月数-1", ParameterType.PregnancyMonth),
        new("PMAX", "人物の最大数", ParameterType.Max),
        new("SMAX", "惑星の最大数", ParameterType.Max),
    ];

    [GeneratedRegex(@"poid\d+", RegexOptions.Compiled)]
    private static partial Regex PoidRegex();

    [GeneratedRegex(@"yome\d+", RegexOptions.Compiled)]
    private static partial Regex YomeRegex();

    [GeneratedRegex(@"eveflg\d+", RegexOptions.Compiled)]
    private static partial Regex EveFlgRegex();

    [GeneratedRegex(@"soid\d+", RegexOptions.Compiled)]
    private static partial Regex SoidRegex();

    [GeneratedRegex(@"cap\d+", RegexOptions.Compiled)]
    private static partial Regex CapRegex();
}