using System.Collections.Generic;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Statuses;

/// <summary>
/// 惑星ステータスIDを表します
/// </summary>
public sealed class PlanetStatus(string name, ParameterType type) : Status(name, type)
{
    public static string DisplayName => "惑星ステータス";

    public static IReadOnlyList<PlanetStatus> Rai7List { get; } =
    [
        new("領王", ParameterType.PersonId),
        new("領主", ParameterType.PersonId),
        new("首都", ParameterType.PlanetId),
        new("人口", ParameterType.Int),
        new("民忠", ParameterType.Int),
        new("資金", ParameterType.Int),
        new("世論", ParameterType.Int),
        new("座標X", ParameterType.Int),
        new("座標Y", ParameterType.Int),
        new("出現", ParameterType.Int),
        new("タイプ", ParameterType.Int),
        new("陸戦数", ParameterType.Int),
        new("攻撃目標", ParameterType.PlanetId),
        new("惑星汎用", ParameterType.Int),
        new("覇王", ParameterType.PersonId),
        new("NON", ParameterType.Unknown),
    ];

    public static IReadOnlyList<PlanetStatus> Rai8List { get; } =
    [
        new("主星", ParameterType.PrimaryPlanetId),
        new("惑星覇王", ParameterType.PersonId),
        new("衛星レベル", ParameterType.Int),
        new("惑星タイプ", ParameterType.Int),
        new("NON", ParameterType.Unknown),
    ];
}