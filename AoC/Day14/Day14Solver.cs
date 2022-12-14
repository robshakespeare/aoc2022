using System.Linq;

namespace AoC.Day14;

public class Day14Solver : ISolver
{
    public string DayName => "Regolith Reservoir";

    static readonly Vector2 StartPoint = new(500, 0);

    public long? SolvePart1(PuzzleInput input)
    {
        var map = ParseMap(input);
        var grid = map.ToGrid((pos, chr) => new Cell(pos, chr));
        var gridStartPoint = new Vector2(grid[0].Select((c, x) => (c, x)).Single(p => p.c.Char == '+').x, 0); // rs-todo: yuk!

        // Simulate the sand pouring until sand starts flowing into the abyss below
        // How many units of sand come to rest before sand starts flowing into the abyss below?
        var abyssReached = false;
        var restingSandCount = 0;
        var possibleDirections = new[]
        {
            GridUtils.South,
            GridUtils.South + GridUtils.West,
            GridUtils.South + GridUtils.East
        };

        while (!abyssReached)
        {
            var position = gridStartPoint;
            var comeToRest = false;

            while (!abyssReached && !comeToRest)
            {
                var nextCell = possibleDirections
                    .Select(dir => position + dir)
                    .Select(nextPosition => grid.SafeGet(nextPosition) ?? new Cell(nextPosition, '.') {IsOutOfBounds = true})
                    .FirstOrDefault(cell => cell.IsAir);

                if (nextCell == null)
                {
                    comeToRest = true;
                    restingSandCount++;
                    grid[(int)position.Y][(int)position.X].Char = 'o';
                }
                else if (nextCell.IsOutOfBounds)
                {
                    abyssReached = true;
                }
                else
                {
                    position = nextCell.Position;
                }
            }
        }

        return restingSandCount;
    }

    class Cell
    {
        public Vector2 Position { get; }
        public char Char { get; set; }
        public bool IsOutOfBounds { get; init; }
        public bool IsAir => Char == '.';

        public Cell(Vector2 position, char chr)
        {
            Position = position;
            Char = chr;
        }
    }

    public long? SolvePart2(PuzzleInput input)
    {
        //var map = ParseMap(input);
        var grid =
            input.ReadLines()
                .Select(ParseLineToPath)
                .SelectMany(PlotPath)
                .Distinct()
                //.SelectMany((line, y) => line.Select((chr, x) => (chr, pos: new Vector2(x, y))))
                .ToDictionary(x => x, x => new Cell(x, '#'));
        //var gridStartPoint = new Vector2(grid[0].Select((c, x) => (c, x)).Single(p => p.c.Char == '+').x, 0); // rs-todo: yuk!

        var gridStartPoint = StartPoint;

        // Simulate the sand pouring until sand starts flowing into the abyss below
        // How many units of sand come to rest before sand starts flowing into the abyss below?
        var abyssReached = false;
        var restingSandCount = 0;
        var possibleDirections = new[]
        {
            GridUtils.South,
            GridUtils.South + GridUtils.West,
            GridUtils.South + GridUtils.East
        };

        var height = grid.Values.Max(x => (int)x.Position.Y);
        var maxHeight = height + 2;

        //grid.ToStringGrid(x => x.Key, x => x.Value.Char, 'X')
        //    .RenderGridToConsole();
        //return null;

        while (!abyssReached)
        {
            var position = gridStartPoint;
            var comeToRest = false;

            while (!abyssReached && !comeToRest)
            {
                var nextCell = possibleDirections
                    .Select(dir => position + dir)
                    //.Select(nextPosition => grid.SafeGet(nextPosition) ?? new Cell(nextPosition, ((int)nextPosition.Y) == maxHeight ? '#' : '.') { IsOutOfBounds = false })

                    .Select(nextPosition => grid.TryGetValue(nextPosition, out var cell)
                                            ? cell
                                            : new Cell(nextPosition, ((int)nextPosition.Y) == maxHeight ? '#' : '.') { IsOutOfBounds = false })

                    .FirstOrDefault(cell => cell.IsAir);

                if (nextCell == null)
                {
                    if (position == gridStartPoint)
                    {
                        abyssReached = true;
                    }

                    comeToRest = true;
                    restingSandCount++;
                    //grid[(int)position.Y][(int)position.X].Char = 'o';

                    grid[position] = new Cell(position, 'o');

                    //if (restingSandCount == 93)
                    //{
                    //    grid.ToStringGrid(x => x.Key, x => x.Value.Char, '.')
                    //            .RenderGridToConsole();
                    //    return null;
                    //}
                }
                else if (nextCell.IsOutOfBounds)
                {
                    abyssReached = true;
                }
                else
                {
                    position = nextCell.Position;
                }
            }
        }

        return restingSandCount;
    }

    static string ParseMap(string input) =>
        input.ReadLines()
            .Select(ParseLineToPath)
            .SelectMany(PlotPath)
            .Append(StartPoint)
            .ToStringGrid(x => x, p => p == StartPoint ? '+' : '#', '.')
            .RenderGridToString();

    static IEnumerable<Vector2> PlotPath(IReadOnlyList<Vector2> path)
    {
        var current = path.First();
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

    static IReadOnlyList<Vector2> ParseLineToPath(string line) =>
        line.Split(" -> ")
            .Select(chunk => chunk.Split(","))
            .Select(pair => new Vector2(int.Parse(pair[0]), int.Parse(pair[1])))
            .ToArray();
}
