namespace AoC;

public class AStarSearch
{
    public record Node(Vector2 Position, int Cost);

    public class Path
    {
        private Path(IEnumerable<Node> nodes, Node currentNode, int totalCost)
        {
            Nodes = nodes;
            CurrentNode = currentNode;
            TotalCost = totalCost;
        }

        public IEnumerable<Node> Nodes { get; }
        public int TotalCost { get; }
        public Node CurrentNode { get; }

        public Path Append(Node node) => new(Nodes.Concat(new[] { node }), node, TotalCost + node.Cost);

        public static Path Begin(Node begin) => new(new[] { begin }, begin, 0);
    }

    public delegate long Heuristic(Node child, Node goal);

    private readonly Func<Node, IEnumerable<Node>> _getSuccessors;
    private readonly Heuristic _heuristic;

    public AStarSearch(Func<Node, IEnumerable<Node>> getSuccessors, Heuristic heuristic)
    {
        _getSuccessors = getSuccessors;
        _heuristic = heuristic;
    }

    /// <summary>
    /// Finds the shortest path between the two specified locations in the specified grid.
    /// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// </summary>
    public Path FindShortestPath(Node start, Node goal)
    {
        var explore = new PriorityQueue<Path, long>();
        explore.Enqueue(Path.Begin(start), 0);

        var seen = new HashSet<Node>();

        while (explore.Count > 0)
        {
            var path = explore.Dequeue(); // this takes out the top priority node
            var node = path.CurrentNode;

            // if node is the goal return the path
            if (node == goal)
            {
                return path;
            }

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                foreach (var child in _getSuccessors(node))
                {
                    var childPath = path.Append(child);
                    explore.Enqueue(childPath, childPath.TotalCost + _heuristic(child, goal)); // the heuristic is added here as a part of the priority
                }

                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }
}
