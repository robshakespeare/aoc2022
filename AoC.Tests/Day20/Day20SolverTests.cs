using AoC.Day19;
using AoC.Day20;

namespace AoC.Tests.Day20;

public class Day20SolverTests
{
    private readonly Day20Solver _sut = new();

    static Day20SolverTests() => Day20Solver.Logger = TestContext.Progress.WriteLine;

    private const string ExampleInput = """
        1
        2
        -3
        3
        -2
        0
        4
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(3);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(7713);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1623178306);
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
