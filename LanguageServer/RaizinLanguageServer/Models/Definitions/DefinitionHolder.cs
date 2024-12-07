using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace RaizinLanguageServer.Models.Definitions;

public sealed class DefinitionHolder : IDefinitionHolder, IDisposable
{
    private IRaizinConfiguration Configuration { get; }
    private ILogger Logger { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DefinitionHolder(IRaizinConfiguration configuration, ILogger logger)
    {
        Configuration = configuration;
        Logger = logger;

        configuration.OnConfigurationChanged += OnConfigurationChanged;
    }

    static DefinitionHolder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Shift_JIS を使うために必要
    }

    /// <inheritdoc/>
    public event Action<IDefinitionHolder>? OnDefinitionChanged;

    #region PersonDefinition

    /// <inheritdoc/>
    IReadOnlyList<PersonDefinition> IDefinitionHolder.PersonDefinitionList => PersonDefinitionList;

    private List<PersonDefinition> PersonDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetPersonDefinition(ReadOnlySpan<char> personId, [NotNullWhen(true)] out PersonDefinition? personDefinition)
    {
        foreach (var def in PersonDefinitionList)
        {
            if (personId.SequenceEqual(def.Id))
            {
                personDefinition = def;
                return true;
            }
        }

        personDefinition = null;
        return false;
    }

    #endregion

    #region PlanetDefinition

    /// <inheritdoc/>
    IReadOnlyList<PlanetDefinition> IDefinitionHolder.PlanetDefinitionList => PlanetDefinitionList;

    private List<PlanetDefinition> PlanetDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetPlanetDefinition(ReadOnlySpan<char> planetId, [NotNullWhen(true)] out PlanetDefinition? planetDefinition)
    {
        foreach (var def in PlanetDefinitionList)
        {
            if (planetId.SequenceEqual(def.Id))
            {
                planetDefinition = def;
                return true;
            }
        }

        planetDefinition = null;
        return false;
    }

    #endregion

    #region StarDefinition

    /// <inheritdoc/>
    IReadOnlyList<StarDefinition> IDefinitionHolder.StarDefinitionList => Configuration.Target is LanguageTarget.Rai7 ? throw new NotSupportedException() : StarDefinitionList;

    private List<StarDefinition> StarDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetStarDefinition(ReadOnlySpan<char> starId, [NotNullWhen(true)] out StarDefinition? starDefinition)
    {
        foreach (var starDef in StarDefinitionList)
        {
            if (starId.SequenceEqual(starDef.Id))
            {
                starDefinition = starDef;
                return true;
            }
        }

        starDefinition = null;
        return false;
    }

    #endregion

    #region ItemDefinition

    /// <inheritdoc/>
    IReadOnlyList<ItemDefinition> IDefinitionHolder.ItemDefinitionList => ItemDefinitionList;

    private List<ItemDefinition> ItemDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetItemDefinition(ReadOnlySpan<char> itemName, [NotNullWhen(true)] out ItemDefinition? itemDefinition)
    {
        foreach (var def in ItemDefinitionList)
        {
            if (itemName.SequenceEqual(def.Name))
            {
                itemDefinition = def;
                return true;
            }
        }

        itemDefinition = null;
        return false;
    }

    #endregion

    #region SkillDefinition

    /// <inheritdoc/>
    IReadOnlyList<SkillDefinition> IDefinitionHolder.SkillDefinitionsList => SkillDefinitionList;

    private List<SkillDefinition> SkillDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetSkillDefinition(ReadOnlySpan<char> skillName, [NotNullWhen(true)] out SkillDefinition? skillDefinition)
    {
        foreach (var def in SkillDefinitionList)
        {
            if (skillName.SequenceEqual(def.Name))
            {
                skillDefinition = def;
                return true;
            }
        }

        skillDefinition = null;
        return false;
    }

    #endregion

    #region ShipDefinition

    /// <inheritdoc/>
    IReadOnlyList<ShipDefinition> IDefinitionHolder.ShipDefinitionsList => ShipDefinitionList;

    private List<ShipDefinition> ShipDefinitionList { get; } = new();

    /// <inheritdoc/>
    public bool TryGetShipDefinition(ReadOnlySpan<char> shipId, [NotNullWhen(true)] out ShipDefinition? shipDefinition)
    {
        foreach (var def in ShipDefinitionList)
        {
            if (shipId.SequenceEqual(def.Id))
            {
                shipDefinition = def;
                return true;
            }
        }

        shipDefinition = null;
        return false;
    }

