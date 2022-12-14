using AoC.Day10;

namespace AoC.Tests.Day10;

public class Day10SolverTests
{
    private readonly Day10Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(13140);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(13720);
    }

    [Test]
    public void Part2Example()
    {
        var expected = """
            ##..##..##..##..##..##..##..##..##..##..
            ###...###...###...###...###...###...###.
            ####....####....####....####....####....
            #####.....#####.....#####.....#####.....
            ######......######......######......####
            #######.......#######.......#######.....
            """.Replace('.', ' ').Replace('#', '█').ReplaceLineEndings();

        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(expected);
    }

    [Test]
    public void Part2ReTest()
    {
        var expected1 = """
            ####.###..#..#.###..#..#.####..##..#..#.
            #....#..#.#..#.#..#.#..#....#.#..#.#..#.
            ###..###..#..#.#..#.####...#..#....####.
            #....#..#.#..#.###..#..#..#...#....#..#.
            #....#..#.#..#.#.#..#..#.#....#..#.#..#.
            #....###...##..#..#.#..#.####..##..#..#.
            """.Replace('.', ' ').Replace('#', '█').ReplaceLineEndings();

        var expected2 = """
            ████ ███  █  █ ███  █  █ ████  ██  █  █ 
            █    █  █ █  █ █  █ █  █    █ █  █ █  █ 
            ███  ███  █  █ █  █ ████   █  █    ████ 
            █    █  █ █  █ ███  █  █  █   █    █  █ 
            █    █  █ █  █ █ █  █  █ █    █  █ █  █ 
            █    ███   ██  █  █ █  █ ████  ██  █  █ 
            """.ReplaceLineEndings();

        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(expected1);
        part2Result.Should().Be(expected2);
    }

    private const string ExampleInput = """
        addx 15
        addx -11
        addx 6
        addx -3
        addx 5
        addx -1
        addx -8
        addx 13
        addx 4
        noop
        addx -1
        addx 5
        addx -1
        addx 5
        addx -1
        addx 5
        addx -1
        addx 5
        addx -1
        addx -35
        addx 1
        addx 24
        addx -19
        addx 1
        addx 16
        addx -11
        noop
        noop
        addx 21
        addx -15
        noop
        noop
        addx -3
        addx 9
        addx 1
        addx -3
        addx 8
        addx 1
        addx 5
        noop
        noop
        noop
        noop
        noop
        addx -36
        noop
        addx 1
        addx 7
        noop
        noop
        noop
        addx 2
        addx 6
        noop
        noop
        noop
        noop
        noop
        addx 1
        noop
        noop
        addx 7
        addx 1
        noop
        addx -13
        addx 13
        addx 7
        noop
        addx 1
        addx -33
        noop
        noop
        noop
        addx 2
        noop
        noop
        noop
        addx 8
        noop
        addx -1
        addx 2
        addx 1
        noop
        addx 17
        addx -9
        addx 1
        addx 1
        addx -3
        addx 11
        noop
        noop
        addx 1
        noop
        addx 1
        noop
        noop
        addx -13
        addx -19
        addx 1
        addx 3
        addx 26
        addx -30
        addx 12
        addx -1
        addx 3
        addx 1
        noop
        noop
        noop
        addx -9
        addx 18
        addx 1
        addx 2
        noop
        noop
        addx 9
        noop
        noop
        noop
        addx -1
        addx 2
        addx -37
        addx 1
        addx 3
        noop
        addx 15
        addx -21
        addx 22
        addx -6
        addx 1
        noop
        addx 2
        addx 1
        noop
        addx -10
        noop
        noop
        addx 20
        addx 1
        addx 2
        addx 2
        addx -6
        addx -11
        noop
        noop
        noop
        """;
}
