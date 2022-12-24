using static AoC.GridUtils;

namespace AoC.Day24;

public class Day24Solver : ISolver
{
    public string DayName => "Blizzard Basin";

    /// <summary>
    /// What is the fewest number of minutes required to avoid the blizzards and reach the goal?
    /// </summary>
    public long? SolvePart1(PuzzleInput input)
    {
        var initialMap = Map.Parse(input);
        return FindShortestPath(initialMap, initialMap.Start, initialMap.Goal).MinuteNumber;
    }

    /// <summary>
    /// How quickly can you make it from the start to the goal, then back to the start, then back to the goal?
    /// </summary>
    public long? SolvePart2(PuzzleInput input)
    {
        var initialMap = Map.Parse(input);

        var expeditions = new List<Expedition>();
        var map = initialMap;

        for (var expeditionNum = 1; expeditionNum <= 3; expeditionNum++)
        {
            var start = expeditionNum == 2 ? map.Goal : map.Start;
            var goal = expeditionNum == 2 ? map.Start : map.Goal;

            var expedition = FindShortestPath(map, start, goal);
            map = expedition.Map;
            expeditions.Add(expedition);

            Logger($"=== Reached goal {expeditionNum}, this expedition's minutes: {expedition.MinuteNumber} ===");
            if (expeditionNum == 2)
            {
                Logger("");
                Logger("Got snacks! 🍩 🍰 🎂 🍪 🍫 🧁 🍨 🥨 😊");
            }

            Logger("");
        }

        return expeditions.Sum(e => e.MinuteNumber);
    }

    private static readonly IReadOnlyList<Vector2> ExpeditionDirections = new[] { North, West, East, South, Vector2.Zero };

    /// <summary>
    /// A* Search, each move costs 1, heuristic is the remaining cost to reach goal (manhattan distance)
    /// On each minute, you can move up, down, left, or right, or you can wait in place.
    /// You cannot share a position with a blizzard.
    /// </summary>
    private static Expedition FindShortestPath(Map map, Vector2 start, Vector2 goal)
    {
        _lastReported = DateTime.MinValue;

        var search = new AStarSearch<Expedition>(
            getSuccessors: current =>
            {
                if ((DateTime.Now - _lastReported).TotalSeconds > 0.25)
                {
                    var distRemain = MathUtils.ManhattanDistance(current.Position, goal);

                    Logger($"Expedition update: Minute: {current.MinuteNumber}, Position: {current.Position}, Distance remaining: {distRemain}");
                    _lastReported = DateTime.Now;
                }

                var nextMinute = current.MinuteNumber + 1;
                var nextMap = current.Map.Successor();

                var expeditions = ExpeditionDirections
                    .Select(nextDirection => current.Position + nextDirection)
                    .Where(nextPosition => !nextMap.BlizzardPositions.Contains(nextPosition) &&
                                           !nextMap.IsOutOfBounds(nextPosition))
                    .Select(nextPosition => new Expedition(nextPosition, nextMap, nextMinute));
                return expeditions;
            },
            getHeuristic: current => MathUtils.ManhattanDistance(current.Position, goal));

        var findShortestPath = search.FindShortestPath(
            starts: new[] { new Expedition(start, map) },
            isGoal: expedition => expedition.Position == goal);

        return findShortestPath.CurrentNode;
    }

    public static Action<string> Logger { get; set; } = Console.WriteLine;
    private static DateTime _lastReported = DateTime.MinValue;

    public class Expedition : IAStarSearchNode, IEquatable<Expedition>
    {
        public Vector2 Position { get; }
        public int MinuteNumber { get; }
        public Map Map { get; }

        public Expedition(Vector2 position, Map map, int minuteNumber = 0)
        {
            Position = position;
            MinuteNumber = minuteNumber;
            Map = map;
        }

        public override bool Equals(object? obj) => obj is Expedition other && Equals(other);

        public bool Equals(Expedition? other) => other != null && Position == other.Position && Map.Equals(other.Map);

        public override int GetHashCode() => HashCode.Combine(Position, Map);

        public override string ToString() => Map.Render(Position);
    }

    public class Map
    {
        public Vector2 WallMin { get; }
        public Vector2 WallMax { get; }
        public Vector2 Start { get; }
        public Vector2 Goal { get; }
        public IReadOnlySet<Vector2> Walls { get; }
        public IReadOnlyList<Blizzard> Blizzards { get; }
        public IReadOnlySet<Vector2> BlizzardPositions { get; }

        private readonly string _template;
        private readonly int _mapIndex;
        private readonly int _cycleSize;
        private readonly Map?[] _mapCache;
        private readonly Lazy<string> _asString;

        public Map(
            Vector2 wallMin, Vector2 wallMax, Vector2 start, Vector2 goal, IReadOnlySet<Vector2> walls, IReadOnlyList<Blizzard> blizzards, string template,
            int cycleSize, Map?[] mapCache)
        {
            _template = template;
            _mapIndex = 0;
            _cycleSize = cycleSize;
            _mapCache = mapCache;
            WallMin = wallMin;
            WallMax = wallMax;
            Start = start;
            Goal = goal;
            Walls = walls;
            Blizzards = blizzards;
            BlizzardPositions = blizzards.Select(b => b.Position).ToHashSet();
            _asString = new Lazy<string>(() => Render());
        }

        public Map(Map s, IReadOnlyList<Blizzard> blizzards, int mapIndex) : this(s.WallMin, s.WallMax, s.Start, s.Goal, s.Walls, blizzards, s._template,
            s._cycleSize, s._mapCache)
        {
            _mapIndex = mapIndex;
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

            var size = wallMax - wallMin - Vector2.One;
            var cycleSize = (int)MathUtils.LeastCommonMultiple((int)size.X, (int)size.Y);
            var mapCache = new Map?[cycleSize];
            var map = new Map(wallMin, wallMax, start, goal, walls, blizzards, template, cycleSize, mapCache);
            mapCache[0] = map;
            return map;
        }

        public bool IsOutOfBounds(Vector2 position) => Walls.Contains(position) ||
                                                       position.X > WallMax.X || position.X < WallMin.X ||
                                                       position.Y > WallMax.Y || position.Y < WallMin.Y;

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

            var mapIndex = (_mapIndex + 1) % _cycleSize;

            return _mapCache[mapIndex] ?? (_mapCache[mapIndex] = new Map(this, Blizzards.Select(NextBlizzard).ToArray(), mapIndex));
        }

        public string Render(Vector2? expeditionPosition = null)
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

        public override string ToString() => _asString.Value;
    }

    public record Blizzard(Vector2 Position, Vector2 Dir, char Char);
}
