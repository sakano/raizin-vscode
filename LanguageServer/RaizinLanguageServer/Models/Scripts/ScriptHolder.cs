using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace RaizinLanguageServer.Models.Scripts;

public sealed class ScriptHolder : IScriptHolder
{
    private readonly Dictionary<DocumentUri, RaizinScript> _scriptDictionary = new();


    /// <inheritdoc/>
    public bool TryGetScript(DocumentUri uri, [NotNullWhen(true)] out RaizinScript? script) => _scriptDictionary.TryGetValue(uri, out script);

    /// <summary>
    /// 指定された URI に対応するスクリプトを設定します。
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="script"></param>
    internal void SetScript(DocumentUri uri, RaizinScript script) => _scriptDictionary[uri] = script;

    /// <summary>
    /// 指定された URI に対応するスクリプトを削除します。
    /// </summary>
    /// <param name="uri"></param>
    internal void RemoveScript(DocumentUri uri) => _scriptDictionary.Remove(uri);
}