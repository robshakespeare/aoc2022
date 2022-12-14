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
    static (int RestingSandCount, Cell[][] Grid) SimulateSandPouring(string map)
    {
        var grid = map.ToGrid((pos, chr) => new Cell(pos, chr));
        var sandSource = new Vector2(x: grid[0].Select(c => c.Char).ToList().IndexOf(SourceChar), y: 0);
        var abyssReached = false;
        var sourceBlocked = false;
        var restingSandCount = 0;

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

                    if (position == sandSource)
                    {
                        sourceBlocked = true;
                    }
                }
                else if (nextAirSpace.IsOutOfBounds)
                {
                    abyssReached = true;
                }
                else
                {
                    position = nextAirSpace.Position;
                }
            }
        }

        return (restingSandCount, grid);
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
        var newLine = Environment.NewLine;

        string Render(string message, bool includeFloor)
        {
            var grid = SimulateSandPouring(ParseMap(input, includeFloor))
                .Grid.SelectMany(line => line).ToStringGrid(x => x.Position, x => x.Char, AirChar).RenderGridToString();
            return $"{message}{newLine}{newLine}{grid}{newLine}";
        }

        var part1 = Render("Part 1 (without floor):", includeFloor: false);
        yield return $"{part1}{newLine}Rendering part 2...";
        await Task.Delay(50);
        var part2 = Render("Part 2 (with floor):", includeFloor: true);
        yield return $"{part1}{newLine}{part2}";
    }
}
