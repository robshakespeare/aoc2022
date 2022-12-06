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

    [Test]
    public void ReadLines_DoesParseEachLineOfStringIntoArrayElements_And_DoesNormalizeLineEndings()
    {
        // ACT
        var result = "hello\nworld\r\n\r\nthis\ris\r\na\ntest".ReadLines();

        // ASSERT
        result.Should().BeEquivalentTo(
            new[]
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
    public void ReadLinesAsLongs_Does_ReadAndReturnsCollectionOfLongs_AsExpected()
    {
        const string input = @"1234
3147483647
4375734798348934
87654";

        // ACT
        var result = input.ReadLinesAsLongs().ToArray();

        // ASSERT
        result.Should().BeEquivalentTo(new[]
        {
            1234,
            3147483647,
            4375734798348934,
            87654
        }, opts => opts.WithStrictOrdering());
    }

    /// <summary>
    /// These tests verify that the method inbuilt to .NET does behave the same as the previous custom one.
    /// </summary>
    public class TheReplaceLineEndingsMethod
    {
        // Carriage Return = \r
        // Line Feed = \n

        [Test]
        public void Does_NormalizeLineEndings_LineFeed()
        {
            // ACT
            var result = "test\nvalue\nhere".ReplaceLineEndings();

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
            var result = "test\rvalue\rhere".ReplaceLineEndings();

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
            var result = "test\r\nvalue\r\nhere".ReplaceLineEndings();

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
            var result = "test\r\n\nvalue\r\r\n\nhere".ReplaceLineEndings();

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
            var result = "\r\n\r\n\r\n\n\n\n\r\r\rhello world".ReplaceLineEndings();

            // ASSERT
            result.Should().Be($"{string.Join("", Enumerable.Repeat(Environment.NewLine, 9))}hello world");
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

    public class TheGetOrAddMethod
    {
        [Test]
        public void GetOrAdd_DoesAdd_AndReturnValue_IfNoKey()
        {
            var dict = new Dictionary<string, long>
            {
                {"example", 45}
            };

            // ACT
            var result = dict.GetOrAdd("example2", () => 78);

            // ASSERT
            result.Should().Be(78);
            dict["example"].Should().Be(45);
            dict["example2"].Should().Be(78);
        }

        [Test]
        public void GetOrAdd_DoesNotAdd_JustReturnsValue_IfHasKey()
        {
            var dict = new Dictionary<string, long>
            {
                {"example", 172}
            };

            var buildValueCallCount = 0;

            // ACT
            var result = dict.GetOrAdd("example", () =>
            {
                buildValueCallCount++;
                return buildValueCallCount;
            });

            // ASSERT
            result.Should().Be(172);
            buildValueCallCount.Should().Be(0);
            dict["example"].Should().Be(172);
        }
    }
}
