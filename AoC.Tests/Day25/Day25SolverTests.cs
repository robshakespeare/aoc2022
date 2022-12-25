using AoC.Day25;

namespace AoC.Tests.Day25;

public class Day25SolverTests
{
    private readonly Day25Solver _sut = new();

    public record NormalNumberSnafuCounterpart(int Number, string Snafu);

    public static IReadOnlyList<TestCaseData> NormalNumberSnafuCounterparts { get; } = """
                0              0
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
               11             21
               12             22
               13            1==
               14            1=-
               15            1=0
               16            1=1
               17            1=2
               18            1-=
               19            1--
               20            1-0
               21            1-1
               22            1-2
               23            10=
               24            10-
               25            100
               61            221
               62            222
               63           1===
              122           10-2
              123           100=
              124           100-
              125           1000
              312           2222
              313          1====
              625          10000
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
        part1Result.Should().Be("20=212=1-12=200=00-1");
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Contain("The hot air balloons quickly carry you to the North Pole.");
        part2Result.Should().Contain("Advent of Code 2022 is complete.");
        part2Result.Should().Contain("Merry Christmas & Happy New Year!");
    }
}
