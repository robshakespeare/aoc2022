namespace AoC.Tests;

public class GeneralExtensionsTests
{
    [Test]
    public void ToEnumerable_Test()
    {
        (5..10).ToEnumerable().Should().BeEquivalentTo(
            new[] {5, 6, 7, 8, 9},
            opts => opts.WithStrictOrdering());
    }

    [Test]
    public void ToArray_Test()
    {
        (0..4).ToArray().Should().BeEquivalentTo(
            new[] {0, 1, 2, 3},
            opts => opts.WithStrictOrdering());
    }

    public class TheNormalizeLineEndingsMethod
    {
        // Carriage Return = \r
        // Line Feed = \n

        [Test]
        public void Does_NormalizeLineEndings_LineFeed()
        {
            // ACT
            var result = "test\nvalue\nhere".NormalizeLineEndings();

            // ASSERT
            result.Split(Environment.NewLine).Should().BeEquivalentTo(
                "test",
                "value",
                "here");
        }

        [Test]
        public void Does_NormalizeLineEndings_CarriageReturn()
        {
            // ACT
            var result = "test\rvalue\rhere".NormalizeLineEndings();

            // ASSERT
            result.Split(Environment.NewLine).Should().BeEquivalentTo(
                "test",
                "value",
                "here");
        }

        [Test]
        public void Does_NormalizeLineEndings_CarriageReturnLineFeed()
        {
            // ACT
            var result = "test\r\nvalue\r\nhere".NormalizeLineEndings();

            // ASSERT
            result.Split(Environment.NewLine).Should().BeEquivalentTo(
                "test",
                "value",
                "here");
        }

        [Test]
        public void Does_NormalizeLineEndings_Mixture()
        {
            // ACT
            var result = "test\r\n\nvalue\r\r\n\nhere".NormalizeLineEndings();

            // ASSERT
            result.Split(Environment.NewLine).Should().BeEquivalentTo(
                "test",
                "",
                "value",
                "",
                "",
                "here");
        }

        [Test]
        public void Does_NormalizeLineEndings_MultipleBlankLines()
        {
            // ACT
            var result = "\r\n\r\n\r\n\n\n\n\r\r\rhello world".NormalizeLineEndings();

            // ASSERT
            result.Should().Be($"{string.Join("", Enumerable.Repeat(Environment.NewLine, 9))}hello world");
        }

        [Test]
        public void Does_NormalizeLineEndings_WhenInputNull_ReturnsEmptyString()
        {
            // ACT
            var result = ((string?) null).NormalizeLineEndings();

            // ASSERT
            result.Should().BeEmpty();
        }
    }


    public class TheAddOrIncrementMethod
    {
        [Test]
        public void AddOrIncrement_DoesAddIfNoKey()
        {
            var dict = new Dictionary<string, long>
            {
                {"example", 12}
            };

            // ACT
            dict.AddOrIncrement("example2", 34);

            // ASSERT
            dict["example"].Should().Be(12);
            dict["example2"].Should().Be(34);
        }

        [Test]
        public void AddOrIncrement_DoesIncrementIfHasKey()
        {
            var dict = new Dictionary<string, long>
            {
                {"example", 12}
            };

            // ACT
            dict.AddOrIncrement("example", 34);

            // ASSERT
            dict["example"].Should().Be(12 + 34);
        }
    }
}
