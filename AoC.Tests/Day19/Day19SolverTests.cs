using AoC.Day19;

namespace AoC.Tests.Day19;

public class Day19SolverTests
{
    private readonly Day19Solver _sut = new();

    static Day19SolverTests() => Day19Solver.Logger = TestContext.Progress.WriteLine;

    private const string ExampleInput = """
        Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
        Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.
        """;

    [Test]
    [LongRunningTest("2.5 seconds")]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(33);
    }

    [Test]
    public void Part1ExampleBlueprint2()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput.ReadLines().Last());

        // ASSERT
        part1ExampleResult.Should().Be(24);
    }

    [Test]
    [Ignore("Long running. Takes ~20 seconds.")]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1659);
    }

    [Test]
    [Ignore("Very long running. Takes ~4 minutes.")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(6804);
    }
}
