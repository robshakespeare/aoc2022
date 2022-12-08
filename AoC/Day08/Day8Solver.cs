namespace AoC.Day08;

public class Day8Solver : ISolver
{
    public string DayName => "Treetop Tree House";

    public long? SolvePart1(PuzzleInput input)
    {
        //input.ReadLines().Select((line, y) => line.Select((c, x) => (new Vector2())))

        var grid = input.ReadLines().Select(line => line.Select(c => int.Parse($"{c}")).ToArray()).ToArray();
        DisplayGrid(grid);

        var width = grid[0].Length;
        var height = grid.Length;

        var visibilities = new List<(Vector2, bool)>();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var position = new Vector2(x, y);

                var treeHeight = grid[y][x];
                var isVisible = GridUtils.DirectionsExcludingDiagonal.Any(direction => IsVisible(grid, position, direction, treeHeight));

                visibilities.Add((position, isVisible));
            }
        }

        Console.WriteLine();

        foreach (var row in visibilities.ToStringGrid(x => x.Item1, x => x.Item2 ? '1' : '0', ' '))
        {
            Console.WriteLine(row);
        }

        return visibilities.Count(x => x.Item2);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    static bool IsVisible(int[][] grid, Vector2 position, Vector2 direction, int height)
    {
        if (IsEdge(grid, position))
        {
            return true;
        }

        var nextPos = position + direction;

        var nextHeight = grid[(int) nextPos.Y][(int) nextPos.X];

        return nextHeight < height && IsVisible(grid, nextPos, direction, height);
    }

    public static bool IsEdge(int[][] grid, Vector2 position)
    {
        var y = (int) position.Y;

        if (y < 1 || y >= grid.Length - 1)
            return true;

        var x = (int) position.X;
        var line = grid[y];
        return x < 1 || x >= line.Length - 1;
    }

    public void DisplayGrid(int[][] grid)
    {
        foreach (var row in grid)
        {
            Console.WriteLine(string.Concat(row));
        }
    }
}
