namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Pyroclastic Flow";

    const int ChamberWidth = 7;

    public long? SolvePart1(PuzzleInput input) => VerticalChamber.BuildAndSimulate(input, targetNumRocks: 2022);

    public long? SolvePart2(PuzzleInput input) => VerticalChamber.BuildAndSimulate(input, targetNumRocks: 1000000000000);

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public class VerticalChamber
    {
        private readonly PuzzleInput _input;
        private readonly long _targetNumRocks;

        private readonly SlidingBuffer<Shape> _recentRestingRocks = new(28);

        private ColumnHeightMap _heightMap = ColumnHeightMap.Create();

        /// <summary>
        /// The height of the tower of rocks inside the chamber.
        /// </summary>
        long Height => _heightMap.Max;

        int LeftWallX { get; }
        int RightWallX { get; }
        int FloorY { get; }

        private VerticalChamber(PuzzleInput input, long targetNumRocks)
        {
            _input = input;
            _targetNumRocks = targetNumRocks;

            // NOTE: Bottom left of the open space in the chamber is 0,0
            LeftWallX = -1;
            RightWallX = ChamberWidth;
            FloorY = 1;
        }

        public static long BuildAndSimulate(PuzzleInput input, long targetNumRocks) => new VerticalChamber(input, targetNumRocks).Simulate();

        /*
         * After a rock appears, it alternates between being pushed by a jet of hot gas one unit
         * (in the direction indicated by the next symbol in the jet pattern) and then falling one unit down.
         *
         * If any movement would cause any part of the rock to move into the walls, floor, or a stopped rock,
         * the movement instead does not occur. If a downward movement would have caused a falling rock to move
         * into the floor or an already-fallen rock, the falling rock stops where it is (having landed on something) and a new rock immediately begins falling.
         *
         * How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
         */
        private long Simulate()
        {
            var shapes = BuildShapesCycle();
            var jets = ParseJetFlowsCycle(_input);

            // Where the value is each of the rock count and current chamber height when the combination occurred
            var candidatePatterns = new Dictionary<(int jetsIndex, int shapesIndex, HeightPattern HeightPattern), List<(long RockCount, long Height)>>();

            const bool doJump = true;
            const bool doLog = false;

            for (var rockCount = 1L; rockCount <= _targetNumRocks; rockCount++)
            {
                // Move rock until it comes to a rest
                var (rock, _) = shapes.Next().Translate(new Vector2(0, -Height - 3));
                var comeToRest = false;

                while (!comeToRest)
                {
                    var jet = jets.Next();
                    (rock, var previous) = rock.Translate(jet);
                    if (HasShapeHitWall(rock) || CollidedWithRestingRock(rock))
                    {
                        rock = previous;
                    }

                    (rock, previous) = rock.Translate(GridUtils.South);
                    if (HasShapeHitFloor(rock) || CollidedWithRestingRock(rock))
                    {
                        rock = previous;
                        comeToRest = true;

                        _recentRestingRocks.Add(rock);
                        _heightMap = _heightMap.Successor(rock);

                        var candidatePattern = candidatePatterns.GetOrAdd(
                            (jets.Index, shapes.Index, _heightMap.GetPatternInRelativeSpace()),
                            () => new List<(long, long)>());

                        candidatePattern.Add((rockCount, Height));

                        var repeatingPatterns = candidatePatterns.Where(x => x.Value.Count > 1).ToArray();

                        if (repeatingPatterns.Length > 0 && doJump)
                        {
                            foreach (var repeatingPattern in repeatingPatterns)
                            {
                                var initialHeight = repeatingPattern.Value[0].Height;
                                var initialRockCount = repeatingPattern.Value[0].RockCount;

                                var remainingRocks = _targetNumRocks - initialRockCount;

                                var patternHeight = repeatingPattern.Value[1].Height - repeatingPattern.Value[0].Height;
                                var patternRockCount = repeatingPattern.Value[1].RockCount - repeatingPattern.Value[0].RockCount;

                                var numOfJumps = remainingRocks / patternRockCount;

                                var newRockCount = initialRockCount + (patternRockCount * numOfJumps);
                                var newHeight = initialHeight + (patternHeight * numOfJumps);

                                if (newRockCount == _targetNumRocks)
                                {
                                    return newHeight;
                                }
                            }
                        }
                    }
                }
            }

            if (doLog)
            {
                var repeatingPatterns = candidatePatterns.Where(x => x.Value.Count > 1).ToArray();

                Logger($"numRepeats: {repeatingPatterns.Length}");

                foreach (var pattern in repeatingPatterns)
                {
                    var prevHeight = 0L;
                    var gaps = pattern.Value.Select(x =>
                    {
                        var gap = x.Height - prevHeight;
                        prevHeight = x.Height;
                        return gap;
                    }).ToArray();

                    Logger($"candidatePattern: {pattern.Key} -- {string.Join(", ", pattern.Value)} -- gaps: {string.Join(", ", gaps)}");
                }
            }

            return Height;
        }

        bool HasShapeHitWall(Shape shape) => shape.Bounds.Min.X <= LeftWallX || shape.Bounds.Max.X >= RightWallX;

        bool HasShapeHitFloor(Shape shape) => shape.Bounds.Max.Y >= FloorY;

        bool CollidedWithRestingRock(Shape rock) => _recentRestingRocks.Buffer.Any(rock.Overlaps);
    }

    record ColumnHeightMap(IReadOnlyList<long> Heights)
    {
        public long Max { get; } = Heights.Max();

        long Min { get; } = Heights.Min();

        public static ColumnHeightMap Create() => new(Enumerable.Repeat(0L, ChamberWidth).ToArray());

        public ColumnHeightMap Successor(Shape shape)
        {
            var newHeights = Heights.ToArray();

            foreach (var pixel in shape.Pixels)
            {
                var x = (int) pixel.X;
                var y = Math.Abs((long) pixel.Y) + 1;

                newHeights[x] = Math.Max(y, newHeights[x]);
            }

            return new ColumnHeightMap(newHeights);
        }

        public HeightPattern GetPatternInRelativeSpace() => new(
            (int) (Heights[0] - Min),
            (int) (Heights[1] - Min),
            (int) (Heights[2] - Min),
            (int) (Heights[3] - Min),
            (int) (Heights[4] - Min),
            (int) (Heights[5] - Min),
            (int) (Heights[6] - Min)
        );
    }

    public readonly record struct HeightPattern(int Col0, int Col1, int Col2, int Col3, int Col4, int Col5, int Col6);

    class SlidingBuffer<T>
    {
        readonly int _maxCount;
        int _current;

        public T?[] Buffer { get; }

        public SlidingBuffer(int maxCount)
        {
            _maxCount = maxCount;
            Buffer = new T[maxCount];
        }

        public void Add(T item) => Buffer[_current = ++_current % _maxCount] = item;
    }

    class Cycle<T>
    {
        readonly T[] _elements;

        public int Index { get; private set; } = -1;

        public Cycle(IEnumerable<T> elements) => _elements = elements.ToArray();

        public T Next() => _elements[Index = ++Index % _elements.Length];
    }

    record Shape(IReadOnlyList<Vector2> Pixels, BoundingBox Bounds)
    {
        public Shape(IReadOnlyList<Vector2> pixels) : this(
            pixels,
            BoundingBox.Create(pixels))
        {
        }

        public (Shape Next, Shape Previous) Translate(Vector2 movement) =>
        (
            new Shape(
                Pixels.Select(p => p + movement).ToArray(),
                new BoundingBox(Bounds.Min + movement, Bounds.Max + movement)),
            this
        );

        public bool Overlaps(Shape? other)
        {
            if (other == null ||
                other.Bounds.Min.Y > Bounds.Max.Y ||
                other.Bounds.Min.X > Bounds.Max.X ||
                other.Bounds.Max.Y < Bounds.Min.Y ||
                other.Bounds.Max.X < Bounds.Min.X)
            {
                return false;
            }

            return Pixels.Intersect(other.Pixels).Any();
        }
    }

    /// <summary>
    /// Note that the bounds (min and max) are inclusive.
    /// </summary>
    record BoundingBox(Vector2 Min, Vector2 Max)
    {
        public static BoundingBox Create(IReadOnlyList<Vector2> pixels)
        {
            var minBounds = new Vector2(pixels.Min(i => i.X), pixels.Min(i => i.Y));
            var maxBounds = new Vector2(pixels.Max(i => i.X), pixels.Max(i => i.Y));
            return new BoundingBox(minBounds, maxBounds);
        }
    }

    static readonly IReadOnlyList<Shape> Shapes = """
         ####
        
         .#.
         ###
         .#.
        
         ..#
         ..#
         ###
        
         #
         #
         #
         #
        
         ##
         ##
         """
        .ReplaceLineEndings("\n").Split("\n\n")
        .Select(chunk =>
        {
            var offset = new Vector2(2, -chunk.Count(c => c == '\n'));
            return new Shape(chunk.ToGrid((v, c) => (v, c))
                .SelectMany(line => line)
                .Where(p => p.c == '#')
                .Select(p => p.v + offset).ToArray());
        }).ToArray();

    static Cycle<Shape> BuildShapesCycle() => new(Shapes);

    static Cycle<Vector2> ParseJetFlowsCycle(string input) => new(input.Select(c => c switch
    {
        '<' => new Vector2(-1, 0),
        '>' => new Vector2(1, 0),
        _ => throw new InvalidOperationException("Invalid jet char: " + c)
    }));
}
