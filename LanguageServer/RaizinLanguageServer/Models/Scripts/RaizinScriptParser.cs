using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using RaizinLanguageServer.Models.Statuses;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServer.Models.Scripts;

/// <summary>
/// <see cref="RawText"/> をパースして <see cref="RaizinScript"/> を生成するクラスです。
/// </summary>
public sealed partial class RaizinScriptParser(CommandRepository commandRepository, StatusRepository statusRepository, IDefinitionHolder definitionHolder)
{
    /// <summary>
    /// 指定されたテキストをパースします。
    /// </summary>
    public RaizinScript Parse(DocumentUri documentUri, RawText rawText)
    {
        var nodes = new List<LineNode>();
        var labels = new List<LabelLineNode>();

        // 0行目から1行ずつパース
        var lineNumber = 0;
        foreach (var lineRange in rawText.Text.EnumerateLineRanges())
        {
            var absoluteLineRange = new AbsoluteRange(lineRange.Start.Value, lineRange.End.Value);
            var node = ParseLine(lineNumber, rawText, absoluteLineRange);

            nodes.Add(node);
            if (node is LabelLineNode labelLine)
            {
                labels.Add(labelLine);
            }

            lineNumber++;
        }

        return new RaizinScript(documentUri, rawText, nodes, labels);
    }

    /// <summary>
    /// 指定された行をパースします。
    /// </summary>
    private LineNode ParseLine(int lineNumber, RawText rawText, AbsoluteRange lineRange)
    {
        var lineText = rawText.Slice(lineRange);
        var indentIndex = lineText.IndexOfAnyExcept(' ', '\t');

        if (indentIndex < 0)
        {
            // インデント文字だけの行は空行
            return new EmptyLineNode(lineNumber, lineRange);
        }

        var firstChar = lineText[indentIndex];

        if (firstChar == '*')
        {
            // "*" から始まる行はラベル行
            var labelRange = new AbsoluteRange(lineRange.Start + indentIndex, lineRange.End);
            var label = lineText.Slice(indentIndex);
            return new LabelLineNode(lineNumber, lineRange, labelRange, label);
        }

        if (firstChar == '/' && indentIndex + 1 < lineRange.End && lineText[indentIndex + 1] == '/')
        {
            // "//" から始まる行はコメント行
            return new CommentLineNode(lineNumber, lineRange);
        }

        if (char.IsAsciiLetter(firstChar) || firstChar == '_')
        {
            // アルファベットから始まる行はコマンド行
            return ParseCommandLine(lineNumber, rawText, lineRange, indentIndex);
        }

        // それ以外は不明な行
        return new UnknownLineNode(lineNumber, lineRange);
    }

    /// <summary>
    /// 指定された行をコマンド行としてパースします。
    /// </summary>
    private CommandLineNode ParseCommandLine(int lineNumber, RawText rawText, AbsoluteRange lineRange, int indentIndex)
    {
        var firstComma = rawText.Slice(lineRange).IndexOf(',');
        if (firstComma < 0)
        {
            // コンマがない場合はコマンド名のみ
            var range = new AbsoluteRange(lineRange.Start + indentIndex, lineRange.End);
            if (!commandRepository.TryGetCommandDefinition(rawText.Slice(range), out var def))
            {
                def = null;
            }

            return new CommandLineNode(def, lineNumber, lineRange, range, []);
        }

        var commandNameRange = new AbsoluteRange(lineRange.Start + indentIndex, lineRange.Start + firstComma);
        if (!commandRepository.TryGetCommandDefinition(rawText.Slice(commandNameRange), out var commandDef))
        {
            commandDef = null;
        }

        var paramIndex = -1;
        var parameterNodes = new List<ParameterNode>();
        foreach (var splitRange in rawText.Slice(lineRange).Slice(firstComma + 1).Split(','))
        {
            paramIndex++;
            if (commandDef is null || !commandDef.TryGetParameterDefinition(paramIndex, out var paramDef)) paramDef = null;

            var paramRange = new AbsoluteRange(lineRange.Start + splitRange.Start.Value + firstComma + 1, lineRange.Start + splitRange.End.Value + firstComma + 1);
            var prevParamNode = parameterNodes.Count > 0 ? parameterNodes[^1] : null;

            ParameterNode paramNode;
            if (paramDef is null)
            {
                paramNode = CreateBasicParameterNode(rawText, paramRange, ParameterType.Unknown, true);
            }
            else if (TryParseAsDirectOnlyParameter(rawText, paramDef.Type, paramRange, out var directParam))
            {
                // 間接参照が無効なパラメータ
                paramNode = directParam;
            }
            else if (TryParseAsReferPrevStatusIdParameter(rawText, paramDef.Type, paramRange, prevParamNode, out var referPrevParam))
            {
                // ReferPrevStatusId パラメータ
                paramNode = referPrevParam;
            }
            else if (TryParseAsPredicateValue(rawText, paramDef.Type, paramDef.SubType, paramRange, prevParamNode, out var predicateValueParam))
            {
                // 判定値パラメータ
                paramNode = predicateValueParam;
            }
            else if (TryParseAsPredicate(rawText, paramDef.Type, paramRange, out var predicateParam))
            {
                // 判定条件パラメータ
                paramNode = predicateParam;
            }
            else if (TryParseAsGetMpid(rawText, paramDef.Type, paramRange, out var getMpidParam))
            {
                // get_mpid パラメータ
                paramNode = getMpidParam;
            }
            else if (TryParseAsCompare(rawText, paramDef.Type, paramRange, paramDef.SubType, out var compareParam))
            {
                // 比較式パラメータ
                paramNode = compareParam;
            }
            else if (TryParseAsIndirectReference(rawText, paramDef.Type.GetExpectedType(), paramRange, out var indirectReference))
            {
                // 間接参照パラメータ
                paramNode = indirectReference;
            }
            else
            {
                // 直接指定パラメータ
                paramNode = CreateBasicParameterNode(rawText, paramRange, paramDef.Type.GetExpectedType(), true);
            }

            parameterNodes.Add(paramNode);
        }

        return new CommandLineNode(commandDef, lineNumber, lineRange, commandNameRange, parameterNodes.ToArray());
    }

