using static AoC.AStarSearch;

namespace AoC.Tests;

public class AStarSearchTests
{
    private static (Node[][] grid, AStarSearch search) Parse(string gridLevels)
    {
        var grid = new PuzzleInput(gridLevels).ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Node(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray();

        var search = new AStarSearch(
            getSuccessors: node => grid.GetAdjacent(GridUtils.DirectionsExcludingDiagonal, node.Position),
            heuristic: (node, goal) => MathUtils.ManhattanDistance(node.Position, goal.Position));

        return (grid, search);
    }

    private static void DisplayPathAsGrid(AStarSearch.Path path)
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

        var (grid, search) = Parse(gridLevels);

        // ACT
        var result = search.FindShortestPath(grid[0][0], grid[^1][^1]);

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

        var (grid, search) = Parse(gridLevels);

        // ACT
        var result = search.FindShortestPath(grid[0][0], grid[^1][^1]);

        // ASSERT
        DisplayPathAsGrid(result);

        result.TotalCost.Should().Be(22);
    }
}
