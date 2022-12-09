namespace AoC.Day09;

public class Day9Solver : ISolver, IVisualize
{
    public string DayName => "Rope Bridge";

    public long? SolvePart1(PuzzleInput input) => SimulateRopeMovement(input, 2);

    public long? SolvePart2(PuzzleInput input) => SimulateRopeMovement(input, 10);

    /// <summary>
    /// Simulates the complete hypothetical series of motions, for the specified rope length.
    /// Returns the number of positions that the tail of the rope visits at least once.
    /// </summary>
    static long SimulateRopeMovement(PuzzleInput input, int numKnotsInRope, Action<Vector2[]>? update = null)
    {
        var knots = Enumerable.Range(0, numKnotsInRope).Select(_ => new Vector2(0, 0)).ToArray();
        var tailVisited = new HashSet<Vector2>();

        foreach (var (dir, amount) in ParseMovements(input))
        {
            for (var move = 0; move < amount; move++)
            {
                // Move the head, and then use the process to move the other knots in turn, using each previous knot as the "base" reference
                var prevKnot = knots[0] += dir;
                for (var knotIdx = 1; knotIdx < knots.Length; knotIdx++)
                {
                    knots[knotIdx] = prevKnot = MoveKnot(knots[knotIdx], prevKnot);
                }

                tailVisited.Add(knots[^1]);
                update?.Invoke(knots);
            }
        }

        return tailVisited.Count;
    }

    private static Vector2 MoveKnot(Vector2 knot, Vector2 prevKnot)
    {
        if (Are2StepsAway(prevKnot, knot) && AreInSameRowOrColumn(prevKnot, knot))
        {
            return knot + Vector2.Normalize(prevKnot - knot); // Move ONE step in that direction to keep up
        }

        if (!AreTouching(prevKnot, knot) && !AreInSameRowOrColumn(prevKnot, knot))
        {
            return knot + Vector2.Clamp(prevKnot - knot, new(-1, -1), new(1, 1)); // Move ONE step DIAGONALLY to keep up
        }

        return knot;
    }

    static bool AreTouching(Vector2 a, Vector2 b) => new Bounds(
        TopLeft: a + GridUtils.North + GridUtils.West,
        BottomRight: a + GridUtils.South + GridUtils.East).Contains(b);

    readonly record struct Bounds(Vector2 TopLeft, Vector2 BottomRight)
    {
        public bool Contains(Vector2 position) =>
            position.X >= TopLeft.X && position.X <= BottomRight.X &&
            position.Y >= TopLeft.Y && position.Y <= BottomRight.Y;
    }

    static bool Are2StepsAway(Vector2 a, Vector2 b) => MathUtils.ManhattanDistance(a, b) == 2;

    static bool AreInSameRowOrColumn(Vector2 a, Vector2 b) => a.X.Round() == b.X.Round() || a.Y.Round() == b.Y.Round();

    record Movement(Vector2 Dir, int Amount);

    static IEnumerable<Movement> ParseMovements(PuzzleInput input) =>
        input.ReadLines()
            .Select(line => line.Split(" "))
            .Select(parts => new Movement(
                parts[0] switch
                {
                    "U" => GridUtils.North,
                    "D" => GridUtils.South,
                    "L" => GridUtils.West,
                    "R" => GridUtils.East,
                    _ => throw new InvalidOperationException("Invalid dir: " + parts[0])
                },
                int.Parse(parts[1])));

    public IReadOnlyCollection<string> GetVisualization(PuzzleInput input)
    {
        var frames = new List<string>();
        SimulateRopeMovement(input, 10, knots =>
            frames.Add(knots.Select((v, i) => (v, i)).Reverse().ToStringGrid(x => x.v, x => x.i == 0 ? 'H' : (char) ('0' + x.i), ' ').RenderGridToString()));
        return frames;
    }
}
