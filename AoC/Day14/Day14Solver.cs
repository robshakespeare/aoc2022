namespace AoC.Day14;

public class Day14Solver : ISolver, IVisualize
{
    public string DayName => "Regolith Reservoir";

    static readonly Vector2 SandSource = new(500, 0);

    static readonly Vector2[] PossibleDirections =
    {
        GridUtils.South,
        GridUtils.South + GridUtils.West,
        GridUtils.South + GridUtils.East
    };

    const char SourceChar = '+';
    const char RockChar = '#';
    const char AirChar = '.';
    const char SandChar = 'o';

    /// <summary>
    /// How many units of sand come to rest before sand starts flowing into the abyss below?
    /// </summary>
    public long? SolvePart1(PuzzleInput input) => SimulateSandPouring(ParseMap(input, includeFloor: false)).RestingSandCount;

    /// <summary>
    /// How many units of sand come to rest before the source of the sand becomes blocked?
    /// </summary>
    public long? SolvePart2(PuzzleInput input) => SimulateSandPouring(ParseMap(input, includeFloor: true)).RestingSandCount;

    /// <summary>
    /// Simulate the sand pouring until sand starts flowing into the abyss below or the source of the sand becomes blocked.
    /// Returns how many units of sand come to rest.
    /// </summary>
    static (int RestingSandCount, List<string> Frames) SimulateSandPouring(string map, bool renderFrames = false)
    {
        var grid = map.ToGrid((pos, chr) => new Cell(pos, chr));
        var sandSource = new Vector2(x: grid[0].Select(c => c.Char).ToList().IndexOf(SourceChar), y: 0);
        var abyssReached = false;
        var sourceBlocked = false;
        var restingSandCount = 0;

        var height = grid.Length;
        var width = grid[0].Length;
        var frames = new List<string>();

        void Render() => grid.SelectMany(line => line).RenderWorldToViewport(x => x.Position, x => x.Char, AirChar, width, height);

        while (!abyssReached && !sourceBlocked)
        {
            var position = sandSource;
            var comeToRest = false;

            while (!abyssReached && !comeToRest)
            {
                var nextAirSpace = GetNextAirSpace(grid, position);

                if (nextAirSpace == null)
                {
                    comeToRest = true;
                    restingSandCount++;
                    grid[(int) position.Y][(int) position.X].Char = SandChar;

                    if (position == sandSource) sourceBlocked = true;
                    if (renderFrames) Render();
                }
                else if (nextAirSpace.IsOutOfBounds) abyssReached = true;
                else position = nextAirSpace.Position;
            }
        }

        return (restingSandCount, frames);
    }

    class Cell
    {
        public Vector2 Position { get; }
        public char Char { get; set; }
        public bool IsOutOfBounds { get; init; }
        public bool IsAir => Char == AirChar;

        public Cell(Vector2 position, char chr)
        {
            Position = position;
            Char = chr;
        }
    }

    static Cell? GetNextAirSpace(Cell[][] grid, Vector2 position)
    {
        foreach (var direction in PossibleDirections)
        {
            var nextPosition = position + direction;
            var x = (int) nextPosition.X;
            var y = (int) nextPosition.Y;

            var outOfBounds = y >= grid.Length || x < 0 || x >= grid[y].Length;
            if (outOfBounds)
            {
                return new Cell(nextPosition, AirChar) {IsOutOfBounds = true};
            }

            var nextCell = grid[y][x];
            if (nextCell.IsAir)
            {
                return nextCell;
            }
        }

        return null;
    }

    static string ParseMap(string input, bool includeFloor)
    {
        var rocks = input.ReadLines()
            .Select(ParseLineToPath)
            .SelectMany(PlotPath)
            .ToArray();

        if (includeFloor)
        {
            var height = rocks.Max(rock => (int) rock.Y);
            var floorY = height + 2;
            var floorWidth = floorY + 1 + floorY;

            rocks = rocks.Concat(PlotPath(new[]
            {
                new Vector2(SandSource.X - (int) (floorWidth / 2f), floorY),
                new Vector2(SandSource.X + (int) (floorWidth / 2f), floorY)
            })).ToArray();
        }

        return rocks
            .Append(SandSource)
            .ToStringGrid(p => p, p => p == SandSource ? SourceChar : RockChar, AirChar)
            .RenderGridToString();
    }

