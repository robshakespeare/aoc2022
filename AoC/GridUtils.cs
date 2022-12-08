namespace AoC;

public static class GridUtils
{
    /// <summary>
    /// North direction, i.e. (0, -1)
    /// </summary>
    public static readonly Vector2 North = new(0, -1);

    /// <summary>
    /// East direction, i.e. (1, 0)
    /// </summary>
    public static readonly Vector2 East = new(1, 0);

    /// <summary>
    /// South direction, i.e. (0, 1)
    /// </summary>
    public static readonly Vector2 South = new(0, 1);

    /// <summary>
    /// West direction, i.e. (-1, 0)
    /// </summary>
    public static readonly Vector2 West = new(-1, 0);

    /// <summary>
    /// All directions in a 2D plane, including diagonal.
    /// </summary>
    public static readonly Vector2[] DirectionsIncludingDiagonal =
    {
        new(-1, -1),
        North,
        new(1, -1),

        West,
        East,

        new(-1, 1),
        South,
        new(1, 1)
    };

    /// <summary>
    /// None diagonal directions in a 2D plane (i.e. North, West, East, South).
    /// </summary>
    public static readonly Vector2[] DirectionsExcludingDiagonal = {North, West, East, South};

    /// <summary>
    /// The center spot, i.e. (0, 0), and all directions in a 2D plane, including diagonal.
    /// </summary>
    public static readonly Vector2[] CenterAndDirectionsIncludingDiagonal =
    {
        new(-1, -1),
        North,
        new(1, -1),

        West,
        Vector2.Zero,
        East,

        new(-1, 1),
        South,
        new(1, 1)
    };

    /// <summary>
    /// Rotates the specified grid around its middle point.
    /// Expects the length of each line (width of the grid) to be equal all the way down.
    /// </summary>
    public static IReadOnlyList<string> RotateGrid(IReadOnlyList<string> pixels, int degrees)
    {
        if (degrees % 90 != 0)
        {
            throw new InvalidOperationException($"Only right angle rotations are supported. Rotation of {degrees}° is invalid.");
        }

        return TransformGrid(pixels, Matrix3x2.CreateRotation(degrees.DegreesToRadians()));
    }

    /// <summary>
    /// Scales the specified grid around its middle point.
    /// Expects the length of each line (width of the grid) to be equal all the way down.
    /// </summary>
    public static IReadOnlyList<string> ScaleGrid(IReadOnlyList<string> pixels, Vector2 scales) =>
        TransformGrid(pixels, Matrix3x2.CreateScale(scales));

    private static IReadOnlyList<string> TransformGrid(IReadOnlyList<string> pixels, Matrix3x2 matrix)
    {
        var newGrid = new Dictionary<Vector2, char>();

        foreach (var (line, y) in pixels.Select((line, y) => (line, y)))
        {
            foreach (var (chr, x) in line.Select((chr, x) => (chr, x)))
            {
                var newPoint = Vector2.Transform(new Vector2(x, y), matrix);
                newGrid.Add(newPoint, chr);
            }
        }

        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);

        foreach (var (p, _) in newGrid)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        var newWidth = (max.X - min.X).Round() + 1;
        var newHeight = (max.Y - min.Y).Round() + 1;
        char[][] newPixels = Enumerable.Range(0, Convert.ToInt32(newHeight)).Select(_ => new char[newWidth]).ToArray();

        var offset = Vector2.Zero - min;
        foreach (var (p, c) in newGrid)
        {
            var lp = p + offset;
            newPixels[lp.Y.Round()][lp.X.Round()] = c;
        }

