using System;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Commands;

/// <summary>
/// パラメータ定義に使う型を表します。
/// </summary>
public enum ParameterDefinitionType
{
    /// <summary>人物ID</summary>
    PersonId, //done
    /// <summary>惑星ID</summary>
    PlanetId, //done
    /// <summary>主星ID</summary>
    PrimaryPlanetId, //done
    /// <summary>恒星ID</summary>
    StarId, //done
    /// <summary>旗艦/艦種ID</summary>
    ShipId, //done

    /// <summary>人物ステータスID</summary>
    PersonStatusId, //done
    /// <summary>惑星ステータス</summary>
    PlanetStatusId, // done

    /// <summary>人物状態ID</summary>
    SituationId, //done
    /// <summary>役職ID</summary>
    JobId, //done

    /// <summary>アイテム名</summary>
    ItemName, //done
    /// <summary>スキル名</summary>
    SkillName, //done

    /// <summary>勲功</summary>
    Honor, // done
    /// <summary>年齢</summary>
    Age,
    /// <summary>妊娠月数</summary>
    PregnancyMonth,
    /// <summary>年</summary>
    Year, // done
    /// <summary>月</summary>
    Month, // done
    /// <summary>イベントフラグ番号</summary>
    EveFlgNo, // done
    /// <summary>最大値</summary>
    Max,
    /// <summary>整数値</summary>
    Int, // done

    /// <summary>選択</summary>
    Choice, // done

    /// <summary>台詞</summary>
    Speech, // done
    /// <summary>ラベル</summary>
    Label, //done
    /// <summary>スクリプトファイル名</summary>
    ScriptFile, // done
    /// <summary>スクリプト以外のファイル名</summary>
    File, // done
    /// <summary>未指定</summary>
    Any, // done

    /// <summary>判定値</summary>
    PredicateValue, //done
    /// <summary>判定式</summary>
    Predicate, // done
    /// <summary>get_mpid パラメータ</summary>
    GetMpid,
    /// <summary>比較式</summary>
    Compare,

    /// <summary>1つ前のパラメータに指定されているステータスのタイプを参照する</summary>
    ReferPrevStatusId,
}

public static class ParameterDefinitionTypeExtensions
{
    /// <summary>
    /// 対応する <see cref="ParameterType"/> を取得します。
    /// </summary>
    public static ParameterType GetExpectedType(this ParameterDefinitionType paramDefType)
    {
        return paramDefType switch
        {
            ParameterDefinitionType.PersonId => ParameterType.PersonId,
            ParameterDefinitionType.PlanetId => ParameterType.PlanetId,
            ParameterDefinitionType.PrimaryPlanetId => ParameterType.PrimaryPlanetId,
            ParameterDefinitionType.StarId => ParameterType.StarId,
            ParameterDefinitionType.ShipId => ParameterType.ShipId,
            ParameterDefinitionType.PersonStatusId => ParameterType.PersonStatusId,
            ParameterDefinitionType.PlanetStatusId => ParameterType.PlanetStatusId,
            ParameterDefinitionType.SituationId => ParameterType.SituationId,
            ParameterDefinitionType.JobId => ParameterType.JobId,
            ParameterDefinitionType.ItemName => ParameterType.ItemName,
            ParameterDefinitionType.SkillName => ParameterType.SkillName,
            ParameterDefinitionType.Honor => ParameterType.Honor,
            ParameterDefinitionType.Age => ParameterType.Age,
            ParameterDefinitionType.PregnancyMonth => ParameterType.PregnancyMonth,
            ParameterDefinitionType.Year => ParameterType.Year,
            ParameterDefinitionType.Month => ParameterType.Month,
            ParameterDefinitionType.EveFlgNo => ParameterType.EveFlgNo,
            ParameterDefinitionType.Max => ParameterType.Max,
            ParameterDefinitionType.Int => ParameterType.Int,
            ParameterDefinitionType.Choice => ParameterType.Choice,
            ParameterDefinitionType.Speech => ParameterType.Speech,
            ParameterDefinitionType.Label => ParameterType.Label,
            ParameterDefinitionType.ScriptFile => ParameterType.ScriptFile,
            ParameterDefinitionType.File => ParameterType.File,
            ParameterDefinitionType.Any => ParameterType.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(paramDefType), paramDefType, null)
        };
    }
}