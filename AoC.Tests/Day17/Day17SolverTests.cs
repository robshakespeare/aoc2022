using AoC.Day17;
using static AoC.Day17.Day17Solver;

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
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1514285714288);
    }

    [Test]
    [LongRunningTest("~ 600ms")]
    public void Part2ReTest()
    {
        // ACT
        var part2ResultObject = _sut.SolvePart2();

        // ASSERT
        var part2Result = part2ResultObject.Should().BeOfType<long>().Subject;
        part2Result.Should().BeLessThan(1541449318430);
        part2Result.Should().NotBe(1541449318401);
        part2Result.Should().NotBe(null);
        part2Result.Should().Be(1541449275365);
    }

    [Test]
    public void Try20KForExample()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(ExampleInput, targetNumRocks: 20000);

        // ASSERT
        result.Should().Be(30288);
    }

    [Test]
    public void Try200KForExample()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(ExampleInput, targetNumRocks: 200000);

        // ASSERT
        result.Should().Be(302861);
    }

    [Test]
    [LongRunningTest("~ 200ms")]
    public void Try10KForReal()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(_sut.GetInputLoader().PuzzleInputPart1, targetNumRocks: 20000);

        // ASSERT
        result.Should().Be(30821);
    }

    [Test]
    [LongRunningTest("~ 200ms")]
    public void Try30KForReal()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(_sut.GetInputLoader().PuzzleInputPart1, targetNumRocks: 30000);

        // ASSERT
        result.Should().Be(46218);
    }

    [Test]
    [LongRunningTest("~ 250ms")]
    public void Try40KForReal()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(_sut.GetInputLoader().PuzzleInputPart1, targetNumRocks: 40000);

        // ASSERT
        result.Should().Be(61652);
    }

    [Test]
    [LongRunningTest("~ 300ms")]
    public void Try100KForReal()
    {
        // ACT
        var result = VerticalChamber.BuildAndSimulate(_sut.GetInputLoader().PuzzleInputPart1, targetNumRocks: 100000);

        // ASSERT
        result.Should().Be(154147);
    }
}
