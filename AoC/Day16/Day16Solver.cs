namespace AoC.Day16;

public class Day16Solver : ISolver
{
    public string DayName => "Proboscidea Volcanium";

    /// <summary>
    /// Returns a dictionary where the key is the source and destination valve, and the value is the number of steps.
    /// </summary>
    static Dictionary<(Valve Source, Valve Dest), int> BuildCostFromValveToValve(IReadOnlyDictionary<string, Valve> valves)
    {
        var search = new AStarSearch<ValveNode>(valveNode => valveNode.Valve.LeadsTo.Select(valveId => valves[valveId]).Select(valve => new ValveNode(valve)));

        var result = new Dictionary<(Valve Source, Valve Dest), int>();

        foreach (var sourceValve in valves.Values)
        {
            foreach (var destValve in valves.Values)
            {
                var path = search.FindShortestPath(new ValveNode(sourceValve), new ValveNode(destValve));

                result.Add((sourceValve, destValve), path.TotalCost);
            }
        }

        return result;
    }

    public record ValveNode(Valve Valve) : IAStarSearchNode
    {
        public int Cost => 1;
    }

    public long? SolvePart1(PuzzleInput input)
    {
        // Thinking, we know we need to take 30 steps
        // Each step can have any number of successors

        var valves = ParseValves(input);

        var costs = BuildCostFromValveToValve(valves);

        //return costs.Count;

        var start = new WorldState(valves, valves["AA"]);

        const int maxSteps = 30;

        // Work out the best amount we can get by opening each valve
        // (which is how many steps it would take to get there, then how much flow we'd get after opening it, note that open costs one step too)


        // Enumerate all possible paths


        //var explore = new PriorityQueue<WorldState, long>();
        //explore.Enqueue(start, long.MaxValue);

        //var seen = new HashSet<WorldState>();

        //while (explore.Count > 0)
        //{
        //    var node = explore.Dequeue(); // this takes out the top priority node

        //    // if node is the goal return
        //    if (node.StepNumber == maxSteps)
        //    {
        //        return node.TotalPressureReleased;
        //    }

        //    // if we've not already seen the node
        //    if (!seen.Contains(node))
        //    {
        //        //foreach (var (child, stepCost) in node.GetSuccessors())
        //        foreach (var child in node.GetSuccessors())
        //        {
        //            var remainingSteps = maxSteps - child.StepNumber;
        //            var heuristic = child.OpenedValve?.FlowRate * remainingSteps ?? 0;

        //            var cost = long.MaxValue - child.TotalPressureReleased - heuristic; // heuristic is how much pressure can be released over the remaining time, for the valve just opened

        //            explore.Enqueue(child, cost);
        //        }

        //        seen.Add(node);
        //    }
        //}

        //throw new InvalidOperationException("No paths found");

        //=====================

        ////var worldStates = new[] { new WorldState(valves, valves["AA"], Array.Empty<Valve>(), null) }.ToList();
        var worldStates = new[] { start }.ToHashSet();

        var greatestTotalPressureReleased = 0;

        for (var stepNumber = 1; stepNumber <= maxSteps; stepNumber++)
        {
            var remainingSteps = maxSteps - stepNumber;

            var newWorldStates = new HashSet<WorldState>();

            foreach (var worldState in worldStates)
            {
                //newWorldStates.AddRange(worldState.GetSuccessors());

                foreach (var successor in worldState.GetSuccessors())
                {
                    // Exclude any successor which has no chance of being the best
                    if (successor.GetGreatestPossibleTotalPressureReleased(costs, remainingSteps) >= greatestTotalPressureReleased)
                    {
                        newWorldStates.Add(successor);

                        greatestTotalPressureReleased = Math.Max(greatestTotalPressureReleased, successor.TotalPressureReleased);
                    }
                }
            }

            worldStates = newWorldStates;

            Reporter?.Invoke($"{new { stepNumber, numWorldStates = worldStates.Count }}");
        }

        //var solvePart1 = worldStates.MaxBy(x => x.TotalPressureReleased);

        //var test = worldStates.FirstOrDefault(x => x.OpenValves == "DD, BB, JJ, HH, EE, CC");

        //if (test != null)
        //{
        //    Console.WriteLine(test);
        //    Console.WriteLine(test.TotalPressureReleased);
        //}

        return greatestTotalPressureReleased; //solvePart1?.TotalPressureReleased ?? -1;
    }

