using AoC.Day23;

namespace AoC.Tests.Day23;

public class Day23SolverTests
{
    private readonly Day23Solver _sut = new();

    static Day23SolverTests() => Day23Solver.Logger = TestContext.Progress.WriteLine;

    private const string ExampleInput = """
        ....#..
        ..###.#
        #...#.#
        .#...##
        #.###..
        ##.#.##
        .#..#..
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(110);
    }

    [Test]
    public void Part1TinyExample()
    {
        // ACT
        var elvesGrid = Day23Solver.Simulate(Day23Solver.ParseElves("""
            .....
            ..##.
            ..#..
            .....
            ..##.
            .....
            """));

        var result = elvesGrid.ToStringGrid(x => x.Key, _ => '#', '.').RenderGridToString();

        // ASSERT
        result.Should().Be("""
            ..#..
            ....#
            #....
            ....#
            .....
            ..#..
            """.ReplaceLineEndings());
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
