using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using RaizinLanguageServer.Models.Statuses;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServer.Models;

/// <summary>
/// Raizin スクリプトの診断情報を提供します
/// </summary>
public class Diagnostics(ILogger logger, ITextDocumentLanguageServer server, DiagnosticRepository diagRepository, IDefinitionHolder definitionHolder, IRaizinConfiguration configuration, StatusRepository statusRepository)
{
    // ReSharper disable once UnusedMember.Local
    private ILogger Logger { get; } = logger;

    public void Diagnose(RaizinScript script)
    {
        var diagnostics = new List<Diagnostic>();

        // 各行を診断
        foreach (var lineNode in script.EnumerateLineNodes())
        {
            switch (lineNode)
            {
                case UnknownLineNode:
                    // 不明な行ノードはエラー
                    diagnostics.Add(diagRepository.Get(DiagnosticRepository.SyntaxError, lineNode));
                    break;
                case LabelLineNode labelLineNode:
                    DiagnoseLabelLine(diagnostics, script.RawText, labelLineNode);
                    break;
                case CommandLineNode commandLine:
                    DiagnoseCommandLine(diagnostics, script, commandLine);
                    break;
            }
        }

        server.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Uri = script.Uri,
            Diagnostics = diagnostics,
        });
    }

    /// <summary>
    /// ラベル行の診断を行います。
    /// </summary>
    private void DiagnoseLabelLine(List<Diagnostic> diagnostics, RawText rawText, LabelLineNode labelLine)
    {
        DiagnoseTrailingWhitespace(diagnostics, rawText, labelLine, labelLine.LabelRange, DiagnosticRepository.WhitespaceAtEndOfLine);

        if (labelLine.LabelNameRange.Length == 0)
        {
            // ラベル名が空
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.EmptyLabelName, labelLine, labelLine.LabelNameRange));
        }
        else if (labelLine.LabelNameRange.Length > 20)
        {
            // ラベル名が長すぎる
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.TooLongLabelName, labelLine, labelLine.LabelNameRange));
        }
    }

    /// <summary>
    /// コマンド行の診断を行います。
    /// </summary>
    private void DiagnoseCommandLine(List<Diagnostic> diagnostics, RaizinScript script, CommandLineNode commandLine)
    {
        if (commandLine.Definition is null)
        {
            // コマンド定義が見つからない
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.UnknownCommandName, commandLine, commandLine.CommandNameRange));
            return;
        }

        for (var paramIndex = 0; paramIndex < commandLine.Parameters.Count; paramIndex++)
        {
            var paramNode = commandLine.Parameters[paramIndex];

            if (paramIndex >= commandLine.Definition.Parameters.Count)
            {
                // パラメータが多すぎるときは残りのパラメータ部分をまとめてエラーにする
                diagnostics.Add(diagRepository.Get(DiagnosticRepository.TooManyParameters, commandLine, new AbsoluteRange(paramNode.Range.Start - 1, commandLine.LineRange.End)));
                break; // 残りのパラメータは診断しない
            }

            var paramDef = commandLine.Definition.Parameters[paramIndex];

            // パラメータが省略されている場合
            if (paramNode.Range.Length == 0)
            {
                if (paramDef.IsRequired)
                {
                    // 省略されているのが必須パラメータならエラー
                    diagnostics.Add(diagRepository.Get(DiagnosticRepository.RequiredParameterMissing, commandLine, paramNode.Range));
                }

                continue; // 以降の診断をスキップ
            }

            DiagnoseParameter(diagnostics, script, commandLine, paramNode, paramDef, true);
        }

        // 引数が足りない場合、残りのパラメータ定義もチェック
        for (var paramIndex = commandLine.Parameters.Count; paramIndex < commandLine.Definition.Parameters.Count; ++paramIndex)
        {
            if (commandLine.Definition.Parameters[paramIndex].IsRequired)
            {
                // 必須パラメータが省略されている
                diagnostics.Add(diagRepository.Get(DiagnosticRepository.RequiredParameterMissing, commandLine.LineNumber,
                    commandLine.LineRange.End - commandLine.LineRange.Start,
                    commandLine.LineRange.End - commandLine.LineRange.Start));
                break;
            }
        }

        // パラメータがない場合はパラメータの診断でチェックされていないので行末に空白がないか診断
        if (commandLine.Parameters.Count == 0)
        {
            DiagnoseTrailingWhitespace(diagnostics, script.RawText, commandLine, commandLine.LineRange, DiagnosticRepository.WhitespaceAtEndOfLine);
        }
    }

    private void DiagnoseParameter(List<Diagnostic> diagnostics, RaizinScript script, LineNode lineNode, ParameterNode paramNode, ParameterDefinition paramDef, bool choiceEnable)
    {
        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                DiagnoseParameter(diagnostics, script, lineNode, indirectNode.StatusParameter, paramDef, false);
                DiagnoseParameter(diagnostics, script, lineNode, indirectNode.IdParameter, paramDef, false);
                return;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                DiagnoseParameter(diagnostics, script, lineNode, getMpidNode.StatusParameter, paramDef, false);
                DiagnoseParameter(diagnostics, script, lineNode, getMpidNode.RightParameter, paramDef, choiceEnable);
                return;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                DiagnoseParameter(diagnostics, script, lineNode, predicateValueNode.ValueParameter, paramDef, choiceEnable);
                return;
            case PredicateParameterNode predicateNode:
                // 判定条件
                DiagnoseParameter(diagnostics, script, lineNode, predicateNode.LeftParameter, paramDef, choiceEnable);
                DiagnoseParameter(diagnostics, script, lineNode, predicateNode.RightParameter, paramDef, choiceEnable);
                return;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var childParam in multipleNode.Parameters) DiagnoseParameter(diagnostics, script, lineNode, childParam, paramDef, choiceEnable);
                return;
            case CompareParameterNode compareNode:
                // 比較式
                DiagnoseParameter(diagnostics, script, lineNode, compareNode.LeftParameter, paramDef, choiceEnable);
                DiagnoseParameter(diagnostics, script, lineNode, compareNode.RightParameter, paramDef, choiceEnable);
                return;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                if (choiceEnable && referStatusNode.ReferStatus is not null && referStatusNode.ReferStatus.TryGetChoiceValue(script.RawText.Slice(referStatusNode.Range), out _))
                {
                    return; // 選択肢に含まれている値が指定されていれば以降の診断をスキップ
                }

                DiagnoseParameter(diagnostics, script, lineNode, referStatusNode.ChildParameter, paramDef, choiceEnable);
                return;
            case VariableParameterNode variableNode:
                // 変数
                if (DiagnoseVariable(diagnostics, lineNode, script.RawText, variableNode)) return; // 問題があれば診断スキップ
                break; // このまま診断継続
        }

        // 選択肢に含まれている値が指定されていれば以降の診断をスキップ
        if (choiceEnable && paramDef.TryGetChoiceValue(script.RawText.Slice(paramNode.Range), out _)) return;

        DiagnoseWhitespace(diagnostics, script.RawText, lineNode, paramNode.Range, DiagnosticRepository.WhitespaceInParameter);


        // パラメータの種類ごとに診断
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
                DiagnoseEnumOrVariableParameter<PersonDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.PersonId, definitionHolder.TryGetPersonDefinition, DiagnosticRepository.UnknownPersonId);
                break;
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
                DiagnoseEnumOrVariableParameter<PlanetDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.PlanetId, definitionHolder.TryGetPlanetDefinition, DiagnosticRepository.UnknownPlanetId);
                break;
            case ParameterType.StarId:
                DiagnoseEnumOrVariableParameter<StarDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, definitionHolder.TryGetStarDefinition, DiagnosticRepository.UnknownStarId);
                break;
            case ParameterType.ShipId:
                DiagnoseEnumOrVariableParameter<ShipDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, definitionHolder.TryGetShipDefinition, DiagnosticRepository.UnknownShipId);
                break;
            case ParameterType.AnyStatusId:
                DiagnoseEnumOrVariableParameter<Status>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, statusRepository.TryGetAnyStatus, DiagnosticRepository.UnknownStatusName);
                break;
            case ParameterType.PersonStatusId:
                DiagnoseEnumOrVariableParameter<PersonStatus>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, statusRepository.TryGetPersonStatus, DiagnosticRepository.UnknownPersonStatus);
                break;
            case ParameterType.PowerStatusId:
                DiagnoseEnumParameter<PowerStatus>(diagnostics, lineNode, script.RawText, paramNode.Range, statusRepository.TryGetPowerStatus, DiagnosticRepository.UnknownPowerStatus);
                break;
            case ParameterType.PlanetStatusId:
                DiagnoseEnumParameter<PlanetStatus>(diagnostics, lineNode, script.RawText, paramNode.Range, statusRepository.TryGetPlanetStatus, DiagnosticRepository.UnknownPlanetStatus);
                break;
            case ParameterType.SituationId:
                DiagnoseEnumOrVariableParameter<SituationDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, definitionHolder.TryGetSituationDefinition, DiagnosticRepository.UnknownSituationId);
                break;
            case ParameterType.JobId:
                DiagnoseEnumOrVariableParameter<JobDefinition>(diagnostics, lineNode, script.RawText, paramNode, ParameterType.Unknown, definitionHolder.TryGetJobDefinition, DiagnosticRepository.UnknownJobId);
                break;
            case ParameterType.ItemName:
                DiagnoseEnumParameter<ItemDefinition>(diagnostics, lineNode, script.RawText, paramNode.Range, definitionHolder.TryGetItemDefinition, DiagnosticRepository.UnknownItemId);
                break;
            case ParameterType.SkillName:
                DiagnoseEnumParameter<SkillDefinition>(diagnostics, lineNode, script.RawText, paramNode.Range, definitionHolder.TryGetSkillDefinition, DiagnosticRepository.UnknownSkillId);
                break;
            case ParameterType.Honor:
            case ParameterType.Age:
            case ParameterType.PregnancyMonth:
            case ParameterType.Year:
            case ParameterType.Month:
            case ParameterType.Max:
            case ParameterType.Int:
                DiagnoseIntParameter(diagnostics, lineNode, script.RawText, paramNode);
                break;
            case ParameterType.Choice:
                DiagnoseEnumParameter<ChoiceValueDefinition>(diagnostics, lineNode, script.RawText, paramNode.Range, paramDef.TryGetChoiceValue, DiagnosticRepository.UnknownParameterValue);
                break;
            case ParameterType.EveFlgNo:
                DiagnoseEveFlgNoParameter(diagnostics, lineNode, script.RawText, paramNode);
                break;
            case ParameterType.Label:
                DiagnoseLabelParameter(diagnostics, lineNode, script, paramNode.Range);
                break;
            case ParameterType.ScriptFile:
                DiagnoseScriptFileParameter(diagnostics, lineNode, script, paramNode.Range);
                break;
        }
    }

    /// <summary>
    /// 変数を診断します。
    /// 問題がある場合は true を返します。
    /// </summary>
    private bool DiagnoseVariable(List<Diagnostic> diagnostics, LineNode lineNode, RawText rawText, VariableParameterNode variableNode)
    {
        if (!variableNode.Variable.IsNameRegex) return false;

        var specifiedValue = rawText.Slice(variableNode.Range).Slice(variableNode.Variable.PrefixLength).TrimStart('0');
        switch (variableNode.Variable.SpecifiedType)
        {
            case ParameterType.PersonId:
                if (!definitionHolder.TryGetPersonDefinition(specifiedValue, out _))
                {
                    diagnostics.Add(diagRepository.Get(DiagnosticRepository.UnknownPersonId, lineNode, new AbsoluteRange(variableNode.Range.Start + variableNode.Variable.PrefixLength, variableNode.Range.End)));
                    return true;
                }

                return false;
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
                if (!definitionHolder.TryGetPlanetDefinition(specifiedValue, out _))
                {
                    diagnostics.Add(diagRepository.Get(DiagnosticRepository.UnknownPlanetId, lineNode, new AbsoluteRange(variableNode.Range.Start + variableNode.Variable.PrefixLength, variableNode.Range.End)));
                    return true;
                }

                return false;

            case ParameterType.EveFlgNo:
                if (int.TryParse(specifiedValue, out var intValue) && !IsValidAsEveFlgNo(intValue))
                {
                    diagnostics.Add(diagRepository.Get(DiagnosticRepository.EventFlagOutOfRange, lineNode, new AbsoluteRange(variableNode.Range.Start + variableNode.Variable.PrefixLength, variableNode.Range.End)));
                    return true;
                }

                return false;
        }

        return false;
    }

    private delegate bool TryGetDelegate<in TValue, TDefinition>(TValue value, [NotNullWhen(true)] out TDefinition? definition) where TValue : allows ref struct;

    /// <summary>
    /// 列挙型パラメータの診断を行います。
    /// 指定された型の変数も許容します。
    /// </summary>
    private void DiagnoseEnumOrVariableParameter<T>(List<Diagnostic> diagnostics, LineNode lineNode, RawText rawText, ParameterNode paramNode, ParameterType expectedType, TryGetDelegate<ReadOnlySpan<char>, T> tryGet, DiagnosticCode code)
    {
        if (paramNode is VariableParameterNode variableNode)
        {
            if (variableNode.Variable.Type == expectedType) return;
            if (variableNode.Variable.Type == ParameterType.Unknown) return;
        }

        var paramText = rawText.Slice(paramNode.Range);
        if (tryGet.Invoke(paramText, out _)) return;
        diagnostics.Add(diagRepository.Get(code, lineNode, paramNode.Range));
    }

    /// <summary>
    /// 列挙型パラメータの診断を行います。
    /// 変数は許容しません。
    /// </summary>
    private void DiagnoseEnumParameter<T>(List<Diagnostic> diagnostics, LineNode lineNode, RawText rawText, AbsoluteRange range, TryGetDelegate<ReadOnlySpan<char>, T> tryGet, DiagnosticCode code)
    {
        var paramText = rawText.Slice(range);
        if (tryGet.Invoke(paramText, out _)) return;
        diagnostics.Add(diagRepository.Get(code, lineNode, range));
    }

    /// <summary>
    /// イベントフラグ番号パラメータの診断を行います。
    /// </summary>
    private void DiagnoseEveFlgNoParameter(List<Diagnostic> diagnostics, LineNode lineNode, RawText rawText, ParameterNode paramNode)
    {
        if (paramNode is VariableParameterNode) return;

        var paramText = rawText.Slice(paramNode.Range);
        if (int.TryParse(paramText, out var intId))
        {
            // 数値が指定されている
            if (!IsValidAsEveFlgNo(intId)) diagnostics.Add(diagRepository.Get(DiagnosticRepository.EventFlagOutOfRange, lineNode, paramNode.Range));
            return;
        }

        diagnostics.Add(diagRepository.Get(DiagnosticRepository.UnknownParameterValue, lineNode, paramNode.Range));
    }

    /// <summary>
    /// 数値パラメータの診断を行います。
    /// </summary>
    private void DiagnoseIntParameter(List<Diagnostic> diagnostics, LineNode lineNode, RawText rawText, ParameterNode paramNode)
    {
        if (paramNode is VariableParameterNode) return;

        var paramText = rawText.Slice(paramNode.Range);
        if (paramText.IsInteger()) return;

        diagnostics.Add(diagRepository.Get(DiagnosticRepository.UnknownParameterValue, lineNode, paramNode.Range));
    }

    /// <summary>
    /// ラベルパラメータの診断を行います。
    /// </summary>
    private void DiagnoseLabelParameter(List<Diagnostic> diagnostics, LineNode lineNode, RaizinScript script, AbsoluteRange range)
    {
        var paramText = script.RawText.Slice(range);
        if (paramText[0] != '*')
        {
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.LabelMustStartWithAsterisk, lineNode, range));
        }

        if (!script.TryGetLabelLineNode(paramText, out _))
        {
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.TargetLabelNotFound, lineNode, range));
        }
    }

    /// <summary>
    /// スクリプトファイルパラメータの診断を行います。
    /// </summary>
    private void DiagnoseScriptFileParameter(List<Diagnostic> diagnostics, LineNode lineNode, RaizinScript script, AbsoluteRange range)
    {
        var paramText = script.RawText.Slice(range);
        if (!configuration.TryGetFullPath($"{paramText}.txt", out _))
        {
            diagnostics.Add(diagRepository.Get(DiagnosticRepository.ScriptFileNotFound, lineNode, range));
        }
    }

    /// <summary>
    /// 前後に空白がないか診断します。
    /// </summary>
    private void DiagnoseWhitespace(List<Diagnostic> diagnostics, RawText rawText, LineNode lineNode, AbsoluteRange range, DiagnosticCode code)
    {
        var text = rawText.Slice(range);

        int startIndex;
        for (startIndex = 0; startIndex < text.Length; startIndex++)
        {
            if (!char.IsWhiteSpace(text[startIndex])) break;
        }

        if (startIndex != 0)
        {
            diagnostics.Add(diagRepository.Get(code, lineNode, new AbsoluteRange(range.Start, range.Start + startIndex)));
        }

        int endIndex;
        for (endIndex = text.Length - 1; endIndex >= 0; endIndex--)
        {
            if (!char.IsWhiteSpace(text[endIndex])) break;
        }

        if (endIndex != text.Length - 1)
        {
            diagnostics.Add(diagRepository.Get(code, lineNode, new AbsoluteRange(range.End - (text.Length - 1) + endIndex, range.End)));
        }
    }

    /// <summary>
    /// 後ろに空白がないか診断します
    /// </summary>
    private void DiagnoseTrailingWhitespace(List<Diagnostic> diagnostics, RawText rawText, LineNode lineNode, AbsoluteRange range, DiagnosticCode code)
    {
        var text = rawText.Slice(range);

        int endIndex;
        for (endIndex = text.Length - 1; endIndex >= 0; endIndex--)
        {
            if (!char.IsWhiteSpace(text[endIndex])) break;
        }

        if (endIndex != text.Length - 1)
        {
            diagnostics.Add(diagRepository.Get(code, lineNode, new AbsoluteRange(range.End - endIndex, range.End)));
        }
    }
    
    /// <summary>
    /// 診断をクリアします。
    /// </summary>
    public void Clear(DocumentUri uri)
    {
        server.PublishDiagnostics(new PublishDiagnosticsParams() {Uri = uri, Diagnostics = Array.Empty<Diagnostic>()});
    }

    /// <summary>
    /// イベントフラグ番号が有効範囲内なら true
    /// </summary>
    private static bool IsValidAsEveFlgNo(int n) => n is >= EventFlagMin and <= EventFlagMax;

    public const int EventFlagMin = 0;
    public const int EventFlagMax = 299;
}