    public Action<string>? Reporter { get; set; }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class WorldState
    {
        public IReadOnlyDictionary<string, Valve> Valves { get; }
        public Valve CurrentValve { get; }
        public Valve? OpenedValve { get; }
        //public Valve[] OpenValves { get; }
        public string OpenValves { get; }
        public int OpenValvesFlowRate { get; }
        public int StepNumber { get; }
        public int TotalPressureReleased { get; }
        public IEnumerable<int> PressuresReleased { get; }

        public override string ToString() => OpenValves;

        public string OpenValvesAscendingOrdered => string.Join(", ", OpenValves.Split(", ").Order());

        public int GetGreatestPossibleTotalPressureReleased(
            Dictionary<(Valve Source, Valve Dest), int> costMap,
            int remainingSteps)
        {
            // For any closed valve, find out cost to move from here to closed valve, add 1 for opening it, then calc delta from remaining steps
            // If delta is greater than zero, calc total pres (steps * flowRate)

            var result = TotalPressureReleased;

            var closedValves = Valves.Values.Where(v => !OpenValves.Contains(v.Id));

            foreach (var closedValve in closedValves)
            {
                var costToReach = costMap[(CurrentValve, closedValve)];

                var costToOpen = costToReach + 1;

                var stepsLeft = remainingSteps - costToOpen;

                if (stepsLeft > 0)
                {
                    result += stepsLeft * closedValve.FlowRate;
                }
            }

            return result;
        }

        public WorldState(IReadOnlyDictionary<string, Valve> valves, Valve currentValve)
        {
            Valves = valves;
            CurrentValve = currentValve;
            OpenValves = ""; //Array.Empty<Valve>();
            OpenValvesFlowRate = 0;
            StepNumber = 0;
            TotalPressureReleased = 0;
            PressuresReleased = Array.Empty<int>();
        }

        public WorldState(
            Valve currentValve,
            WorldState previousWorldState,
            bool openValve = false)
        {
            Valves = previousWorldState.Valves;
            CurrentValve = currentValve;
            OpenValves = previousWorldState.OpenValves;
            OpenValvesFlowRate = previousWorldState.OpenValvesFlowRate;
            StepNumber = previousWorldState.StepNumber + 1;
            TotalPressureReleased = previousWorldState.TotalPressureReleased;
            PressuresReleased = previousWorldState.PressuresReleased;

            //Valves = valves;
            //CurrentValve = currentValve;
            //OpenValves = openValves;
            //StepNumber = previousWorldState == null ? 1 : previousWorldState.StepNumber + 1;
            //PressureReleased += OpenValves.Sum(v => v.FlowRate); 

            TotalPressureReleased += OpenValvesFlowRate; // In each step, any open valves release more pressure
            PressuresReleased = PressuresReleased.Append(OpenValvesFlowRate); //.ToArray();

            if (openValve)
            {
                //OpenValves = OpenValves.Append(CurrentValve).ToArray(); // rs-todo: string more optimal?
                OpenedValve = CurrentValve;

                string msg = $"{StepNumber}:{OpenedValve.Id}";

                OpenValves = OpenValves == "" ? msg : $"{OpenValves}, {msg}";
                OpenValvesFlowRate += OpenedValve.FlowRate;
            }
        }

        public IEnumerable<WorldState> GetSuccessors()
        {
            // For each, we can either open our current valve
            if (CurrentValve.FlowRate > 0 && !OpenValves.Contains(CurrentValve.Id))
            {
                yield return new WorldState(CurrentValve, this, openValve: true);

                //yield return new WorldState(Valves, CurrentValve, OpenValves.Append(CurrentValve).ToArray(), this);
            }

            // Or, move to another valve
            foreach (var nextValveId in CurrentValve.LeadsTo)
            {
                //if (OpenValves.All(openValve => openValve.Id != nextValveId)) // if don't want to go back
                //{   
                //}

                var nextValve = Valves[nextValveId];
                yield return new WorldState(nextValve, this);
            }

            // A valid step is to just stay where we are!
            if (OpenValvesFlowRate > 0)
            {
                yield return this;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((WorldState) obj);
        }

        protected bool Equals(WorldState other) => CurrentValve.Equals(other.CurrentValve) && OpenValves == other.OpenValves;

        public override int GetHashCode() => HashCode.Combine(CurrentValve, OpenValves);
    }

    public record Valve(string Id, int FlowRate, IReadOnlyList<string> LeadsTo)
    {
        public string LeadsToIds { get; } = string.Join(", ", LeadsTo);
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
