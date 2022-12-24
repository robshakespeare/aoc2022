using static AoC.Day24.Day24Solver;
using static AoC.GridUtils;

namespace AoC.Day24;

public class Day24Solver : ISolver
{
    public string DayName => "Blizzard Basin";

    private DateTime _lastReported = DateTime.MinValue;

    public long? SolvePart1(PuzzleInput input)
    {
        // A* Search, each move costs 1, heuristic is the remaining cost to reach goal (manhattan distance)
        // On each minute, you can move up, down, left, or right, or you can wait in place.
        // You cannot share a position with a blizzard.

        var nextDirections = new[] { North, West, East, South, Vector2.Zero };
        var initialMap = Map.Parse(input);
        var goal = initialMap.Goal;
        var search = new AStarSearch<Expedition>(
            getSuccessors: current =>
            {
                if ((DateTime.Now - _lastReported).TotalSeconds > 10)
                {
                    var distRemain = MathUtils.ManhattanDistance(current.Position, goal);

                    Console.WriteLine($"Expedition update: MinuteNumber: {current.MinuteNumber}, Pos: {current.Position}, distRemain: {distRemain}");
                    _lastReported = DateTime.Now;
                }

                var nextMinute = current.MinuteNumber + 1;
                var nextMap = current.Map.Successor();

                var nextBlizzards = nextMap.Blizzards.Select(b => b.Position).ToHashSet();

                var expeditions = nextDirections
                    .Select(nextDirection => current.Position + nextDirection)
                    .Where(nextPosition => /*nextPosition != initialMap.Start &&*/
                                           !nextBlizzards.Contains(nextPosition) &&
                                           !initialMap.IsOutOfBounds(nextPosition))
                    .Select(nextPosition => new Expedition(nextPosition, nextMap, nextMinute));
                return expeditions;
            },
            getHeuristic: current => MathUtils.ManhattanDistance(current.Position, goal));

        var shortestPath = search.FindShortestPath(
            starts: new[] { new Expedition(initialMap.Start, initialMap, 0) },
            isGoal: expedition => expedition.Position == goal);

        var path = shortestPath.Nodes.ToArray();

        Console.WriteLine(path[^2].ToString());
        Console.WriteLine();

        Console.WriteLine(shortestPath.CurrentNode.ToString());
        Console.WriteLine();

        return shortestPath.TotalCost;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Expedition : IAStarSearchNode, IEquatable<Expedition>
    {
        public Vector2 Position { get; }
        public int MinuteNumber { get; }
        public Map Map => _map ?? throw new InvalidOperationException("Goal map is unknown");

        private readonly Map? _map;

        public Expedition(Vector2 position, Map map, int minuteNumber)
        {
            Position = position;
            MinuteNumber = minuteNumber;
            _map = map;
        }

        //public override bool Equals(object? obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((Expedition)obj);
        //}

        //public bool Equals(Expedition? other)
        //{
        //    if (ReferenceEquals(null, other)) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return Position.Equals(other.Position) && MinuteNumber == other.MinuteNumber;
        //}

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(Position, MinuteNumber);
        //}

        public override bool Equals(object? obj) => obj is Expedition other && Equals(other);

        public bool Equals(Expedition? other) => other != null &&
                                                 Position == other.Position &&
                                                 //MinuteNumber == other.MinuteNumber &&
                                                 Map.Equals(other.Map);

        public override int GetHashCode() => HashCode.Combine(Position, Map);

        public int Cost => 1;

        public override string ToString() => Map.Render(Position);
    }

    public class Map//record Map(
        //Vector2 WallMin, Vector2 WallMax, Vector2 Start, Vector2 Goal, IReadOnlySet<Vector2> Walls, IReadOnlyList<Blizzard> Blizzards)
    {
        public Vector2 WallMin { get; }
        public Vector2 WallMax { get; }
        public Vector2 Start { get; }
        public Vector2 Goal { get; }
        public IReadOnlySet<Vector2> Walls { get; }
        public IReadOnlyList<Blizzard> Blizzards { get; }

        private readonly string _template;

        private readonly Lazy<string> _asString;

        public Map(
            Vector2 wallMin, Vector2 wallMax, Vector2 start, Vector2 goal, IReadOnlySet<Vector2> walls, IReadOnlyList<Blizzard> blizzards, string template)
        {
            _template = template;
            WallMin = wallMin;
            WallMax = wallMax;
            Start = start;
            Goal = goal;
            Walls = walls;
            Blizzards = blizzards;
            _asString = new Lazy<string>(() => Render(null));
        }

        public Map(Map s, IReadOnlyList<Blizzard> blizzards) : this(s.WallMin, s.WallMax, s.Start, s.Goal, s.Walls, s.Blizzards, s._template)
        {
            Blizzards = blizzards;
        }

        public bool Equals(Map? other) => other != null && _asString.Value == other._asString.Value;

        public override bool Equals(object? obj) => obj is Map other && Equals(other);

        public override int GetHashCode() => _asString.Value.GetHashCode();

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

            var template = walls.Select(p => (pos: p, chr: '#'))
                .Append((pos: start, chr: 'S'))
                .Append((pos: goal, chr: 'G'))
                .ToStringGrid(c => c.pos, c => c.chr, '.').RenderGridToString();

            return new Map(wallMin, wallMax, start, goal, walls, blizzards, template);
        }

        public bool IsOutOfBounds(Vector2 position) => Walls.Contains(position) ||
                                                       position.X > WallMax.X ||
                                                       position.X < WallMin.X ||
                                                       position.Y > WallMax.Y ||
                                                       position.Y < WallMin.Y;

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

            return new Map(this, Blizzards.Select(NextBlizzard).ToArray());
        }

        public string Render(Vector2? expeditionPosition)
        {
            var grid = _template.ReadLines().Select(line => line.ToCharArray()).ToArray();

            foreach (var blizzard in Blizzards)
            {
                grid[(int)blizzard.Position.Y][(int)blizzard.Position.X] = blizzard.Char;
            }

            if (expeditionPosition != null)
            {
                grid[(int)expeditionPosition.Value.Y][(int)expeditionPosition.Value.X] = 'E';
            }

            return string.Join(Environment.NewLine, grid.Select(line => string.Concat(line)));
        }

        //public string ToString(Vector2? expeditionPosition) =>
        //    Walls.Select(p => (pos: p, chr: '#'))
        //        .Concat(Blizzards.Select(b => (pos: b.Position, chr: b.Char)))
        //        .Append((pos: expeditionPosition ?? Start, chr: expeditionPosition != null ? 'E' : '.'))
        //        .ToStringGrid(c => c.pos, c => c.chr, '.')
        //        .RenderGridToString();

        public override string ToString() => _asString.Value;
    }

    public record Blizzard(Vector2 Position, Vector2 Dir, char Char);
}
