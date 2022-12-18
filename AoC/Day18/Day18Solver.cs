namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Boiling Boulders";

    /// <summary>
    /// Count up all the sides that aren't connected to another cube.
    /// </summary>
    public long? SolvePart1(PuzzleInput input) => ScanEdges(ParseInputToCubeMap(input)).DisconnectedEdges.Count;

    /// <summary>
    /// What is the exterior surface area of the scanned lava droplet?
    /// </summary>
    public long? SolvePart2(PuzzleInput input)
    {
        var lavaCubes = ParseInputToCubeMap(input);

        // Work out the min and max bounds of our world
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);

        foreach (var cube in lavaCubes)
        {
            min = Vector3.Min(min, cube.Position);
            max = Vector3.Max(max, cube.Position);
        }

        min -= Vector3.One;
        max += Vector3.One;

        bool IsWithinBounds(Cube cube) =>
            cube.Position.X >= min.X && cube.Position.X <= max.X &&
            cube.Position.Y >= min.Y && cube.Position.Y <= max.Y &&
            cube.Position.Z >= min.Z && cube.Position.Z <= max.Z;

        // Explore the world to find all of the outside space around the shape made by the lava cube
        var explore = new Queue<Cube>(new[] {new Cube(min)});
        var spaces = new HashSet<Cube>();

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
                    // Only look at ones within our bounds and if we've not yet reached any lave cubes,
                    // that means we're still in the outside space, so add that to the queue to explore
                    if (IsWithinBounds(nextCube) && !lavaCubes.Contains(nextCube))
                    {
                        explore.Enqueue(nextCube);
                    }
                }
            }
        }

        // Build which cubes are air cavities
        var airCavities = new HashSet<Cube>();

        for (var z = (int) min.Z; z <= (int) max.Z; z++)
        for (var y = (int) min.Y; y <= (int) max.Y; y++)
        for (var x = (int) min.X; x <= (int) max.X; x++)
            airCavities.Add(new Cube(new Vector3(x, y, z)));

        airCavities.ExceptWith(lavaCubes);
        airCavities.ExceptWith(spaces);

        // Finally, calculate the exterior surface area!
        var (_, _, disconnectedEdges) = ScanEdges(lavaCubes);
        var (_, _, internalDisconnectedEdges) = ScanEdges(airCavities);

        return disconnectedEdges.Except(internalDisconnectedEdges).Count();
    }

    public Action<string> Logger { get; set; } = Console.WriteLine;

    /// <summary>
    /// Scan the specified 3D map of cubes, looking for any connected (i.e. shared) edges.
    /// </summary>
    static (IReadOnlySet<Vector3> DistinctEdges, IReadOnlySet<Vector3> ConnectedEdges, IReadOnlySet<Vector3> DisconnectedEdges) ScanEdges(
        IReadOnlySet<Cube> cubeMap)
    {
        var distinctEdges = new HashSet<Vector3>();
        var connectedEdges = new HashSet<Vector3>();

        foreach (var cube in cubeMap)
        {
            distinctEdges.UnionWith(cube.Edges);

            foreach (var sharedEdge in cube.GetSharedEdges(cubeMap))
            {
                connectedEdges.Add(sharedEdge);
            }
        }

        return (distinctEdges, connectedEdges, distinctEdges.Except(connectedEdges).ToHashSet());
    }

    static IReadOnlySet<Cube> ParseInputToCubeMap(PuzzleInput input) => input.ReadLines().Select(line =>
    {
        var coords = line.Split(',').Select(float.Parse).ToArray();
        return new Cube(new Vector3(coords));
    }).ToHashSet();

    static readonly IReadOnlyList<Vector3> Axis3d = new Vector3[]
    {
        new(1, 0, 0),
        new(-1, 0, 0),
        new(0, -1, 0),
        new(0, 1, 0),
        new(0, 0, -1),
        new(0, 0, 1)
    };

    sealed class Cube
    {
        public Vector3 Position { get; }
        public IReadOnlySet<Vector3> Edges { get; }

        public Cube(Vector3 position)
        {
            Position = position;
            Edges = new HashSet<Vector3>(new[]
            {
                Position + new Vector3(0.5f, 0, 0),
                Position + new Vector3(-0.5f, 0, 0),
                Position + new Vector3(0, -0.5f, 0),
                Position + new Vector3(0, 0.5f, 0),
                Position + new Vector3(0, 0, -0.5f),
                Position + new Vector3(0, 0, 0.5f)
            });
        }

        public IEnumerable<Vector3> GetSharedEdges(IReadOnlySet<Cube> cubeMap)
        {
            var nextCubes = Axis3d.Select(dir => new Cube(Position + dir));

            foreach (var nextCube in nextCubes)
            {
                if (cubeMap.Contains(nextCube))
                {
                    var sharedEdge = Edges.Intersect(nextCube.Edges).Single();
                    yield return sharedEdge;
                }
            }
        }

        public override bool Equals(object? obj) => obj is Cube other && Position == other.Position;

        public override int GetHashCode() => Position.GetHashCode();

        public override string ToString() => Position.ToString();
    }
}