    #endregion

    #region SituationDefinition

    /// <inheritdoc/>
    public IReadOnlyList<SituationDefinition> SituationDefinitionList => Configuration.Target is LanguageTarget.Rai7 ? SituationDefinition.Rai7List : SituationDefinition.Rai8List;

    /// <inheritdoc/>
    public bool TryGetSituationDefinition(ReadOnlySpan<char> situationId, [NotNullWhen(true)] out SituationDefinition? situationDefinition)
    {
        foreach (var situationDef in SituationDefinitionList)
        {
            if (situationId.SequenceEqual(situationDef.ScriptValue))
            {
                situationDefinition = situationDef;
                return true;
            }
        }

        situationDefinition = null;
        return false;
    }

    #endregion

    #region JobDefinition

    /// <inheritdoc/>
    public IReadOnlyList<JobDefinition> JobDefinitionList => Configuration.Target is LanguageTarget.Rai7 ? JobDefinition.Rai7List : JobDefinition.Rai8List;

    /// <inheritdoc/>
    public bool TryGetJobDefinition(ReadOnlySpan<char> jobId, [NotNullWhen(true)] out JobDefinition? jobDefinition)
    {
        foreach (var jobDef in JobDefinitionList)
        {
            if (jobId.SequenceEqual(jobDef.ScriptValue))
            {
                jobDefinition = jobDef;
                return true;
            }
        }

        jobDefinition = null;
        return false;
    }

    #endregion

    #region HonorDefinition

    /// <inheritdoc/>
    public IReadOnlyList<HonorDefinition> HonorDefinitionList => HonorDefinition.List;

    /// <inheritdoc/>
    public bool TryGetHonorDefinition(ReadOnlySpan<char> honor, [NotNullWhen(true)] out HonorDefinition? honorDefinition)
    {
        foreach (var honorDef in HonorDefinitionList)
        {
            if (honor.SequenceEqual(honorDef.ScriptValue))
            {
                honorDefinition = honorDef;
                return true;
            }
        }

        honorDefinition = null;
        return false;
    }

    #endregion

    #region VariableDefinition

    /// <inheritdoc/>
    public IReadOnlyList<VariableDefinition> VariableDefinitionList => VariableDefinition.List;

    /// <inheritdoc/>
    public bool TryGetVariableDefinition(ReadOnlySpan<char> variableName, [NotNullWhen(true)] out VariableDefinition? variableDefinition)
    {
        if (variableName.Length == 0)
        {
            variableDefinition = null;
            return false;
        }

        foreach (var def in VariableDefinitionList)
        {
            if (def.IsNameRegex)
            {
                if (def.NameRegex.IsMatch(variableName))
                {
                    variableDefinition = def;
                    return true;
                }
            }
            else
            {
                if (variableName.SequenceEqual(def.Name))
                {
                    variableDefinition = def;
                    return true;
                }
            }
        }

        variableDefinition = null;
        return false;
    }

    #endregion

    #region ReadPersonDefinitionFile

    /// <summary>
    /// 設定が変更された際に定義ファイルを読み込み直す
    /// </summary>
    private void OnConfigurationChanged()
    {
        ReadPersonDefinitionFile();
        ReadPlanetDefinitionFile();
        ReadItemDefinitionFile();
        ReadSkillDefinitionFile();
        ReadShipDefinitionFile();

        OnDefinitionChanged?.Invoke(this);
    }

    /// <summary>
    /// psonN.csv を読み込みます
    /// </summary>
    private void ReadPersonDefinitionFile()
    {
        PersonDefinitionList.Clear();

        if (!Configuration.TryGetPsonFullPath(out var psonPath)) return;

        ReadDefinitionFile(psonPath, static (list, lineNumber, fields) =>
        {
            if (fields.Length < 2) return;
            if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1])) return;
            if (!int.TryParse(fields[0], out var id)) return;

