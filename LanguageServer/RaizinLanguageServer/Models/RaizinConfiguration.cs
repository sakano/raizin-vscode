using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace RaizinLanguageServer.Models;

public sealed class RaizinConfiguration : IRaizinConfiguration
{
    private DocumentUri? WorkspaceUri { get; set; }

    /// <inheritdoc/>
    public LanguageTarget Target { get; private set; } = LanguageTarget.Rai8;

    private string? _psonPath;
    private string? _pnetPath;
    private string? _itemPath;
    private string? _skillPath;
    private string? _rai7ShipPath;
    private string? _rai8ShipPath;

    public void Initialize(InitializeParams request)
    {
        if (request.WorkspaceFolders is not null)
        {
            foreach (var workspaceFolder in request.WorkspaceFolders)
            {
                WorkspaceUri = workspaceFolder.Uri;
            }
        }
    }

    public void Update(DidChangeConfigurationParams request)
    {
        if (request.Settings is not JObject settingObject) return;

        if (settingObject["raizin-vscode"] is not JObject raizinSettings) return;
        if (raizinSettings.TryGetValue("target", out var target))
        {
            Target = target.Value<string>() == "rai7" ? LanguageTarget.Rai7 : LanguageTarget.Rai8;
        }

        if (raizinSettings.TryGetValue("psonPath", out var psonPath))
        {
            _psonPath = psonPath.Value<string>();
        }

        if (raizinSettings.TryGetValue("pnetPath", out var pnetPath))
        {
            _pnetPath = pnetPath.Value<string>();
        }

        if (raizinSettings.TryGetValue("itemPath", out var itemPath))
        {
            _itemPath = itemPath.Value<string>();
        }

        if (raizinSettings.TryGetValue("skillPath", out var skillPath))
        {
            _skillPath = skillPath.Value<string>();
        }

        if (raizinSettings.TryGetValue("rai7ShipPath", out var rai7ShipPath))
        {
            _rai7ShipPath = rai7ShipPath.Value<string>();
        }

        if (raizinSettings.TryGetValue("rai8ShipPath", out var rai8ShipPath))
        {
            _rai8ShipPath = rai8ShipPath.Value<string>();
        }

        OnConfigurationChanged?.Invoke();
    }

    /// <inheritdoc/>
    public bool TryGetPsonFullPath([NotNullWhen(true)] out string? path) => TryGetFullPath(_psonPath, out path);

    /// <inheritdoc/>
    public bool TryGetPnetFullPath([NotNullWhen(true)] out string? path) => TryGetFullPath(_pnetPath, out path);

    /// <inheritdoc/>
    public bool TryGetItemFullPath([NotNullWhen(true)] out string? path) => TryGetFullPath(_itemPath, out path);

    /// <inheritdoc/>
    public bool TryGetSkillFullPath([NotNullWhen(true)] out string? path) => TryGetFullPath(_skillPath, out path);

    /// <inheritdoc/>
    public bool TryGetShipFullPath([NotNullWhen(true)] out string? path) => TryGetFullPath(Target == LanguageTarget.Rai7 ? _rai7ShipPath : _rai8ShipPath, out path);

    /// <inheritdoc/>
    public bool TryGetFullPath(string? rawPath, [NotNullWhen(true)] out string? path)
    {
        if (rawPath is not null)
        {
            if (Path.IsPathRooted(rawPath))
            {
                path = rawPath;
                return File.Exists(path);
            }

            if (WorkspaceUri is not null)
            {
                path = Path.Combine(WorkspaceUri.ToUri().LocalPath, rawPath);
                return File.Exists(path);
            }
        }

        path = null;
        return false;
    }

    public event Action? OnConfigurationChanged;
    public bool TryGetFullPath(string empty) => throw new NotImplementedException();
}