using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RaizinLanguageServer.Models.Commands;

/// <summary>
/// 指定されたコマンド名に対応する CommandDefinition を取得します。
/// </summary>
public sealed partial class CommandRepository
{
    private readonly Repository _rai7Repository;
    private readonly Repository _rai8Repository;

    private IRaizinConfiguration Configuration { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="configuration"></param>
    public CommandRepository(IRaizinConfiguration configuration)
    {
        Configuration = configuration;

        var rai7CommandDictionary = new Dictionary<string, CommandDefinition>(CommandList.Count);
        var rai8CommandDictionary = new Dictionary<string, CommandDefinition>(CommandList.Count);

        _rai7Repository = new Repository
        {
            CommandLookup = rai7CommandDictionary.GetAlternateLookup<ReadOnlySpan<char>>(),
            RegexCommandList = new List<CommandDefinition>(),
        };

        _rai8Repository = new Repository
        {
            CommandLookup = rai8CommandDictionary.GetAlternateLookup<ReadOnlySpan<char>>(),
            RegexCommandList = new List<CommandDefinition>(),
        };

        foreach (var c in CommandList)
        {
            if (c.CommandNameRegex is null)
            {
                if (c.IsRai7) rai7CommandDictionary.Add(c.CommandName, c);
                if (c.IsRai8) rai8CommandDictionary.Add(c.CommandName, c);
            }
            else
            {
                if (c.IsRai7) _rai7Repository.RegexCommandList.Add(c);
                if (c.IsRai8) _rai8Repository.RegexCommandList.Add(c);
            }
        }
    }

    /// <summary>
    /// 指定されたコマンド名に対応する CommandDefinition を取得します。
    /// </summary>
    public bool TryGetCommandDefinition(ReadOnlySpan<char> commandName, [NotNullWhen(true)] out CommandDefinition? command)
    {
        var repository = Configuration.Target is LanguageTarget.Rai7 ? _rai7Repository : _rai8Repository;

        if (repository.CommandLookup.TryGetValue(commandName, out command))
        {
            return true;
        }

        foreach (var c in repository.RegexCommandList)
        {
            if (c.CommandNameRegex!.IsMatch(commandName))
            {
                command = c;
                return true;
            }
        }

        return false;
    }


    private readonly struct Repository
    {
        public Dictionary<string, CommandDefinition>.AlternateLookup<ReadOnlySpan<char>> CommandLookup { get; init; }
        public List<CommandDefinition> RegexCommandList { get; init; }
    }
}