namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hill Climbing Algorithm";

    public long? SolvePart1(PuzzleInput input)
    {
        var (heightmap, nodes, end) = ParseHeightmap(input);
        var start = nodes.Single(node => node.Char == 'S');
        return FindShortestPathToEnd(heightmap, new[] {start}, end);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var (heightmap, nodes, end) = ParseHeightmap(input);
        var starts = nodes.Where(node => node.Char == 'a');
        return FindShortestPathToEnd(heightmap, starts, end);
    }

    static (Node[][] Heightmap, Node[] Nodes, Node End) ParseHeightmap(PuzzleInput input)
    {
        static char TransformElevation(char c) => c switch
        {
            'S' => 'a',
            'E' => 'z',
            _ => c
        };

        var heightmap = input.ReadLines().Select(
            (line, y) => line.Select((chr, x) => new Node(new Vector2(x, y), TransformElevation(chr), chr)).ToArray()).ToArray();
        var nodes = heightmap.SelectMany(line => line).ToArray();
        var end = nodes.Single(node => node.Char == 'E');
        return (heightmap, nodes, end);
    }

    record Node(Vector2 Position, char Elevation, char Char) : IAStarSearchNode
    {
        public int Cost => 1;
    }

    static long FindShortestPathToEnd(Node[][] heightmap, IEnumerable<Node> starts, Node end)
    {
        var search = new AStarSearch<Node>(node =>
        {
            var currentNode = heightmap[(int) node.Position.Y][(int) node.Position.X];
            return heightmap
                .GetAdjacent(currentNode.Position, GridUtils.DirectionsExcludingDiagonal)
                .Where(nextNode => nextNode.Elevation <= currentNode.Elevation + 1);
        });

        return search.FindShortestPath(starts, end).TotalCost;
    }
}
