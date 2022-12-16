namespace AoC.Day16;

public class Day16Solver : ISolver
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

        ExploreGreatestTotalPressureReleased(
            context,
            new [] { new Actor("Self", valves["AA"], maxSteps) });

        return context.GreatestTotalPressureReleased;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        const int maxSteps = 26;

        var valves = ParseValves(input);
        var costMap = BuildCostFromValveToValve(valves);
        var context = new Context(valves, costMap, maxSteps);

        ExploreGreatestTotalPressureReleased(
            context,
            new[]
            {
                new Actor("Self", valves["AA"], maxSteps),
                new Actor("Elephant", valves["AA"], maxSteps)
            });

        return context.GreatestTotalPressureReleased;
    }

    record Valve(string Id, int FlowRate, IReadOnlyList<string> LeadsTo) : IAStarSearchNode
    {
        public int Cost => 1;
    }

    readonly record struct Actor(string Name, Valve CurrentValve, int RemainingSteps);

    record Context(
        IReadOnlyDictionary<string, Valve> Valves,
        Dictionary<(Valve Source, Valve Dest), int> CostMap,
        int MaxSteps)
    {
        public int GreatestTotalPressureReleased { get; set; }
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

    void ExploreGreatestTotalPressureReleased(
        Context context,
        Actor[] actors,
        string openValves = "",
        int currentTotal = 0)
    {
        var actor = actors.MaxBy(x => x.RemainingSteps);

        var valvesToClose = new Queue<Valve>(context.Valves.Values.Where(valve => valve.FlowRate > 0 && !openValves.Contains(valve.Id)));

        while (valvesToClose.Count > 0)
        {
            var valveToClose = valvesToClose.Dequeue();
            var costToReach = context.CostMap[(actor.CurrentValve, valveToClose)];
            var costToOpen = costToReach + 1;
            var stepsLeft = actor.RemainingSteps - costToOpen;

            if (stepsLeft > 0)
            {
                var newTotal = currentTotal + stepsLeft * valveToClose.FlowRate;
                var stepNumber = context.MaxSteps - stepsLeft;
                var newOpenValves = $"{openValves},{stepNumber}:{valveToClose.Id}";

                if (stepNumber < context.MaxSteps)
                {
                    ExploreGreatestTotalPressureReleased(
                        context,
                        actors.Select(a => a.Name != actor.Name ? a : actor with {CurrentValve = valveToClose, RemainingSteps = stepsLeft}).ToArray(),
                        newOpenValves,
                        newTotal);
                }
            }
        }

        context.GreatestTotalPressureReleased = Math.Max(context.GreatestTotalPressureReleased, currentTotal);

        if (DateTime.Now - _lastReported > TimeSpan.FromSeconds(30))
        {
            var message = $"GreatestTotalPressureReleased: {context.GreatestTotalPressureReleased} // {DateTime.Now:O}";
            (Reporter ?? Console.WriteLine).Invoke(message);
            _lastReported = DateTime.Now;
        }
    }

    static IReadOnlyDictionary<string, Valve> ParseValves(string input) =>
        ParseInputRegex.Matches(input).Select(match => new Valve(
            match.Groups["valve"].Value,
            int.Parse(match.Groups["flowRate"].Value),
            match.Groups["leadsTo"].Value.Split(", "))).ToDictionary(valve => valve.Id);

    static readonly Regex ParseInputRegex = new(
        @"Valve (?<valve>[^ ]+) has flow rate=(?<flowRate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<leadsTo>[A-Z, ]+)",
        RegexOptions.Compiled);
}
