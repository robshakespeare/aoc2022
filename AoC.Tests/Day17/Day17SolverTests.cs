using AoC.Day17;

namespace AoC.Tests.Day17;

public class Day17SolverTests
{
    private readonly Day17Solver _sut = new();

    private const string ExampleInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

    [Test]
    public void Part1Example()
    {
        Day17Solver.Logger = TestContext.Progress.WriteLine;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(3068);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(3109);
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
