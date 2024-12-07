using System;
using System.Diagnostics.CodeAnalysis;

namespace RaizinLanguageServer.Models;

public interface IRaizinConfiguration
{
    public const string LanguageServerName = "RaizinLanguageServer";
    public const string LanguageServerVersion = "1.0.0";

    public const string Language = "raizin";

    /// <summary>
    /// ターゲット言語バージョン
    /// </summary>
    public LanguageTarget Target { get; }

    /// <summary>
    /// psonN.csv ファイルのパスを取得する
    /// </summary>
    public bool TryGetPsonFullPath([NotNullWhen(true)] out string? path);

    /// <summary>
    /// pnetN.csv ファイルのパスを取得する
    /// </summary>
    public bool TryGetPnetFullPath([NotNullWhen(true)] out string? path);

    /// <summary>
    /// item.csv ファイルのパスを取得する
    /// </summary>
    public bool TryGetItemFullPath([NotNullWhen(true)] out string? path);

    /// <summary>
    /// sk_base.csv ファイルのパスを取得する
    /// </summary>
    public bool TryGetSkillFullPath([NotNullWhen(true)] out string? path);

    /// <summary>
    /// 艦種定義ファイルのパスを取得する
    /// </summary>
    public bool TryGetShipFullPath([NotNullWhen(true)] out string? path);

    /// <summary>
    /// 指定されたファイルのフルパスを取得する
    /// </summary>
    public bool TryGetFullPath(string path, [NotNullWhen(true)] out string? fullPath);

    /// <summary>
    /// 設定が変更された際に発生するイベント
    /// </summary>
    public event Action OnConfigurationChanged;
}