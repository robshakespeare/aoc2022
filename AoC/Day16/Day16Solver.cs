namespace AoC.Day16;

public partial class Day16Solver : ISolver
{
    public string DayName => "Proboscidea Volcanium";

    public long? SolvePart1(PuzzleInput input) => Explorer.BuildAndExplore(input, maxSteps: 30).LargestTotalPressureReleased;

    public long? SolvePart2(PuzzleInput input)
    {
        var explorer = Explorer.BuildAndExplore(input, maxSteps: 26);

        // The elephant must have turned off a totally different set of valves, if it turned off at least one of ours, that's not valid
        // So cross join the paths, and exclude any which overlap or their total is less than the greatest individual total, and then get the largest pair.
        var largestPairedTotalPressureReleased = explorer.Paths.SelectMany(
            path1 => explorer.Paths
                .Where(path2 => path1.TotalPressureReleased + path2.TotalPressureReleased > explorer.LargestTotalPressureReleased &&
                                (path1.Valves & path2.Valves) == 0 /* i.e. they don't overlap */)
                .Select(path2 => path1.TotalPressureReleased + path2.TotalPressureReleased)).Max();

        return largestPairedTotalPressureReleased;
    }

    public Action<string> Logger { get; set; } = Console.WriteLine;

    record Valve(string Id, long BitId, int FlowRate, IReadOnlyList<string> LeadsTo) : IAStarSearchNode;

    class Explorer
    {
        Explorer(
            IReadOnlyList<Valve> valvesWithFlow,
            Dictionary<(Valve Source, Valve Dest), int> costMap,
            int maxSteps)
        {
            ValvesWithFlow = valvesWithFlow;
            CostMap = costMap;
            MaxSteps = maxSteps;
        }

        public int LargestTotalPressureReleased { get; private set; }
        public List<(long Valves, int TotalPressureReleased)> Paths { get; } = new();

        IReadOnlyList<Valve> ValvesWithFlow { get; }
        Dictionary<(Valve Source, Valve Dest), int> CostMap { get; }
        int MaxSteps { get; }

        void ExploreLargestTotalPressureReleased(
            Valve currentValve,
            int remainingSteps,
            long openValvesMip = 0,
            int currentTotal = 0)
        {
            var valvesToClose = ValvesWithFlow.Where(valve => (valve.BitId & openValvesMip) == 0 /* i.e. valve not yet open */);

            foreach (var valveToClose in valvesToClose)
            {
                var costToReach = CostMap[(currentValve, valveToClose)];
                var costToOpen = costToReach + 1;
                var stepsLeft = remainingSteps - costToOpen;

                if (stepsLeft > 0)
                {
                    var newTotal = currentTotal + stepsLeft * valveToClose.FlowRate;
                    var stepNumber = MaxSteps - stepsLeft;
                    var newOpenValvesMip = openValvesMip | valveToClose.BitId;

                    if (stepNumber < MaxSteps)
                    {
                        ExploreLargestTotalPressureReleased(
                            valveToClose,
                            stepsLeft,
                            newOpenValvesMip,
                            newTotal);
                    }
                }
            }

            Paths.Add((openValvesMip, currentTotal));
            LargestTotalPressureReleased = Math.Max(LargestTotalPressureReleased, currentTotal);
        }

        public static Explorer BuildAndExplore(string input, int maxSteps)
        {
            var valves = ParseValves(input);
            var costMap = BuildCostFromValveToValve(valves);
            var valvesWithFlow = valves.Values.Where(v => v.FlowRate > 0).ToArray();
            var explorer = new Explorer(valvesWithFlow, costMap, maxSteps);

            explorer.ExploreLargestTotalPressureReleased(valves["AA"], maxSteps);

            return explorer;
        }
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

    static IReadOnlyDictionary<string, Valve> ParseValves(string input) =>
        ParseInputRegex.Matches(input).Select((match, i) => new Valve(
            match.Groups["valve"].Value,
            1L << i,
            int.Parse(match.Groups["flowRate"].Value),
            match.Groups["leadsTo"].Value.Split(", "))).ToDictionary(valve => valve.Id);

    static readonly Regex ParseInputRegex = BuildParseInputRegex();

    [GeneratedRegex(@"Valve (?<valve>[^ ]+) has flow rate=(?<flowRate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<leadsTo>[A-Z, ]+)", RegexOptions.Compiled)]
    private static partial Regex BuildParseInputRegex();
}
