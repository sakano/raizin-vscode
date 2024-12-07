using System;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServerTests;

[TestFixture(TestOf = typeof(SpanLineEnumerator))]
public class SpanLineEnumeratorTest
{
    [Test]
    public void TestEmptySpan()
    {
        var enumerator = new SpanLineEnumerator(ReadOnlySpan<char>.Empty);
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new Range(0, 0)));

        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void TestSingleLine()
    {
        var text = "This is a single line";
        var enumerator = new SpanLineEnumerator(text.AsSpan());

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new Range(0, text.Length)));
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo(text));

        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void TestMultipleLines()
    {
        var text = "First line\nSecond line\r\nThird line\rFourth line";
        var enumerator = new SpanLineEnumerator(text.AsSpan());

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo("First line"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo("Second line"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo("Third line"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo("Fourth line"));

        Assert.That(enumerator.MoveNext(), Is.False);
    }

    [Test]
    public void TestTrailingNewline()
    {
        var text = "Line with trailing newline\n";
        var enumerator = new SpanLineEnumerator(text.AsSpan());

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo("Line with trailing newline"));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(text.AsSpan(enumerator.Current).ToString(), Is.EqualTo(""));

        Assert.That(enumerator.MoveNext(), Is.False);
    }
}