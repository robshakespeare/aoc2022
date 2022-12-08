namespace AoC.Day08;
using MoreLinq;

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
                var treeHeight = grid[y][x];
                var isVisible = false;
                foreach (var direction in GridUtils.DirectionsExcludingDiagonal)
                {
                    var trees = GetTrees(grid, new Vector2(x, y), direction);

                    isVisible |= trees.TakeWhile(otherTree => otherTree < treeHeight).Count() == trees.Count;
                }

                var position = new Vector2(x, y);

                
                //var isVisible = GridUtils.DirectionsExcludingDiagonal.Any(direction => IsVisible(grid, position, direction, treeHeight));

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
        var grid = input.ReadLines().Select(line => line.Select(c => int.Parse($"{c}")).ToArray()).ToArray();
        DisplayGrid(grid);

        var width = grid[0].Length;
        var height = grid.Length;

        var visibilities = new List<(Vector2, int)>();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var treeHeight = grid[y][x];
                //var isVisible = false;
                var listVisible = new List<int[]>();
                foreach (var direction in GridUtils.DirectionsExcludingDiagonal)
                {
                    var trees = GetTrees(grid, new Vector2(x, y), direction);

                    //isVisible |= trees.TakeWhile(otherTree => otherTree < treeHeight).Count() == trees.Count;
                    //var reached = false;
                    var them = trees.TakeUntil(otherTree => otherTree >= treeHeight).ToArray();
                    listVisible.Add(them);
                }

                var position = new Vector2(x, y);


                //var isVisible = GridUtils.DirectionsExcludingDiagonal.Any(direction => IsVisible(grid, position, direction, treeHeight));

                visibilities.Add((position, listVisible.Aggregate(1, (agg, cur) => agg * cur.Length)));
                //if (isVisible)
                //{
                    
                //}
            }
        }

        //Console.WriteLine();

        //foreach (var row in visibilities.ToStringGrid(x => x.Item1, x => x.Item2 ? '1' : '0', ' '))
        //{
        //    Console.WriteLine(row);
        //}

        return visibilities.Max(x => x.Item2);
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

    static IReadOnlyList<int> GetTrees(int[][] grid, Vector2 position, Vector2 direction)
    {
        var trees = new List<int>();
        while (!IsOutOfBounds(grid, position += direction))
        {
            var tree = grid[(int) position.Y][(int) position.X];
            trees.Add(tree);
        }

        return trees;

        //if (IsEdge(grid, position))
        //{
        //    return true;
        //}

        //var nextPos = position + direction;

        //var nextHeight = grid[(int)nextPos.Y][(int)nextPos.X];

        //var isVisible2 = nextHeight < height && IsVisible2(grid, nextPos, direction, height);

        //if (isVisible2)
        //{

        //}

        //return isVisible2;
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

    public static bool IsOutOfBounds(int[][] grid, Vector2 position)
    {
        var y = (int)position.Y;

        if (y < 0 || y >= grid.Length)
            return true;

        var x = (int)position.X;
        var line = grid[y];
        return x < 0 || x >= line.Length;
    }

    public void DisplayGrid(int[][] grid)
    {
        foreach (var row in grid)
        {
            Console.WriteLine(string.Concat(row));
        }
    }
}
