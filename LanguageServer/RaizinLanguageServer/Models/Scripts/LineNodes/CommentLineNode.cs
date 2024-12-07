namespace RaizinLanguageServer.Models.Scripts.LineNodes;

/// <summary>
/// コメント行を表す LineNode です。
/// </summary>
public sealed class CommentLineNode(int lineNumber, AbsoluteRange lineRange) : LineNode(lineNumber, lineRange)
{
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(CommentLineNode)}(LineNumber = {LineNumber}, LineRange = {LineRange})";
}