namespace RaizinLanguageServer.Models.Scripts.ParameterNodes;

/// <summary>
/// パラメータの型を表します。
/// </summary>
public enum ParameterType
{
    /// <summary>人物ID</summary>
    PersonId,
    /// <summary>惑星ID</summary>
    PlanetId,
    /// <summary>主星ID</summary>
    PrimaryPlanetId,
    /// <summary>恒星ID</summary>
    StarId,
    /// <summary>旗艦/艦種ID</summary>
    ShipId,

    /// <summary>なにかしらのステータスID</summary>
    AnyStatusId,
    /// <summary>人物ステータスID</summary>
    PersonStatusId,
    /// <summary>勢力ステータスID</summary>
    PowerStatusId,
    /// <summary>惑星ステータスID</summary>
    PlanetStatusId,

    /// <summary>人物状態ID</summary>
    SituationId,
    /// <summary>役職ID</summary>
    JobId,

    /// <summary>アイテム名</summary>
    ItemName, //done
    /// <summary>スキル名</summary>
    SkillName, //done

    /// <summary>勲功</summary>
    Honor,
    /// <summary>年齢</summary>
    Age,
    /// <summary>妊娠月数</summary>
    PregnancyMonth,
    /// <summary>年</summary>
    Year,
    /// <summary>月</summary>
    Month,
    /// <summary>イベントフラグ番号</summary>
    EveFlgNo,
    /// <summary>最大値</summary>
    Max,
    /// <summary>整数値</summary>
    Int,

    /// <summary>選択</summary>
    Choice,

    /// <summary>台詞</summary>
    Speech,
    /// <summary>ラベル</summary>
    Label,
    /// <summary>スクリプトファイル名</summary>
    ScriptFile,
    /// <summary>スクリプト以外のファイル名</summary>
    File,
    /// <summary>不明</summary>
    Unknown
}