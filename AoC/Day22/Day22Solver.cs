namespace AoC.Day22;

public partial class Day22Solver : ISolver
{
    public string DayName => "Monkey Map";

    private static readonly Matrix3x2 RotateRightClockwise = Matrix3x2.CreateRotation(90.DegreesToRadians());
    private static readonly Matrix3x2 RotateLeftCounterclockwise = Matrix3x2.CreateRotation(-90.DegreesToRadians());

    public long? SolvePart1(PuzzleInput input) => Map.Create(input).FollowInstructions();

    public long? SolvePart2(PuzzleInput input)
    {
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

        return null;
    }

    public record Map(
        Dictionary<Vector2, Cell> Cells,
        string[] Instructions,
        Vector2 Min,
        Vector2 Max,
        Dictionary<int, Line> MinMaxX,
        Dictionary<int, Line> MinMaxY,
        int FaceSize)
    {
        public static Map Create(string input)
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

            return new Map(cells, instructions, min, max, minMaxX, minMaxY, faceSize).AssignFaceIds();
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
                    if (cells.TryGetValue(isX
                                ? new Vector2(component, otherComponent)
                                : new Vector2(otherComponent, component),
                            out var cell) &&
                        !cell.IsVoid)
                    {
                        min = Math.Min(min, component);
                        max = Math.Max(max, component);
                    }
                }

                minMax[otherComponent] = new Line(min, max);
            }

            return minMax;
        }

        private Map AssignFaceIds()
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
                    cell.FaceId = n.ToString().Single();
                }
            }

            return this;
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

        public (Vector2 resultPosition, Vector2 resultDir) HandleWrapAround(Vector2 position, Vector2 dir)
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

        public long FollowInstructions()
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

                            // If we go off the grid, wrap around
                            (position, dir) = HandleWrapAround(position, dir);

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

            // rs-todo: bring back:
            //var isExample = (int)Max.Y == 12;
            //if (isExample)
            //{
            //    Cells.ToStringGrid(x => x.Key, x => x.Value.Tile, ' ').RenderGridToConsole();
            //}

            Cells.ToStringGrid(x => x.Key, x => x.Value.FaceId, ' ').RenderGridToConsole(); // rs-todo: rem this

            return 1000 * row + 4 * column + facing;
        }
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