    static IEnumerable<Vector2> PlotPath(Vector2[] path)
    {
        var current = path[0];
        yield return current;

        foreach (var target in path.Skip(1))
        {
            var dir = Vector2.Normalize(target - current);
            while (current != target)
            {
                current += dir;
                yield return current;
            }
        }
    }

    static Vector2[] ParseLineToPath(string line) => line.Split(" -> ")
        .Select(chunk => chunk.Split(","))
        .Select(pair => new Vector2(int.Parse(pair[0]), int.Parse(pair[1])))
        .ToArray();

    public async IAsyncEnumerable<string> GetVisualizationAsync(PuzzleInput input)
    {
        var frames = SimulateSandPouring(ParseMap(input, includeFloor: false), renderFrames: true).Frames;

        foreach (var frame in frames)
        {
            yield return frame;
            await Task.Delay(1);
        }

        //var map = ParseMap(input, includeFloor: false);
        //var grid = map.ToGrid((pos, chr) => new Cell(pos, chr));
        //var sandSource = new Vector2(x: grid[0].Select(c => c.Char).ToList().IndexOf(SourceChar), y: 0);
        //var abyssReached = false;
        //var sourceBlocked = false;
        ////var restingSandCount = 0;

        //var height = grid.Length;
        //var width = grid[0].Length;

        //string Render(Cell[][] grid) => grid.SelectMany(line => line).RenderWorldToViewport(x => x.Position, x => x.Char, AirChar, width, height);

        //while (!abyssReached && !sourceBlocked)
        //{
        //    var position = sandSource;
        //    var comeToRest = false;

        //    while (!abyssReached && !comeToRest)
        //    {
        //        var nextAirSpace = GetNextAirSpace(grid, position);

        //        if (nextAirSpace == null)
        //        {
        //            comeToRest = true;
        //            //restingSandCount++;
        //            grid[(int)position.Y][(int)position.X].Char = SandChar;

        //            yield return Render(grid);
        //            await Task.Delay(1);

        //            if (position == sandSource)
        //            {
        //                sourceBlocked = true;
        //            }
        //        }
        //        else if (nextAirSpace.IsOutOfBounds)
        //        {
        //            abyssReached = true;
        //        }
        //        else
        //        {
        //            position = nextAirSpace.Position;
        //        }
        //    }
        //}

        //var newLine = Environment.NewLine;

        //string Render(Cell[][] grid) => grid.SelectMany(line => line).ToStringGrid(x => x.Position, x => x.Char, AirChar).RenderGridToString();
        //string RenderFrame(string message, string grid) => $"{message}{newLine}{newLine}{grid}{newLine}";

        //var part1Frames = new List<string>();
        //SimulateSandPouring(ParseMap(input, includeFloor: false), grid => part1Frames.Add(Render(grid)));

        //var count = 0;
        //foreach (var part1Frame in part1Frames)
        //{
        //    yield return RenderFrame("Part 1 (without floor):", part1Frame);
        //    await Task.Delay(1);
        //    count++;

        //    if (count > 30)
        //    {
        //        break;
        //    }
        //}

        //var part1 = RenderFrame("Part 1 (without floor):", part1Frames[^1]);
        //yield return $"{part1}{newLine}Rendering part 2...";
        //await Task.Delay(1);

        //var part2FinalFrame = Render(SimulateSandPouring(ParseMap(input, includeFloor: true)).Grid);
        //var part2 = RenderFrame("Part 2 (with floor):", part2FinalFrame);
        //yield return $"{part1}{newLine}{part2}";
    }
}
