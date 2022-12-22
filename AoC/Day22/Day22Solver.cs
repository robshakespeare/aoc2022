using static System.Environment;

namespace AoC.Day22;

public partial class Day22Solver : ISolver
{
    public string DayName => "Monkey Map";

    private static readonly Matrix3x2 RotateRightClockwise = Matrix3x2.CreateRotation(90.DegreesToRadians());
    private static readonly Matrix3x2 RotateLeftCounterclockwise = Matrix3x2.CreateRotation(-90.DegreesToRadians());

    public long? SolvePart1(PuzzleInput input)
    {
        var parts = input.ToString().Split($"{NewLine}{NewLine}");

        var map = Map.Create(parts[0]);
        var instructions = ParseInstructions(parts[1]);

        // You begin the path in the leftmost open tile of the top row of tiles. Initially, you are facing to the right
        // right -- East
        // down -- South
        // left -- West
        // up -- North

        var dir = GridUtils.East;
        var position = map.LocateStart();

        // Do the plotting!
        foreach (var instruction in instructions)
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
                        position += dir;

                        // If we go off the grid, wrap around
                        position = map.HandleWrapAround(position, dir);

                        // If the next position is wall, stop instruction, and ensure position is last position (which must have been a open tile)
                        var cell = map.Cells[position];
                        if (cell.IsWall)
                        {
                            position = prevPosition;
                            break;
                        }

                        cell.Tile = DirectionToArrow(dir);
                    }

                    break;
            }

            map.Cells[position].Tile = DirectionToArrow(dir);
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

        return 1000 * row + 4 * column + facing;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record Map(Dictionary<Vector2, Cell> Cells, Vector2 Min, Vector2 Max, Dictionary<int, Line> MinMaxX, Dictionary<int, Line> MinMaxY)
    {
        public static Map Create(string input)
        {
            var cells = input.ReadLines()
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

            return new Map(cells, min, max, minMaxX, minMaxY);
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

        public Vector2 HandleWrapAround(Vector2 position, Vector2 dir)
        {
            if (Math.Abs(dir.Y) != 0)
            {
                var minMaxY = MinMaxY[(int)position.X];

                if (position.Y > minMaxY.Max)
                {
                    position.Y = minMaxY.Min;
                }

                if (position.Y < minMaxY.Min)
                {
                    position.Y = minMaxY.Max;
                }
            }
            else
            {
                var minMaxX = MinMaxX[(int)position.Y];

                if (position.X > minMaxX.Max)
                {
                    position.X = minMaxX.Min;
                }

                if (position.X < minMaxX.Min)
                {
                    position.X = minMaxX.Max;
                }
            }

            return position;
        }
    }

    public record Line(int Min, int Max);

    public record Cell(Vector2 Position, char Tile)
    {
        public char Tile { get; set; } = Tile;

        public bool IsWall => Tile == '#';

        public bool IsVoid => Tile == ' ';

        public bool IsOpen => !IsWall && !IsVoid;
    }

    static char DirectionToArrow(Vector2 dir) => dir switch
    {
        _ when dir == GridUtils.East => '>',
        _ when dir == GridUtils.South => 'v',
        _ when dir == GridUtils.West => '<',
        _ when dir == GridUtils.North => '^',
        _ => throw new InvalidOperationException("Invalid dir: " + dir)
    };

    static IReadOnlyCollection<string> ParseInstructions(string instructions) =>
        ParseInstructionsRegex.Matches(instructions).Select(match => match.Value).ToArray();

    static readonly Regex ParseInstructionsRegex = BuildParseInstructionsRegex();

    [GeneratedRegex(@"\d+|L|R", RegexOptions.Compiled)]
    private static partial Regex BuildParseInstructionsRegex();
}
