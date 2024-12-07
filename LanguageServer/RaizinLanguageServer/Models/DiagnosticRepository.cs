using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Statuses;
using RaizinLanguageServer.Utilities;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RaizinLanguageServer.Models;

public class DiagnosticRepository
{
    /// <summary>
    /// 指定位置に対応する診断情報を取得します。
    /// </summary>
    public Diagnostic Get(DiagnosticCode def, int lineNumber, int startCharacter, int endCharacter)
    {
        return new Diagnostic
        {
            Range = new Range(lineNumber, startCharacter, lineNumber, endCharacter),
            Message = def.Message,
            Severity = def.Severity,
            Tags = def.Tags,
            Source = "raizin-vscode",
        };
    }

    /// <inheritdoc cref="Get(DiagnosticCode, int, int, int)"/>
    public Diagnostic Get(DiagnosticCode def, LineNode lineNode, AbsoluteRange range)
    {
        if (range.Start < lineNode.LineRange.Start) ExceptionHelper.ThrowArgumentOutOfRangeException(nameof(range));
        if (lineNode.LineRange.End < range.End) ExceptionHelper.ThrowArgumentOutOfRangeException(nameof(range));

        return new Diagnostic
        {
            Range = new Range
            {
                Start = new(lineNode.LineNumber, range.Start - lineNode.LineRange.Start),
                End = new(lineNode.LineNumber, range.End - lineNode.LineRange.Start),
            },
            Message = def.Message,
            Severity = def.Severity,
            Tags = def.Tags,
            Source = "raizin-vscode",
        };
    }

    /// <inheritdoc cref="Get(DiagnosticCode, int, int, int)"/>
    public Diagnostic Get(DiagnosticCode def, LineNode lineNode)
    {
        return new Diagnostic
        {
            Range = new Range(
                lineNode.LineNumber,
                lineNode.LineRange.Start - lineNode.LineRange.Start,
                lineNode.LineNumber,
                lineNode.LineRange.End - lineNode.LineRange.Start),
            Message = def.Message,
            Severity = def.Severity,
            Tags = def.Tags,
            Source = "raizin-vscode",
        };
    }

    private static readonly Container<DiagnosticTag> UnnecessaryTags = new(DiagnosticTag.Unnecessary);

    public static readonly DiagnosticCode WhitespaceAtEndOfLine = new(DiagnosticSeverity.Error, "行末に空白文字があります");
    public static readonly DiagnosticCode SyntaxError = new(DiagnosticSeverity.Error, "構文エラーです");
    public static readonly DiagnosticCode UnknownCommandName = new(DiagnosticSeverity.Information, "不明なコマンド名です");
    public static readonly DiagnosticCode TooManyParameters = new(DiagnosticSeverity.Information, "パラメータが多すぎます", UnnecessaryTags);
    public static readonly DiagnosticCode RequiredParameterMissing = new(DiagnosticSeverity.Warning, "必須パラメータは省略できません");
    public static readonly DiagnosticCode WhitespaceInParameter = new(DiagnosticSeverity.Error, "パラメータの前後に空白文字があります");
    public static readonly DiagnosticCode UnknownParameterValue = new(DiagnosticSeverity.Warning, "不明なパラメータ値です");
    public static readonly DiagnosticCode UnknownPersonId = new(DiagnosticSeverity.Warning, $"不明な{PersonDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownPlanetId = new(DiagnosticSeverity.Warning, $"不明な{PlanetDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownStarId = new(DiagnosticSeverity.Warning, $"不明な{StarDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownShipId = new(DiagnosticSeverity.Warning, $"不明な{ShipDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownSituationId = new(DiagnosticSeverity.Warning, $"不明な{SituationDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownJobId = new(DiagnosticSeverity.Warning, $"不明な{JobDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownItemId = new(DiagnosticSeverity.Warning, $"不明な{ItemDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownSkillId = new(DiagnosticSeverity.Warning, $"不明な{SkillDefinition.DisplayName}です");
    public static readonly DiagnosticCode UnknownPersonStatus = new(DiagnosticSeverity.Warning, $"不明な{PersonStatus.DisplayName}です");
    public static readonly DiagnosticCode UnknownPlanetStatus = new(DiagnosticSeverity.Warning, $"不明な{PlanetStatus.DisplayName}です");
    public static readonly DiagnosticCode UnknownPowerStatus = new(DiagnosticSeverity.Warning, $"不明な{PowerStatus.DisplayName}です");
    public static readonly DiagnosticCode UnknownStatusName = new(DiagnosticSeverity.Warning, "不明なステータス名です");
    public static readonly DiagnosticCode EventFlagOutOfRange = new(DiagnosticSeverity.Warning, $"イベントフラグ番号は{Diagnostics.EventFlagMin}以上{Diagnostics.EventFlagMax}です");
    public static readonly DiagnosticCode EmptyLabelName = new(DiagnosticSeverity.Error, "ラベル名が空です");
    public static readonly DiagnosticCode TooLongLabelName = new(DiagnosticSeverity.Warning, "ラベル名は20文字以内にしてください");
    public static readonly DiagnosticCode LabelMustStartWithAsterisk = new(DiagnosticSeverity.Error, "ラベル名は * で始まる必要があります");
    public static readonly DiagnosticCode TargetLabelNotFound = new(DiagnosticSeverity.Error, "対象ラベルが見つかりません");
    public static readonly DiagnosticCode ScriptFileNotFound = new(DiagnosticSeverity.Error, "ファイルが見つかりません");
}

public sealed record DiagnosticCode(DiagnosticSeverity Severity, string Message, Container<DiagnosticTag>? Tags = null);