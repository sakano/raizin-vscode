using System;
using RaizinLanguageServer.Utilities;

namespace RaizinLanguageServerTests;

[TestFixture]
public class StringExtensionsTest
{
    [Test]
    public void TestIsInteger_WithIntegerString_ReturnsTrue()
    {
        Assert.That("12345".IsInteger(), Is.True);
    }

    [Test]
    public void TestIsInteger_WithNonIntegerString_ReturnsFalse()
    {
        Assert.That("123a45".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithEmptyString_ReturnsFalse()
    {
        Assert.That("".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithNegativeString_ReturnsFalse()
    {
        Assert.That("-".IsInteger(), Is.False);
    }


    [Test]
    public void TestIsInteger_WithPositiveString_ReturnsFalse()
    {
        Assert.That("+".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithPositiveIntegerString_ReturnsTrue()
    {
        Assert.That("+12345".IsInteger(), Is.True);
    }

    [Test]
    public void TestIsInteger_WithNegativeIntegerString_ReturnsTrue()
    {
        Assert.That("-12345".IsInteger(), Is.True);
    }

    [Test]
    public void TestIsInteger_WithDoubleNegativeIntegerString_ReturnsFalse()
    {
        Assert.That("--12345".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithWhitespaceString_ReturnsFalse()
    {
        Assert.That("   ".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithUntrimmedString_ReturnsFalse()
    {
        Assert.That("1 ".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithFullWidthIntegerString_ReturnsFalse()
    {
        Assert.That("１２３４５".IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithPositiveIntegerSpan_ReturnsTrue()
    {
        Assert.That("67890".AsSpan().IsInteger(), Is.True);
    }

    [Test]
    public void TestIsInteger_WithNegativeIntegerSpan_ReturnsTrue()
    {
        Assert.That("-67890".AsSpan().IsInteger(), Is.True);
    }

    [Test]
    public void TestIsInteger_WithNonIntegerSpan_ReturnsFalse()
    {
        Assert.That("678a90".AsSpan().IsInteger(), Is.False);
    }

    [Test]
    public void TestIsInteger_WithEmptySpan_ReturnsFalse()
    {
        Assert.That(ReadOnlySpan<char>.Empty.IsInteger(), Is.False);
    }
}