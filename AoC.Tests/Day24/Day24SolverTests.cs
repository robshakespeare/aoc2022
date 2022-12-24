using AoC.Day24;

namespace AoC.Tests.Day24;

public class Day24SolverTests
{
    private readonly Day24Solver _sut = new();

    private const string ExampleInput = """
        #.######
        #>>.<^<#
        #.<..<<#
        #>v.><>#
        #<^v^^>#
        ######.#
        """;

    [Test]
    public void MapSuccessorTests()
    {
        var initialMap = Day24Solver.Map.Parse("""
            #.#####
            #.....#
            #>....#
            #.....#
            #...v.#
            #.....#
            #####.#
            """);

        // ACT
        Console.WriteLine("=== Initial Grid ===");
        Console.WriteLine(initialMap.ToString());
        Console.WriteLine();

        var map = initialMap;
        for (var i = 1; i <= 5; i++)
        {
            map = map.Successor();

            Console.WriteLine($"=== After minute {i} ===");
            Console.WriteLine(map.ToString());
            Console.WriteLine();

            if (i < 5)
            {
                map.ToString().Should().NotBe(initialMap.ToString());
            }
        }

        // ASSERT
        map.ToString().Should().Be("""
            #.#####
            #.....#
            #>....#
            #.....#
            #...v.#
            #.....#
            #####.#
            """.ReplaceLineEndings());
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(18);
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
