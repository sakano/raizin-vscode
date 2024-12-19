using System.Collections.Generic;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Statuses;

/// <summary>
/// 人物のステータスIDを表します
/// </summary>
public sealed class PersonStatus(string name, string id, ParameterType type) : Status(name, type)
{
    public static string DisplayName => "人物ステータス";

    /// <summary>
    /// ステータスID
    /// </summary>
    public string Id { get; } = id;

    public static IReadOnlyList<PersonStatus> Rai7List { get; } =
    [
        new("状態", "0", ParameterType.SituationId)
        {
            MultipleValueEnabled = true,
            Description = "1: 覇王種子  \n2: 仕官中  \n3: 死亡  \n4: 海賊  \n5: 在野・放浪中  \n6: 仮死  \n7: 村娘  \n8: 捕虜  \n9: 修行中  \n10: アシスタント  \n13: プレイヤー部下プレー用種子  \n15: 精母  \n16: 歌娘  \n18: 平民  \n19: 大衆大奥",
        },
        new("忠誠", "1", ParameterType.Int),
        new("年齢", "2", ParameterType.Age),
        new("性別", "3", ParameterType.Choice)
        {
            Choices = [new("1", "男"), new("2", "女")],
            Description = "1: 男  \n2: 女",
        },
        new("勲功", "4", ParameterType.Honor)
        {
            Description = "13000: 覇王  \n12750: 上皇  \n12700: 太后  \n12600: 婿  \n12300: 王子  \n12200: 妃  \n12100: 姫  \n12080: 王族  \n11000: 将軍  \n10000: 分将  \n9000: 分１位  \n8000: 分２位  \n7000: 分３位  \n6000: 分４位  \n5000: 分５位  \n4000: 分６位  \n3000: 分７位  \n2000: 分８位",
        },
        new("旧勲功", "27", ParameterType.Honor)
        {
            Description = "13000: 覇王  \n12750: 上皇  \n12700: 太后  \n12600: 婿  \n12300: 王子  \n12200: 妃  \n12100: 姫  \n12080: 王族  \n11000: 将軍  \n10000: 分将  \n9000: 分１位  \n8000: 分２位  \n7000: 分３位  \n6000: 分４位  \n5000: 分５位  \n4000: 分６位  \n3000: 分７位  \n2000: 分８位",
        },
        new("知力", "5", ParameterType.Int),
        new("智謀", "5", ParameterType.Int),
        new("攻撃", "6", ParameterType.Int),
        new("防御", "7", ParameterType.Int),
        new("格闘", "8", ParameterType.Int),
        new("外交", "9", ParameterType.Int),
        new("交渉", "9", ParameterType.Int),
        new("奇策", "10", ParameterType.Int),
        new("友好", "12", ParameterType.Int)
        {
            Choices = [new("25", "嫌悪"), new("75", "信頼"), new("110", "性奴隷")],
            Description = "0-25: 嫌悪  \n25-74: 普通  \n75-100: 信頼  \n110: 性奴隷",
        },
        new("親密", "12", ParameterType.Int)
        {
            Choices = [new("25", "嫌悪"), new("75", "信頼"), new("110", "性奴隷")],
            Description = "0-25: 嫌悪  \n25-74: 普通  \n75-100: 信頼  \n110: 性奴隷",
        },
        new("覇王", "21", ParameterType.PersonId),
        new("旧覇王", "26", ParameterType.PersonId),
        new("派閥", "20", ParameterType.PersonId),
        new("部下", "31", ParameterType.PersonId),
        new("仕官年数", "23", ParameterType.Int),
        new("役職", "13", ParameterType.JobId)
        {
            Description = "2: 艦隊総司令  \n3: 妾  \n4: 特命担当  \n5: 諜報担当  \n7: 艦隊司令  \n9: 技術担当  \n16: 人事担当  \n17: 経済担当  \n18: 防衛担当  \n19: 補給担当",
        },
        new("信念", "15", ParameterType.Int),
        new("性格", "11", ParameterType.Choice)
        {
            Choices = [new("11", "トラ"), new("12", "サル"), new("13", "コアラ"), new("14", "オオカミ"), new("21", "ペガサス"), new("22", "ゾウ"), new("23", "ライオン"), new("24", "チータ"), new("31", "黒ヒョウ"), new("32", "ヒツジ"), new("41", "コジカ"), new("42", "タヌキ")],
            Description = "11: トラ  \n12: サル  \n13: コアラ  \n14: オオカミ  \n21: ペガサス  \n22: ゾウ  \n23: ライオン  \n24: チータ  \n31: 黒ヒョウ  \n32: ヒツジ  \n41: コジカ  \n42: タヌキ",
        },
        new("オルド性格", "18", ParameterType.Choice)
        {
            Choices = [new("0", "きまぐれ型"), new("1", "忠誠度型"), new("2", "君主の経験依存型"), new("3", "絶対させない型"), new("4", "欲情型"), new("5", "極端忠誠型"),],
            Description = "0: きまぐれ型  \n1: 忠誠度型  \n2: 君主の経験依存型  \n3: 絶対させない型  \n4: 欲情型  \n5: 極端忠誠型",
        },
        new("オルド回数", "-99", ParameterType.Int),
        new("初膜", "14", ParameterType.Choice)
        {
            Choices = [new("0", "なし"), new("1", "あり")],
            Description = "0: なし  \n1: あり",
        },
        new("懐妊", "16", ParameterType.PregnancyMonth)
        {
            Description = "0なら妊娠していない。1以上なら妊娠何ヶ月目かを表す",
        },
        new("不妊", "22", ParameterType.Choice)
        {
            Choices = [new("0", "不妊でない"), new("1", "不妊")],
            Description = "0: 不妊でない  \n1: 不妊",
        },
        new("婚姻", "17", ParameterType.PersonId)
        {
            Description = "結婚相手の人物ID。0なら未婚",
        },
        new("妻", "17", ParameterType.PersonId)
        {
            Description = "結婚相手の人物ID。0なら未婚",
        },
        new("父", "24", ParameterType.PersonId),
        new("元父", "25", ParameterType.PersonId),
        new("種父", "-99", ParameterType.PersonId),
        new("母", "-99", ParameterType.PersonId),
        new("面会", "28", ParameterType.Int),
        new("弱み", "30", ParameterType.Int),
        new("特殊", "19", ParameterType.Choice)
        {
            Choices = [new("0", "通常"), new("1", "バイオノイド"), new("2", "重要人物")],
            Description = "0: 通常  \n1: バイオノイド  \n2: 重要人物",
        },
        new("人物汎用", "32", ParameterType.Int),
        new("負傷", "33", ParameterType.Choice)
        {
            Choices = [new("0", "負傷なし"), new("1", "軽傷"), new("2", "重傷")],
            Description = "0: 負傷なし  \n1: 軽傷  \n2: 重傷",
        },
        new("親仇", "-99", ParameterType.PersonId),
        new("夫仇", "-99", ParameterType.PersonId),
        new("子仇", "-99", ParameterType.PersonId),
        new("子供数", "-99", ParameterType.Int),
        new("子供男", "-99", ParameterType.Int),
        new("子供女", "-99", ParameterType.Int),
        new("交際中", "-99", ParameterType.Int),
        new("旗艦", "-99", ParameterType.ShipId),
        new("NON", "-1", ParameterType.Unknown),
    ];