    /// <summary>
    /// 間接参照が無効なパラメータをパースします。
    /// </summary>
    private bool TryParseAsDirectOnlyParameter(RawText rawText, ParameterDefinitionType paramDefType, AbsoluteRange paramRange, [NotNullWhen(true)] out ParameterNode? paramNode)
    {
        switch (paramDefType)
        {
            case ParameterDefinitionType.Label:
                paramNode = CreateBasicParameterNode(rawText, paramRange, ParameterType.Label, false);
                return true;
            case ParameterDefinitionType.ScriptFile:
                paramNode = CreateBasicParameterNode(rawText, paramRange, ParameterType.ScriptFile, false);
                return true;
            case ParameterDefinitionType.File:
                paramNode = CreateBasicParameterNode(rawText, paramRange, ParameterType.File, false);
                return true;
            case ParameterDefinitionType.PersonStatusId:
                if (!statusRepository.TryGetPersonStatus(rawText.Slice(paramRange), out var personStatus)) personStatus = null;
                paramNode = new StatusParameterNode(paramRange, ParameterType.PersonStatusId, personStatus);
                return true;
            case ParameterDefinitionType.PlanetStatusId:
                if (!statusRepository.TryGetPlanetStatus(rawText.Slice(paramRange), out var planetStatus)) planetStatus = null;
                paramNode = new StatusParameterNode(paramRange, ParameterType.PlanetStatusId, planetStatus);
                return true;
            case ParameterDefinitionType.Speech:
                var parts = new List<SpeechParameterNode.Part>();
                var index = 0;
                foreach (var match in SpeechRegex.EnumerateMatches(rawText.Slice(paramRange)))
                {
                    if (index != match.Index) parts.Add(new(false, new AbsoluteRange(paramRange.Start + index, paramRange.Start + match.Index)));
                    index = match.Index + match.Length;
                    parts.Add(new(true, new AbsoluteRange(paramRange.Start + match.Index, paramRange.Start + index)));
                }

                if (index != paramRange.Length) parts.Add(new(false, new AbsoluteRange(paramRange.Start + index, paramRange.End)));

                paramNode = new SpeechParameterNode(paramRange, parts);
                return true;
            default:
                paramNode = null;
                return false;
        }
    }

    /// <summary>
    /// 1つ前のパラメータに指定されているステータスの値を指定するパラメータをパースします。
    /// </summary>
    private bool TryParseAsReferPrevStatusIdParameter(RawText rawText, ParameterDefinitionType paramDefType, AbsoluteRange paramRange, ParameterNode? prevParamNode, [NotNullWhen(true)] out ParameterNode? paramNode)
    {
        if (paramDefType is not ParameterDefinitionType.ReferPrevStatusId)
        {
            paramNode = null;
            return false;
        }

        var expectedType = ParameterType.Unknown;
        var prevStatusNode = prevParamNode as StatusParameterNode;
        if (prevStatusNode is not null && prevStatusNode.Status is not null)
        {
            expectedType = prevStatusNode.Status.Type;
        }

        if (TryParseAsIndirectReference(rawText, expectedType, paramRange, out var indirectReference))
        {
            paramNode = indirectReference;
        }
        else
        {
            paramNode = CreateBasicParameterNode(rawText, paramRange, expectedType, true);
        }

        paramNode = new ReferStatusParameterNode(paramRange, expectedType, prevStatusNode?.Status, paramNode);

        return true;
    }

