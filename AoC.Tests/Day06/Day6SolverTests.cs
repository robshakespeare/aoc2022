using AoC.Day06;

namespace AoC.Tests.Day06;

public class Day6SolverTests
{
    private readonly Day6Solver _sut = new();

    private const string ExampleInput1 = "mjqjpqmgbljsphdztnvjfqwrcgsmlb";
    private const string ExampleInput2 = "bvwbjplbgvbhsrlpgdmjqwftvncz";
    private const string ExampleInput3 = "nppdvjthqldpwncqszvftbrmjlhg";
    private const string ExampleInput4 = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg";
    private const string ExampleInput5 = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw";

    [TestCase(ExampleInput1, 7)]
    [TestCase(ExampleInput2, 5)]
    [TestCase(ExampleInput3, 6)]
    [TestCase(ExampleInput4, 10)]
    [TestCase(ExampleInput5, 11)]
    public void Part1Examples(string exampleInput, int expectedResult)
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(exampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(expectedResult);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1794);
    }

    [TestCase(ExampleInput1, 19)]
    [TestCase(ExampleInput2, 23)]
    [TestCase(ExampleInput3, 23)]
    [TestCase(ExampleInput4, 29)]
    [TestCase(ExampleInput5, 26)]
    public void Part2Examples(string exampleInput, int expectedResult)
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(exampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(expectedResult);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(2851);
    }
}
