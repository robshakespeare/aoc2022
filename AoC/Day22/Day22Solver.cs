namespace AoC.Day22;

public partial class Day22Solver : ISolver
{
    public string DayName => "Monkey Map";

    private static readonly Matrix3x2 RotateRightClockwise = Matrix3x2.CreateRotation(90.DegreesToRadians());
    private static readonly Matrix3x2 RotateLeftCounterclockwise = Matrix3x2.CreateRotation(-90.DegreesToRadians());

    public long? SolvePart1(PuzzleInput input) => Map.Create(input, isCube: false).FollowInstructions().Password;

    // 1 has 3 fall offs
    // 2 has 3 fall offs
    // 3 has 2 fall offs
    // 4 has 1 fall off
    // 5 has 2 fall offs
    // 6 has 3 fall offs

    // 1-4 already
    // 1-2: 180° (hence 2-1: 180°)
    // 1-3: -90° (hence 3-1: 90°)
    // 1-6: 180° (hence 6-1: 180°)

    // 2-3 already
    // 2-1: 180° (hence 1-2: 180°)
    // 2-5: 180° (hence 5-2: 180°)
    // 2-6: 90° (hence 6-2: -90°)

    // 3-2 already
    // 3-4 already
    // 3-1: 90° (hence 1-3: -90°)
    // 3-5: -90° (hence 5-3: 90°)

    // 4-1 already
    // 4-3 already
    // 4-5 already
    // 4-6: 90° (hence 6-4: -90°)

    // 5-4 already
    // 5-6 already
    // 5-2: 180° (hence 2-5: 180°)
    // 5-3: 90° (hence 3-5: -90°)

    // 6-5 already
    // 6-1: 180° (hence 1-6: 180°)
    // 6-2: -90° (hence 2-6: 90°)
    // 6-4: -90° (hence 4-6: 90°)
    public long? SolvePart2(PuzzleInput input) => Map.Create(input, isCube: true).FollowInstructions().Password;

    private static string[] ManuallyCalculatedExamplePairs =
    {
        "NU",
        "DI",
        "BV",
        "KT",
        "AE",
        "HW",
        "GS"
    };

    private static string[] ManuallyCalculatedRealPairs =
    {
        "GJ",
        "AV",
        "EW",
        "LM",
        "FR",
        "SV",
        "DP"
    };

