namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Day 18: Boiling Boulders";

    public long? SolvePart1(PuzzleInput input) => CalculateSurfaceArea(ParseInputToCubes(input));

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record Cube(/*int Id, */Vector3 Position)
    {
        public Vector3 Min { get; } = Position;

        public Vector3 Max { get; } = Position + Vector3.One;

        //public IReadOnlyList<Surface> Sides { get; } = GetSides().ToArray();

        public IReadOnlyList<Surface> GetSurfaces()
        {
            var a = Min;
            var b = new Vector3(Max.X, Min.Y, Min.Z);
            var c = new Vector3(Min.X, Max.Y, Min.Z);
            var d = new Vector3(Max.X, Max.Y, Min.Z);
            var e = new Vector3(Min.X, Min.Y, Max.Z);
            var f = new Vector3(Max.X, Min.Y, Max.Z);
            var g = new Vector3(Min.X, Max.Y, Max.Z);
            var h = Max;

            if (new[] { a, b, c, d, e, f, g, h }.Distinct().Count() != 8)
            {
                throw new InvalidOperationException("KEH? cube VERTEXES");
            }

            return new Surface[]
            {
                new(a, d),
                new(b, h),
                new(e, h),
                new(a, g),
                new(a, f),
                new(c, h)
            };
        }
    }

    public record Surface(Vector3 Min, Vector3 Max);

    public static IReadOnlyList<Cube> ParseInputToCubes(PuzzleInput input) => input.ReadLines().Select((line, i) =>
    {
        var coords = line.Split(',').Select(int.Parse).ToArray();
        return new Cube(/*i, */new Vector3(coords[0], coords[1], coords[2]));
    }).ToArray();

    /// <summary>
    /// Count up all the sides that aren't connected to another cube.
    /// </summary>
    public long CalculateSurfaceArea(IReadOnlyList<Cube> cubes)
    {
        // Do not test a cube against itself!
        // Any cube whose 3D Manhattan Distance is greater than one, they share no edges, so we can quickly eliminate them
        // So, for any pair whose 3D Manhattan Distance == 1, find the shared side (note they are cubes, so can only share one side)
        // Keep a list of these sides, then get distinct count of the list, that gives us our shared sides
        // Keep a list of all sides, then get distinct count of the list, that gives us all of our sides
        // Take shared off of all, I think!!

        //if (cubes.Distinct().Count() != cubes.Count)
        //{
        //    throw new InvalidOperationException("KEH?");
        //}

        var pairs = cubes.SelectMany(
            cube1 => cubes
                .Where(cube2 => cube1.Position != cube2.Position)
                .Select(cube2 => (cube1, cube2)));

        var distinctSurfaces = new HashSet<Surface>();
        var connectedSurfaces = new HashSet<Surface>();

        foreach (var (cube1, cube2) in pairs)
        {
            var cube1Surfaces = cube1.GetSurfaces();
            foreach (var surface in cube1Surfaces)
            {
                distinctSurfaces.Add(surface);
            }

            var cube2Surfaces = cube2.GetSurfaces();
            foreach (var surface in cube2Surfaces)
            {
                distinctSurfaces.Add(surface);
            }

            if (cube1Surfaces.Distinct().Count() != 6)
            {
                throw new InvalidOperationException("KEH? cube1 surfaces");
            }

            if (cube2Surfaces.Distinct().Count() != 6)
            {
                throw new InvalidOperationException("KEH? cube2 surfaces");
            }

            // Find shared surface, but only for touching cubes
            if (MathUtils.ManhattanDistance(cube1.Position, cube2.Position) == 1)
            {
                var sharedSurface = cube1Surfaces.Intersect(cube2Surfaces).Single();
                connectedSurfaces.Add(sharedSurface);
            }
        }

        return distinctSurfaces.Count - connectedSurfaces.Count;
    }
}
