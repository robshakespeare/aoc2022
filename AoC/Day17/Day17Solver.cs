using System.Collections;

namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Pyroclastic Flow";

    public long? SolvePart1(PuzzleInput input)
    {
        //var shapes = BuildShapesCycle();
        //var jets = ParseJetFlowsCycle(input);

        const int numRocks = 2022;

        var chamber = new VerticalChamber();

        chamber.Simulate(input, numRocks);


        return chamber.Height;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public class VerticalChamber
    {
        public int Width { get; }
        public int LeftWallX { get; }
        public int RightWallX { get; }
        public int FloorY { get; }

        /// <summary>
        /// The height of the tower of rocks inside the chamber.
        /// </summary>
        public int Height { get; private set; }

        private readonly List<Shape> _restingRocks = new();
        private readonly SlidingBuffer<Shape> _recentRestingRocks = new(28);

        public VerticalChamber(int width = 7)
        {
            Width = width;

            // NOTE: Bottom left of the open space in the chamber is 0,0
            LeftWallX = -1;
            RightWallX = width;
            FloorY = 1;
        }

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
        public void Simulate(PuzzleInput input, int numRocks)
        {
            var shapes = BuildShapesCycle();
            var jets = ParseJetFlowsCycle(input);

            for (var rockNumber = 1; rockNumber <= numRocks; rockNumber++)
            {
                // Move rock until it comes to a rest
                // ReSharper disable once RedundantAssignment
                var (rock, previous) = shapes.Next().Translate(new Vector2(0, -Height - 3));
                var comeToRest = false;

                while (!comeToRest)
                {
                    var jet = jets.Next();
                    (rock, previous) = rock.Translate(jet);
                    if (HasShapeHitWall(rock) || CollidedWithRestingRock(rock))
                    {
                        rock = previous;
                    }

                    (rock, previous) = rock.Translate(GridUtils.South);
                    if (HasShapeHitFloor(rock) || CollidedWithRestingRock(rock))
                    {
                        rock = previous;
                        comeToRest = true;
                        _restingRocks.Add(rock);
                        _recentRestingRocks.Add(rock);
                        Height = Math.Max(Height, Math.Abs((int) rock.Bounds.Min.Y) + 1);
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

            //Logger($"After rock {numRocks} (Height: {Height}):");
            //Logger(_restingRocks.SelectMany(s => s.Pixels).ToStringGrid(x => x, _ => '#', '.').RenderGridToString());
            //Logger("");
            //Logger("");
        }

        bool HasShapeHitWall(Shape shape) => shape.Bounds.Min.X <= LeftWallX || shape.Bounds.Max.X >= RightWallX;

        bool HasShapeHitFloor(Shape shape) => shape.Bounds.Max.Y >= FloorY;

        bool CollidedWithRestingRock(Shape rock) => _recentRestingRocks.Any(rock.Overlaps); // rs-todo: needs optimising!!

        public string Debug()
        {
            var buffer = new StringBuilder();
            buffer.AppendLine($"Chamber height: {Height}):");
            buffer.AppendLine(_restingRocks.SelectMany(s => s.Pixels).ToStringGrid(x => x, _ => '#', '.').RenderGridToString());
            buffer.AppendLine();
            return buffer.ToString();
        }
    }

    class SlidingBuffer<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _maxCount;

        public SlidingBuffer(int maxCount)
        {
            _maxCount = maxCount;
            _queue = new Queue<T>(maxCount);
        }

        public void Add(T item)
        {
            if (_queue.Count == _maxCount) _queue.Dequeue();
            _queue.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Cycle<T>
    {
        readonly T[] _elements;
        int _index = -1;

        public Cycle(IEnumerable<T> elements) => _elements = elements.ToArray();

        public T Next() => _elements[++_index % _elements.Length];
    }

    record Shape(IReadOnlyList<Vector2> Pixels, BoundingBox Bounds/*, int PixelMip*/)
    {
        public Shape(IReadOnlyList<Vector2> pixels) : this(pixels, BoundingBox.Create(pixels))
        {
        }

        public (Shape Next, Shape Previous) Translate(Vector2 movement) =>
            (new Shape(
                Pixels.Select(p => p + movement).ToArray(),
                new BoundingBox(Bounds.Min + movement, Bounds.Max + movement)), this);

        public bool Overlaps(Shape other)
        {
            if (other.Bounds.Min.Y > Bounds.Max.Y ||
                other.Bounds.Min.X > Bounds.Max.X ||
                other.Bounds.Max.Y < Bounds.Min.Y ||
                other.Bounds.Max.X < Bounds.Min.X)
            {
                return false;
            }

            return Pixels.Intersect(other.Pixels).Any(); // rs-todo: optimize?

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
        public bool Contains(Vector2 position) =>
            position.X >= Min.X && position.X <= Max.X &&
            position.Y >= Min.Y && position.Y <= Max.Y;

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
