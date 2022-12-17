using AoC.Day17;

namespace AoC.Tests.Day17;

public class Day17SolverTests
{
    private readonly Day17Solver _sut = new();

    private const string ExampleInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

    static Day17SolverTests() => Day17Solver.Logger = TestContext.Progress.WriteLine;

    [Test]
    public void Part1Example()
    {
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
    public void Part1VerifyOptimizationsForExample()
    {
        // ACT
        var result = _sut.SolvePart1(ExampleInput, numRocks: 20000);

        // ASSERT
        result.Should().Be(30288);
    }

    [Test]
    public void Part1VerifyOptimizationsForReal()
    {
        // ACT
        var result = _sut.SolvePart1(_sut.GetInputLoader().PuzzleInputPart1, numRocks: 20000);

        // ASSERT
        result.Should().Be(30821);
    }

    [Test]
    public void Part1Try30K()
    {
        // ACT
        var result = _sut.SolvePart1(_sut.GetInputLoader().PuzzleInputPart1, numRocks: 30000);

        // ASSERT
        result.Should().BeGreaterThan(30288);
    }

    [Test]
    public void Part1Try40K()
    {
        // ACT
        var result = _sut.SolvePart1(_sut.GetInputLoader().PuzzleInputPart1, numRocks: 40000);

        // ASSERT
        result.Should().BeGreaterThan(30288);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1514285714288);
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