    public record Map(
        Dictionary<Vector2, Cell> Cells,
        string[] Instructions,
        bool IsCube,
        Vector2 Min,
        Vector2 Max,
        Dictionary<int, Line> MinMaxX,
        Dictionary<int, Line> MinMaxY,
        int FaceSize)
    {
        public Face[] Faces { get; private set; } = Array.Empty<Face>();

        public static Map Create(string input, bool isCube)
        {
            var parts = input.Split($"{Environment.NewLine}{Environment.NewLine}");
            var instructions = ParseInstructions(parts[1]);

            var cells = parts[0].ReadLines()
                .SelectMany(
                    (line, y) => line.Select(
                        (chr, x) => new Cell(new Vector2(x + 1, y + 1), chr)))
                .ToDictionary(cell => cell.Position);

            var min = new Vector2(float.MaxValue);
            var max = new Vector2(float.MinValue);

            foreach (var (p, _) in cells)
            {
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }

            var minMaxX = BuildMinMax(true, cells, min, max); // MinMaxX: Where Key is Y
            var minMaxY = BuildMinMax(false, cells, min, max); // MinMaxY: Where Key is X

            var faceSize = (int)Math.Max(max.X, max.Y) / 4;

            return new Map(cells, instructions, isCube, min, max, minMaxX, minMaxY, faceSize).AssignFaces();
        }

        private static Dictionary<int, Line> BuildMinMax(bool isX, Dictionary<Vector2, Cell> cells, Vector2 mapMin, Vector2 mapMax)
        {
            var minMax = new Dictionary<int, Line>();

            var componentIndex = isX ? 0 : 1; // i.e. component is X when isX, or Y when !isX
            var otherComponentIndex = isX ? 1 : 0; // i.e. otherComponent is Y when isX, or X when !isX

            for (var otherComponent = (int)mapMin[otherComponentIndex]; otherComponent <= (int)mapMax[otherComponentIndex]; otherComponent++)
            {
                var min = int.MaxValue;
                var max = 0;

                for (var component = (int)mapMin[componentIndex]; component <= (int)mapMax[componentIndex]; component++)
                {
                    if (!IsVoid(cells, isX
                            ? new Vector2(component, otherComponent)
                            : new Vector2(otherComponent, component)))
                    {
                        min = Math.Min(min, component);
                        max = Math.Max(max, component);
                    }
                }

                minMax[otherComponent] = new Line(min, max);
            }

            return minMax;
        }

        private Map AssignFaces()
        {
            var ids = new Dictionary<int, List<Cell>>();

            foreach (var cell in Cells.Values.Where(cell => !cell.IsVoid))
            {
                var id = (int)Math.Ceiling(cell.Position.Y / FaceSize) * 10 + (int)Math.Ceiling(cell.Position.X / FaceSize);
                ids.GetOrAdd(id, () => new List<Cell>()).Add(cell);
            }

            foreach (var (cells, n) in ids.Values.Select((cells, idx) => (cells, n: idx + 1)))
            {
                foreach (var cell in cells)
                {
                    cell.FaceId = n.ToString()[0];
                }
            }

            Faces = Cells.Values
                .Where(cell => !cell.IsVoid)
                .GroupBy(cell => cell.FaceId)
                .Select(grp => Face.Create(grp.Key, grp.ToDictionary(cell => cell.Position), this)).ToArray();

            PairEdgesOfEachFace();

            return this;
        }

        private void PairEdgesOfEachFace()
        {
            var pairs = FaceSize == 50 ? ManuallyCalculatedRealPairs : ManuallyCalculatedExamplePairs;

            var outerEdges = Faces.SelectMany(f => f.Edges.Where(e => e.IsOuter)).ToDictionary(e => e.Id);

            foreach (var (a, b) in pairs.Select(pair => (outerEdges[pair[0]], outerEdges[pair[1]])))
            {
                a.PairedEdge = b;
                b.PairedEdge = a;
            }

            //var outerEdges = Faces.SelectMany(f => f.Edges.Where(e => e.IsOuter)).ToArray();

            //Edge[] GetUnpaired() => outerEdges.Where(e => e.PairedEdge == null).ToArray();

            //IEnumerable<(Edge A, Edge B)> GetRemainingCandidates()
            //{
            //    var unpaired = GetUnpaired();

            //    return unpaired.SelectMany(a => unpaired
            //            .Where(b => a.Face != b.Face)
            //            .Select(b => string.Concat(new[] { a.Id, b.Id }.Order())))
            //        .Distinct()
            //        .Select(s => (unpaired.First(e => e.Id == s[0]), unpaired.First(e => e.Id == s[1])));
            //}

            //Console.WriteLine($"#unpaired: {GetUnpaired().Length}");

            //// Pair the 90 degree edges first
            //foreach (var (a, b) in GetRemainingCandidates())
            //{
            //    // Check they're on the same corner
            //    if (a.Line.Min + a.Normal == b.Line.Min + b.Normal ||
            //        a.Line.Max + a.Normal == b.Line.Max + b.Normal ||
            //        a.Line.Max + a.Normal == b.Line.Min + b.Normal)
            //    {
            //        Console.WriteLine($"{a.Id}{b.Id}: angle {MathUtils.AngleBetween(a.Line.Dir, b.Line.Dir)}");
            //        a.PairedEdge = b;
            //        b.PairedEdge = a;
            //    }
            //}

            //// Pair the "same facing" ones
            //foreach (var (a, b) in GetRemainingCandidates()
            //             .Where(x => x.A.Normal == x.B.Normal)
            //             //.Where(x => x.A.Line.Max + x.A.Line.Dir != x.B.Line.Min - x.B.Line.Dir)
            //             .GroupBy(x => x.A.Normal)
            //             .Where(x =>
            //             {
            //                 //Console.WriteLine(string.Join(", ", x.Select(x => $"huh: {x.A.Id}{x.B.Id}")));
            //                 return x.Count() == 1;
            //             })
            //             .Select(x => x.First()))
            //{
            //    Console.WriteLine($"{a.Id}{b.Id}: same face");
            //    a.PairedEdge = b;
            //    b.PairedEdge = a;
            //}

            //Console.WriteLine($"#unpaired: {GetUnpaired().Length}");

            ////// Sanity check
            ////if (GetUnpaired().Length != 4)
            ////{
            ////    throw new InvalidOperationException("Unexpected cube state");
            ////}

            //// Use process of elimination to pair the remaining 4
            //foreach (var (a, b) in GetRemainingCandidates())
            //{
            //    Console.WriteLine($"{a.Id}{b.Id}: one option");
            //}


        }

        public Vector2 LocateStart()
        {
            var dir = GridUtils.East;
            var position = Min;

            while (!Cells[position].IsOpen)
            {
                position += dir;
            }

            return position;
        }

        public bool IsVoid(Vector2 position) => IsVoid(Cells, position);

        private static bool IsVoid(Dictionary<Vector2, Cell> cells, Vector2 position) => !cells.TryGetValue(position, out var cell) || cell.IsVoid;

        public (Vector2 resultPosition, Vector2 resultDir) HandleWrapAround3d(Vector2 position, Vector2 dir)
        {
            // If we reached a void, then step back one in the opposite dir to get on our edge
            // And then map that position to the corresponding position on the paired edge

            throw new InvalidOperationException("rs-todo!");
        }

        public (Vector2 resultPosition, Vector2 resultDir) HandleWrapAround2d(Vector2 position, Vector2 dir)
        {
            var isYMove = Math.Abs(dir.Y) != 0;
            var componentIndex = isYMove ? 1 : 0;
            var otherComponentIndex = isYMove ? 0 : 1;
            var minMax = (isYMove ? MinMaxY : MinMaxX)[(int)position[otherComponentIndex]];
            var pos = position[componentIndex];

            if (pos > minMax.Max)
            {
                position[componentIndex] = minMax.Min;
            }

            if (pos < minMax.Min)
            {
                position[componentIndex] = minMax.Max;
            }

            return (position, dir);
        }

        public (long Password, Map Map) FollowInstructions()
        {
            // You begin the path in the leftmost open tile of the top row of tiles. Initially, you are facing to the right
            // right -- East
            // down -- South
            // left -- West
            // up -- North

            var dir = GridUtils.East;
            var position = LocateStart();
            Cells[position].Tile = DirectionToArrow(dir);

            // Do the plotting!
            foreach (var instruction in Instructions)
            {
                switch (instruction)
                {
                    case "R":
                        dir = Vector2.Transform(dir, RotateRightClockwise);
                        break;
                    case "L":
                        dir = Vector2.Transform(dir, RotateLeftCounterclockwise);
                        break;
                    default:
                        var movement = int.Parse(instruction);

                        // Incrementally move position by that amount
                        for (var move = 0; move < movement; move++)
                        {
                            var prevPosition = position;
                            var preDir = dir;

                            position += dir;
                            (position, dir) = IsCube
                                ? HandleWrapAround3d(position, dir)
                                : HandleWrapAround2d(position, dir);

                            //if (IsCube)
                            //{
                            //    // rs-todo: check for if we're currently on an edge, and if so, just jump to corresponding pos and dir
                            //    if (isOnEdge)
                            //    {
                            //        throw new InvalidOperationException("rs-todo!");
                            //    }
                            //    else
                            //    {
                            //        position += dir;
                            //    }
                            //}
                            //else
                            //{
                            //    position += dir;
                            //    (position, dir) = HandleWrapAround2d(position, dir);
                            //}

                            // If the next position is wall, stop instruction, and ensure position is last position (which must have been a open tile), and dir is last dir
                            var cell = Cells[position];
                            if (cell.IsWall)
                            {
                                position = prevPosition;
                                dir = preDir;
                                break;
                            }

                            cell.Tile = DirectionToArrow(dir);
                        }

                        break;
                }

                Cells[position].Tile = DirectionToArrow(dir);
            }

            // The final password is the sum of 1000 times the row (Y), 4 times the column (X), and the facing.
            // Facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^).

            var row = (long)position.Y;
            var column = (long)position.X;
            var facing = DirectionToArrow(dir) switch
            {
                '>' => 0,
                'v' => 1,
                '<' => 2,
                '^' => 3,
                _ => throw new InvalidOperationException("Invalid dir")
            };

            var password = 1000 * row + 4 * column + facing;

            // rs-todo: rem this temp logging
            //Cells.ToStringGrid(x => x.Key, x => x.Value.FaceId, ' ').RenderGridToConsole(); // rs-todo: rem this

            Faces.SelectMany(x => x.Edges.Where(e => e.IsOuter).SelectMany(e => e.Positions.Select(p => (e, p))))
                .ToStringGrid(x => x.p, x => x.e.Id, ' ').RenderGridToConsole(); // rs-todo: rem this

            return (password, this);
        }
    }

