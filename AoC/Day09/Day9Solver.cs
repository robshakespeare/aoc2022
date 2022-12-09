using System.Security.Cryptography.X509Certificates;

namespace AoC.Day09;

public class Day9Solver : ISolver
{
    public string DayName => "Rope Bridge";

    public long? SolvePart1(PuzzleInput input)
    {
        var head = new Vector2(0, 0);
        var tail = new Vector2(0, 0);
        var tailVisited = new HashSet<Vector2>();

        foreach (var (command, dir, amount) in ParseMovements(input))
        {
            for (var move = 0; move < amount; move++)
            {
                var prevHead = head;
                head += dir;

                if (Are2StepsAway(head, tail) &&
                    AreInSameRowOrColumn(head, tail))
                {
                    // Move one step in that dir to keep up
                    tail += dir;
                }
                else if (!AreTouching(head, tail) &&
                         !AreInSameRowOrColumn(head, tail))
                {
                    // Move one step DIAGONALLY to keep up

                    var diagonal = Vector2.Clamp(Vector2.Normalize(prevHead - tail), new Vector2(-0.5f), new Vector2(0.5f)) * 2;

                    tail += diagonal;
                }

                if (head.X.Round() != (long) head.X || head.Y.Round() != (long) head.Y)
                {
                    throw new InvalidOperationException("Head wrong at: " + new { head, command, dir, amount});
                }

                if (tail.X.Round() != (long)tail.X || tail.Y.Round() != (long)tail.Y)
                {
                    throw new InvalidOperationException("Tail wrong at: " + new { tail, command, dir, amount });
                }

                tailVisited.Add(tail);
            }


            var dictionary = new Dictionary<char, Vector2>
            {
                {'s', Vector2.Zero},
                {'H', head},
                {'T', tail}
            };

            //Console.WriteLine($"Head: {head} -- Tail: {tail}");
            //Console.WriteLine($"{command} {amount}");
            //dictionary.ToStringGrid(x => x.Value, x => x.Key, '.').RenderGridToConsole();
        }

        return tailVisited.Count;
    }

    static long GridDistance(Vector2 a, Vector2 b)
    {
        return (long)Math.Floor(Math.Abs((a - b).Length()));
        //return Math.Abs((a - b).Length()).Round();
    }

    static bool AreTouching(Vector2 a, Vector2 b)
    {
        return GridUtils.CenterAndDirectionsIncludingDiagonal.Select(x => a + x).ToHashSet().Contains(b);

        var tl = a + GridUtils.North + GridUtils.West;
        //var tr = a + GridUtils.North + GridUtils.East;
        //var bl = a + GridUtils.South + GridUtils.West;
        var br = a + GridUtils.South + GridUtils.East;

        var bounds = new Bounds(tl, br);

        return bounds.Contains(b);
    }

    public readonly record struct Bounds(Vector2 TopLeft, Vector2 BottomRight)
    {
        public bool Contains(Vector2 position) =>
            position.X >= TopLeft.X &&
            position.Y >= TopLeft.Y &&
            position.X <= BottomRight.X &&
            position.Y <= BottomRight.Y;
    }

    //static bool AreTouching(Vector2 a, Vector2 b) => GridDistance(a, b) <= 1; //MathUtils.ManhattanDistance(a, b) <= 2;

    static bool Are2StepsAway(Vector2 a, Vector2 b) => MathUtils.ManhattanDistance(a, b) == 2;

    static bool AreInSameRowOrColumn(Vector2 a, Vector2 b) => a.X.Round() == b.X.Round() || a.Y.Round() == b.Y.Round();

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record Movement(string Command, Vector2 Dir, int Amount);

    static IEnumerable<Movement> ParseMovements(PuzzleInput input) =>
        input.ReadLines()
            .Select(line => line.Split(" "))
            .Select(parts => new Movement(
                parts[0],
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
