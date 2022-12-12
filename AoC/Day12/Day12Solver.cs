using static AoC.AStarSearch;

namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hill Climbing Algorithm";

    public long? SolvePart1(PuzzleInput input)
    {
        // rs-todo: need tidy!
        var grid = input.ReadLines().Select((line, y) => line.Select((chr, x) => (Pos: new Vector2(x, y), Chr: chr)).ToArray()).ToArray();

        var cellLookup = grid.SelectMany(x => x).ToArray();

        var startCell = cellLookup.Single(x => x.Chr == 'S');
        var goalCell = cellLookup.Single(x => x.Chr == 'E');

        char Transform(char c) => c switch
        {
            'S' => 'a',
            'E' => 'z',
            _ => c
        };

        var search = new AStarSearch(
            node =>
            {
                var currentLevel = grid[(int) node.Position.Y][(int) node.Position.X];

                var currentLevelChar = Transform(currentLevel.Chr);

                return GridUtils.DirectionsExcludingDiagonal.Select(nextDir =>
                    {
                        var nextPosition = nextDir + currentLevel.Pos;

                        (Vector2 Pos, char Chr) nextLevel;
                        try
                        {
                            nextLevel = grid[(int) nextPosition.Y][(int) nextPosition.X];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            nextLevel = (nextPosition, '-');
                        }

                        return (nextPosition, chr: Transform(nextLevel.Chr));
                    })
                    .Where(x => x.chr != '-' && x.chr <= (currentLevelChar + 1))
                    .Select(x => new Node(x.nextPosition, 1));
            },
            (_, _) => 1);

        return search.FindShortestPath(
            new Node(startCell.Pos, 1),
            new Node(goalCell.Pos, 1)).TotalCost;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    ///// <summary>
    ///// Finds the shortest path between the two specified locations in the specified grid.
    ///// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    ///// </summary>
    //public Path FindShortestPath(Node start, Node goal)
    //{
    //    var explore = new PriorityQueue<Path, long>();
    //    explore.Enqueue(Path.Begin(start), 0);

    //    var seen = new HashSet<Node>();

    //    while (explore.Count > 0)
    //    {
    //        var path = explore.Dequeue(); // this takes out the top priority node
    //        var node = path.CurrentNode;

    //        // if node is the goal return the path
    //        if (node == goal)
    //        {
    //            return path;
    //        }

    //        // if we've not already seen the node
    //        if (!seen.Contains(node))
    //        {
    //            foreach (var child in _getSuccessors(node))
    //            {
    //                var childPath = path.Append(child);
    //                explore.Enqueue(childPath, childPath.TotalCost + _heuristic(child, goal)); // the heuristic is added here as a part of the priority
    //            }

    //            seen.Add(node);
    //        }
    //    }

    //    throw new InvalidOperationException("No paths found");
    //}
}
