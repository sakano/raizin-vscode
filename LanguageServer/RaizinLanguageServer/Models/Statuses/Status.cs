using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RaizinLanguageServer.Models.Commands;
using RaizinLanguageServer.Models.Definitions;
using RaizinLanguageServer.Models.Scripts.ParameterNodes;

namespace RaizinLanguageServer.Models.Statuses;

/// <summary>
/// ステータスを表します
/// </summary>
public abstract class Status(string name, ParameterType type) : IDescriptiveValue
{
    /// <summary>
    /// ステータス名
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// ステータスを取り出した結果の型
    /// </summary>
    public ParameterType Type { get; } = type;

    /// <summary>
    /// 選択肢
    /// </summary>
    public IReadOnlyList<ChoiceValueDefinition> Choices { get; protected init; } = Array.Empty<ChoiceValueDefinition>();

    /// <summary>
    /// 複数指定が有効か
    /// </summary>
    public bool MultipleValueEnabled { get; protected init; }

    /// <inheritdoc/>
    public string ScriptValue => Name;

    /// <inheritdoc/>
    public string SearchValue => Name;

    /// <inheritdoc/>
    public virtual string? ShortDescription => null;

    /// <inheritdoc/>
    public string? Description { get; protected init; }

    /// <summary>
    /// 指定された値の選択肢定義を取得します。
    /// </summary>
    public bool TryGetChoiceValue(ReadOnlySpan<char> text, [NotNullWhen(true)] out ChoiceValueDefinition? choiceDef)
    {
        foreach (var choice in Choices)
        {
            if (text.SequenceEqual(choice.ScriptValue))
            {
                choiceDef = choice;
                return true;
            }
        }

        choiceDef = null;
        return false;
    }
}