    public static IReadOnlyList<PersonStatus> Rai8List { get; } =
    [
        new("状態", "0", ParameterType.SituationId)
        {
            MultipleValueEnabled = true,
            Description = "1: 覇王種子  \n2: 仕官中  \n3: 死亡  \n4: 海賊  \n5: 在野・放浪中  \n6: 仮死  \n7: 村娘  \n8: 捕虜  \n9: 修行中  \n10: アシスタント  \n13: プレイヤー部下プレー用種子  \n15: 精母  \n18: 平民  \n19: 大衆大奥  \n20: プレイヤーの部下  \n21: 大奥取締役  \n22: 収容所所長  \n26: 商人  \n27: 商人奴隷  \n28: 商人奴隷  \n31: 潜入中諜報員  \n",
        },
        new("年齢", "2", ParameterType.Age),
        new("性別", "3", ParameterType.Choice)
        {
            Choices = [new("1", "男"), new("2", "女")],
            Description = "1: 男  \n2: 女",
        },
        new("勲功", "4", ParameterType.Honor)
        {
            Description = "13000: 覇王  \n12750: 上皇  \n12700: 太后  \n12600: 婿  \n12300: 王子  \n12200: 妃  \n12100: 姫  \n12080: 王族  \n11000: 将軍  \n10000: 分将  \n9000: 分１位  \n8000: 分２位  \n7000: 分３位  \n6000: 分４位  \n5000: 分５位  \n4000: 分６位  \n3000: 分７位  \n2000: 分８位",
        },
        new("旧勲功", "-99", ParameterType.Honor)
        {
            Description = "13000: 覇王  \n12750: 上皇  \n12700: 太后  \n12600: 婿  \n12300: 王子  \n12200: 妃  \n12100: 姫  \n12080: 王族  \n11000: 将軍  \n10000: 分将  \n9000: 分１位  \n8000: 分２位  \n7000: 分３位  \n6000: 分４位  \n5000: 分５位  \n4000: 分６位  \n3000: 分７位  \n2000: 分８位",
        },
        new("戦術", "6", ParameterType.Int),
        new("知略", "5", ParameterType.Int),
        new("交渉", "9", ParameterType.Int),
        new("内政", "7", ParameterType.Int),
        new("技術", "10", ParameterType.Int),
        new("格闘", "8", ParameterType.Int),
        new("親密", "12", ParameterType.Int)
        {
            Choices = [new("35", "嫌悪"), new("75", "信頼"), new("110", "性奴隷")],
            Description = "0-35: 嫌悪  \n35-74: 普通  \n75-100: 信頼  \n110: 性奴隷",
        },
        new("覇王", "21", ParameterType.PersonId),
        new("旧覇王", "26", ParameterType.PersonId),
        new("担当", "13", ParameterType.JobId)
        {
            Description = "2: 艦隊総司令  \n3: 妾  \n4: 特命担当  \n5: 諜報担当  \n7: 艦隊司令  \n9: 技術担当  \n16: 人事担当  \n17: 経済担当  \n18: 防衛担当  \n19: 補給担当",
        },
        new("性格", "18", ParameterType.Choice)
        {
            Choices = [new("11", "トラ"), new("12", "サル"), new("13", "コアラ"), new("14", "オオカミ"), new("21", "ペガサス"), new("22", "ゾウ"), new("23", "ライオン"), new("24", "チータ"), new("31", "黒ヒョウ"), new("32", "ヒツジ"), new("41", "コジカ"), new("42", "タヌキ")],
            Description = "11: トラ  \n12: サル  \n13: コアラ  \n14: オオカミ  \n21: ペガサス  \n22: ゾウ  \n23: ライオン  \n24: チータ  \n31: 黒ヒョウ  \n32: ヒツジ  \n41: コジカ  \n42: タヌキ",
        },
        new("オルド性格", "-99", ParameterType.Choice)
        {
            Choices = [new("0", "きまぐれ型"), new("1", "忠誠度型"), new("2", "君主の経験依存型"), new("3", "絶対させない型"), new("4", "欲情型"), new("5", "極端忠誠型")],
            Description = "0: きまぐれ型  \n1: 忠誠度型  \n2: 君主の経験依存型  \n3: 絶対させない型  \n4: 欲情型  \n5: 極端忠誠型",
        },
        new("オルド回数", "-99", ParameterType.Int),
        new("初膜", "14", ParameterType.Choice)
        {
            Choices = [new("0", "なし"), new("1", "あり")],
            Description = "0: なし  \n1: あり",
        },
        new("懐妊", "16", ParameterType.PregnancyMonth)
        {
            Description = "0なら妊娠していない。1以上なら妊娠何ヶ月目かを表す",
        },
        new("不妊", "-99", ParameterType.Choice)
        {
            Choices = [new("0", "不妊でない"), new("1", "不妊")],
            Description = "0: 不妊でない  \n1: 不妊",
        },
        new("婚姻", "17", ParameterType.PersonId)
        {
            Description = "結婚相手の人物ID。0なら未婚",
        },
        new("父", "24", ParameterType.PersonId),
        new("種父", "-99", ParameterType.PersonId),
        new("母", "-99", ParameterType.PersonId),
        new("面会", "28", ParameterType.Int),
        new("特殊", "19", ParameterType.Choice)
        {
            Choices = [new("0", "通常"), new("1", "バイオノイド"), new("2", "重要人物")],
            Description = "0: 通常  \n1: バイオノイド  \n2: 重要人物",
        },
        new("寿命", "28", ParameterType.Int),
        new("夜這", "28", ParameterType.Choice) { Choices = [new("0", ""), new("1", "")] },
        new("人物汎用", "-99", ParameterType.Int),
        new("子供数", "-99", ParameterType.Int),
        new("子供男", "-99", ParameterType.Int),
        new("子供女", "-99", ParameterType.Int),
        new("特種B", "-99", ParameterType.Int),
        new("特種C", "-99", ParameterType.Int),
        new("NON", "-1", ParameterType.Unknown),
    ];
}