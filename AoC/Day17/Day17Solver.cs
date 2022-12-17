namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Pyroclastic Flow";

    const int ChamberWidth = 7;

    public long? SolvePart1(PuzzleInput input) => SolvePart1(input, numRocks: 2022);

    public long? SolvePart1(PuzzleInput input, int numRocks)
    {
        var chamber = VerticalChamber.BuildAndSimulate(input, numRocks);

        //File.WriteAllText(Path.Combine(
        //    @"C:\Users\Rob.Shakespeare\OneDrive\AoC 2022\Day17",
        //    $"{DateTime.Now:O}.txt".Replace(":", "-")),
        //    chamber.Debug());

        return chamber.Height;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var chamber = VerticalChamber.BuildAndSimulate(input, numRocks: 1000000000000);

        //File.WriteAllText(Path.Combine(
        //    @"C:\Users\Rob.Shakespeare\OneDrive\AoC 2022\Day17",
        //    $"{DateTime.Now:O}.txt".Replace(":", "-")),
        //    chamber.Debug());

        return chamber.Height;
    }

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public class VerticalChamber
    {
        private readonly PuzzleInput _input;
        private readonly long _numRocks;

        public int Width { get; }
        public int LeftWallX { get; }
        public int RightWallX { get; }
        public int FloorY { get; }

        ///// <summary>
        ///// The height of the tower of rocks inside the chamber.
        ///// </summary>
        public long Height => _heightMap.Max;

        private ColumnHeightMap _heightMap = ColumnHeightMap.Create();

        //private readonly List<Shape> _restingRocks = new();
        private readonly SlidingBuffer<Shape> _recentRestingRocks = new(100); //new(28);

        public VerticalChamber(PuzzleInput input, long numRocks, int width = 7)
        {
            _input = input;
            _numRocks = numRocks;
            Width = width;

            // NOTE: Bottom left of the open space in the chamber is 0,0
            LeftWallX = -1;
            RightWallX = width;
            FloorY = 1;
        }

        record ColumnHeightMap(IReadOnlyList<long> Heights)
        {
            public long Max { get; } = Heights.Max();

            public long Min { get; } = Heights.Min();

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

            public static ColumnHeightMap FromPattern(HeightPattern pattern, long maxHeight)
            {
                var relativeHeights = new[] {pattern.Col0, pattern.Col1, pattern.Col2, pattern.Col3, pattern.Col4, pattern.Col5, pattern.Col6};
                var max = relativeHeights.Max();

                // each - max
                return new ColumnHeightMap(relativeHeights.Select(relativeHeight => maxHeight + (relativeHeight - max)).ToArray());
            }
        }

        readonly record struct HeightPattern(int Col0, int Col1, int Col2, int Col3, int Col4, int Col5, int Col6);

        public static VerticalChamber BuildAndSimulate(PuzzleInput input, long numRocks) => new VerticalChamber(input, numRocks).Simulate();

        // How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
        /*
         * After a rock appears, it alternates between being pushed by a jet of hot gas one unit
         * (in the direction indicated by the next symbol in the jet pattern) and then falling one unit down.
         *
         * If any movement would cause any part of the rock to move into the walls, floor, or a stopped rock,
         * the movement instead does not occur. If a downward movement would have caused a falling rock to move
         * into the floor or an already-fallen rock, the falling rock stops where it is (having landed on something) and a new rock immediately begins falling.
         *
         */
        private VerticalChamber Simulate()
        {
            var shapes = BuildShapesCycle();
            var jets = ParseJetFlowsCycle(_input);

            // Where the value is each of the rock number/count when the combination occurred
            var candidatePatterns = new Dictionary<(int jetsIndex, int shapesIndex, HeightPattern HeightPattern), List<(long RockNum, long Height)>>();
            //var candidatePatterns = new Dictionary<HeightPattern, List<long>>();

            var jumped = false;

            for (var rockNumber = 1L; rockNumber <= _numRocks; rockNumber++)
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
                        //_restingRocks.Add(rock);
                        _recentRestingRocks.Add(rock);
                        //Height = Math.Max(Height, Math.Abs((long) rock.Bounds.Min.Y) + 1);

                        //if (Height % 500 == 0)
                        //{
                        //    Logger($"{Height} {rockNumber}");
                        //}

                        _heightMap = _heightMap.Successor(rock);

                        var candidatePattern = candidatePatterns.GetOrAdd(
                            (jets.Index, shapes.Index, _heightMap.GetPatternInRelativeSpace()),
                            () => new List<(long, long)>());

                        //var candidatePattern = candidatePatterns.GetOrAdd(
                        //    _heightMap.GetPatternInRelativeSpace(),
                        //    () => new List<long>());

                        candidatePattern.Add((rockNumber, Height)); // rs-todo: `candidatePatterns` needs better name!

                        var repeats1 = candidatePatterns.Where(x => x.Value.Count > 1).ToArray();

                        if (repeats1.Length > 0 && !jumped)
                        {
                            jumped = true;

                            // rs-todo:
                            var chosenPattern = repeats1[0];

                            var initialHeight = chosenPattern.Value[0].Height;
                            var initialRockCount = chosenPattern.Value[0].RockNum;

                            var remainingRocks = _numRocks - initialRockCount;

                            var patternHeight = chosenPattern.Value[1].Height - chosenPattern.Value[0].Height;

                            var patternRockCount = chosenPattern.Value[1].RockNum - chosenPattern.Value[0].RockNum;

                            var numOfJumps = remainingRocks / patternRockCount;

                            var newRockCount = initialRockCount + (patternRockCount * numOfJumps);
                            var newHeight = initialHeight + (patternHeight * numOfJumps);
                            var heightDelta = newHeight - Height;

                            jets.Index = chosenPattern.Key.jetsIndex;
                            shapes.Index = chosenPattern.Key.shapesIndex;
                            _heightMap = ColumnHeightMap.FromPattern(chosenPattern.Key.HeightPattern, newHeight);
                            rockNumber = newRockCount;

                            // Shift the whole window too!!
                            var delta = new Vector2(0, -heightDelta);
                            _recentRestingRocks.Buffer = _recentRestingRocks.Buffer.Select(s => s?.Translate(delta).Next).ToArray();
                        }

                        //var numRepeats = candidatePatterns.Count(x => x.Value.Count > 1);

                        //if (numRepeats > 0)
                        //{
                        //    //Logger($"#candidatePatterns: {numRepeats}");
                        //}
                    }
                }

                //if (rockNumber <= 10)
                //{
                //    Logger($"After rock {rockNumber} (Height: {Height}):");
                //    Logger(_restingRocks.SelectMany(s => s.Pixels).ToStringGrid(x => x, _ => '#', '.').RenderGridToString());
                //    Logger("");
                //    Logger("");
                //}
                //else
                //{
                //    break;
                //}

                //Logger($"Rock num done: {rockNumber}");
            }

            var repeats = candidatePatterns.Where(x => x.Value.Count > 1).ToArray();

            Logger($"numRepeats: {repeats.Length}");

            foreach (var candidatePattern in repeats)
            {
                Logger($"candidatePattern: {candidatePattern.Key} -- {string.Join(", ", candidatePattern.Value)}");
            }

            //var chosenPattern = repeats.MaxBy(x => x.Value[0]);

            //var chosenPattern = repeats[0]; //.MaxBy(x => x.Value[0]);

            //chosenPattern.

            //Logger($"After rock {numRocks} (Height: {Height}):");
            //Logger(_restingRocks.SelectMany(s => s.Pixels).ToStringGrid(x => x, _ => '#', '.').RenderGridToString());
            //Logger("");
            //Logger("");

            return this;
        }

        bool HasShapeHitWall(Shape shape) => shape.Bounds.Min.X <= LeftWallX || shape.Bounds.Max.X >= RightWallX;

        bool HasShapeHitFloor(Shape shape) => shape.Bounds.Max.Y >= FloorY;

        // rs-todo: use recent!:
        bool CollidedWithRestingRock(Shape rock) => _recentRestingRocks.Buffer.Any(rock.Overlaps); // rs-todo: would a mip make it even quicker?
        //bool CollidedWithRestingRock(Shape rock) => _restingRocks.Any(rock.Overlaps); // rs-todo: would a mip make it even quicker?

        //public string Debug()
        //{
        //    var buffer = new StringBuilder();
        //    buffer.AppendLine($"Num of shapes dropped: {_numRocks} -- Result Chamber height: {Height}):");
        //    buffer.AppendLine(_restingRocks.SelectMany(s => s.Pixels).ToStringGrid(x => x, _ => '#', '.').RenderGridToString());
        //    buffer.AppendLine();
        //    return buffer.ToString();
        //}
    }

    class SlidingBuffer<T>
    {
        private readonly int _maxCount;
        private int _current;

        public T?[] Buffer { get; set; }

        public SlidingBuffer(int maxCount)
        {
            _maxCount = maxCount;
            Buffer = new T[maxCount];
        }

        public void Add(T item)
        {
            Buffer[_current] = item;
            _current = (_current + 1) % _maxCount;
        }
    }

    //class SlidingBuffer<T>
    //{
    //    public readonly Queue<T> Buffer;
    //    private readonly int _maxCount;

    //    public SlidingBuffer(int maxCount)
    //    {
    //        _maxCount = maxCount;
    //        Buffer = new Queue<T>(maxCount);
    //    }

    //    public void Add(T item)
    //    {
    //        if (Buffer.Count == _maxCount)
    //            Buffer.Dequeue();
    //        Buffer.Enqueue(item);
    //    }
    //}

    class Cycle<T>
    {
        readonly T[] _elements;

        public int Index { get; set; } = -1;

        public Cycle(IEnumerable<T> elements) => _elements = elements.ToArray();

        public T Next()
        {
            return _elements[Index = ++Index % _elements.Length];

            //return _elements[++Index % _elements.Length];
        }

        //readonly T[] _elements;
        //public int Index { get; private set; } = -1;

        //public Cycle(IEnumerable<T> elements) => _elements = elements.ToArray();

        //public T Next() => _elements[++Index % _elements.Length];
    }

    record Shape(IReadOnlyList<Vector2> Pixels, BoundingBox Bounds, int PixelMipMap)
    {
        public Shape(IReadOnlyList<Vector2> pixels, Vector2 offset) : this(
            pixels,
            BoundingBox.Create(pixels),
            BuildMipMap(pixels.Select(v => v - offset)))
        {
        }

        static int BuildMipMap(IEnumerable<Vector2> pixels) // rs-todo: doesn't work, either fix or remove!
        {
            // Shift 1 by Y + 5
            // Shift 1 by X
            var result = 0;

            foreach (var pixel in pixels)
            {
                var self = 1 << Math.Abs((int) pixel.X);
                self <<= Math.Abs((int) pixel.Y) * 5;

                result |= self;
                //result |= 1 << Math.Abs((int) pixel.X);
                //result |= 1 << Math.Abs((int) pixel.Y) + 10;
            }

            return result;
        }

        public (Shape Next, Shape Previous) Translate(Vector2 movement) =>
        (
            new Shape(
                Pixels.Select(p => p + movement).ToArray(),
                new BoundingBox(Bounds.Min + movement, Bounds.Max + movement),
                PixelMipMap),
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

            //return (PixelMipMap & other.PixelMipMap) != 0; /* i.e. they overlap */

            return Pixels.Intersect(other.Pixels).Any();

            ////(Bounds.Contains(other.Bounds.Min) || Bounds.Contains(other.Bounds.Max)) &&
            //return (other.Bounds.Contains(Bounds.Min) || other.Bounds.Contains(Bounds.Max)) &&
            //       Pixels.Intersect(other.Pixels).Any();
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
                .Select(p => p.v + offset).ToArray(), offset);
        }).ToArray();

    static Cycle<Shape> BuildShapesCycle() => new(Shapes);

    static Cycle<Vector2> ParseJetFlowsCycle(string input) => new(input.Select(c => c switch
    {
        '<' => new Vector2(-1, 0),
        '>' => new Vector2(1, 0),
        _ => throw new InvalidOperationException("Invalid jet char: " + c)
    }));
}
