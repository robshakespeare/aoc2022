using static AoC.Day18.Day18Solver;

namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Day 18: Boiling Boulders";

    /// <summary>
    /// Count up all the sides that aren't connected to another cube.
    /// </summary>
    public long? SolvePart1(PuzzleInput input)
    {
        var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);

        //var disconnectedSurfaces = cubes.SelectMany(cube => cube.DisconnectedSurfaces).Distinct().ToArray();

        var disconnectedSurfaces = distinctSurfaces.Except(connectedSurfaces);

        return disconnectedSurfaces.Count();

        //return distinctSurfaces.Count - connectedSurfaces.Count;
    }

    /// <summary>
    /// What is the exterior surface area of your scanned lava droplet?
    /// </summary>
    public long? SolvePart2(PuzzleInput input)
    {
        var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);

        var disconnectedSurfaces = cubes.SelectMany(cube => cube.DisconnectedSurfaces).Distinct().ToArray();

        // Do we just need to get the outer most surfaces?
        // Casting ray out from each disconnected surface, if it intersects no other disconnected surface, that means its an outside edge

        var externalSurfaces = new HashSet<Surface>();

        foreach (var (disconnectedSurface, n) in disconnectedSurfaces.Select((s, i) => (s, n: i + 1)))
        {
            Logger($"Checking disconnectedSurface {n} / {disconnectedSurfaces.Length}");

            //var isExternal = disconnectedSurfaces
            //    .Where(otherDisconnectedSurface => disconnectedSurface != otherDisconnectedSurface)
            //    .All(otherDisconnectedSurface => !AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface));

            var isInternal = disconnectedSurfaces
                .Where(otherDisconnectedSurface => disconnectedSurface != otherDisconnectedSurface)
                .Any(otherDisconnectedSurface => AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface));

            //var isExternal = true;

            //foreach (var otherDisconnectedSurface in disconnectedSurfaces)
            //{
            //    if (disconnectedSurface != otherDisconnectedSurface)
            //    {
            //        if (!AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface))
            //        {
            //            isExternal = false;
            //        }
            //    }
            //}

            if (!isInternal)
            {
                externalSurfaces.Add(disconnectedSurface);
            }

            Logger($"Done disconnectedSurface {n} / {disconnectedSurfaces.Length}");
        }

        //foreach (var (disconnectedSurface, n) in disconnectedSurfaces.Select((s, i) => (s, n: i + 1)))
        //{
        //    Logger($"Checking disconnectedSurface {n} / {disconnectedSurfaces.Length}");

        //    var isExternal = true;

        //    foreach (var otherDisconnectedSurface in disconnectedSurfaces)
        //    {
        //        if (disconnectedSurface != otherDisconnectedSurface)
        //        {
        //            if (!AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface))
        //            {
        //                isExternal = false;
        //            }
        //        }
        //    }

        //    if (isExternal)
        //    {
        //        externalSurfaces.Add(disconnectedSurface);
        //    }

        //    Logger($"Done disconnectedSurface {n} / {disconnectedSurfaces.Length}");
        //}

        var internalSurfaces = disconnectedSurfaces.Except(externalSurfaces).ToArray();

        return externalSurfaces.Count;

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

    public Action<string> Logger { get; set; } = Console.WriteLine;

    // l0 is the origin of the ray and l is the ray direction

    // Ray origin and direction is current surface, plane normal and point on plane is the other surface
    public static bool AreDirectlyFacing(Surface currentSurface, Surface otherSurface)
    {
        if (FacingPlaneIntersect(otherSurface.Normal, otherSurface.Min, currentSurface.Min, currentSurface.Normal, out var intersect, out _))
        {
            //var intersect = currentSurface.Min + (currentSurface.Normal * t);

            // Intersect must be within bounds of other surface
            return intersect == otherSurface.Min;
            //{
            //}

            //return
            //    intersect.X >= otherSurface.Min.X && intersect.X <= otherSurface.Max.X &
            //    intersect.Y >= otherSurface.Min.Y && intersect.Y <= otherSurface.Max.Y &
            //    intersect.Z >= otherSurface.Min.Z && intersect.Z <= otherSurface.Max.Z;
        }

        return false;
    }

    static bool FacingPlaneIntersect(Vector3 planeNormal, Vector3 pointOnPlane, Vector3 rayOrigin, Vector3 rayDirection, out Vector3 intersect, out float t)
    {
        // assuming vectors are all normalized
        var denom = Vector3.Dot(planeNormal, rayDirection);
        //Math.Abs(denom) != 0 /*> 1e-6*/)
        if (denom < 0) // i.e. if facing towards each other
        {
            var dir = pointOnPlane - rayOrigin;
            t = Vector3.Dot(dir, planeNormal) / denom;
            if (t > 0)
            {
                intersect = rayOrigin + (rayDirection * t);
                return true;
            }

            //return t >= 0;
        }

        t = default;
        intersect = default;
        return false;
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
        //private readonly Lazy<Vector3> _normal;

        public Cube(Vector3 position)
        {
            Position = position;
            Min = Position;
            Max = Position + Vector3.One;
            Surfaces = new HashSet<Surface>(GetSurfaces());
            _disconnectedSurfaces = new HashSet<Surface>(Surfaces);
            _connectedSurfaces = new HashSet<Surface>();
            //_normal = new Lazy<Vector3>(() => Vector3.Normalize(Vector3.Cross(Min, Max)));
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
                //new(a, d, new(0, 0, -1)),
                //new(b, h, new(1, 0, 0)),
                //new(e, h, new(0, 0, 1)),
                //new(a, g, new(-1, 0, 0)),
                //new(a, f, new(0, -1, 0)),
                //new(c, h, new(0, 1, 0))

                new(a, d, new(0, 0, -1)),
                new(b, h, new(1, 0, 0)),
                new(e, h, new(0, 0, 1)),
                new(a, g, new(-1, 0, 0)),
                new(a, f, new(0, -1, 0)),
                new(c, h, new(0, 1, 0))
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

    public record Surface(Vector3 Min, Vector3 Max, Vector3 Normal)
    {
        //public Vector3 Normal { get; } = CalculateNormal(Min, Max);

        //private static Vector3 CalculateNormal(Vector3 min, Vector3 max)
        //{
        //    var a = min;
        //    var b = new Vector3(max.X, min.Y, min.Z);
        //    var e = new Vector3(min.X, min.Y, max.Z);

        //    var ba = b - a;
        //    var ea = e - a;

        //    return Vector3.Cross(ba, ea);
        //    //return Vector3.Normalize(Vector3.Cross(ba, ea));
        //}
    }

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
            }
        }

        return new ScanResult(cubes, distinctSurfaces, connectedSurfaces);
    }
}
