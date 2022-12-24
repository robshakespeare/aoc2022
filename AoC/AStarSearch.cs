namespace AoC;

/// <summary>
/// Represents a node in the A* Search algorithm.
/// IMPORTANT: Must have value equality, so either be a struct, record or implement object equality.
/// </summary>
public interface IAStarSearchNode
{
    int Cost => 1;
}

public class AStarSearch<TNode> where TNode : IAStarSearchNode, IEquatable<TNode>
{
    public class Path
    {
        private Path(IEnumerable<TNode> nodes, TNode currentNode, int totalCost)
        {
            Nodes = nodes;
            CurrentNode = currentNode;
            TotalCost = totalCost;
        }

        public IEnumerable<TNode> Nodes { get; }
        public int TotalCost { get; }
        public TNode CurrentNode { get; }

        public Path Append(TNode node) => new(Nodes.Append(node), node, TotalCost + node.Cost);

        public static Path Begin(TNode begin) => new(new[] {begin}, begin, 0);
    }

    public delegate long Heuristic(TNode child);

    private readonly Func<TNode, IEnumerable<TNode>> _getSuccessors;
    private readonly Heuristic _getHeuristic;

    /// <summary>
    /// Initializes a class to do quick searching using A* Search algorithm.
    /// IMPORTANT: Generic type T must have value equality, so either be a struct, record or implement object equality.
    /// </summary>
    /// <param name="getSuccessors">Delegate to call to get the next possible moves from the specified current node.</param>
    /// <param name="getHeuristic">
    ///     Delegate to call to get the heuristic for reaching the goal from the specified current node;
    ///     or null to use no heuristic (i.e. no heuristic will strictly be a Dijkstra Search).
    ///     The heuristic is the estimated remaining cost to reach the goal from the current node.
    /// </param>
    public AStarSearch(Func<TNode, IEnumerable<TNode>> getSuccessors, Heuristic? getHeuristic = null)
    {
        _getSuccessors = getSuccessors;
        _getHeuristic = getHeuristic ?? (_ => 0);
    }

    /// <summary>
    /// Finds the shortest path between a start point and a goal.
    /// </summary>
    public Path FindShortestPath(TNode start, TNode goal) => FindShortestPath(new[] {start}, goal);

    /// <summary>
    /// Finds the shortest path between any number of start points and a goal.
    /// </summary>
    public Path FindShortestPath(IEnumerable<TNode> starts, TNode goal) => FindShortestPath(starts, node => node.Equals(goal));

    /// <summary>
    /// Finds the shortest path between any number of start points and reaching a goal.
    /// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// </summary>
    public Path FindShortestPath(IEnumerable<TNode> starts, Func<TNode, bool> isGoal)
    {
        var explore = new PriorityQueue<Path, long>(starts.Select(start => (Path.Begin(start), 0L)));
        var seen = new HashSet<TNode>();

        while (explore.Count > 0)
        {
            var path = explore.Dequeue(); // this takes out the top priority node
            var node = path.CurrentNode;

            // if node is the goal return the path
            if (isGoal(node))
            {
                return path;
            }

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                foreach (var child in _getSuccessors(node))
                {
                    var childPath = path.Append(child);
                    explore.Enqueue(childPath, childPath.TotalCost + _getHeuristic(child)); // the heuristic is added here as a part of the priority
                }

                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }
}
