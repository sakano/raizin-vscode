using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RaizinLanguageServer.Models.Commands;

/// <summary>
/// コマンドのパラメータ定義を表します。
/// </summary>
public sealed record ParameterDefinition
{
    /// <summary>
    /// パラメータ名を取得します。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// パラメータ値の型を取得します。
    /// </summary>
    public ParameterDefinitionType Type { get; }
    
    /// <summary>
    /// パラメータ値のサブタイプを取得します。
    /// </summary>
    public ParameterDefinitionType SubType { get; }

    /// <summary>
    /// パラメータが必須かどうかを取得します。
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// パラメータのヒントを取得します。
    /// </summary>
    public string? InlayHint { get; }

    /// <summary>
    /// 選択肢として選べるパラメータ値を取得します。
    /// ParameterType に関わらずこのリストに含まれるパラメータ値は有効です。
    /// </summary>
    public IReadOnlyList<ChoiceValueDefinition> Choices { get; init; } = Array.Empty<ChoiceValueDefinition>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ParameterDefinition(string name, ParameterDefinitionType type, ParameterDefinitionType subType, bool isRequired, string? inlayHint)
    {
        Name = name;
        Type = type;
        SubType = subType;
        IsRequired = isRequired;
        InlayHint = inlayHint;
    }

    /// <summary>
    /// 選択肢のパラメータ値からパラメータ値定義を取得します。
    /// </summary>
    public bool TryGetChoiceValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out ChoiceValueDefinition? definition)
    {
        foreach (var choice in Choices)
        {
            if (value.SequenceEqual(choice.ScriptValue))
            {
                definition = choice;
                return true;
            }
        }

        definition = null;
        return false;
    }
}