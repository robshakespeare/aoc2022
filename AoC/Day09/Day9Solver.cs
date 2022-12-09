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
                //var prevHead = head;
                head += dir;

                if (Are2StepsAway(head, tail) &&
                    AreInSameRowOrColumn(head, tail))
                {
                    // Move one step in that dir to keep up
                    //tail += dir;
                    tail += Vector2.Normalize(head - tail);
                }
                else if (!AreTouching(head, tail) &&
                         !AreInSameRowOrColumn(head, tail))
                {
                    // Move one step DIAGONALLY to keep up

                    //var diagonal = Vector2.Clamp(Vector2.Normalize((head - dir) - tail), new Vector2(-0.5f), new Vector2(0.5f)) * 2;

                    var changeX = head.X > tail.X ? 1 : -1;
                    var changeY = head.Y > tail.Y ? 1 : -1;

                    var diagonal = new Vector2(changeX, changeY);

                    tail += diagonal;
                }

                // rs-todo: rem this:
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


            //var dictionary = new Dictionary<char, Vector2>
            //{
            //    {'s', Vector2.Zero},
            //    {'H', head},
            //    {'T', tail}
            //};

            //Console.WriteLine($"Head: {head} -- Tail: {tail}");
            //Console.WriteLine($"{command} {amount}");
            //dictionary.ToStringGrid(x => x.Value, x => x.Key, '.').RenderGridToConsole();
        }

        return tailVisited.Count;
    }

    //static long GridDistance(Vector2 a, Vector2 b)
    //{
    //    return (long)Math.Floor(Math.Abs((a - b).Length()));
    //    //return Math.Abs((a - b).Length()).Round();
    //}

    static bool AreTouching(Vector2 a, Vector2 b)
    {
        // rs-todo: Try using bounds!

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
        // So, move the had, and then use the process to move each Knot in turn, using each previous Knot as the "base"

        var head = new Vector2(0, 0);
        const int numKnots = 10;
        var otherKnots = Enumerable.Range(1, numKnots - 1).Select(_ => new Vector2(0, 0)).ToArray();

        //var tail = new Vector2(0, 0);
        var tailVisited = new HashSet<Vector2>();

        foreach (var (command, dir, amount) in ParseMovements(input))
        {
            for (var move = 0; move < amount; move++)
            {
                // Move the head
                head += dir;

                // Move the other knots
                //var prevHead = head;
                var prevKnot = head;
                for (var i = 0; i < otherKnots.Length; i++)
                {
                    var saved = otherKnots[i];
                    var knot = otherKnots[i];

                    if (Are2StepsAway(prevKnot, knot) &&
                        AreInSameRowOrColumn(prevKnot, knot))
                    {
                        // Move one step in that dir to keep up
                        // tail += Vector2.Normalize(head - tail);
                        knot += Vector2.Normalize(prevKnot - knot);
                    }
                    else if (!AreTouching(prevKnot, knot) &&
                             !AreInSameRowOrColumn(prevKnot, knot))
                    {
                        // Move one step DIAGONALLY to keep up

                        //var diagonal = Vector2.Clamp(Vector2.Normalize(prevHead - knot), new Vector2(-0.5f), new Vector2(0.5f)) * 2;
                        //var diagonal = Vector2.Clamp(Vector2.Normalize(Vector2.Zero - knot), new Vector2(-0.5f), new Vector2(0.5f)) * 2;

                        //var changeX = prevKnot.X - knot.Y;

                        var changeX = prevKnot.X > knot.X ? 1 : -1;
                        var changeY = prevKnot.Y > knot.Y ? 1 : -1;

                        var diagonal = new Vector2(changeX, changeY);

                        knot += diagonal;
                    }

                    // rs-todo: rem this:
                    if (prevKnot.X.Round() != (long)prevKnot.X || prevKnot.Y.Round() != (long)prevKnot.Y)
                    {
                        throw new InvalidOperationException("Head wrong at: " + new {head = prevKnot, command, dir, amount });
                    }

                    if (knot.X.Round() != (long)knot.X || knot.Y.Round() != (long)knot.Y)
                    {
                        throw new InvalidOperationException("Tail wrong at: " + new { tail = knot, command, dir, amount });
                    }

                    otherKnots[i] = knot;
                    prevKnot = knot;

                    //prevHead = saved;
                }

                tailVisited.Add(otherKnots[^1]);
            }

            //var dictionary = new Dictionary<char, Vector2>
            //{
            //    {'s', Vector2.Zero},
            //    {'H', head},
            //    {'T', tail}
            //};

            //Console.WriteLine($"Head: {head} -- Tail: {tail}");
            //Console.WriteLine($"{command} {amount}");
            //dictionary.ToStringGrid(x => x.Value, x => x.Key, '.').RenderGridToConsole();
        }

        return tailVisited.Count;
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
