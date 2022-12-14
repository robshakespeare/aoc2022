namespace AoC.Day14;

public class Day14Solver : ISolver
{
    public string DayName => "Regolith Reservoir";

    static readonly Vector2 StartPoint = new(500, 0);

    public long? SolvePart1(PuzzleInput input)
    {
        //var grid = input.ReadLines()
        //    .Select(ParseLineToPath)
        //    .SelectMany(PlotPath)
        //    .Append(new Vector2(500, 0))
        //    .ToStringGrid(x => x, p => p == new Vector2(500, 0) ? '+' : '#', '.')
        //    .RenderGridToConsole();

        var startPoint = new Vector2(500, 0);
        var map = input.ReadLines()
            .Select(ParseLineToPath)
            .SelectMany(PlotPath)
            .Append(startPoint)
            .ToStringGrid(x => x, p => p == startPoint ? '+' : '#', '.')
            .RenderGridToString();

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
                //abyssReached = grid.SafeGet(position) == null;

                //if (!abyssReached)
                //{

                var nextCell = possibleDirections
                    .Select(dir => position + dir)
                    .Select(nextPosition => grid.SafeGet(nextPosition) ?? new Cell(nextPosition, '.') {IsOutOfBounds = true})
                    .FirstOrDefault(cell => cell.IsAir);

                //grid.Select()

                //var nextPosition = grid.GetAdjacent(position, possibleDirections).FirstOrDefault(x => x.IsAir)?.Position;

                if (nextCell == null)
                {
                    comeToRest = true;
                    restingSandCount++;
                    grid[(int)position.Y][(int)position.X].Char = 'o';

                    //if (restingSandCount == 24)
                    //{
                    //    grid.SelectMany(x => x).ToStringGrid(x => x.Position, x => x.Char, 'X')
                    //        .RenderGridToConsole();
                    //    //return null;
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

                //if (grid.SafeGet(position) == null)
                //{
                //    abyssReached = true;
                //}
                //if (nextPosition != null)
                //{
                //    position = nextPosition.Value;
                //}
                //else
                //else
                //{
                //    comeToRest = true;
                //    restingSandCount++;
                //    grid[(int)position.Y][(int)position.X].Char = 'o';

                //    if (restingSandCount == 24)
                //    {
                //        grid.SelectMany(x => x).ToStringGrid(x => x.Position, x => x.Char, 'X')
                //            .RenderGridToConsole();
                //        //return null;
                //    }
                //}

                //}
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

    // rs-todo: try and combine this with the other one!!!
    //static IEnumerable<T> GetAdjacent(IReadOnlyList<IReadOnlyList<char>> grid, Vector2 position, Vector2[] directions)
    //{
    //    foreach (var dir in directions)
    //    {
    //        var adjacent = grid.trygt.(position + dir);
    //        if (adjacent != null)
    //        {
    //            yield return adjacent;
    //        }
    //    }
    //}

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
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