    /// <summary>
    /// 間接参照のパラメータとしてパースします。
    /// </summary>
    private bool TryParseAsIndirectReference(RawText rawText, ParameterType expectedType, AbsoluteRange paramRange, [NotNullWhen(true)] out ParameterNode? paramNode)
    {
        paramNode = null;
        if (paramRange.Length == 0) return false;

        var paramText = rawText.Slice(paramRange);
        var index = paramText.IndexOf(':');
        if (index < 0) return false;

        // ステータス名とIDの範囲を取得
        var statusNameRange = new AbsoluteRange(paramRange.Start, paramRange.Start + index);
        var idRange = new AbsoluteRange(paramRange.Start + index + 1, paramRange.End);

        // ステータス名からステータスと型を取得
        var statusName = rawText.Slice(statusNameRange);
        Status? status = null;
        var expectedIdType = ParameterType.Unknown;
        if (statusRepository.TryGetPersonStatus(statusName, out var personStatus))
        {
            status = personStatus;
            expectedIdType = ParameterType.PersonId;
        }
        else if (statusRepository.TryGetPowerStatus(statusName, out var powerStatus))
        {
            status = powerStatus;
            expectedIdType = ParameterType.PersonId;
        }
        else if (statusRepository.TryGetPlanetStatus(statusName, out var planetStatus))
        {
            status = planetStatus;
            expectedIdType = ParameterType.PlanetId;
        }

        // 生成
        var statusParam = new StatusParameterNode(statusNameRange, ParameterType.AnyStatusId, status); // 補完で全ての候補が出るように AnyStatusId を指定
        var idParam = CreateBasicParameterNode(rawText, idRange, expectedIdType, true);
        paramNode = new IndirectReferenceParameterNode(paramRange, expectedType, statusParam, idParam);
        return true;
    }

    /// <summary>
    /// 判定値パラメータとしてパースします。
    /// </summary>
    private bool TryParseAsPredicateValue(RawText rawText, ParameterDefinitionType paramDefType, ParameterDefinitionType subDefType, AbsoluteRange paramRange, ParameterNode? prevParamNode, [NotNullWhen(true)] out PredicateValueParameterNode? paramNode)
    {
        if (paramDefType is not ParameterDefinitionType.PredicateValue)
        {
            paramNode = null;
            return false;
        }

        // 演算子と値の範囲を取得
        var paramText = rawText.Slice(paramRange);
        AbsoluteRange opRange;
        AbsoluteRange valueRange;
        if (paramText.Length > 0 && paramText[0] is '<' or '>' or '=' or '!')
        {
            // >= や <= は使えないので演算子は先頭1文字だけ
            opRange = new AbsoluteRange(paramRange.Start, paramRange.Start + 1);
            valueRange = new AbsoluteRange(paramRange.Start + 1, paramRange.End);
        }
        else
        {
            opRange = new AbsoluteRange(paramRange.Start, paramRange.Start);
            valueRange = new AbsoluteRange(paramRange.Start, paramRange.End);
        }

        // パラメータを生成
        ParameterNode valueParam;
        if (TryParseAsReferPrevStatusIdParameter(rawText, subDefType, valueRange, prevParamNode, out var referPrevParam))
        {
            valueParam = referPrevParam;
        }
        else if (TryParseAsIndirectReference(rawText, subDefType.GetExpectedType(), valueRange, out var indirectReference))
        {
            valueParam = indirectReference;
        }
        else
        {
            valueParam = CreateBasicParameterNode(rawText, valueRange, subDefType.GetExpectedType(), true);
        }

        paramNode = new PredicateValueParameterNode(paramRange, valueParam.ExpectedType, opRange, valueParam);
        return true;
    }