    public record Face(char Id, Dictionary<Vector2, Cell> Cells, Vector2 Min, Vector2 Max, Map Map)
    {
        public Edge[] Edges { get; private set; } = Array.Empty<Edge>();

        public static Face Create(char id, Dictionary<Vector2, Cell> cells, Map map)
        {
            var min = new Vector2(float.MaxValue);
            var max = new Vector2(float.MinValue);

            foreach (var (p, _) in cells)
            {
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }

            return new Face(id, cells, min, max, map).AssignEdges();
        }

        private Face AssignEdges()
        {
            var tl = Min;
            var tr = new Vector2(Max.X, Min.Y);
            var bl = new Vector2(Min.X, Max.Y);
            var br = Max;

            var faceN = (Id - '1') * 4;

            Edges = new[]
            {
                new Edge((char)('A' + faceN + 0), new Line2(tl, tr), GridUtils.North, this), // top: tl -> tr
                new Edge((char)('A' + faceN + 1), new Line2(tr, br), GridUtils.East, this), //  rgt: tr -> br
                new Edge((char)('A' + faceN + 2), new Line2(bl, br), GridUtils.South, this), // bot: bl -> br
                new Edge((char)('A' + faceN + 3), new Line2(tl, bl), GridUtils.West, this) //   lef: tl -> bl
            }.ToArray();

            return this;
        }
    }

