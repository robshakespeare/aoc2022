using System.Diagnostics;

namespace AoC.Day19;

public partial class Day19Solver : ISolver
{
    public string DayName => "Not Enough Minerals";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input)
    {
        var blueprints = ParseBlueprints(input, finalMinuteNumber: 24, isPart2: false);

        return blueprints
            .Select(blueprint => new WorldState(blueprint))
            .Select(FindLargestPossibleGeocodeCount)
            .Sum(x => x.Blueprint.Id * x.BestGeodeCount);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var blueprints = ParseBlueprints(input, finalMinuteNumber: 32, isPart2: true).Take(3);
        var bestGeodeCounts = new List<int>();

        foreach (var blueprint in blueprints)
        {
            Logger($"Started Blueprint {blueprint.Id} @ {DateTime.Now:s}");
            var stopwatch = Stopwatch.StartNew();
            bestGeodeCounts.Add(FindLargestPossibleGeocodeCount(new WorldState(blueprint)).BestGeodeCount);
            Logger($"Completed Blueprint {blueprint.Id}, time taken: {stopwatch.Elapsed}{Environment.NewLine}");
        }

        return bestGeodeCounts.Aggregate((agg, cur) => agg * cur);
    }

    static (int BestGeodeCount, Blueprint Blueprint) FindLargestPossibleGeocodeCount(WorldState startingWorldState)
    {
        var blueprint = startingWorldState.Blueprint;
        var bestGeodeCount = 0;
        var explore = new PriorityQueue<WorldState, long>(new[] { (startingWorldState, 0L) });
        var seen = new HashSet<WorldState>();

        while (explore.Count > 0)
        {
            var world = explore.Dequeue();

            if (world.MinuteNumber < world.Blueprint.FinalMinuteNumber &&
                !seen.Contains(world))
            {
                foreach (var nextWorld in world.GetSuccessors())
                {
                    bestGeodeCount = Math.Max(bestGeodeCount, nextWorld.GeodeCount);
                    if (nextWorld.GeodeCount >= bestGeodeCount - 10 || blueprint.IsPart2)
                    {
                        //var etc = Math.Max(
                        //    nextWorld.GetEstimatedCostToNextProduce(blueprint.GeodeBotOreCost, nextWorld.OreCount, nextWorld.OreProductionRate),
                        //    nextWorld.GetEstimatedCostToNextProduce(blueprint.GeodeBotObsidianCost, nextWorld.ObsidianCount, nextWorld.ObsidianProductionRate));
                        explore.Enqueue(nextWorld, nextWorld.GetEstimatedCostToProduceNextGeodeBot());
                    }
                }

                seen.Add(world);
            }
        }

        Logger($"Blueprint {blueprint.Id}. Best geode count: {bestGeodeCount}");

        return (bestGeodeCount, blueprint);
    }

    record WorldState(Blueprint Blueprint)
    {
        public int MinuteNumber { get; private set; }
        public Blueprint Blueprint { get; } = Blueprint;

        int OreProductionRate { get; init; } = 1;
        int ClayProductionRate { get; init; }
        int ObsidianProductionRate { get; init; }
        int GeodeProductionRate { get; init; }
        int OreCount { get; set; }
        int ClayCount { get; set; }
        int ObsidianCount { get; set; }
        public int GeodeCount { get; private set; }

        public int GetEstimatedCostToProduceNextGeodeBot() => Math.Max(
            GetEstimatedCostToNextProduce(Blueprint.GeodeBotOreCost, OreCount, OreProductionRate),
            GetEstimatedCostToNextProduce(Blueprint.GeodeBotObsidianCost, ObsidianCount, ObsidianProductionRate));

        static int GetEstimatedCostToNextProduce(int resourceCost, int currentResourceCount, int productionRate)
        {
            if (productionRate == 0)
            {
                return int.MaxValue;
            }

            return currentResourceCount >= resourceCost
                ? 1
                : 1 + (resourceCost - currentResourceCount + productionRate - 1) / productionRate;
        }

        public IEnumerable<WorldState> GetSuccessors()
        {
            if (MinuteNumber >= Blueprint.FinalMinuteNumber)
            {
                return Array.Empty<WorldState>();
            }

            // We could spend our resources building bots (resource producers), one option is to always never build a bot tho
            var successors = new List<WorldState> { new(this) };

            if (Blueprint.OreBotOreCost <= OreCount &&
                OreCount < Blueprint.MaxOreConsumptionRate)
            {
                successors.Add(this with
                {
                    OreProductionRate = OreProductionRate + 1,
                    OreCount = OreCount - Blueprint.OreBotOreCost
                });
            }

            if (Blueprint.ClayBotOreCost <= OreCount &&
                ClayCount < Blueprint.ObsidianBotClayCost)
            {
                successors.Add(this with
                {
                    ClayProductionRate = ClayProductionRate + 1,
                    OreCount = OreCount - Blueprint.ClayBotOreCost
                });
            }

            if (Blueprint.ObsidianBotOreCost <= OreCount &&
                Blueprint.ObsidianBotClayCost <= ClayCount &&
                ObsidianCount < Blueprint.GeodeBotObsidianCost)
            {
                successors.Add(this with
                {
                    ObsidianProductionRate = ObsidianProductionRate + 1,
                    OreCount = OreCount - Blueprint.ObsidianBotOreCost,
                    ClayCount = ClayCount - Blueprint.ObsidianBotClayCost,
                });
            }

            if (Blueprint.GeodeBotOreCost <= OreCount &&
                Blueprint.GeodeBotObsidianCost <= ObsidianCount)
            {
                successors.Add(this with
                {
                    GeodeProductionRate = GeodeProductionRate + 1,
                    OreCount = OreCount - Blueprint.GeodeBotOreCost,
                    ObsidianCount = ObsidianCount - Blueprint.GeodeBotObsidianCost,
                });
            }

            // Then, given our CURRENT number of bots, we can then collect our resources and increment the world's minute number
            // Our new bots then become ready at END of turn, and will produce resources in the next turn
            var nextMinuteNumber = MinuteNumber + 1;
            foreach (var successor in successors)
            {
                successor.MinuteNumber = nextMinuteNumber;
                successor.OreCount += OreProductionRate;
                successor.ClayCount += ClayProductionRate;
                successor.ObsidianCount += ObsidianProductionRate;
                successor.GeodeCount += GeodeProductionRate;
            }

            return successors.Where(x => x.IsValidSuccessor());
        }

        private bool IsValidSuccessor()
        {
            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneGeodeProducer && GeodeProductionRate == 0)
            {
                return false;
            }

            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneObsidianProducer && ObsidianProductionRate == 0)
            {
                return false;
            }

            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneClayProducer && ClayProductionRate == 0)
            {
                return false;
            }

            return true;
        }
    }

    public sealed class Blueprint
    {
        public Blueprint(
            int id,
            int oreBotOreCost,
            int clayBotOreCost,
            int obsidianBotOreCost,
            int obsidianBotClayCost,
            int geodeBotOreCost,
            int geodeBotObsidianCost,
            int finalMinuteNumber,
            bool isPart2)
        {
            Id = id;
            OreBotOreCost = oreBotOreCost;
            ClayBotOreCost = clayBotOreCost;
            ObsidianBotOreCost = obsidianBotOreCost;
            ObsidianBotClayCost = obsidianBotClayCost;
            GeodeBotOreCost = geodeBotOreCost;
            GeodeBotObsidianCost = geodeBotObsidianCost;

            FinalMinuteNumber = finalMinuteNumber;
            IsPart2 = isPart2;

            MaxOreConsumptionRate = ClayBotOreCost + ObsidianBotOreCost + GeodeBotOreCost;

            CutOffForAtLeastOneGeodeProducer = finalMinuteNumber - 1;
            CutOffForAtLeastOneObsidianProducer = CutOffForAtLeastOneGeodeProducer - 2;
            CutOffForAtLeastOneClayProducer = CutOffForAtLeastOneObsidianProducer - 2;
        }

        public int Id { get; }
        public int OreBotOreCost { get; }
        public int ClayBotOreCost { get; }
        public int ObsidianBotOreCost { get; }
        public int ObsidianBotClayCost { get; }
        public int GeodeBotOreCost { get; }
        public int GeodeBotObsidianCost { get; }

        public int FinalMinuteNumber { get; }
        public bool IsPart2 { get; }

        public int CutOffForAtLeastOneClayProducer { get; }
        public int CutOffForAtLeastOneObsidianProducer { get; }
        public int CutOffForAtLeastOneGeodeProducer { get; }

        public int MaxOreConsumptionRate { get; }

        public override bool Equals(object? obj) => obj is Blueprint other && Id == other.Id;

        public override int GetHashCode() => Id;

        public override string ToString() =>
            new {Id, OreBotOreCost, ClayBotOreCost, ObsidianBotOreCost, ObsidianBotClayCost, GeodeBotOreCost, GeodeBotObsidianCost}.ToString()!;
    }

    static IEnumerable<Blueprint> ParseBlueprints(string input, int finalMinuteNumber, bool isPart2) =>
        ParseInputRegex.Matches(input).Select(match => new Blueprint(
            int.Parse(match.Groups["blueprintId"].Value),
            int.Parse(match.Groups["oreBotOreCost"].Value),
            int.Parse(match.Groups["clayBotOreCost"].Value),
            int.Parse(match.Groups["obsidianBotOreCost"].Value),
            int.Parse(match.Groups["obsidianBotClayCost"].Value),
            int.Parse(match.Groups["geodeBotOreCost"].Value),
            int.Parse(match.Groups["geodeBotObsidianCost"].Value),
            finalMinuteNumber,
            isPart2)).ToArray();

    static readonly Regex ParseInputRegex = BuildParseInputRegex();

    [GeneratedRegex(
        @"Blueprint (?<blueprintId>\d+):.+Each ore robot costs (?<oreBotOreCost>\d+) ore. Each clay robot costs (?<clayBotOreCost>\d+) ore. Each obsidian robot costs (?<obsidianBotOreCost>\d+) ore and (?<obsidianBotClayCost>\d+) clay. Each geode robot costs (?<geodeBotOreCost>\d+) ore and (?<geodeBotObsidianCost>\d+) obsidian.",
        RegexOptions.Compiled)]
    private static partial Regex BuildParseInputRegex();
}
