using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// コマンド行を表す LineNode です。
/// 以下の形式で構成されます。
/// {コマンド名},{パラメータ1},{パラメータ2},...
/// </summary>
public sealed class CommandLineNode : LineNode
{
    /// <summary>
    /// コマンド定義を取得します。
    /// </summary>
    public CommandDefinition? Definition { get; }

    /// <summary>
    /// コマンド名の範囲を取得します。
    /// </summary>
    public AbsoluteRange CommandNameRange { get; }

    /// <summary>
    /// パラメータを取得します。
    /// </summary>
    public IReadOnlyList<ParameterNode> Parameters => _parameters;

    private readonly ParameterNode[] _parameters;

    /// <summary>
    /// 指定した文字位置がコマンド名の範囲に含まれるかを取得します。
    /// </summary>
    public bool InCommandNameRange(Position position)
    {
        if (position.Line != LineNumber) return false;
        var absolutePosition = position.Character + LineRange.Start;
        return CommandNameRange.InRange(absolutePosition);
    }

    /// <summary>
    /// 指定したパラメータ番号のパラメータノードを取得します。
    /// </summary>
    public bool TryGetParameterNode(int paramIndex, [NotNullWhen(true)] out ParameterNode? parameterNode)
    {
        if (_parameters.Length <= paramIndex)
        {
            parameterNode = null;
            return false;
        }

        parameterNode = _parameters[paramIndex];
        return true;
    }

    /// <summary>
    /// 指定した文字位置が何番目のパラメータに対応するかを取得します。
    /// </summary>
    public bool TryGetParameterIndex(Position position, out int paramIndex)
    {
        if (position.Line != LineNumber || _parameters.Length == 0)
        {
            paramIndex = -1;
            return false;
        }

        var absolutePosition = position.Character + LineRange.Start;
        if (absolutePosition < CommandNameRange.End)
        {
            // 位置がコマンド名範囲より前
            paramIndex = -1;
            return false;
        }

        paramIndex = 0;
        foreach (var param in _parameters)
        {
            if (absolutePosition <= param.Range.End)
            {
                return true;
            }

            paramIndex++;
        }

        // パラメータの範囲より後ろの文字位置
        paramIndex = -1;
        return false;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CommandLineNode(CommandDefinition? definition, int lineNumber, AbsoluteRange lineRange, AbsoluteRange commandNameRange, ParameterNode[] parameterNodes) : base(lineNumber, lineRange)
    {
        Definition = definition;
        CommandNameRange = commandNameRange;
        _parameters = parameterNodes;
    }

    public override string ToString() => $"{nameof(CommandLineNode)}(LineNumber = {LineNumber}, LineRange = {LineRange}, CommandNameRange = {CommandNameRange})";
}