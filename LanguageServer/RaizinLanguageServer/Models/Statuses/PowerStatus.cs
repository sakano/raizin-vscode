using System.Collections.Generic;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Statuses;

/// <summary>
/// 勢力ステータスIDを表します
/// </summary>
public sealed class PowerStatus(string name, ParameterType type) : Status(name, type)
{
    public static string DisplayName => "勢力ステータス";

    public static IReadOnlyList<PowerStatus> Rai7List { get; } =
    [
        new("資金", ParameterType.Int),
        new("世論", ParameterType.Int),
        new("攻撃目標", ParameterType.PlanetId),
        new("部下数", ParameterType.Int),
        new("派閥数", ParameterType.Int),
        new("リリース残数艦艇", ParameterType.Int),
        new("リリース残数艦載", ParameterType.Int),
        new("リリース残数陸戦", ParameterType.Int),
        new("兵器系列", ParameterType.Choice) { Choices = [new("1", "ANTEL"), new("2", "サイコプラス"), new("3", "KMD"), new("4", "ジパング")] },
    ];

    public static IReadOnlyList<PowerStatus> Rai8List { get; } =
    [
        new("部下数", ParameterType.Int),
        new("惑星数", ParameterType.Int),
        new("主星ID", ParameterType.PrimaryPlanetId),
    ];
}