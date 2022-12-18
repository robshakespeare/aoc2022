using System.Linq;
using System.Xml.Linq;
using static AoC.Day18.Day18Solver;

namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Day 18: Boiling Boulders";

    //IReadOnlyList<Vector3> GetEdges(Vector3 position)
    //{
    //    //var a = Min;
    //    //var b = new Vector3(Max.X, Min.Y, Min.Z);
    //    //var c = new Vector3(Min.X, Max.Y, Min.Z);
    //    //var d = new Vector3(Max.X, Max.Y, Min.Z);
    //    //var e = new Vector3(Min.X, Min.Y, Max.Z);
    //    //var f = new Vector3(Max.X, Min.Y, Max.Z);
    //    //var g = new Vector3(Min.X, Max.Y, Max.Z);
    //    //var h = Max;

    //    return new Vector3[]
    //    {
    //        //new(a, d, new(0, 0, -1)),
    //        //new(b, h, new(1, 0, 0)),
    //        //new(e, h, new(0, 0, 1)),
    //        //new(a, g, new(-1, 0, 0)),
    //        //new(a, f, new(0, -1, 0)),
    //        //new(c, h, new(0, 1, 0))

    //        position + new Vector3(0.5f, 0, 0),
    //        position + new Vector3(-0.5f, 0, 0),
    //        position + new Vector3(0, -0.5f, 0),
    //        position + new Vector3(0, 0.5f, 0),
    //        position + new Vector3(0, 0, -0.5f),
    //        position + new Vector3(0, 0, 0.5f)
    //    };
    //}

    /// <summary>
    /// Count up all the sides that aren't connected to another cube.
    /// </summary>
    public long? SolvePart1(PuzzleInput input)
    {
        var lavaDropletCubes = ParseInputToCubeMap(input);

        var (distinctEdges, connectedEdges, disconnectedEdges) = ScanEdges(lavaDropletCubes);

        //var disconnectedEdges = distinctEdges.Except(connectedEdges); // rs-todo: maybe this should be returned by the method above
        return disconnectedEdges.Count();

        return distinctEdges.Count - connectedEdges.Count;

        //var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);
        ////var disconnectedSurfaces = cubes.SelectMany(cube => cube.DisconnectedSurfaces).Distinct().ToArray();
        //var disconnectedSurfaces = distinctSurfaces.Except(connectedSurfaces);
        //return disconnectedSurfaces.Count();
    }

    private static (IReadOnlySet<Vector3> DistinctEdges, IReadOnlySet<Vector3> ConnectedEdges, IReadOnlySet<Vector3> DisconnectedEdges) ScanEdges(
        IReadOnlySet<Cube> cubeMap)
    {
        var distinctEdges = new HashSet<Vector3>();
        var connectedEdges = new HashSet<Vector3>();

        foreach (var cube in cubeMap)
        {
            distinctEdges.UnionWith(cube.Edges);

            foreach (var (_, sharedEdge) in cube.GetTouchingNeighbors(cubeMap))
            {
                connectedEdges.Add(sharedEdge);
            }
        }

        return (distinctEdges, connectedEdges, distinctEdges.Except(connectedEdges).ToHashSet());
    }

    public static IReadOnlyList<Vector3> Axis3d = new Vector3[]
    {
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, -1, 0),
        new(0, 1, 0),
        new(0, 0, -1),
        new(0, 0, 1)
    };

    //static IEnumerable<Vector3> OuterPositions(Vector3 cube, IDictionary<Vector3, char> cubeMap)
    //{
    //    return Axis3d
    //        .Select(dir => cube + dir)
    //        .Where(nextPos => !cubeMap.ContainsKey(nextPos));
    //}

    //static Vector3 RoundUp(Vector3 v)
    //{
    //    return new Vector3(
    //        (int) Math.Round(v.X, MidpointRounding.AwayFromZero),
    //        (int) Math.Round(v.Y, MidpointRounding.AwayFromZero),
    //        (int) Math.Round(v.Z, MidpointRounding.AwayFromZero));
    //}

    /// <summary>
    /// What is the exterior surface area of your scanned lava droplet?
    /// </summary>
    public long? SolvePart2(PuzzleInput input)
    {
        var lavaDropletCubes = ParseInputToCubeMap(input);

        var (distinctEdges, connectedEdges, disconnectedEdges) = ScanEdges(lavaDropletCubes);
        //var disconnectedEdges = distinctEdges.Except(connectedEdges);

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var cube in lavaDropletCubes)
        {
            min = Vector3.Min(min, cube.Position); // - Vector3.One;
            max = Vector3.Max(max, cube.Position); // + Vector3.One;
        }

        min -= Vector3.One;
        max += Vector3.One;

        bool IsWithinBounds(Cube cube) =>
            cube.Position.X >= min.X && cube.Position.X <= max.X &&
            cube.Position.Y >= min.Y && cube.Position.Y <= max.Y &&
            cube.Position.Z >= min.Z && cube.Position.Z <= max.Z;

        var explore = new Queue<Cube>(new[] {new Cube(min)});
        var spaces = new HashSet<Cube>();
        var externalCubes = new HashSet<Cube>();

        while (explore.Count > 0)
        {
            var space = explore.Dequeue();

            // Only explore ones we've not already explored
            if (!spaces.Contains(space))
            {
                spaces.Add(space);

                var nextCubes = Axis3d.Select(dir => new Cube(space.Position + dir));

                foreach (var nextCube in nextCubes)
                {
                    // Only look at ones within our bounds
                    if (IsWithinBounds(nextCube))
                    {
                        // If the next cube is actually occupied in the cube map, that means we found a truly external cube, so keep track of it
                        // Else, that means we're still in space, so add that to the queue to explore, if we've not seen it already
                        if (lavaDropletCubes.Contains(nextCube))
                        {
                            externalCubes.Add(nextCube);
                        }
                        else
                        {
                            explore.Enqueue(nextCube);
                        }
                    }
                }
            }

            //// if we've not already seen the cube
            //if (!seen.Contains(cube))
            //{
            //    foreach (var child in _getSuccessors(node))
            //    {
            //        var childPath = path.Append(child);
            //        explore.Enqueue(childPath, childPath.TotalCost + _getHeuristic(child, goal)); // the heuristic is added here as a part of the priority
            //    }

            //    seen.Add(node);
            //}
        }

        //{
        //    //var min = new Vector3(float.MaxValue);
        //    //var max = new Vector3(float.MinValue);

        //    //foreach (var p in cubes)
        //    //{
        //    //    min = Vector3.Min(min, p);
        //    //    max = Vector3.Max(max, p);
        //    //}

        //    var width = (int) (max.X - min.X + 1); // + 2; // Note: + 1 to make inclusive, and then +2 so the maps align with the "edge" maps below
        //    var height = (int) (max.Y - min.Y + 1); // + 2;

        //    for (var z = (int)min.Z; z <= (int)max.Z; z++)
        //    {
        //        var thisZ = z;
        //        Logger($"Z: {z}");
        //        Logger(spaces.Where(c => (int)c.Position.Z == thisZ)
        //            .RenderWorldToViewport(c => new Vector2(c.Position.X, c.Position.Y), _ => '@', '.', viewportWidth: width, viewportHeight: height));
        //        Logger("");
        //    }
        //}

        var airCavities = new HashSet<Cube>();

        for (var z = (int)min.Z; z <= (int)max.Z; z++)
        {
            for (var y = (int)min.Y; y <= (int)max.Y; y++)
            {
                for (var x = (int)min.X; x <= (int)max.X; x++)
                {
                    airCavities.Add(new Cube(new Vector3(x, y, z)));
                }
            }
        }

        airCavities.ExceptWith(lavaDropletCubes);
        airCavities.ExceptWith(spaces);

        {
            //var min = new Vector3(float.MaxValue);
            //var max = new Vector3(float.MinValue);

            //foreach (var p in cubes)
            //{
            //    min = Vector3.Min(min, p);
            //    max = Vector3.Max(max, p);
            //}

            var width = (int)(max.X - min.X + 1); // + 2; // Note: + 1 to make inclusive, and then +2 so the maps align with the "edge" maps below
            var height = (int)(max.Y - min.Y + 1); // + 2;

            for (var z = (int)min.Z; z <= (int)max.Z; z++)
            {
                var thisZ = z;
                Logger($"Z: {z}");

                var airIsZ = airCavities.Where(c => (int)c.Position.Z == thisZ).ToArray();

                if (airIsZ.Length == 0)
                {
                    Logger("None!");
                }
                else
                {
                    Logger(airIsZ
                        .RenderWorldToViewport(c => new Vector2(c.Position.X, c.Position.Y), _ => 'A', '.', viewportWidth: width, viewportHeight: height));
                }
                
                Logger("");
            }
        }

        var (internalDistinctEdges, internalConnectedEdges, internalDisconnectedEdges) = ScanEdges(airCavities);

        return disconnectedEdges.Except(internalDisconnectedEdges).Count();

        //return distinctEdges.Count - connectedEdges.Count - internalDistinctEdges.Count - internalConnectedEdges.Count;

        //return null;

        //// Now, we know all of our external cubes
        //// So, reduce our known disconnected edges to only those of our external cubes, and then we have our distinct exterior edges, and hence exterior surface area.
        //var externalCubeEdges = externalCubes.SelectMany(externalCube => externalCube.Edges).ToHashSet();

        //var disconnectedExternalEdges = disconnectedEdges.Intersect(externalCubeEdges);

        //return disconnectedExternalEdges.Count();

        //var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);

        //var disconnectedSurfaces = distinctSurfaces.Except(connectedSurfaces);

        //return null;

        //var cubes = ParseInputToCubePositions(input);

        //using var sw = new StreamWriter($@"C:\Users\Rob.Shakespeare\OneDrive\AoC 2022\Day18\Logs\{$"{DateTime.Now:O}".Replace(":", "-")}.txt");

        ////var (cubes, distinctSurfaces, connectedSurfaces) = ScanCubes(input);

        ////var disconnectedSurfaces = cubes.SelectMany(cube => cube.DisconnectedSurfaces).Distinct().ToArray();

        ////var disconnectedSurfaces = distinctSurfaces.Except(connectedSurfaces);

        //var cubesMap = cubes.ToHashSet();

        //var cubesMap2 = cubes/*.Select(c => c.Position)*/.ToDictionary(x => x, _ => '#');

        ////var disconnectedSurfaces = cubes.SelectMany(cube => cube.DisconnectedSurfaces).Distinct().ToArray();

        //// Do we just need to get the outer most surfaces?
        //// Casting ray out from each disconnected surface, if it intersects no other disconnected surface, that means its an outside edge

        //{
        //    var min = new Vector3(float.MaxValue);
        //    var max = new Vector3(float.MinValue);

        //    foreach (var p in cubes)
        //    {
        //        min = Vector3.Min(min, p);
        //        max = Vector3.Max(max, p);
        //    }

        //    var width = (int)(max.X - min.X + 1) + 2; // Note: + 1 to make inclusive, and then +2 so the maps align with the "edge" maps below
        //    var height = (int)(max.Y - min.Y + 1) + 2;

        //    for (var z = (int) min.Z; z <= (int) max.Z; z++)
        //    {
        //        var thisZ = z;
        //        sw.WriteLine($"Z: {z}");
        //        sw.WriteLine(cubes.Where(c => (int)c.Z == thisZ)
        //            .RenderWorldToViewport(c => new Vector2(c.X, c.Y), _ => '#', '.', viewportWidth: width, viewportHeight: height));
        //        sw.WriteLine("");
        //    }
        //}


        //sw.WriteLine("");
        //sw.WriteLine("==== Outer traces, is this correct?");
        //sw.WriteLine("");

        //var outerCubes = cubesMap2.Keys.SelectMany(x => OuterPositions(x, cubesMap2)).Distinct().ToArray();

        ////var outerCubes = disconnectedSurfaces.Select(RoundUp).Distinct().ToArray();

        //{
        //    var min = new Vector3(float.MaxValue);
        //    var max = new Vector3(float.MinValue);

        //    foreach (var p in outerCubes)
        //    {
        //        min = Vector3.Min(min, p);
        //        max = Vector3.Max(max, p);
        //    }

        //    var width = (int)(max.X - min.X + 1);
        //    var height = (int)(max.Y - min.Y + 1);

        //    for (var z = (int)min.Z; z <= (int)max.Z; z++)
        //    {
        //        var thisZ = z;
        //        sw.WriteLine($"Z: {z}");
        //        sw.WriteLine(outerCubes.Where(c => (int)c.Z == thisZ)
        //            .RenderWorldToViewport(c => new Vector2(c.X, c.Y), _ => '@', '.', viewportWidth: width, viewportHeight: height));
        //        sw.WriteLine("");
        //    }
        //}

        //return null;

        ////var externalSurfaces = new HashSet<Surface>();

        ////var hmInternals = new Dictionary<Surface, HashSet<Surface>>();

        ////foreach (var (disconnectedSurface, n) in disconnectedSurfaces.Select((s, i) => (s, n: i + 1)))
        ////{
        ////    //Logger($"Checking disconnectedSurface {n} / {disconnectedSurfaces.Length}");

        ////    var isExternal = disconnectedSurfaces
        ////        .Where(otherDisconnectedSurface => !disconnectedSurface.Equals(otherDisconnectedSurface))
        ////        .All(otherDisconnectedSurface => !AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface));

        ////    var internals = disconnectedSurfaces
        ////        .Where(otherDisconnectedSurface => !disconnectedSurface.Equals(otherDisconnectedSurface))
        ////        .Where(otherDisconnectedSurface => AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface));

        ////    var hmInternal = hmInternals.GetOrAdd(disconnectedSurface, () => new HashSet<Surface>());
        ////    hmInternal.UnionWith(internals);

        ////    //var isExternal = true;

        ////    //foreach (var otherDisconnectedSurface in disconnectedSurfaces)
        ////    //{
        ////    //    if (disconnectedSurface != otherDisconnectedSurface)
        ////    //    {
        ////    //        if (!AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface))
        ////    //        {
        ////    //            isExternal = false;
        ////    //        }
        ////    //    }
        ////    //}

        ////    if (isExternal) //if (!isInternal)
        ////    {
        ////        externalSurfaces.Add(disconnectedSurface);
        ////    }

        ////    //Logger($"Done disconnectedSurface {n} / {disconnectedSurfaces.Length}");
        ////}

        //////foreach (var (disconnectedSurface, n) in disconnectedSurfaces.Select((s, i) => (s, n: i + 1)))
        //////{
        //////    Logger($"Checking disconnectedSurface {n} / {disconnectedSurfaces.Length}");

        //////    var isExternal = true;

        //////    foreach (var otherDisconnectedSurface in disconnectedSurfaces)
        //////    {
        //////        if (disconnectedSurface != otherDisconnectedSurface)
        //////        {
        //////            if (!AreDirectlyFacing(disconnectedSurface, otherDisconnectedSurface))
        //////            {
        //////                isExternal = false;
        //////            }
        //////        }
        //////    }

        //////    if (isExternal)
        //////    {
        //////        externalSurfaces.Add(disconnectedSurface);
        //////    }

        //////    Logger($"Done disconnectedSurface {n} / {disconnectedSurfaces.Length}");
        //////}

        ////var internalSurfaces = disconnectedSurfaces.Except(externalSurfaces).ToArray();

        ////var expectedCube = new Cube(new(2, 2, 5));

        ////Logger($"Expected internal surfaces: {string.Join(" -- ", expectedCube.Surfaces)}");

        ////Logger($"actual internal surfaces: {string.Join(" -- ", internalSurfaces.Select(x => x.ToString()))}");

        ////var incorrectInternalSurfaces = internalSurfaces.Except(expectedCube.Surfaces).ToArray();
        ////Logger($"incorrect internal surfaces: {string.Join(" -- ", incorrectInternalSurfaces.Select(x => x.ToString()))}");

        ////foreach (var incorrectInternalSurface in incorrectInternalSurfaces)
        ////{
        ////    var incorrectlyFacing = hmInternals[incorrectInternalSurface];

        ////    Logger("----");
        ////    Logger("");
        ////    Logger($"incorrectInternalSurface: {incorrectInternalSurface}");
        ////    Logger("incorrectlyFacing:");
        ////    Logger(string.Join(Environment.NewLine, incorrectlyFacing));
        ////}

        //////expectedCube.Surfaces

        ////return externalSurfaces.Count;

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
        if (FacingPlaneIntersect(otherSurface.Normal, otherSurface.Position, currentSurface.Position, currentSurface.Normal, out var intersect, out _))
        {
            //var intersect = currentSurface.Min + (currentSurface.Normal * t);

            // Intersect must be within bounds of other surface
            return intersect == otherSurface.Position;
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

    public record Edge(Vector3 HalfPos, Vector3 ActualPos);

    public class Cube
    {
        public Vector3 Position { get; }
        //public Vector3 Min { get; }
        //public Vector3 Max { get; }
        public IReadOnlySet<Vector3> Edges { get; }
        public IReadOnlySet<Surface> Surfaces { get; }
        public IReadOnlySet<Vector3> DisconnectedSurfaces => _disconnectedSurfaces;
        public IReadOnlySet<Vector3> ConnectedSurfaces => _connectedSurfaces;

        private readonly HashSet<Vector3> _disconnectedSurfaces;
        private readonly HashSet<Vector3> _connectedSurfaces;
        //private readonly Lazy<Vector3> _normal;

        public Cube(Vector3 position)
        {
            Position = position;
            Edges = new HashSet<Vector3>(GetEdges());
            if (Edges.Count != 6)
            {
                throw new InvalidOperationException("incorrect edges count!");
            }

            //Min = Position;
            //Max = Position + Vector3.One;
            Surfaces = new HashSet<Surface>(GetSurfaces());

            if (Surfaces.Count != 6)
            {
                throw new InvalidOperationException("incorrect surface count!");
            }

            _disconnectedSurfaces = new HashSet<Vector3>(Edges);
            _connectedSurfaces = new HashSet<Vector3>();
            //_normal = new Lazy<Vector3>(() => Vector3.Normalize(Vector3.Cross(Min, Max)));
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Cube) obj);
        }

        protected bool Equals(Cube other) => Position.Equals(other.Position);

        public override int GetHashCode() => Position.GetHashCode();

        public IEnumerable<(Cube Neighbor, Vector3 SharedEdge)> GetTouchingNeighbors(IReadOnlySet<Cube> cubeMap)
        {
            var nextCubes = Axis3d.Select(dir => new Cube(Position + dir));

            foreach (var nextCube in nextCubes)
            {
                if (cubeMap.Contains(nextCube))
                {
                    var sharedEdge = Edges.Intersect(nextCube.Edges).Single();
                    yield return (nextCube, sharedEdge);
                }
            }

            //var distinctSurfaces = new HashSet<Vector3>();
            //var connectedSurfaces = new HashSet<Vector3>();

            //foreach (var (cube1, cube2) in pairs)
            //{
            //    distinctSurfaces.UnionWith(cube1.Edges);
            //    distinctSurfaces.UnionWith(cube2.Edges);

            //    // Find shared surface, but only for touching cubes
            //    if (MathUtils.ManhattanDistance(cube1.Position, cube2.Position) == 1)
            //    {
            //        //cube1.AdjacentCubes.Add(cube2.Position);
            //        //cube2.AdjacentCubes.Add(cube1.Position);

            //        var sharedSurface = cube1.DisconnectedSurfaces.Intersect(cube2.DisconnectedSurfaces).Cast<Vector3?>().FirstOrDefault();

            //        if (sharedSurface != null)
            //        {
            //            connectedSurfaces.Add(sharedSurface.Value);

            //            cube1.MarkConnectedSurface(sharedSurface.Value);
            //            cube2.MarkConnectedSurface(sharedSurface.Value);
            //        }
            //    }
            //}

            //return new ScanResult2(cubes, distinctSurfaces, connectedSurfaces);
        }

        //private IReadOnlyList<Edge> GetEdges()
        //{
        //    //var a = Min;
        //    //var b = new Vector3(Max.X, Min.Y, Min.Z);
        //    //var c = new Vector3(Min.X, Max.Y, Min.Z);
        //    //var d = new Vector3(Max.X, Max.Y, Min.Z);
        //    //var e = new Vector3(Min.X, Min.Y, Max.Z);
        //    //var f = new Vector3(Max.X, Min.Y, Max.Z);
        //    //var g = new Vector3(Min.X, Max.Y, Max.Z);
        //    //var h = Max;

        //    return new Edge[]
        //    {
        //        //new(a, d, new(0, 0, -1)),
        //        //new(b, h, new(1, 0, 0)),
        //        //new(e, h, new(0, 0, 1)),
        //        //new(a, g, new(-1, 0, 0)),
        //        //new(a, f, new(0, -1, 0)),
        //        //new(c, h, new(0, 1, 0))

        //        new(Position + new Vector3(0.5f, 0, 0), Position + new Vector3(1, 0, 0)),
        //        new(Position + new Vector3(-0.5f, 0, 0), Position + new Vector3(-1, 0, 0)),
        //        new(Position + new Vector3(0, -0.5f, 0), Position + new Vector3(0, -1, 0)),
        //        new(Position + new Vector3(0, 0.5f, 0), Position + new Vector3(0, 1, 0)),
        //        new(Position + new Vector3(0, 0, -0.5f), Position + new Vector3(0, 0, -1)),
        //        new(Position + new Vector3(0, 0, 0.5f), Position + new Vector3(0, 0, 1))
        //    };
        //}

        // GOOD:
        private IReadOnlyList<Vector3> GetEdges()
        {
            //var a = Min;
            //var b = new Vector3(Max.X, Min.Y, Min.Z);
            //var c = new Vector3(Min.X, Max.Y, Min.Z);
            //var d = new Vector3(Max.X, Max.Y, Min.Z);
            //var e = new Vector3(Min.X, Min.Y, Max.Z);
            //var f = new Vector3(Max.X, Min.Y, Max.Z);
            //var g = new Vector3(Min.X, Max.Y, Max.Z);
            //var h = Max;

            return new Vector3[]
            {
                //new(a, d, new(0, 0, -1)),
                //new(b, h, new(1, 0, 0)),
                //new(e, h, new(0, 0, 1)),
                //new(a, g, new(-1, 0, 0)),
                //new(a, f, new(0, -1, 0)),
                //new(c, h, new(0, 1, 0))

                Position + new Vector3(0.5f, 0, 0),
                Position + new Vector3(-0.5f, 0, 0),
                Position + new Vector3(0, -0.5f, 0),
                Position + new Vector3(0, 0.5f, 0),
                Position + new Vector3(0, 0, -0.5f),
                Position + new Vector3(0, 0, 0.5f)
            };
        }

        private IReadOnlyList<Surface> GetSurfaces()
        {
            //var a = Min;
            //var b = new Vector3(Max.X, Min.Y, Min.Z);
            //var c = new Vector3(Min.X, Max.Y, Min.Z);
            //var d = new Vector3(Max.X, Max.Y, Min.Z);
            //var e = new Vector3(Min.X, Min.Y, Max.Z);
            //var f = new Vector3(Max.X, Min.Y, Max.Z);
            //var g = new Vector3(Min.X, Max.Y, Max.Z);
            //var h = Max;

            return new Surface[]
            {
                //new(a, d, new(0, 0, -1)),
                //new(b, h, new(1, 0, 0)),
                //new(e, h, new(0, 0, 1)),
                //new(a, g, new(-1, 0, 0)),
                //new(a, f, new(0, -1, 0)),
                //new(c, h, new(0, 1, 0))

                new(Position, new(1, 0, 0)),
                new(Position, new(-1, 0, 0)),
                new(Position, new(0, -1, 0)),
                new(Position, new(0, 1, 0)),
                new(Position, new(0, 0, -1)),
                new(Position, new(0, 0, 1))
            };
        }

        //private IReadOnlyList<Surface> GetSurfaces()
        //{
        //    var a = Min;
        //    var b = new Vector3(Max.X, Min.Y, Min.Z);
        //    var c = new Vector3(Min.X, Max.Y, Min.Z);
        //    var d = new Vector3(Max.X, Max.Y, Min.Z);
        //    var e = new Vector3(Min.X, Min.Y, Max.Z);
        //    var f = new Vector3(Max.X, Min.Y, Max.Z);
        //    var g = new Vector3(Min.X, Max.Y, Max.Z);
        //    var h = Max;

        //    return new Surface[]
        //    {
        //        //new(a, d, new(0, 0, -1)),
        //        //new(b, h, new(1, 0, 0)),
        //        //new(e, h, new(0, 0, 1)),
        //        //new(a, g, new(-1, 0, 0)),
        //        //new(a, f, new(0, -1, 0)),
        //        //new(c, h, new(0, 1, 0))

        //        new(a, d, new(0, 0, -1)),
        //        new(b, h, new(1, 0, 0)),
        //        new(e, h, new(0, 0, 1)),
        //        new(a, g, new(-1, 0, 0)),
        //        new(a, f, new(0, -1, 0)),
        //        new(c, h, new(0, 1, 0))
        //    };
        //}

        public void MarkConnectedSurface(Vector3 connectedSurface)
        {
            if (!Edges.Contains(connectedSurface))
            {
                throw new InvalidOperationException("Unexpected: tried to remove a surface from disconnected that we don't have");
            }

            _disconnectedSurfaces.Remove(connectedSurface);
            _connectedSurfaces.Add(connectedSurface);
        }

        //public HashSet<Vector3> AdjacentCubes = new();

        //public bool IsEnclosed => AdjacentCubes.Count == 6;
    }

    //public record Surface(Vector3 Min, Vector3 Max);

    public class Surface
    {
        public Vector3 Position { get; }
        //public Vector3 Max { get; }
        public Vector3 Normal { get; }

        public Surface(Vector3 position, /*Vector3 max,*/ Vector3 normal)
        {
            Position = position;
            //Max = max;
            Normal = normal;
        }

        public Cube GetOuterCube() => new(Position + Normal);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Surface)obj);
        }

        protected bool Equals(Surface other) => Position.Equals(other.Position) && Normal.Equals(other.Normal);

        public override int GetHashCode() => HashCode.Combine(Position, Normal);

        public override string ToString() => new {Position, Normal}.ToString()!;

        //public override bool Equals(object? obj)
        //{
        //    if (obj is null) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    return obj.GetType() == GetType() && Equals((Surface)obj);
        //}

        //protected bool Equals(Surface other)
        //{
        //    return Min.Equals(other.Min);
        //}

        //public override int GetHashCode()
        //{
        //    return Min.GetHashCode();
        //}

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

    public record ScanResult2(
        IReadOnlyList<Cube> Cubes,
        IReadOnlySet<Vector3> DistinctSurfaces,
        IReadOnlySet<Vector3> ConnectedSurfaces);

    static IReadOnlyList<Cube> ParseInputToCubes(PuzzleInput input) => input.ReadLines().Select(line =>
    {
        var coords = line.Split(',').Select(int.Parse).ToArray();
        return new Cube(new Vector3(coords[0], coords[1], coords[2]));
    }).ToArray();

    static IReadOnlySet<Cube> ParseInputToCubeMap(PuzzleInput input) => ParseInputToCubes(input).ToHashSet();

    static IReadOnlyList<Vector3> ParseInputToCubePositions(PuzzleInput input) => input.ReadLines().Select(line =>
    {
        var coords = line.Split(',').Select(int.Parse).ToArray();
        return new Vector3(coords[0], coords[1], coords[2]);
    }).ToArray();

    static ScanResult2 ScanCubes(PuzzleInput input)
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

        var distinctSurfaces = new HashSet<Vector3>();
        var connectedSurfaces = new HashSet<Vector3>();

        foreach (var (cube1, cube2) in pairs)
        {
            distinctSurfaces.UnionWith(cube1.Edges);
            distinctSurfaces.UnionWith(cube2.Edges);

            // Find shared surface, but only for touching cubes
            if (MathUtils.ManhattanDistance(cube1.Position, cube2.Position) == 1)
            {
                //cube1.AdjacentCubes.Add(cube2.Position);
                //cube2.AdjacentCubes.Add(cube1.Position);

                var sharedSurface = cube1.DisconnectedSurfaces.Intersect(cube2.DisconnectedSurfaces).Cast<Vector3?>().FirstOrDefault();

                if (sharedSurface != null)
                {
                    connectedSurfaces.Add(sharedSurface.Value);

                    cube1.MarkConnectedSurface(sharedSurface.Value);
                    cube2.MarkConnectedSurface(sharedSurface.Value);
                }
            }
        }

        return new ScanResult2(cubes, distinctSurfaces, connectedSurfaces);
    }

    //static ScanResult ScanCubes(PuzzleInput input)
    //{
    //    var cubes = ParseInputToCubes(input);

    //    // Do not test a cube against itself!
    //    // Any cube whose 3D Manhattan Distance is greater than one, they share no edges, so we can quickly eliminate them
    //    // So, for any pair whose 3D Manhattan Distance == 1, find the shared side (note they are cubes, so can only share one side)
    //    // Keep a list of these sides, then get distinct count of the list, that gives us our shared sides
    //    // Keep a list of all sides, then get distinct count of the list, that gives us all of our sides
    //    // Take shared off of all, I think!!

    //    var pairs = cubes.SelectMany(
    //        cube1 => cubes
    //            .Where(cube2 => cube1.Position != cube2.Position)
    //            .Select(cube2 => (cube1, cube2)));

    //    var distinctSurfaces = new HashSet<Surface>();
    //    var connectedSurfaces = new HashSet<Surface>();

    //    foreach (var (cube1, cube2) in pairs)
    //    {
    //        distinctSurfaces.UnionWith(cube1.Surfaces);
    //        distinctSurfaces.UnionWith(cube2.Surfaces);

    //        // Find shared surface, but only for touching cubes
    //        if (MathUtils.ManhattanDistance(cube1.Position, cube2.Position) == 1)
    //        {
    //            //cube1.AdjacentCubes.Add(cube2.Position);
    //            //cube2.AdjacentCubes.Add(cube1.Position);

    //            var sharedSurface = cube1.DisconnectedSurfaces.Intersect(cube2.DisconnectedSurfaces).FirstOrDefault();

    //            if (sharedSurface != null)
    //            {
    //                connectedSurfaces.Add(sharedSurface);

    //                cube1.MarkConnectedSurface(sharedSurface);
    //                cube2.MarkConnectedSurface(sharedSurface);
    //            }
    //        }
    //    }

    //    return new ScanResult(cubes, distinctSurfaces, connectedSurfaces);
    //}
}
