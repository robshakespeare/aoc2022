namespace AoC.Day16;

public partial class Day16Solver : ISolver
{
    public string DayName => "Proboscidea Volcanium";

    public Action<string>? Reporter { get; set; }
    DateTime _lastReported = DateTime.Now;

    public long? SolvePart1(PuzzleInput input)
    {
        const int maxSteps = 30;

        var valves = ParseValves(input);
        var costMap = BuildCostFromValveToValve(valves);
        var context = new Context(valves, costMap, maxSteps);

        ExploreLargestTotalPressureReleased(context, valves["AA"], maxSteps);

        return context.LargestTotalPressureReleased;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        const int maxSteps = 26;

        var valves = ParseValves(input);
        var costMap = BuildCostFromValveToValve(valves);
        var context = new Context(valves, costMap, maxSteps);

        ExploreLargestTotalPressureReleased(context, valves["AA"], maxSteps);

        // The elephant must have turned off a totally different set of valves, if it turned off at least one of ours, that's not valid
        // So cross join the paths, and exclude any which overlap or their total is less than the greatest individual total, and then get the largest pair.
        var largestPairedTotalPressureReleased = context.Paths.SelectMany(
            path1 => context.Paths
                .Where(path2 => path1.TotalPressureReleased + path2.TotalPressureReleased > context.LargestTotalPressureReleased &&
                                (path1.Valves & path2.Valves) == 0 /* i.e. they don't overlap */)
                .Select(path2 => path1.TotalPressureReleased + path2.TotalPressureReleased)).Max();

        return largestPairedTotalPressureReleased;
    }

    public record Valve(string Id, long BitId, int FlowRate, IReadOnlyList<string> LeadsTo) : IAStarSearchNode
    {
        public int Cost => 1;
    }

    public record Context(
        IReadOnlyDictionary<string, Valve> Valves,
        Dictionary<(Valve Source, Valve Dest), int> CostMap,
        int MaxSteps)
    {
        public int LargestTotalPressureReleased { get; set; }
        public IReadOnlyList<Valve> ValvesWithFlow { get; } = Valves.Values.Where(v => v.FlowRate > 0).ToArray();
        public List<(long Valves, int TotalPressureReleased)> Paths { get; } = new();
    }

    /// <summary>
    /// Returns a dictionary where the key is the source and destination valve, and the value is the number of steps.
    /// </summary>
    static Dictionary<(Valve Source, Valve Dest), int> BuildCostFromValveToValve(IReadOnlyDictionary<string, Valve> valves)
    {
        var search = new AStarSearch<Valve>(valve => valve.LeadsTo.Select(nextValveId => valves[nextValveId]));
        var result = new Dictionary<(Valve Source, Valve Dest), int>();

        foreach (var sourceValve in valves.Values)
        {
            foreach (var destValve in valves.Values)
            {
                var path = search.FindShortestPath(sourceValve, destValve);
                result.Add((sourceValve, destValve), path.TotalCost);
            }
        }

        return result;
    }

    void ExploreLargestTotalPressureReleased(
        Context context,
        Valve currentValve,
        int remainingSteps,
        long openValvesMip = 0,
        int currentTotal = 0)
    {
        var valvesToClose = context.ValvesWithFlow.Where(valve => (valve.BitId & openValvesMip) == 0 /* i.e. valve not yet open */);

        foreach (var valveToClose in valvesToClose)
        {
            var costToReach = context.CostMap[(currentValve, valveToClose)];
            var costToOpen = costToReach + 1;
            var stepsLeft = remainingSteps - costToOpen;

            if (stepsLeft > 0)
            {
                var newTotal = currentTotal + stepsLeft * valveToClose.FlowRate;
                var stepNumber = context.MaxSteps - stepsLeft;
                var newOpenValvesMip = openValvesMip | valveToClose.BitId;

                if (stepNumber < context.MaxSteps)
                {
                    ExploreLargestTotalPressureReleased(
                        context,
                        valveToClose,
                        stepsLeft,
                        newOpenValvesMip,
                        newTotal);
                }
            }
        }

        context.Paths.Add((openValvesMip, currentTotal));
        context.LargestTotalPressureReleased = Math.Max(context.LargestTotalPressureReleased, currentTotal);

        if (DateTime.Now - _lastReported > TimeSpan.FromSeconds(30))
        {
            var message = $"GreatestTotalPressureReleased: {context.LargestTotalPressureReleased} // {DateTime.Now:O}";
            (Reporter ?? Console.WriteLine).Invoke(message);
            _lastReported = DateTime.Now;
        }
    }

    static IReadOnlyDictionary<string, Valve> ParseValves(string input) =>
        ParseInputRegex.Matches(input).Select((match, i) => new Valve(
            match.Groups["valve"].Value,
            1L << i,
            int.Parse(match.Groups["flowRate"].Value),
            match.Groups["leadsTo"].Value.Split(", "))).ToDictionary(valve => valve.Id);

    private static readonly Regex ParseInputRegex = BuildParseInputRegex();

    [GeneratedRegex(@"Valve (?<valve>[^ ]+) has flow rate=(?<flowRate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<leadsTo>[A-Z, ]+)", RegexOptions.Compiled)]
    private static partial Regex BuildParseInputRegex();
}
