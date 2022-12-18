namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Day 18: Boiling Boulders";

    /// <summary>
    /// Count up all the sides that aren't connected to another cube.
    /// </summary>
    public long? SolvePart1(PuzzleInput input)
    {
        var (_, distinctSurfaces, connectedSurfaces) = ScanCubes(input);
        return distinctSurfaces.Count - connectedSurfaces.Count;
    }

    /// <summary>
    /// What is the exterior surface area of your scanned lava droplet?
    /// </summary>
    public long? SolvePart2(PuzzleInput input)
    {
        var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);

        // Do we just need to get the outer most surfaces?

        return null;

        //var cubes = ParseInputToCubes(input);

        //var part1SurfaceArea = CalculateSurfaceArea(cubes);

        //// Minus 6 off for every cube that is totally enclosed

        //// Actually, minus off the distinct count of all of the surfaces of the cubes that are totally enclosed
        //var enclosedSurfaces = new HashSet<Surface>();

        //foreach (var enclosedCube in cubes.Where(cube => cube.IsEnclosed))
        //{
        //    foreach (var surface in enclosedCube.GetSurfaces())
        //    {
        //        enclosedSurfaces.Add(surface);
        //    }
        //}

        //return part1SurfaceArea - enclosedSurfaces.Count;
    }

    public class Cube
    {
        public Vector3 Position { get; }
        public Vector3 Min { get; }
        public Vector3 Max { get; }
        public IReadOnlySet<Surface> Surfaces { get; }
        public IReadOnlySet<Surface> DisconnectedSurfaces => _disconnectedSurfaces;
        public IReadOnlySet<Surface> ConnectedSurfaces => _connectedSurfaces;

        private readonly HashSet<Surface> _disconnectedSurfaces;
        private readonly HashSet<Surface> _connectedSurfaces;

        public Cube(Vector3 position)
        {
            Position = position;
            Min = Position;
            Max = Position + Vector3.One;
            Surfaces = new HashSet<Surface>(GetSurfaces());
            _disconnectedSurfaces = new HashSet<Surface>(Surfaces);
            _connectedSurfaces = new HashSet<Surface>();
        }

        private IReadOnlyList<Surface> GetSurfaces()
        {
            var a = Min;
            var b = new Vector3(Max.X, Min.Y, Min.Z);
            var c = new Vector3(Min.X, Max.Y, Min.Z);
            var d = new Vector3(Max.X, Max.Y, Min.Z);
            var e = new Vector3(Min.X, Min.Y, Max.Z);
            var f = new Vector3(Max.X, Min.Y, Max.Z);
            var g = new Vector3(Min.X, Max.Y, Max.Z);
            var h = Max;

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

        public void MarkConnectedSurface(Surface connectedSurface)
        {
            if (!Surfaces.Contains(connectedSurface))
            {
                throw new InvalidOperationException("Unexpected: tried to remove a surface from disconnected that we don't have");
            }

            _disconnectedSurfaces.Remove(connectedSurface);
            _connectedSurfaces.Add(connectedSurface);
        }

        //public HashSet<Vector3> AdjacentCubes = new();

        //public bool IsEnclosed => AdjacentCubes.Count == 6;
    }

    public record Surface(Vector3 Min, Vector3 Max);

    public record ScanResult(
        IReadOnlyList<Cube> Cubes,
        IReadOnlySet<Surface> DistinctSurfaces,
        IReadOnlySet<Surface> ConnectedSurfaces);

    static IReadOnlyList<Cube> ParseInputToCubes(PuzzleInput input) => input.ReadLines().Select(line =>
    {
        var coords = line.Split(',').Select(int.Parse).ToArray();
        return new Cube(new Vector3(coords[0], coords[1], coords[2]));
    }).ToArray();

    static ScanResult ScanCubes(PuzzleInput input)
    {
        var cubes = ParseInputToCubes(input);

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
            distinctSurfaces.UnionWith(cube1.Surfaces);
            distinctSurfaces.UnionWith(cube2.Surfaces);

            ////var cube1Surfaces = cube1.Surfaces;
            //foreach (var surface in cube1.Surfaces)
            //{
            //    distinctSurfaces.Add(surface);
            //}

            ////var cube2Surfaces = cube2.Surfaces;
            //foreach (var surface in cube2.Surfaces)
            //{
            //    distinctSurfaces.Add(surface);
            //}

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
                //cube1.AdjacentCubes.Add(cube2.Position);
                //cube2.AdjacentCubes.Add(cube1.Position);

                var sharedSurface = cube1.DisconnectedSurfaces.Intersect(cube2.DisconnectedSurfaces).FirstOrDefault();

                if (sharedSurface != null)
                {
                    connectedSurfaces.Add(sharedSurface);

                    cube1.MarkConnectedSurface(sharedSurface);
                    cube2.MarkConnectedSurface(sharedSurface);
                }

                //var sharedSurface = FindSharedSurface(cube1Surfaces, cube2Surfaces);
                //connectedSurfaces.Add(sharedSurface);
            }
        }

        return new ScanResult(cubes, distinctSurfaces, connectedSurfaces);

        //return distinctSurfaces.Count - connectedSurfaces.Count;
    }

    //static Surface FindSharedSurface(IReadOnlyList<Surface> cube1Surfaces, IReadOnlyList<Surface> cube2Surfaces)
    //{
    //    foreach (var cube1Surface in cube1Surfaces)
    //    {
    //        foreach (var cube2Surface in cube2Surfaces)
    //        {
    //            if (cube1Surface == cube2Surface)
    //            {
    //                return cube1Surface;
    //            }
    //        }
    //    }

    //    throw new InvalidOperationException("Unexpected: shared surface not found");
    //}
}
