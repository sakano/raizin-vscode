using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace RaizinLanguageServer.Models.Commands;

/// <summary>
/// コマンドの定義を表します。
/// </summary>
public sealed record CommandDefinition
{
    public string CommandName { get; }

    public Regex? CommandNameRegex { get; init; } = null;

    public bool IsControl { get; }

    public bool IsRai7 { get; }

    public bool IsRai8 { get; }

    public bool IsDeprecated { get; init; } = false;

    public IReadOnlyList<ParameterDefinition> Parameters { get; init; } = Array.Empty<ParameterDefinition>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CommandDefinition(string commandName, bool isControl, bool isRai7, bool isRai8)
    {
        CommandName = commandName;
        IsControl = isControl;
        IsRai7 = isRai7;
        IsRai8 = isRai8;
    }

    public string Description { get; init; } = "";

    /// <summary>
    /// シグネチャを取得します。
    /// </summary>
    public string GetSignature()
    {
        if (Parameters.Count == 0) return CommandName;
        return $"{CommandName},{string.Join(',', Parameters.Select(static def => def.Name))}";
    }

    /// <summary>
    /// 指定されたパラメータ番号のパラメータ定義を取得します。
    /// </summary>
    public bool TryGetParameterDefinition(int parameterIndex, [NotNullWhen(true)] out ParameterDefinition? parameterDefinition)
    {
        if (parameterIndex < 0 || Parameters.Count <= parameterIndex)
        {
            parameterDefinition = null;
            return false;
        }

        parameterDefinition = Parameters[parameterIndex];
        return true;
    }
}