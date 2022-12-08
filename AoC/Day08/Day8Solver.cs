namespace AoC.Day08;
using MoreLinq;

public class Day8Solver : ISolver
{
    public string DayName => "Treetop Tree House";

    public long? SolvePart1(PuzzleInput input) => ParseTrees(input).Count(tree => tree.IsVisible);

    public long? SolvePart2(PuzzleInput input) => ParseTrees(input).Max(tree => tree.ScenicScore);

    public record Tree(Vector2 Position, char Height, bool IsVisible, int ScenicScore);

    static IReadOnlyList<Tree> ParseTrees(PuzzleInput input)
    {
        var grid = input.ReadLines().ToArray();

        var gridWidth = grid[0].Length;
        var gridHeight = grid.Length;

        var trees = new List<Tree>();

        for (var y = 0; y < gridHeight; y++)
        {
            for (var x = 0; x < gridWidth; x++)
            {
                var treeHeight = grid[y][x];
                var position = new Vector2(x, y);

                bool IsShorter(char otherTreeHeight) => otherTreeHeight < treeHeight;
                bool IsTallerOrEqual(char otherTreeHeight) => otherTreeHeight >= treeHeight;

                var isVisible = false;
                var scenicScore = 1;

                foreach (var direction in GridUtils.DirectionsExcludingDiagonal)
                {
                    var dirTrees = GetTreesInDirection(grid, new Vector2(x, y), direction);

                    isVisible |= dirTrees.TakeWhile(IsShorter).Count() == dirTrees.Count;
                    scenicScore *= dirTrees.TakeUntil(IsTallerOrEqual).Count();
                }

                trees.Add(new Tree(position, treeHeight, isVisible, scenicScore));
            }
        }

        return trees;
    }

    static IReadOnlyList<char> GetTreesInDirection(string[] grid, Vector2 position, Vector2 direction)
    {
        var trees = new List<char>();
        char? tree;
        while ((tree = grid.SafeGet(position += direction)) != null)
        {
            trees.Add(tree.Value);
        }

        return trees;
    }
}
