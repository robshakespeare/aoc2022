namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Day 18: Boiling Boulders";

    public long? SolvePart1(PuzzleInput input) => CalculateSurfaceArea(ParseInputToCubes(input));

    public long? SolvePart2(PuzzleInput input)
    {
        var cubes = ParseInputToCubes(input);

        var part1SurfaceArea = CalculateSurfaceArea(cubes);

        // Minus 6 off for every cube that is totally enclosed

        // Actually, minus off the distinct count of all of the surfaces of the cubes that are totally enclosed
        var enclosedSurfaces = new HashSet<Surface>();

        foreach (var enclosedCube in cubes.Where(cube => cube.IsEnclosed))
        {
            foreach (var surface in enclosedCube.GetSurfaces())
            {
                enclosedSurfaces.Add(surface);
            }
        }

        return part1SurfaceArea - enclosedSurfaces.Count;
    }

    public record Cube(/*int Id, *//*SimpleV3 Min, SimpleV3 Max*/ Vector3 Position)
    {
        public Vector3 Min { get; } = Position;

        public Vector3 Max { get; } = Position + Vector3.One;

        //public SimpleV3 Max { get; } = SimpleV3.Create(Position + Vector3.One;

        //public IReadOnlyList<Surface> Sides { get; } = GetSides().ToArray();

        public IReadOnlyList<Surface> GetSurfaces()
        {
            var a = SimpleV3.Create(Min.X, Min.Y, Min.Z);
            var b = SimpleV3.Create(Max.X, Min.Y, Min.Z);
            var c = SimpleV3.Create(Min.X, Max.Y, Min.Z);
            var d = SimpleV3.Create(Max.X, Max.Y, Min.Z);
            var e = SimpleV3.Create(Min.X, Min.Y, Max.Z);
            var f = SimpleV3.Create(Max.X, Min.Y, Max.Z);
            var g = SimpleV3.Create(Min.X, Max.Y, Max.Z);
            var h = SimpleV3.Create(Max.X, Max.Y, Max.Z);

            //if (new[] { a, b, c, d, e, f, g, h }.Distinct().Count() != 8)
            //{
            //    throw new InvalidOperationException("KEH? cube VERTEXES");
            //}

            return new Surface[]
            {
                Surface.Create(a, d),
                Surface.Create(b, h),
                Surface.Create(e, h),
                Surface.Create(a, g),
                Surface.Create(a, f),
                Surface.Create(c, h)
            };
        }

        public HashSet<Vector3> AdjacentCubes = new();

        public bool IsEnclosed => AdjacentCubes.Count == 6;
    }

    public struct SimpleV3
    {
        public int Value { get; }

        public SimpleV3(int value) => Value = value;

        public static SimpleV3 Create(float x, float y, float z) => Create((int) x, (int) y, (int) z);

        public static SimpleV3 Create(int x, int y, int z)
        {
            if (x > 31 || y > 31 || z > 31)
            {
                throw new NotSupportedException("Only designed for max of 5 bit element each");
            }

            return new SimpleV3(x | (y << 5) | (z << 10));
        }
    }

    //public record Surface(Vector3 Min, Vector3 Max);

    public readonly struct Surface
    {
        public int Value { get; }

        public Surface(int value) => Value = value;

        public static Surface Create(SimpleV3 min, SimpleV3 max) => new(min.Value | (max.Value << 15));
    }

    //Surface(Vector3 Min, Vector3 Max);

    public static IReadOnlyList<Cube> ParseInputToCubes(PuzzleInput input) => input.ReadLines().Select((line, i) =>
    {
        var coords = line.Split(',').Select(int.Parse).ToArray();
        return new Cube(new Vector3(coords[0], coords[1], coords[2]));
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

            //if (cube1Surfaces.Distinct().Count() != 6)
            //{
            //    throw new InvalidOperationException("KEH? cube1 surfaces");
            //}

            //if (cube2Surfaces.Distinct().Count() != 6)
            //{
            //    throw new InvalidOperationException("KEH? cube2 surfaces");
            //}

            // Find shared surface, but only for touching cubes
            if (MathUtils.ManhattanDistance(cube1.Position, cube2.Position) == 1)
            {
                cube1.AdjacentCubes.Add(cube2.Position);
                cube2.AdjacentCubes.Add(cube1.Position);

                var sharedSurface = cube1Surfaces.Intersect(cube2Surfaces).First();
                connectedSurfaces.Add(sharedSurface);

                //var sharedSurface = FindSharedSurface(cube1Surfaces, cube2Surfaces);
                //connectedSurfaces.Add(sharedSurface);
            }
        }

        return distinctSurfaces.Count - connectedSurfaces.Count;
    }

    static Surface FindSharedSurface(IReadOnlyList<Surface> cube1Surfaces, IReadOnlyList<Surface> cube2Surfaces)
    {
        foreach (var cube1Surface in cube1Surfaces)
        {
            foreach (var cube2Surface in cube2Surfaces)
            {
                if (cube1Surface.Value == cube2Surface.Value)
                {
                    return cube1Surface;
                }
            }
        }

        throw new InvalidOperationException("Unexpected: shared surface not found");
    }
}
