namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// 空行を表す LineNode です。
/// </summary>
public sealed class EmptyLineNode(int lineNumber, AbsoluteRange lineRange) : LineNode(lineNumber, lineRange)
{
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(EmptyLineNode)}(LineNumber = {LineNumber}, LineRange = {LineRange})";
}