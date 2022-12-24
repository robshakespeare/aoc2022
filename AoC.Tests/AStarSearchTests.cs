namespace AoC.Tests;

public class AStarSearchTests
{
    record Node(Vector2 Position, int Cost) : IAStarSearchNode;

    private static Node[][] ParseGrid(string gridLevels) =>
        gridLevels.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Node(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray();

    private static AStarSearch<Node> BuildSearch(Node[][] grid, Node goal) =>
        new(getSuccessors: node => grid.GetAdjacent(node.Position, GridUtils.DirectionsExcludingDiagonal),
            getHeuristic: node => MathUtils.ManhattanDistance(node.Position, goal.Position));

    private static void DisplayPathAsGrid(AStarSearch<Node>.Path path)
    {
        var lines = path.Nodes.ToStringGrid(x => x.Position, x => x.Cost.ToString().Single(), '.');

        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    [Test]
    public void FindShortestPath_Test()
    {
        const string gridLevels = @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581";

        var grid = ParseGrid(gridLevels);
        var start = grid[0][0];
        var goal = grid[^1][^1];
        var search = BuildSearch(grid, goal);

        // ACT
        var result = search.FindShortestPath(start, goal);

        // ASSERT
        DisplayPathAsGrid(result);

        using (new AssertionScope())
        {
            result.TotalCost.Should().Be(40);
            result.CurrentNode.Should().Be(grid[^1][^1]);
            result.Nodes.Select(x => x.Cost).Should().BeEquivalentTo(new[]
            {
                1, 1, 2, 1, 3, 6, 5, 1, 1, 1, 5, 1, 1, 3, 2, 3, 2, 1, 1
            }, opts => opts.WithStrictOrdering());
        }
    }

    [Test]
    public void FindShortestPath_CanGoUp()
    {
        const string gridLevels = @"191119999
191919999
191919111
111919141
999911171";

        var grid = ParseGrid(gridLevels);
        var start = grid[0][0];
        var goal = grid[^1][^1];
        var search = BuildSearch(grid, goal);

        // ACT
        var result = search.FindShortestPath(start, goal);

        // ASSERT
        DisplayPathAsGrid(result);

        result.TotalCost.Should().Be(22);
    }

    public class AStarSearchExtraFeaturesTests
    {
        private const string ElevationsGrid = """
            aabqponm
            abcryxxl
            accszzxk
            acctuvwj
            abdefghi
            """;

        private record Node2(Vector2 Position, char Char) : IAStarSearchNode;

        private readonly Node2[][] _grid;
        private readonly AStarSearch<Node2> _search;

        public AStarSearchExtraFeaturesTests()
        {
            _grid = ElevationsGrid.ToGrid((pos, c) => new Node2(pos, c));

            _search = new AStarSearch<Node2>(getSuccessors: node =>
            {
                var currentNode = _grid.Get(node.Position);
                return _grid
                    .GetAdjacent(currentNode.Position, GridUtils.DirectionsExcludingDiagonal)
                    .Where(nextNode => nextNode.Char <= currentNode.Char + 1);
            });
        }

        [Test]
        public void FindShortestPath_CanHaveNoHeuristic()
        {
            // ACT
            var result = _search.FindShortestPath(_grid[0][0], _grid[2][5]);

            // ASSERT
            result.TotalCost.Should().Be(31);
            string.Concat(result.Nodes.Select(x => x.Char)).Should().Be("aabcccdefghijklmnopqrstuvwxxxyzz");
        }

        [Test]
        public void FindShortestPath_CanHaveMultipleStartPoints()
        {
            // ACT
            var result = _search.FindShortestPath(
                _grid.SelectMany(row => row).Where(x => x.Char == 'a'),
                _grid[2][5]);

            // ASSERT
            result.TotalCost.Should().Be(29);
            string.Concat(result.Nodes.Select(x => x.Char)).Should().Be("abccdefghijklmnopqrstuvwxxxyzz");
        }
    }
}
