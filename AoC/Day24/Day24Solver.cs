using static AoC.GridUtils;

namespace AoC.Day24;

public class Day24Solver : ISolver
{
    public string DayName => "Blizzard Basin";

    public long? SolvePart1(PuzzleInput input)
    {
        // A* Search, each move costs 1, heuristic is the remaining cost to reach goal (manhattan distance)
        // On each minute, you can move up, down, left, or right, or you can wait in place.
        // You cannot share a position with a blizzard.

        var nextDirections = new[] { North, West, East, South, Vector2.Zero };
        var initialMap = Map.Parse(input);
        var search = new AStarSearch<Expedition>(
            getSuccessors: current =>
            {
                var nextMap = current.Map.Successor();

                var nextBlizzards = nextMap.Blizzards.Select(b => b.Position).ToHashSet();

                return nextDirections
                    .Select(nextDirection => current.Position + nextDirection)
                    .Where(nextPosition => nextPosition != initialMap.Start &&
                                           !nextBlizzards.Contains(nextPosition) &&
                                           !initialMap.IsWall(nextPosition))
                    .Select(nextPosition => new Expedition(nextPosition, nextMap));
            },
            getHeuristic: (current, goal) => MathUtils.ManhattanDistance(current.Position, goal.Position));

        return search.FindShortestPath(
            start: new Expedition(initialMap.Start, initialMap),
            goal: Expedition.Goal(initialMap.Goal)
        ).TotalCost;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Expedition : IAStarSearchNode, IEquatable<Expedition>
    {
        public Vector2 Position { get; }
        public Map Map => _map ?? throw new InvalidOperationException("Goal map is unknown");

        private readonly Map? _map;

        public Expedition(Vector2 position, Map map)
        {
            Position = position;
            _map = map;
        }

        private Expedition(Vector2 position)
        {
            Position = position;
        }

        public static Expedition Goal(Vector2 position) => new(position);

        public override bool Equals(object? obj) => obj is Expedition other && Equals(other);

        public bool Equals(Expedition? other) => other != null && other.Position == Position;

        public override int GetHashCode() => Position.GetHashCode();

        public int Cost => 1;
    }

    public record Map(
        Vector2 WallMin, Vector2 WallMax, Vector2 Start, Vector2 Goal, IReadOnlySet<Vector2> Walls, IReadOnlyList<Blizzard> Blizzards)
    {
        //public Vector2 WallMin { get; }
        //public Vector2 WallMax { get; }
        //public Vector2 Start { get; }
        //public Vector2 Goal { get; }
        //public IReadOnlySet<Vector2> Walls { get; }
        //public IReadOnlyList<Blizzard> Blizzards { get; }

        //public Map(
        //    Vector2 wallMin, Vector2 wallMax, Vector2 start, Vector2 goal, IReadOnlySet<Vector2> walls, IReadOnlyList<Blizzard> blizzards)
        //{
        //    WallMin = wallMin;
        //    WallMax = wallMax;
        //    Start = start;
        //    Goal = goal;
        //    Walls = walls;
        //    Blizzards = blizzards;
        //}

        public static Map Parse(string input)
        {
            var inputGrid = input.ReadLines().Select(
                    (line, y) => line.Select((chr, x) => (chr, x))
                        .Select(p => (pos: new Vector2(p.x, y), p.chr))
                        .ToArray())
                .ToArray();
            var inputCells = inputGrid.SelectMany(line => line).ToArray();

            var wallMin = new Vector2(0, 0);
            var wallMax = new Vector2(inputCells.Max(c => c.pos.X), inputCells.Max(c => c.pos.Y));

            var start = inputGrid[0].Single(c => c.chr == '.').pos;
            var goal = inputGrid[^1].Single(c => c.chr == '.').pos;

            var walls = inputCells.Where(c => c.chr == '#').Select(c => c.pos).ToHashSet();

            // up (^), down (v), left (<), or right (>)
            var blizzards = inputCells.Where(c => c.chr is not ('.' or '#')).Select(
                b => new Blizzard(b.pos, b.chr switch
                {
                    '^' => North,
                    'v' => South,
                    '<' => West,
                    '>' => East,
                    _ => throw new InvalidOperationException("Invalid blizzard char: " + b.chr)
                }, b.chr)).ToArray();

            return new Map(wallMin, wallMax, start, goal, walls, blizzards);
        }

        public bool IsWall(Vector2 position) => Walls.Contains(position);

        /// <summary>
        /// In one minute, each blizzard moves one position in the direction it is pointing.
        /// Due to conservation of blizzard energy, as a blizzard reaches the wall of the valley,
        /// a new blizzard forms on the opposite side of the valley moving in the same direction.
        /// Because blizzards are made of tiny snowflakes, they pass right through each other. i.e. can occupy same space.
        /// </summary>
        public Map Successor()
        {
            Blizzard NextBlizzard(Blizzard prevBlizzard)
            {
                var nextPosition = prevBlizzard.Position + prevBlizzard.Dir;

                if (nextPosition.X >= WallMax.X)
                {
                    nextPosition.X = WallMin.X + 1;
                }
                else if (nextPosition.X <= WallMin.X)
                {
                    nextPosition.X = WallMax.X - 1;
                }
                else if (nextPosition.Y >= WallMax.Y)
                {
                    nextPosition.Y = WallMin.Y + 1;
                }
                else if (nextPosition.Y <= WallMin.Y)
                {
                    nextPosition.Y = WallMax.Y - 1;
                }

                return prevBlizzard with { Position = nextPosition };
            }

            return this with
            {
                Blizzards = Blizzards.Select(NextBlizzard).ToArray()
            };
        }

        public override string ToString() =>
            Walls.Select(p => (pos: p, chr: '#'))
                .Concat(Blizzards.Select(b => (pos: b.Position, chr: b.Char)))
                .ToStringGrid(c => c.pos, c => c.chr, '.')
                .RenderGridToString();
    }

    public record Blizzard(Vector2 Position, Vector2 Dir, char Char);
}
