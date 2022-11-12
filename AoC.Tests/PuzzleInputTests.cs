namespace AoC.Tests;

public class PuzzleInputTests
{
    // Carriage Return = \r
    // Line Feed = \n

    [Test]
    public void Ctor_Does_NormalizeLineEndings_LineFeed()
    {
        // ACT
        var result = new PuzzleInput("test\nvalue\nhere");

        // ASSERT
        result.ToString().Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void Ctor_Does_NormalizeLineEndings_CarriageReturn()
    {
        // ACT
        var result = new PuzzleInput("test\rvalue\rhere");

        // ASSERT
        result.ToString().Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void Ctor_Does_NormalizeLineEndings_CarriageReturnLineFeed()
    {
        // ACT
        var result = new PuzzleInput("test\r\nvalue\r\nhere");

        // ASSERT
        result.ToString().Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void Ctor_Does_NormalizeLineEndings_Mixture()
    {
        // ACT
        var result = new PuzzleInput("test\r\n\nvalue\r\r\n\nhere");

        // ASSERT
        result.ToString().Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "",
            "value",
            "",
            "",
            "here");
    }

    [Test]
    public void Ctor_Does_NormalizeLineEndings_MultipleBlankLines()
    {
        // ACT
        var result = new PuzzleInput("\r\n\r\n\r\n\n\n\n\r\r\rhello world");

        // ASSERT
        result.ToString().Should().Be($"{string.Join("", Enumerable.Repeat(Environment.NewLine, 9))}hello world");
    }

    [Test]
    public void Ctor_Does_NormalizeLineEndings_WhenInputNull_ReturnsEmptyString()
    {
        // ACT
        var result = new PuzzleInput(null);

        // ASSERT
        result.ToString().Should().BeEmpty();
    }

    [Test]
    public void Ctor_Does_TrimTrailingWhitespace()
    {
        // ACT
        var result = new PuzzleInput("  hello\nworld \t \r\n\r\n\r\n\n\n\n\r\r\r");

        // ASSERT
        result.ToString().Should().BeEquivalentTo($"  hello{Environment.NewLine}world");
    }

    [Test]
    public void ReadLines_DoesParseEachLineOfStringIntoArrayElements_And_DoesNormalizeLineEndings()
    {
        // ACT
        var result = new PuzzleInput("hello\nworld\r\n\r\nthis\ris\r\na\ntest").ReadLines();

        // ASSERT
        result.Should().BeEquivalentTo(
            new []
            {
                "hello",
                "world",
                "",
                "this",
                "is",
                "a",
                "test"
            },
            opts => opts.WithStrictOrdering());
    }

    [Test]
    public void ReadLines_DoesTrimTrailingLineEndings()
    {
        // ACT
        var result = new PuzzleInput("hello\r\n\r\n\n\n\r\nworld\r\n\n\n\r\n").ReadLines();

        // ASSERT
        result.Should().BeEquivalentTo(
            new []
            {
                "hello",
                "",
                "",
                "",
                "",
                "world"
            },
            opts => opts.WithStrictOrdering());
    }

    [Test]
    public void ReadLines_DoesTrimTrailingLineEndings_AndResultInAnEmptyCollectionIfInputIsJustNewLines()
    {
        // ACT
        var result = new PuzzleInput("\r\n\r\n\r\n\n\n\r\n").ReadLines();

        // ASSERT
        result.Should().BeEmpty();
    }

    [Test]
    public void ReadLines_WhenInputNull_ReturnsEmptyCollection()
    {
        // ACT
        var result = new PuzzleInput(null).ReadLines().ToArray();

        // ASSERT
        result.Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("  \t  ")]
    public void ReadLinesAsLongs_WhenInputIsEmpty_ReturnsEmptyCollection(string? input)
    {
        // ACT
        var result = new PuzzleInput(input).ReadLinesAsLongs().ToArray();

        // ASSERT
        result.Should().BeEmpty();
    }

    [Test]
    public void ReadLinesAsLongs_Does_ReadAndReturnsCollectionOfLongs_AsExpected()
    {
        const string input = @"1234
3147483647
4375734798348934
87654";

        // ACT
        var result = new PuzzleInput(input).ReadLinesAsLongs().ToArray();

        // ASSERT
        result.Should().BeEquivalentTo(new[]
        {
            1234,
            3147483647,
            4375734798348934,
            87654
        }, opts => opts.WithStrictOrdering());
    }
}