    public record Edge(char Id, Line2 Line, Vector2 Normal, Face Face)
    {
        public HashSet<Vector2> Positions { get; } = Enumerable.Range(0, (int)Vector2.Distance(Line.Max + Line.Dir, Line.Min))
            .Select(i => Line.Min + Line.Dir * i)
            .ToHashSet();

        public bool IsOuter { get; } = Face.Map.IsVoid(Line.Min + Normal);

        public Edge? PairedEdge { get; set; }
    }

    /// <remarks>
    /// Min and Max are inclusive.
    /// </remarks>
    public record Line2(Vector2 Min, Vector2 Max)
    {
        public Vector2 Dir { get; } = Vector2.Normalize(Max - Min);
    }

    public record Line(int Min, int Max);

    public record Cell(Vector2 Position, char Tile)
    {
        public char Tile { get; set; } = Tile;

        public bool IsWall => Tile == '#';

        public bool IsVoid => Tile == ' ';

        public bool IsOpen => !IsWall && !IsVoid;

        public char FaceId { get; set; } = ' ';
    }

    static char DirectionToArrow(Vector2 dir) => dir switch
    {
        _ when dir == GridUtils.East => '>',
        _ when dir == GridUtils.South => 'v',
        _ when dir == GridUtils.West => '<',
        _ when dir == GridUtils.North => '^',
        _ => throw new InvalidOperationException("Invalid dir: " + dir)
    };

    static string[] ParseInstructions(string instructions) =>
        ParseInstructionsRegex.Matches(instructions).Select(match => match.Value).ToArray();

    static readonly Regex ParseInstructionsRegex = BuildParseInstructionsRegex();

    [GeneratedRegex(@"\d+|L|R", RegexOptions.Compiled)]
    private static partial Regex BuildParseInstructionsRegex();
}
