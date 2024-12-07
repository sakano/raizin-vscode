using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models;
using RaizinLanguageServer.Models.Scripts;
using RaizinLanguageServer.Models.Scripts.LineNodes;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServer.Handlers;

/// <summary>
/// シンタックスハイライトを提供します。
/// </summary>
public sealed partial class SemanticTokensHandler(IScriptHolder scriptHolder) : SemanticTokensHandlerBase
{
    protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken) => Task.FromResult(new SemanticTokensDocument(RegistrationOptions.Legend));

    protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities) =>
        new()
        {
            DocumentSelector = TextDocumentSelector.ForLanguage(IRaizinConfiguration.Language),
            Legend = new SemanticTokensLegend
            {
                TokenTypes = capability.TokenTypes,
                TokenModifiers = capability.TokenModifiers,
            },
            Full = new SemanticTokensCapabilityRequestFull
            {
                Delta = true
            },
            Range = true,
        };

    /// <summary>
    /// トークン化がリクエストされたときに呼び出されます。
    /// </summary>
    protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
    {
        if (!scriptHolder.TryGetScript(identifier.TextDocument.Uri, out var script))
        {
            return Task.CompletedTask;
        }

        foreach (var lineNode in script.EnumerateLineNodes())
        {
            switch (lineNode)
            {
                case CommentLineNode:
                    // コメント行のトークンを追加
                    PushToken(builder, lineNode, lineNode.LineRange, SemanticTokenType.Comment);
                    break;
                case LabelLineNode:
                    // ラベル行のトークンを追加
                    PushToken(builder, lineNode, lineNode.LineRange, SemanticTokenType.Label);
                    break;
                case CommandLineNode commandLine:
                {
                    // コマンド名のトークンを追加
                    var isKeyword = commandLine.Definition?.IsControl ?? false;
                    PushToken(builder, commandLine, commandLine.CommandNameRange, isKeyword ? SemanticTokenType.Keyword : SemanticTokenType.Function);

                    if (commandLine.Definition is null) break; // コマンド定義が見つからない

                    // パラメータのトークンを追加
                    foreach (var paramNode in commandLine.Parameters)
                    {
                        TokenizeParameterNode(builder, script.RawText, commandLine, paramNode);
                    }

                    break;
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// パラメータノードをトークン化します。
    /// </summary>
    private void TokenizeParameterNode(SemanticTokensBuilder builder, RawText rawText, LineNode lineNode, ParameterNode paramNode)
    {
        if (paramNode.Range.Length == 0) return; // パラメータ値が空

        // 階層があるノードは再帰的に処理する
        switch (paramNode)
        {
            case IndirectReferenceParameterNode indirectNode:
                // 間接参照
                TokenizeParameterNode(builder, rawText, lineNode, indirectNode.StatusParameter);
                TokenizeParameterNode(builder, rawText, lineNode, indirectNode.IdParameter);
                return;
            case GetMpidParameterNode getMpidNode:
                // get_mpid
                TokenizeParameterNode(builder, rawText, lineNode, getMpidNode.StatusParameter);
                PushToken(builder, lineNode, getMpidNode.OperatorRange, SemanticTokenType.Operator);
                TokenizeParameterNode(builder, rawText, lineNode, getMpidNode.RightParameter);
                return;
            case PredicateValueParameterNode predicateValueNode:
                // 判定値
                PushToken(builder, lineNode, predicateValueNode.OperatorRange, SemanticTokenType.Operator);
                TokenizeParameterNode(builder, rawText, lineNode, predicateValueNode.ValueParameter);
                return;
            case PredicateParameterNode predicateNode:
                // 判定条件
                TokenizeParameterNode(builder, rawText, lineNode, predicateNode.LeftParameter);
                PushToken(builder, lineNode, predicateNode.OperatorRange, SemanticTokenType.Operator);
                TokenizeParameterNode(builder, rawText, lineNode, predicateNode.RightParameter);
                return;
            case MultipleParameterNode multipleNode:
                // 複数指定パラメータ
                foreach (var childParam in multipleNode.Parameters) TokenizeParameterNode(builder, rawText, lineNode, childParam);
                return;
            case CompareParameterNode compareNode:
                // 比較式
                TokenizeParameterNode(builder, rawText, lineNode, compareNode.LeftParameter);
                PushToken(builder, lineNode, compareNode.OperatorRange, SemanticTokenType.Operator);
                TokenizeParameterNode(builder, rawText, lineNode, compareNode.RightParameter);
                return;
            case ReferStatusParameterNode referStatusNode:
                // ステータス参照
                TokenizeParameterNode(builder, rawText, lineNode, referStatusNode.ChildParameter);
                return;
            case VariableParameterNode variableNode:
                // 変数
                PushToken(builder, lineNode, variableNode.Range, SemanticTokenType.Variable); // 変数はここで追加してしまう
                return;
        }

        // 一番下のノードの種類に従ってトークンを追加
        switch (paramNode.ExpectedType)
        {
            case ParameterType.PersonId:
            case ParameterType.PlanetId:
            case ParameterType.PrimaryPlanetId:
            case ParameterType.StarId:
            case ParameterType.ShipId:
            case ParameterType.SituationId:
            case ParameterType.JobId:
            case ParameterType.ItemName:
            case ParameterType.SkillName:
            case ParameterType.Choice:
                // 列挙型のパラメータのトークンを追加
                PushToken(builder, lineNode, paramNode.Range, SemanticTokenType.EnumMember);
                break;
            case ParameterType.PersonStatusId:
            case ParameterType.PowerStatusId:
            case ParameterType.PlanetStatusId:
            case ParameterType.AnyStatusId:
                // ステータスIDのトークンを追加
                PushToken(builder, lineNode, paramNode.Range, SemanticTokenType.Macro);
                break;
            case ParameterType.Honor:
            case ParameterType.Age:
            case ParameterType.PregnancyMonth:
            case ParameterType.Year:
            case ParameterType.Month:
            case ParameterType.EveFlgNo:
            case ParameterType.Max:
            case ParameterType.Int:
                // 数値指定のトークンを追加
                if (rawText.Slice(paramNode.Range).IsInteger()) PushToken(builder, lineNode, paramNode.Range, SemanticTokenType.Number);
                break;
            case ParameterType.Label:
                // ラベルパラメータのトークンを追加
                PushToken(builder, lineNode, paramNode.Range, SemanticTokenType.Label);
                break;
            case ParameterType.Speech:
                // 台詞パラメータのトークンを追加
                var speechNode = (SpeechParameterNode)paramNode;
                foreach (var part in speechNode.Parts)
                {
                    PushToken(builder, lineNode, part.Range, part.IsSpecial ? SemanticTokenType.Variable : SemanticTokenType.String);
                }

                break;
            case ParameterType.ScriptFile:
            case ParameterType.File:
            case ParameterType.Unknown:
                break;
        }
    }

    /// <summary>
    /// トークンを追加します。
    /// </summary>
    private static void PushToken(SemanticTokensBuilder builder, LineNode lineNode, AbsoluteRange range, SemanticTokenType tokenType)
    {
        if (range.Length == 0) return;
        builder.Push(lineNode.LineNumber,
            range.Start - lineNode.LineRange.Start,
            range.Length,
            tokenType, Array.Empty<SemanticTokenModifier>());
    }

    private static readonly Regex SpeechRegex = GenerateSpeechRegex();

    /// <summary>
    /// 台詞中で使える変数の検索用正規表現
    /// </summary>
    [GeneratedRegex(@"(?:call_pid[1-4]{2}|cf_pid[1-4]|[sp]_name_pval|[sp]_name_eveflg\(?:[0-9]+\)|p[ok]?_name[1-4]|player_name|p_kaikyu[1-4]|(?:tin|man|tane|siru)_str|cr|buf_str[1-3]|\[.*\])", RegexOptions.Compiled)]
    private static partial Regex GenerateSpeechRegex();
}