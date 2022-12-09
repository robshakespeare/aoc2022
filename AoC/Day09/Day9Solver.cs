namespace AoC.Day09;

public class Day9Solver : ISolver
{
    public string DayName => "Rope Bridge";

    public long? SolvePart1(PuzzleInput input) => SimulateRopeMovement(input, 2);

    public long? SolvePart2(PuzzleInput input) => SimulateRopeMovement(input, 10);

    /// <summary>
    /// Simulates the complete hypothetical series of motions, for the specified rope length.
    /// Returns the number of positions that the tail of the rope visits at least once.
    /// </summary>
    static long SimulateRopeMovement(PuzzleInput input, int numKnotsInRope)
    {
        var head = new Vector2(0, 0);
        var otherKnots = Enumerable.Range(1, numKnotsInRope - 1).Select(_ => new Vector2(0, 0)).ToArray();
        var tailVisited = new HashSet<Vector2>();

        foreach (var (dir, amount) in ParseMovements(input))
        {
            for (var move = 0; move < amount; move++)
            {
                // Move the head
                head += dir;

                // And now use the process to move the other knots in turn, using each previous knot as the "base" reference
                var prevKnot = head;
                for (var knot = 0; knot < otherKnots.Length; knot++)
                {
                    otherKnots[knot] = prevKnot = MoveKnot(otherKnots[knot], prevKnot);
                }

                tailVisited.Add(otherKnots[^1]);
            }
        }

        return tailVisited.Count;
    }

    private static Vector2 MoveKnot(Vector2 knot, Vector2 prevKnot)
    {
        if (Are2StepsAway(prevKnot, knot) && AreInSameRowOrColumn(prevKnot, knot))
        {
            // Move ONE step in that direction to keep up
            return knot + Vector2.Normalize(prevKnot - knot);
        }

        if (!AreTouching(prevKnot, knot) && !AreInSameRowOrColumn(prevKnot, knot))
        {
            // Move ONE step DIAGONALLY to keep up
            var changeX = prevKnot.X > knot.X ? 1 : -1;
            var changeY = prevKnot.Y > knot.Y ? 1 : -1;
            var diagonal = new Vector2(changeX, changeY);
            return knot + diagonal;
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
}
