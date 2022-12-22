using AoC.Day22;

namespace AoC.Tests.Day22;

public class Day22SolverTests
{
    private readonly Day22Solver _sut = new();

    private const string ExampleInput = """
                ...#
                .#..
                #...
                ....
        ...#.......#
        ........#...
        ..#....#....
        ..........#.
                ...#....
                .....#..
                .#......
                ......#.

        10R5L5R10L4R5L5
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(6032);
    }

    [Test]
    public void Part1Example_ResultMapIsAsExpected()
    {
        // ACT
        var part1ExampleResult = Day22Solver.Map.Create(ExampleInput.ReplaceLineEndings(), isCube: false).FollowInstructions();

        var map = part1ExampleResult.Map.Cells.ToStringGrid(x => x.Key, x => x.Value.Tile, ' ').RenderGridToConsole();

        // ASSERT
        map.Should().Be("""
                    >>v#    
                    .#v.    
                    #.v.    
                    ..v.    
            ...#...v..v#    
            >>>v...>#.>>    
            ..#v...#....    
            ...>>>>v..#.    
                    ...#....
                    .....#..
                    .#......
                    ......#.
            """.ReplaceLineEndings());
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1428);
    }

    //[Test]
    //public void Part2Example()
    //{
    //    // ACT
    //    var part2ExampleResult = _sut.SolvePart2(ExampleInput);

    //    // ASSERT
    //    part2ExampleResult.Should().Be(5031);
    //}

    //[Test]
    //public void Part2Example_ResultMapIsAsExpected()
    //{
    //    // ACT
    //    var part2ExampleResult = Day22Solver.Map.Create(ExampleInput.ReplaceLineEndings(), isCube: true).FollowInstructions();

    //    var map = part2ExampleResult.Map.Cells.ToStringGrid(x => x.Key, x => x.Value.Tile, ' ').RenderGridToConsole();

    //    // ASSERT
    //    map.Should().Be("""
    //                >>v#    
    //                .#v.    
    //                #.v.    
    //                ..v.    
    //        ...#..^...v#    
    //        .>>>>>^.#.>>    
    //        .^#....#....    
    //        .^........#.    
    //                ...#..v.
    //                .....#v.
    //                .#v<<<<.
    //                ..v...#.
    //        """.ReplaceLineEndings());
    //}

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(142380);
    }

    public class RotationExploration
    {
        [Test]
        public void RotateClockwiseTests()
        {
            // right -- East
            // down -- South
            // left -- West
            // up -- North

            // R = clockwise

            var dir = GridUtils.East;
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.East);

            dir = MathUtils.RotateDirection(dir, 90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.South);

            dir = MathUtils.RotateDirection(dir, 90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.West);

            dir = MathUtils.RotateDirection(dir, 90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.North);

            dir = MathUtils.RotateDirection(dir, 90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.East);
        }

        [Test]
        public void RotateCounterclockwiseTests()
        {
            // right -- East
            // down -- South
            // left -- West
            // up -- North

            // L = counterclockwise

            var dir = GridUtils.East;
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.East);

            dir = MathUtils.RotateDirection(dir, -90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.North);

            dir = MathUtils.RotateDirection(dir, -90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.West);

            dir = MathUtils.RotateDirection(dir, -90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.South);

            dir = MathUtils.RotateDirection(dir, -90);
            Console.WriteLine(dir);
            dir.Should().Be(GridUtils.East);
        }
    }
}
