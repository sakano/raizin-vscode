using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace RaizinLanguageServer.Models.Scripts;

/// <summary>
/// 使用中の Raizin スクリプトを提供するインターフェース
/// </summary>
public interface IScriptHolder
{
    /// <summary>
    /// 指定された URI の Raizin スクリプトを取得します。
    /// </summary>
    public bool TryGetScript(DocumentUri uri, [NotNullWhen(true)] out RaizinScript? script);
}