            list.Add(new PersonDefinition
            {
                Id = id.ToString(),
                Name = fields[1],
                LineNumber = lineNumber,
            });
        }, PersonDefinitionList);
    }

    /// <summary>
    /// pnetN.csv を読み込みます
    /// </summary>
    private void ReadPlanetDefinitionFile()
    {
        PlanetDefinitionList.Clear();
        StarDefinitionList.Clear();

        if (!Configuration.TryGetPnetFullPath(out var pnetPath)) return;

        if (Configuration.Target is LanguageTarget.Rai8)
        {
            ReadDefinitionFile(pnetPath, static (lists, lineNumber, fields) =>
            {
                if (fields.Length < 10) return;
                if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1]) || string.IsNullOrEmpty(fields[7]) || string.IsNullOrEmpty(fields[9])) return;
                if (!int.TryParse(fields[0], out var id)) return;

                // 恒星
                if (fields[7] == "1")
                {
                    lists.Star.Add(new StarDefinition
                    {
                        Id = id.ToString(),
                        Name = fields[1],
                        LineNumber = lineNumber,
                    });
                }
                else
                {
                    lists.Planet.Add(new PlanetDefinition
                    {
                        Id = id.ToString(),
                        Name = fields[1],
                        LineNumber = lineNumber,
                        IsPrimary = fields[9] != "0",
                    });
                }
            }, (Planet: PlanetDefinitionList, Star: StarDefinitionList));
        }
        else
        {
            ReadDefinitionFile(pnetPath, static (list, lineNumber, fields) =>
            {
                if (fields.Length < 2) return;
                if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1])) return;
                if (!int.TryParse(fields[0], out var id)) return;

                list.Add(new PlanetDefinition
                {
                    Id = id.ToString(),
                    Name = fields[1],
                    LineNumber = lineNumber,
                    IsPrimary = true,
                });
            }, PlanetDefinitionList);
        }
    }

    /// <summary>
    /// item.csv を読み込みます
    /// </summary>
    private void ReadItemDefinitionFile()
    {
        ItemDefinitionList.Clear();

        if (!Configuration.TryGetItemFullPath(out var itemPath)) return;

        if (Configuration.Target is LanguageTarget.Rai8)
        {
            ReadDefinitionFile(itemPath, static (list, lineNumber, fields) =>
            {
                if (fields.Length < 2) return;
                if (string.IsNullOrEmpty(fields[1])) return;

                if (list.FirstOrDefault(x => x.Name == fields[1]) != null) return;
                list.Add(new ItemDefinition
                {
                    Name = fields[1],
                    LineNumber = lineNumber
                });
            }, ItemDefinitionList);
        }
        else
        {
            ReadDefinitionFile(itemPath, static (list, lineNumber, fields) =>
            {
                if (fields.Length < 1) return;
                if (string.IsNullOrEmpty(fields[0])) return;

                list.Add(new ItemDefinition
                {
                    Name = fields[0],
                    LineNumber = lineNumber,
                });
            }, ItemDefinitionList);
        }
    }

    /// <summary>
    /// sk_base.csv を読み込みます
    /// </summary>
    private void ReadSkillDefinitionFile()
    {
        SkillDefinitionList.Clear();

        if (!Configuration.TryGetSkillFullPath(out var skillPath)) return;

        ReadDefinitionFile(skillPath, static (list, lineNumber, fields) =>
        {
            if (fields.Length < 2) return;
            if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1])) return;

            list.Add(new SkillDefinition
            {
                Name = fields[1],
                LineNumber = lineNumber,
            });
        }, SkillDefinitionList);
    }

    /// <summary>
    /// 艦種定義ファイルを読み込みます
    /// </summary>
    private void ReadShipDefinitionFile()
    {
        ShipDefinitionList.Clear();

        if (!Configuration.TryGetShipFullPath(out var shipPath)) return;

        ReadDefinitionFile(shipPath, static (list, lineNumber, fields) =>
        {
            if (fields.Length < 2) return;
            if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1])) return;
            if (!int.TryParse(fields[0], out var id)) return;

            list.Add(new ShipDefinition
            {
                Id = id.ToString(),
                Name = fields[1],
                LineNumber = lineNumber,
            });
        }, ShipDefinitionList);
    }

    /// <summary>
    /// 定義ファイルを読み込みます
    /// </summary>
    private void ReadDefinitionFile<T>(string path, Action<T, int, string[]> handler, T state)
    {
        try
        {
            using var parser = new StreamReader(path, Encoding.GetEncoding("Shift_JIS"));

            var lineNumber = -1;
            while (parser.ReadLine() is { } line)
            {
                lineNumber++;
                if (line.AsSpan().TrimStart().StartsWith("//", StringComparison.Ordinal)) continue; // コメント行

                var fields = line.Split(',', StringSplitOptions.TrimEntries);
                handler.Invoke(state, lineNumber, fields);
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to load csv file. message:{Message}, path:{Path}", e.Message, path);
        }
    }

    public void Dispose()
    {
        Configuration.OnConfigurationChanged -= OnConfigurationChanged;
        OnDefinitionChanged = null;
    }

    #endregion
}