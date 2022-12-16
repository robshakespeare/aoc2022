namespace AoC.Day16;

public class Day16Solver : ISolver
{
    public string DayName => "Proboscidea Volcanium";

    public Action<string>? Reporter { get; set; }

    public long? SolvePart1(PuzzleInput input)
    {
        const int maxSteps = 30;

        var valves = ParseValves(input);
        var costMap = BuildCostFromValveToValve(valves);
        var context = new Context(valves, costMap, maxSteps);

        ExploreGreatestTotalPressureReleased(
            context,
            new Actor("Self", valves["AA"], maxSteps));

        return context.GreatestTotalPressureReleased;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record Valve(string Id, int FlowRate, IReadOnlyList<string> LeadsTo) : IAStarSearchNode
    {
        public int Cost => 1;
    }

    public readonly record struct Actor(string Name, Valve CurrentValve, int RemainingSteps);

    public record Context(
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

    static void ExploreGreatestTotalPressureReleased(
        Context context,
        Actor actor,
        string openValves = "",
        int currentTotal = 0)
    {
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
                        actor with
                        {
                            CurrentValve = valveToClose,
                            RemainingSteps = stepsLeft
                        },
                        newOpenValves,
                        newTotal);
                }
            }
        }

        context.GreatestTotalPressureReleased = Math.Max(context.GreatestTotalPressureReleased, currentTotal);
    }

    static IReadOnlyDictionary<string, Valve> ParseValves(string input) =>
        ParseInputRegex.Matches(input).Select(match => new Valve(
            match.Groups["valve"].Value,
            int.Parse(match.Groups["flowRate"].Value),
            match.Groups["leadsTo"].Value.Split(", "))).ToDictionary(valve => valve.Id);

    static readonly Regex ParseInputRegex = new(
        @"Valve (?<valve>[^ ]+) has flow rate=(?<flowRate>\d+); tunnel(s)? lead(s)? to valve(s)? (?<leadsTo>[A-Z, ]+)",
        RegexOptions.Compiled);

    //public class WorldState
    //{
    //    public IReadOnlyDictionary<string, Valve> Valves { get; }
    //    public Valve CurrentValve { get; }
    //    public Valve? OpenedValve { get; }
    //    //public Valve[] OpenValves { get; }
    //    public string OpenValves { get; }
    //    public int OpenValvesFlowRate { get; }
    //    public int StepNumber { get; }
    //    public int TotalPressureReleased { get; }
    //    public IEnumerable<int> PressuresReleased { get; }

    //    public override string ToString() => OpenValves;

    //    public string OpenValvesAscendingOrdered => string.Join(", ", OpenValves.Split(", ").Order());

    //    //public int GetGreatestPossibleTotalPressureReleased(
    //    //    Dictionary<(Valve Source, Valve Dest), int> costMap,
    //    //    int remainingSteps,
    //    //    int maxSteps)
    //    //{
    //    //    // For any closed valve, find out cost to move from here to closed valve, add 1 for opening it, then calc delta from remaining steps
    //    //    // If delta is greater than zero, calc total pres (steps * flowRate)

    //    //    //var result = TotalPressureReleased;

    //    //    //var closedValves = Valves.Values.Where(v => !OpenValves.Contains(v.Id));

    //    //    //foreach (var closedValve in closedValves)
    //    //    //{
    //    //    //    var costToReach = costMap[(CurrentValve, closedValve)];

    //    //    //    var costToOpen = costToReach + 1;

    //    //    //    var stepsLeft = remainingSteps - costToOpen;

    //    //    //    if (stepsLeft > 0)
    //    //    //    {
    //    //    //        result += stepsLeft * closedValve.FlowRate;
    //    //    //    }
    //    //    //}

    //    //    //return result;

    //    //    var context = new Context();

    //    //    GetGreatestPossibleTotalPressureReleasedX(
    //    //        Valves,
    //    //        OpenValves,
    //    //        TotalPressureReleased,
    //    //        CurrentValve,
    //    //        costMap,
    //    //        remainingSteps,
    //    //        maxSteps,
    //    //        context);

    //    //    return context.GreatestTotalPressureReleased;
    //    //}

        

    //    public WorldState(IReadOnlyDictionary<string, Valve> valves, Valve currentValve)
    //    {
    //        Valves = valves;
    //        CurrentValve = currentValve;
    //        OpenValves = ""; //Array.Empty<Valve>();
    //        OpenValvesFlowRate = 0;
    //        StepNumber = 0;
    //        TotalPressureReleased = 0;
    //        PressuresReleased = Array.Empty<int>();
    //    }

    //    public WorldState(
    //        Valve currentValve,
    //        WorldState previousWorldState,
    //        bool openValve = false)
    //    {
    //        Valves = previousWorldState.Valves;
    //        CurrentValve = currentValve;
    //        OpenValves = previousWorldState.OpenValves;
    //        OpenValvesFlowRate = previousWorldState.OpenValvesFlowRate;
    //        StepNumber = previousWorldState.StepNumber + 1;
    //        TotalPressureReleased = previousWorldState.TotalPressureReleased;
    //        PressuresReleased = previousWorldState.PressuresReleased;

    //        //Valves = valves;
    //        //CurrentValve = currentValve;
    //        //OpenValves = openValves;
    //        //StepNumber = previousWorldState == null ? 1 : previousWorldState.StepNumber + 1;
    //        //PressureReleased += OpenValves.Sum(v => v.FlowRate); 

    //        TotalPressureReleased += OpenValvesFlowRate; // In each step, any open valves release more pressure
    //        PressuresReleased = PressuresReleased.Append(OpenValvesFlowRate); //.ToArray();

    //        if (openValve)
    //        {
    //            //OpenValves = OpenValves.Append(CurrentValve).ToArray(); // Q: string more optimal?
    //            OpenedValve = CurrentValve;

    //            string msg = $"{StepNumber}:{OpenedValve.Id}";

    //            OpenValves = OpenValves == "" ? msg : $"{OpenValves}, {msg}";
    //            OpenValvesFlowRate += OpenedValve.FlowRate;
    //        }
    //    }

    //    public IEnumerable<WorldState> GetSuccessors()
    //    {
    //        // For each, we can either open our current valve
    //        if (CurrentValve.FlowRate > 0 && !OpenValves.Contains(CurrentValve.Id))
    //        {
    //            yield return new WorldState(CurrentValve, this, openValve: true);

    //            //yield return new WorldState(Valves, CurrentValve, OpenValves.Append(CurrentValve).ToArray(), this);
    //        }

    //        // Or, move to another valve
    //        foreach (var nextValveId in CurrentValve.LeadsTo)
    //        {
    //            //if (OpenValves.All(openValve => openValve.Id != nextValveId)) // if don't want to go back
    //            //{   
    //            //}

    //            var nextValve = Valves[nextValveId];
    //            yield return new WorldState(nextValve, this);
    //        }

    //        // A valid step is to just stay where we are!
    //        if (OpenValvesFlowRate > 0)
    //        {
    //            yield return this;
    //        }
    //    }

    //    public override bool Equals(object? obj)
    //    {
    //        if (obj is null) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        return obj.GetType() == GetType() && Equals((WorldState) obj);
    //    }

    //    protected bool Equals(WorldState other) => CurrentValve.Equals(other.CurrentValve) && OpenValves == other.OpenValves;

    //    public override int GetHashCode() => HashCode.Combine(CurrentValve, OpenValves);
    //}
}
