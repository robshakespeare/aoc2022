namespace AoC.Day08;

public class Day8Solver : ISolver
{
    public string DayName => "Treetop Tree House";

    public long? SolvePart1(PuzzleInput input) => ParseTrees(input).Count(tree => tree.IsVisible);

    public long? SolvePart2(PuzzleInput input) => ParseTrees(input).Max(tree => tree.ScenicScore);

    public record Tree(Vector2 Position, char Height, bool IsVisible, int ScenicScore);

    static IEnumerable<Tree> ParseTrees(PuzzleInput input)
    {
        var grid = input.ReadLines().ToArray();

        var gridWidth = grid[0].Length;
        var gridHeight = grid.Length;

        return Enumerable.Range(0, gridHeight)
            .SelectMany(y => Enumerable.Range(0, gridWidth).Select(x =>
            {
                var position = new Vector2(x, y);
                var treeUnderConsideration = grid.Get(position);

                var isVisible = false;
                var scenicScore = 1;

                foreach (var direction in new[] {GridUtils.North, GridUtils.West, GridUtils.East, GridUtils.South})
                {
                    var (treesInDirection, edgeReached) = GetVisibleTreesInDirection(grid, position, direction, treeUnderConsideration);
                    isVisible |= edgeReached;
                    scenicScore *= treesInDirection.Count;
                }

                return new Tree(position, treeUnderConsideration, isVisible, scenicScore);
            }));
    }

    static (IReadOnlyList<char> Trees, bool EdgeReached) GetVisibleTreesInDirection(
        string[] grid,
        Vector2 position,
        Vector2 direction,
        char treeUnderConsideration)
    {
        var trees = new List<char>();

        bool edgeReached;
        char currentTree;
        do
        {
            position += direction;
            edgeReached = !grid.TryGet(position, out currentTree);

            if (!edgeReached)
            {
                trees.Add(currentTree);
            }
        } while (!edgeReached && currentTree < treeUnderConsideration);

        return (trees, edgeReached);
    }
}
