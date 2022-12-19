namespace AoC.Day19;

public partial class Day19Solver : ISolver
{
    public string DayName => "Not Enough Minerals";

    private const int FinalMinuteNumber = 24;

    public long? SolvePart1(PuzzleInput input)
    {
        // We just need the greatest number of geodes collected
        // So, we need to get to the largest number of geodes collecting robots the quickest
        // Once we have that setup, it can just go on
        // So, can we work backwards to from the geode collector working out the best path?
        // Most likely, we need to exclude paths/world states where there is no point a building bit X anymore, because it will have no effect
        // i.e. its resources generated could never be used by any other bot in the remaining time

        var blueprints = ParseBlueprints(input);

        var explore = new Queue<WorldState>(new[] {new WorldState(blueprints[0]/*, MinuteNumber: 0, NumOreBots: 1*/)});
        var seen = new HashSet<WorldState>();
        var completedWorlds = new HashSet<WorldState>();

        while (explore.Count > 0)
        {
            var world = explore.Dequeue();

            // Only explore ones we've not already explored
            if (!seen.Contains(world))
            {
                seen.Add(world);

                // If this world has reached the end, log it; else, explore its successors
                if (world.MinuteNumber == FinalMinuteNumber)
                {
                    completedWorlds.Add(world);
                }
                else
                {
                    foreach (var nextWorld in world.GetSuccessors())
                    {
                        explore.Enqueue(nextWorld);
                    }
                }
            }
        }

        return completedWorlds.Max(x => x.GeodeCount);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record WorldState(Blueprint Blueprint)
        //int MinuteNumber,
        //int NumOreBots,
        //int NumClayBots = 0,
        //int NumObsidianBots = 0,
        //int NumGeodeBots = 0,
        //int OreCount = 0,
        //int ClayCount = 0,
        //int ObsidianCount = 0,
        //int GeodeCount = 0)
    {
        public int MinuteNumber { get; private set; }
        public int NumOreBots { get; private init; } = 1;
        public int NumClayBots { get; private init; }
        public int NumObsidianBots { get; private init; }
        public int NumGeodeBots { get; private init; }
        public int OreCount { get; private set; }
        public int ClayCount { get; private set; }
        public int ObsidianCount { get; private set; }
        public int GeodeCount { get; private set; }

        public IReadOnlyList<WorldState> GetSuccessors()
        {
            if (MinuteNumber >= FinalMinuteNumber)
            {
                return Array.Empty<WorldState>();
            }

            // We could spend our resources building bots, one option is to always never build a bot tho
            var successors = new List<WorldState> {new(this)};

            if (Blueprint.OreBotOreCost <= OreCount)
            {
                successors.Add(this with
                {
                    NumOreBots = NumOreBots + 1,
                    OreCount = OreCount - Blueprint.OreBotOreCost
                });
            }

            if (Blueprint.ClayBotOreCost <= OreCount)
            {
                successors.Add(this with
                {
                    NumClayBots = NumClayBots + 1,
                    OreCount = OreCount - Blueprint.ClayBotOreCost
                });
            }

            if (Blueprint.ObsidianBotOreCost <= OreCount &&
                Blueprint.ObsidianBotClayCost <= ClayCount)
            {
                successors.Add(this with
                {
                    NumObsidianBots = NumObsidianBots + 1,
                    OreCount = OreCount - Blueprint.ObsidianBotOreCost,
                    ClayCount = ClayCount - Blueprint.ObsidianBotClayCost,
                });
            }

            if (Blueprint.GeodeBotOreCost <= OreCount &&
                Blueprint.GeodeBotObsidianCost <= ObsidianCount)
            {
                successors.Add(this with
                {
                    NumGeodeBots = NumGeodeBots + 1,
                    OreCount = OreCount - Blueprint.GeodeBotOreCost,
                    ObsidianCount = ObsidianCount - Blueprint.GeodeBotObsidianCost,
                });
            }

            // Then, given our number of bots, we can then collect our resources (increment the world's minute number)
            // rs-todo: use our current world's state to collect the resources!
            var nextMinuteNumber = MinuteNumber + 1;
            foreach (var successor in successors)
            {
                successor.MinuteNumber = nextMinuteNumber;
                successor.OreCount += NumOreBots;
                successor.ClayCount += NumClayBots;
                successor.ObsidianCount += NumObsidianBots;
                successor.GeodeCount += NumGeodeBots;
            }

            // Our new bots then become ready (we don't have to do anything tho, we created them above, but used our current bots to collect new resources)

            return successors;
        }
    }

    public record Blueprint(
        int Id,
        int OreBotOreCost,
        int ClayBotOreCost,
        int ObsidianBotOreCost,
        int ObsidianBotClayCost,
        int GeodeBotOreCost,
        int GeodeBotObsidianCost);

    static IReadOnlyList<Blueprint> ParseBlueprints(string input)
    {
        return ParseInputRegex.Matches(input).Select(match => new Blueprint(
            int.Parse(match.Groups["blueprintId"].Value),
            int.Parse(match.Groups["oreBotOreCost"].Value),
            int.Parse(match.Groups["clayBotOreCost"].Value),
            int.Parse(match.Groups["obsidianBotOreCost"].Value),
            int.Parse(match.Groups["obsidianBotClayCost"].Value),
            int.Parse(match.Groups["geodeBotOreCost"].Value),
            int.Parse(match.Groups["geodeBotObsidianCost"].Value))).ToArray();
    }

    // Blueprint (?<blueprintId>\d+):.+Each ore robot costs (?<oreBotOreCost>\d+) ore. Each clay robot costs (?<clayBotOreCost>\d+) ore. Each obsidian robot costs (?<obsidianBotOreCost>\d+) ore and (?<obsidianBotClayCost>\d+) clay. Each geode robot costs (?<geodeBotOreCost>\d+) ore and (?<geodeBotObsidianCost>\d+) obsidian.
    static readonly Regex ParseInputRegex = BuildParseInputRegex();

    [GeneratedRegex(
        @"Blueprint (?<blueprintId>\d+):.+Each ore robot costs (?<oreBotOreCost>\d+) ore. Each clay robot costs (?<clayBotOreCost>\d+) ore. Each obsidian robot costs (?<obsidianBotOreCost>\d+) ore and (?<obsidianBotClayCost>\d+) clay. Each geode robot costs (?<geodeBotOreCost>\d+) ore and (?<geodeBotObsidianCost>\d+) obsidian.",
        RegexOptions.Compiled)]
    private static partial Regex BuildParseInputRegex();
}
