using AoC.Day25;

namespace AoC.Tests.Day25;

public class Day25SolverTests
{
    private readonly Day25Solver _sut = new();

    private const string ExampleInput = """
        1=-0-2
        12111
        2=0=
        21
        2=01
        111
        20012
        112
        1=-1=
        1-12
        12
        1=
        122
        """;

    public record NormalNumberSnafuCounterpart(int Number, string Snafu);

    public static IReadOnlyList<TestCaseData> NormalNumberSnafuCounterparts { get; } = """
                1              1
                2              2
                3             1=
                4             1-
                5             10
                6             11
                7             12
                8             2=
                9             2-
               10             20
               15            1=0
               20            1-0
              976          2=-01
             2022         1=11-2
            12345        1-0---0
        314159265  1121-1110-1=0
        """.ReadLines().Select(line =>
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new TestCaseData(new NormalNumberSnafuCounterpart(int.Parse(parts[0]), parts[1]));
    }).ToArray();

    [Test]
    public void DoesHaveTestCaseData()
    {
        var testCaseData = NormalNumberSnafuCounterparts;

        Console.WriteLine(string.Join(Environment.NewLine, testCaseData));

        // ASSERT
        testCaseData.Should().NotBeEmpty();
        NormalNumberSnafuCounterparts.Should().BeSameAs(testCaseData);
    }

    [TestCaseSource(nameof(NormalNumberSnafuCounterparts))]
    public void SnafuToNormalNumber_Tests(NormalNumberSnafuCounterpart testCase)
    {
        // ACT
        var resultNumber = Day25Solver.SnafuToNormalNumber(testCase.Snafu);

        // ASSERT
        resultNumber.Should().Be(testCase.Number);
    }

    [TestCaseSource(nameof(NormalNumberSnafuCounterparts))]
    public void NormalNumberToSnafu_Tests(NormalNumberSnafuCounterpart testCase)
    {
        // ACT
        var resultNumber = Day25Solver.NormalNumberToSnafu(testCase.Number);

        // ASSERT
        resultNumber.Should().Be(testCase.Snafu);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be("2=-1=0");
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(null);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(null);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(null);
    }
}
