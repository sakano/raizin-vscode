namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// 不明な行を表す LineNode です。
/// </summary>
public sealed class UnknownLineNode(int lineNumber, AbsoluteRange lineRange) : LineNode(lineNumber, lineRange)
{
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(UnknownLineNode)}(LineNumber = {LineNumber}, LineRange = {LineRange})";
}