using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServer.Models.Statuses;

/// <summary>
/// ステータスを保持します
/// </summary>
public sealed class StatusRepository(IRaizinConfiguration configuration)
{
    /// <summary>
    /// 人物ステータスリスト
    /// </summary>
    public IReadOnlyList<PersonStatus> PersonStatusList => configuration.Target is LanguageTarget.Rai7 ? PersonStatus.Rai7List : PersonStatus.Rai8List;

    /// <summary>
    /// 勢力ステータスリスト
    /// </summary>
    public IReadOnlyList<PowerStatus> PowerStatusList => configuration.Target is LanguageTarget.Rai7 ? PowerStatus.Rai7List : PowerStatus.Rai8List;

    /// <summary>
    /// 惑星ステータス定義リスト
    /// </summary>
    public IReadOnlyList<PlanetStatus> PlanetStatusList => configuration.Target is LanguageTarget.Rai7 ? PlanetStatus.Rai7List : PlanetStatus.Rai8List;

    /// <summary>
    /// 全てのステータスを列挙します
    /// </summary>
    public IEnumerable<Status> EnumerateAllStatus() => PersonStatusList.Concat<Status>(PowerStatusList).Concat<Status>(PlanetStatusList);

    /// <summary>
    /// ステータス名からステータス定義を取得します。
    /// </summary>
    public bool TryGetAnyStatus(ReadOnlySpan<char> statusName, [NotNullWhen(true)] out Status? statusDefinition)
    {
        if (TryGetPersonStatus(statusName, out var personStatus))
        {
            statusDefinition = personStatus;
            return true;
        }

        if (TryGetPowerStatus(statusName, out var powerStatus))
        {
            statusDefinition = powerStatus;
            return true;
        }

        if (TryGetPlanetStatus(statusName, out var planetStatus))
        {
            statusDefinition = planetStatus;
            return true;
        }

        statusDefinition = null;
        return false;
    }

    /// <summary>
    /// 人物ステータス名からステータス定義を取得します。
    /// </summary>
    public bool TryGetPersonStatus(ReadOnlySpan<char> statusName, [NotNullWhen(true)] out PersonStatus? statusDefinition)
    {
        if (statusName.Length == 0)
        {
            statusDefinition = null;
            return false;
        }

        if (statusName.IsInteger())
        {
            foreach (var statusDef in PersonStatusList)
            {
                if (statusName.SequenceEqual(statusDef.Id))
                {
                    statusDefinition = statusDef;
                    return true;
                }
            }
        }
        else
        {
            foreach (var statusDef in PersonStatusList)
            {
                if (statusName.SequenceEqual(statusDef.Name))
                {
                    statusDefinition = statusDef;
                    return true;
                }
            }
        }

        statusDefinition = null;
        return false;
    }

    /// <summary>
    /// 勢力ステータス名から勢力ステータス定義を取得します。
    /// </summary>
    public bool TryGetPowerStatus(ReadOnlySpan<char> statusName, [NotNullWhen(true)] out PowerStatus? statusDefinition)
    {
        if (statusName.Length == 0)
        {
            statusDefinition = null;
            return false;
        }

        foreach (var statusDef in PowerStatusList)
        {
            if (statusName.SequenceEqual(statusDef.Name))
            {
                statusDefinition = statusDef;
                return true;
            }
        }

        statusDefinition = null;
        return false;
    }

    /// <summary>
    /// 惑星ステータス名から惑星ステータス定義を取得します。
    /// </summary>
    public bool TryGetPlanetStatus(ReadOnlySpan<char> statusName, [NotNullWhen(true)] out PlanetStatus? statusDefinition)
    {
        if (statusName.Length == 0)
        {
            statusDefinition = null;
            return false;
        }

        foreach (var statusDef in PlanetStatusList)
        {
            if (statusName.SequenceEqual(statusDef.Name))
            {
                statusDefinition = statusDef;
                return true;
            }
        }

        statusDefinition = null;
        return false;
    }
}