        return newPixels.Select(newLine => new string(newLine)).ToArray();
    }

    /// <summary>
    /// Builds and returns 2D grid of text from the specified list of items that have a position.
    /// </summary>
    public static IReadOnlyList<string> ToStringGrid<T>(
        this IEnumerable<T> items,
        Func<T, Vector2> positionSelector,
        Func<T, char> charSelector,
        char defaultChar)
    {
        return items.ToGrid(positionSelector, charSelector, _ => defaultChar)
            .Select(line => new string(line.ToArray()))
            .ToArray();
    }

    /// <summary>
    /// Builds and returns 2D grid of items from the specified list of items that have a position.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<TOut>> ToGrid<TIn, TOut>(
        this IEnumerable<TIn> items,
        Func<TIn, Vector2> positionSelector,
        Func<TIn, TOut> resultItemSelector,
        Func<Vector2, TOut> resultItemFactory)
    {
        items = items.ToArray();

        var itemMap = items.GroupBy(positionSelector).ToDictionary(grp => positionSelector(grp.Last()), grp => grp.Last());

        var minBounds = new Vector2(itemMap.Min(i => i.Key.X), itemMap.Min(i => i.Key.Y));
        var maxBounds = new Vector2(itemMap.Max(i => i.Key.X), itemMap.Max(i => i.Key.Y));

        var grid = new List<IReadOnlyList<TOut>>();

        for (var y = minBounds.Y; y <= maxBounds.Y; y++)
        {
            var line = new List<TOut>();
            for (var x = minBounds.X; x <= maxBounds.X; x++)
            {
                line.Add(itemMap.TryGetValue(new Vector2(x, y), out var item)
                    ? resultItemSelector(item)
                    : resultItemFactory(new Vector2(x, y)));
            }

            grid.Add(line);
        }

        return grid;
    }

    /// <summary>
    /// Gets the adjacent items in the grid, from all directions including diagonal.
    /// </summary>
    public static IEnumerable<T> GetAdjacent<T>(this T[][] grid, Vector2 position) where T : class =>
        grid.GetAdjacent(DirectionsIncludingDiagonal, position);

    /// <summary>
    /// Gets the adjacent items in the grid, from any of the specified directions.
    /// </summary>
    public static IEnumerable<T> GetAdjacent<T>(this T[][] grid, Vector2[] directions, Vector2 position) where T : class
    {
        foreach (var dir in directions)
        {
            var adjacent = grid.SafeGet(position + dir);
            if (adjacent != null)
            {
                yield return adjacent;
            }
        }
    }

    /// <summary>
    /// Gets the item from the grid at the specified position, or null if that position is out of the bounds of the grid.
    /// </summary>
    public static T? SafeGet<T>(this T[][] grid, Vector2 position) where T : class
    {
        var y = position.Y.Round();

        if (y < 0 || y >= grid.Length)
            return null;

        var x = position.X.Round();
        var line = grid[y];
        return x < 0 || x >= line.Length ? null : line[x];
    }

    /// <summary>
    /// Gets the item from the grid at the specified position
    /// NOTE: Throws exception if that position is out of the bounds of the grid, use <see cref="SafeGet"/> if the position is not yet checked.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException" />
    public static char Get(this string[] grid, Vector2 position) => grid[(int) position.Y][(int) position.X];

    /// <summary>
    /// Gets the character from the grid at the specified position, or null if that position is out of the bounds of the grid.
    /// </summary>
    public static bool SafeGet(this string[] grid, Vector2 position, out char chr)
    {
        var y = (int) position.Y;

        if (y < 0 || y >= grid.Length)
        {
            chr = default;
            return false;
        }

        var x = (int) position.X;
        var line = grid[y];

        if (x < 0 || x >= line.Length)
        {
            chr = default;
            return false;
        }

        chr = line[x];
        return true;
    }

    /// <summary>
    /// Renders the specified grid of characters to a string and returns that string.
    /// </summary>
    public static string RenderGridToString(this IEnumerable<IEnumerable<char>> grid) =>
        string.Join(Environment.NewLine, grid.Select(line => string.Concat(line)));

    /// <summary>
    /// Renders the specified grid of characters to the console.
    /// </summary>
    public static string RenderGridToConsole(this IEnumerable<IEnumerable<char>> grid)
    {
        var renderedGrid = grid.RenderGridToString();
        Console.WriteLine(renderedGrid);
        Console.WriteLine();
        return renderedGrid;
    }
}