    /// <summary>
    /// 条件判定パラメータとしてパースします。
    /// </summary>
    private bool TryParseAsPredicate(RawText rawText, ParameterDefinitionType paramDefType, AbsoluteRange paramRange, [NotNullWhen(true)] out PredicateParameterNode? paramNode)
    {
        if (paramDefType is not ParameterDefinitionType.Predicate)
        {
            paramNode = null;
            return false;
        }

        // 演算子と左辺右辺の範囲を取得
        SplitExpression(rawText, paramRange, true, out var leftRange, out var opRange, out var rightRange);

        // 左辺をパース
        if (!TryParseAsIndirectReference(rawText, ParameterType.Unknown, leftRange, out var leftParameterNode)) leftParameterNode = null;

        // 左辺の値から右辺のタイプを推測
        var rightExpectedType = ParameterType.Unknown;
        var leftIndirectNode = leftParameterNode as IndirectReferenceParameterNode;
        if (leftIndirectNode is not null)
        {
            rightExpectedType = leftIndirectNode.StatusParameter.Status?.Type ?? ParameterType.Unknown;
        }
        else if (definitionHolder.TryGetVariableDefinition(rawText.Slice(leftRange), out var varDef))
        {
            rightExpectedType = varDef.Type;
        }

        // 右辺をパース
        if (!TryParseAsIndirectReference(rawText, rightExpectedType, rightRange, out var rightParameterNode))
        {
            if (leftParameterNode is StatusParameterNode { Status.MultipleValueEnabled: true } statusParam)
            {
                if (!TryParseAsMultiple(rawText, statusParam.Status.Type, rightRange, out rightParameterNode))
                {
                    rightParameterNode = null;
                }
            }
        }

        if (rightParameterNode is null)
        {
            rightParameterNode = CreateBasicParameterNode(rawText, rightRange, rightExpectedType, true);
            if (leftIndirectNode is not null && leftIndirectNode.StatusParameter.Status is not null)
            {
                rightParameterNode = new ReferStatusParameterNode(rightRange, rightExpectedType, leftIndirectNode.StatusParameter.Status, rightParameterNode);
            }
        }

        if (leftParameterNode is null)
        {
            // 右辺から左辺のタイプを推測
            var leftExpectedType = ParameterType.Unknown;
            var rightIndirectNode = rightParameterNode as IndirectReferenceParameterNode;
            if (rightIndirectNode is not null)
            {
                leftExpectedType = rightIndirectNode.StatusParameter.Status?.Type ?? ParameterType.Unknown;
            }
            else if (definitionHolder.TryGetVariableDefinition(rawText.Slice(rightRange), out var varDef))
            {
                leftExpectedType = varDef.Type;
            }

            leftParameterNode = CreateBasicParameterNode(rawText, leftRange, leftExpectedType, true);
            if (rightIndirectNode is not null && rightIndirectNode.StatusParameter.Status is not null)
            {
                leftParameterNode = new ReferStatusParameterNode(leftRange, leftExpectedType, rightIndirectNode.StatusParameter.Status, leftParameterNode);
            }
        }

        paramNode = new PredicateParameterNode(paramRange, ParameterType.Unknown, leftParameterNode, rightParameterNode, opRange);
        return true;
    }

    /// <summary>
    /// get_mpid コマンドのパラメータとしてパースします。
    /// </summary>
    private bool TryParseAsGetMpid(RawText rawText, ParameterDefinitionType paramDefType, AbsoluteRange paramRange, [NotNullWhen(true)] out ParameterNode? paramNode)
    {
        if (paramDefType is not ParameterDefinitionType.GetMpid)
        {
            paramNode = null;
            return false;
        }

        SplitExpression(rawText, paramRange, true, out var leftRange, out var opRange, out var rightRange);

        // 左辺をパース(間接参照は無効)
        if (!statusRepository.TryGetPersonStatus(rawText.Slice(leftRange), out var leftStatus)) leftStatus = null;
        var leftStatusNode = new StatusParameterNode(leftRange, ParameterType.PersonStatusId, leftStatus);

        // 右辺をパース
        var rightExpectedType = leftStatus?.Type ?? ParameterType.Unknown;
        if (!TryParseAsIndirectReference(rawText, rightExpectedType, rightRange, out var rightParameterNode))
        {
            if (leftStatus is not null && leftStatus.MultipleValueEnabled)
            {
                if (!TryParseAsMultiple(rawText, rightExpectedType, rightRange, out rightParameterNode))
                {
                    rightParameterNode = null;
                }
            }

            if (rightParameterNode is null)
            {
                rightParameterNode = CreateBasicParameterNode(rawText, rightRange, rightExpectedType, true);
                rightParameterNode = new ReferStatusParameterNode(rightRange, rightExpectedType, leftStatus, rightParameterNode);
            }
        }

        paramNode = new GetMpidParameterNode(paramRange, ParameterType.Unknown, leftStatusNode, rightParameterNode, opRange);
        return true;
    }

