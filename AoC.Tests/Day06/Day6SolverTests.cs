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

    [Test]
    public void Part1Examples()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(7);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1794);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(19);
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