    /// <summary>
    /// 比較式パラメータとしてパースします。
    /// </summary>
    private bool TryParseAsCompare(RawText rawText, ParameterDefinitionType paramDefType, AbsoluteRange paramRange, ParameterDefinitionType subType, [NotNullWhen(true)] out ParameterNode? paramNode)
    {
        if (paramDefType is not ParameterDefinitionType.Compare)
        {
            paramNode = null;
            return false;
        }

        // 演算子と左辺右辺の範囲を取得
        SplitExpression(rawText, paramRange, false, out var leftRange, out var opRange, out var rightRange);

        var expectedType = subType.GetExpectedType();

        // 左辺をパース
        if (!TryParseAsIndirectReference(rawText, expectedType, leftRange, out var leftParameterNode))
        {
            leftParameterNode = CreateBasicParameterNode(rawText, leftRange, expectedType, true);
        }

        // 右辺をパース
        if (!TryParseAsIndirectReference(rawText, expectedType, rightRange, out var rightParameterNode))
        {
            rightParameterNode = CreateBasicParameterNode(rawText, rightRange, expectedType, true);
        }

        paramNode = new CompareParameterNode(paramRange, ParameterType.Unknown, leftParameterNode, rightParameterNode, opRange);
        return true;
    }

    /// <summary>
    /// 複数パラメータとしてパースします。
    /// </summary>
    private static bool TryParseAsMultiple(RawText rawText, ParameterType expectedType, AbsoluteRange paramRange, [NotNullWhen(true)] out ParameterNode? parameterNode)
    {
        var paramText = rawText.Slice(paramRange);
        if (paramText.IndexOf('|') < 0)
        {
            parameterNode = null;
            return false;
        }

        List<ParameterNode> parameters = new();
        foreach (var range in paramText.Split('|'))
        {
            var absoluteRange = new AbsoluteRange(paramRange.Start + range.Start.Value, paramRange.Start + range.End.Value);
            parameters.Add(new ParameterNode(absoluteRange, expectedType));
        }

        parameterNode = new MultipleParameterNode(paramRange, expectedType, parameters);
        return true;
    }

    /// <summary>
    /// 基本的なパラメータノードを生成します。
    /// </summary>
    private ParameterNode CreateBasicParameterNode(RawText rawText, AbsoluteRange paramRange, ParameterType expectedType, bool variableEnabled)
    {
        if (variableEnabled && definitionHolder.TryGetVariableDefinition(rawText.Slice(paramRange), out var varDef))
        {
            return new VariableParameterNode(paramRange, expectedType, varDef);
        }

        return new ParameterNode(paramRange, expectedType);
    }

    /// <summary>
    /// 式を左辺/演算子/右辺に分割します。
    /// </summary>
    private static void SplitExpression(RawText rawText, AbsoluteRange paramRange, bool twoLettersOpEnabled, out AbsoluteRange leftRange, out AbsoluteRange opRange, out AbsoluteRange rightRange)
    {
        var paramText = rawText.Slice(paramRange);
        var opIndex = paramText.IndexOfAny(['=', '>', '<', '!']);
        if (opIndex < 0)
        {
            leftRange = paramRange;
            rightRange = new AbsoluteRange(paramRange.End, paramRange.End);
            opRange = new AbsoluteRange(paramRange.End, paramRange.End);
        }
        else
        {
            leftRange = new(paramRange.Start, paramRange.Start + opIndex);
            if (twoLettersOpEnabled && paramText[opIndex] is '>' or '<' && paramText.Length > opIndex + 1 && paramText[opIndex + 1] is '=')
            {
                // >= または <=
                rightRange = new AbsoluteRange(paramRange.Start + opIndex + 2, paramRange.End);
                opRange = new AbsoluteRange(paramRange.Start + opIndex, paramRange.Start + opIndex + 2);
            }
            else
            {
                rightRange = new AbsoluteRange(paramRange.Start + opIndex + 1, paramRange.End);
                opRange = new AbsoluteRange(paramRange.Start + opIndex, paramRange.Start + opIndex + 1);
            }
        }
    }

    private static readonly Regex SpeechRegex = GenerateSpeechRegex();

    /// <summary>
    /// 台詞中で使える変数の検索用正規表現
    /// </summary>
    [GeneratedRegex(@"(?:call_pid[1-4]{1,3}|callself_pid[1-4]|cf_pid[1-4]|[sp]_name_pval|[sp]_name_eveflg\(?:[0-9]+\)|p[ok]?_name[1-4]|player_name|p_kaikyu[1-4]|(?:tin|man|tane|siru)_str|cr|buf_str[1-3]|str_eveflg\(\d+\)|\[.*\])", RegexOptions.Compiled)]
    private static partial Regex GenerateSpeechRegex();
}