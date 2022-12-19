using System.Diagnostics;
using static AoC.Day19.Day19Solver;

namespace AoC.Day19;

public partial class Day19Solver : ISolver
{
    public string DayName => "Not Enough Minerals";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input)
    {
        // Recurse backwards, saying how quickly can we get X

        // e.g. how quickly can we get 7 obsidian and 2 ore?
        // So, to get that, we need obsidian, so how quickly 3 ore and 14 clay
        // So, to get that, we need clay, so how quickly 2 ore

        // We just need the greatest number of geodes collected
        // So, we need to get to the largest number of geodes collecting robots the quickest
        // Once we have that setup, it can just go on
        // So, can we work backwards to from the geode collector working out the best path?
        // Most likely, we need to exclude paths/world states where there is no point a building bit X anymore, because it will have no effect
        // i.e. its resources generated could never be used by any other bot in the remaining time

        //const int finalMinuteNumber = 24;

        //return -1; // rs-todo: only temp

        const int finalMinuteNumber = 24;

        var blueprints = ParseBlueprints(input, finalMinuteNumber);

        return blueprints
            .Select(blueprint => new WorldState(blueprint))
            .Select(FindBestWorldPart1)
            .Sum(x => x?.GetQualityLevel() ?? 0);

        //foreach (var blueprint in blueprints)
        //{
        //    Logger($"BP {blueprint.Id}, QuickestTimeToCreateGeodeBot: {blueprint.QuickestTimeToCreateGeodeBot}, QuickestTimeToCreateObsidianBot: {blueprint.QuickestTimeToCreateObsidianBot}");
        //}

        //return null;

        ///////

        //var bestWorlds = new List<WorldState>();

        //var stopwatch = Stopwatch.StartNew();

        //foreach (var blueprint in blueprints)
        //{
        //    Logger($"Processing blueprint: {blueprint.Id} -- Elapsed:{stopwatch.Elapsed}");

        //    var initialWorldState = new WorldState(blueprint);

        //    var worlds = FindShortestPath(new[] { initialWorldState }, /*finalMinuteNumber*/ w => w.MinuteNumber == FinalMinuteNumber);

        //    var bestWorld = worlds.MaxBy(x => x.Item1.GeodeCount)!;
        //    bestWorlds.Add(bestWorld.Item1);

        //    Logger($"UPDATE: Processed blueprint: {blueprint.Id} -- Elapsed:{stopwatch.Elapsed}");
        //    Logger(blueprint.ToString());
        //    Logger("Timeline:");
        //    Logger(WorldState.GetTimeline(bestWorld.Item2));
        //    Logger("");
        //}

        //return bestWorlds.Sum(x => x.GetQualityLevel());

        //var explore = new Queue<WorldState>(new[] {initialWorldState});
        //var seen = new HashSet<WorldState>();
        //var completedWorlds = new HashSet<WorldState>();

        //while (explore.Count > 0)
        //{
        //    var world = explore.Dequeue();

        //    // Only explore ones we've not already explored
        //    if (!seen.Contains(world))
        //    {
        //        seen.Add(world);

        //        // If this world has reached the end, log it; else, explore its successors
        //        if (world.MinuteNumber == FinalMinuteNumber)
        //        {
        //            completedWorlds.Add(world);
        //        }
        //        else
        //        {
        //            foreach (var nextWorld in world.GetSuccessors())
        //            {
        //                explore.Enqueue(nextWorld);
        //            }
        //        }
        //    }
        //}

        //return null;

        ////var world1 = FindShortestPath(initialWorldState, world => world.OreCount == 2 && world.NumClayBots == 1);
        //var worlds = FindShortestPath(new [] { initialWorldState }, world => world.ClayCount >= 14);

        //var worlds2 = FindShortestPath(worlds, world => world.ObsidianCount >= 7);

        ////var worlds3 = FindShortestPath(worlds, world => world.ObsidianCount >= 7);

        ////return null;

        ////var world2 = FindShortestPath(world1, world => world.ObsidianCount == 1);

        //var explore = new Queue<WorldState>(worlds2);
        //var seen = new HashSet<WorldState>();
        //var completedWorlds = new HashSet<WorldState>();

        //while (explore.Count > 0)
        //{
        //    var world = explore.Dequeue();

        //    // Only explore ones we've not already explored
        //    if (!seen.Contains(world))
        //    {
        //        seen.Add(world);

        //        // If this world has reached the end, log it; else, explore its successors
        //        if (world.MinuteNumber == FinalMinuteNumber)
        //        {
        //            completedWorlds.Add(world);
        //        }
        //        else
        //        {
        //            foreach (var nextWorld in world.GetSuccessors())
        //            {
        //                explore.Enqueue(nextWorld);
        //            }
        //        }
        //    }
        //}

        //var bp1Geodes = completedWorlds.MaxBy(x => x.GeodeCount);

        ////////

        //initialWorldState = new WorldState(blueprints[0]);

        ////var world1 = FindShortestPath(initialWorldState, world => world.OreCount == 2 && world.NumClayBots == 1);
        //worlds = FindShortestPath(new[] { initialWorldState }, world => world.ClayCount >= 8);

        //worlds2 = FindShortestPath(worlds, world => world.ObsidianCount >= 12);

        ////var worlds3 = FindShortestPath(worlds, world => world.ObsidianCount >= 7);

        ////return null;

        ////var world2 = FindShortestPath(world1, world => world.ObsidianCount == 1);

        //explore = new Queue<WorldState>(worlds2);
        //seen = new HashSet<WorldState>();
        //completedWorlds = new HashSet<WorldState>();

        //while (explore.Count > 0)
        //{
        //    var world = explore.Dequeue();

        //    // Only explore ones we've not already explored
        //    if (!seen.Contains(world))
        //    {
        //        seen.Add(world);

        //        // If this world has reached the end, log it; else, explore its successors
        //        if (world.MinuteNumber == FinalMinuteNumber)
        //        {
        //            completedWorlds.Add(world);
        //        }
        //        else
        //        {
        //            foreach (var nextWorld in world.GetSuccessors())
        //            {
        //                explore.Enqueue(nextWorld);
        //            }
        //        }
        //    }
        //}

        //var bp2Geodes = completedWorlds.MaxBy(x => x.GeodeCount);

        //return (bp1Geodes.Blueprint.Id * bp1Geodes.GeodeCount) + (bp2Geodes.Blueprint.Id * bp2Geodes.GeodeCount);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        const int finalMinuteNumber = 32;

        var blueprints = ParseBlueprints(input, finalMinuteNumber).Take(3).ToArray();

        var stopwatch = Stopwatch.StartNew();

        Logger($"Started BP 1 {DateTime.Now:s}");
        stopwatch.Reset();
        var geodeCountBp1 = FindBestWorldPart1(new WorldState(blueprints[0]))?.GeodeCount ?? 1;
        Logger($"BP 1 done, time taken: {stopwatch.Elapsed}");

        Logger($"Started BP 2 {DateTime.Now:s}");
        stopwatch.Reset();
        var geodeCountBp2 = FindBestWorldPart1(new WorldState(blueprints[1]))?.GeodeCount ?? 1;
        Logger($"BP 2 done, time taken: {stopwatch.Elapsed}");

        int geodeCountBp3 = 1;
        if (blueprints.Length > 2)
        {
            Logger($"Started BP 3 {DateTime.Now:s}");
            stopwatch.Reset();
            geodeCountBp3 = FindBestWorldPart1(new WorldState(blueprints[2]))?.GeodeCount ?? 1;
            Logger($"BP 3 done, time taken: {stopwatch.Elapsed}");
        }

        return geodeCountBp1 * geodeCountBp2 * geodeCountBp3;

        return blueprints
            .Select(blueprint => new WorldState(blueprint))
            .Select(FindBestWorldPart1)
            .Aggregate(1L, (agg, cur) => agg * cur?.GeodeCount ?? 0);

        return null;

        //const int finalMinuteNumber = 32;

        //var blueprints = ParseBlueprints(input).Take(3);

        /////////

        //var bestWorlds = new List<WorldState>();

        //var stopwatch = Stopwatch.StartNew();

        //foreach (var blueprint in blueprints)
        //{
        //    Logger($"Processing blueprint: {blueprint.Id} -- Elapsed:{stopwatch.Elapsed}");

        //    var initialWorldState = new WorldState(blueprint);

        //    var worlds = FindShortestPath(new[] { initialWorldState }, w => w.MinuteNumber == finalMinuteNumber);

        //    bestWorlds.Add(worlds.MaxBy(x => x.GeodeCount)!);

        //    Logger($"UPDATE: Processed blueprint: {blueprint.Id} -- Elapsed:{stopwatch.Elapsed}");
        //}

        //return bestWorlds.Aggregate(1L, (agg, cur) => agg * cur.GeodeCount);
    }

    //public record ShortestPathBasic(int? MinuteNumber, IReadOnlySet<WorldState> Worlds);

    //static int FindShortestPathBasic(WorldState startingWorldState, Func<WorldState, bool> isGoal)
    //{
    //    var explore = new PriorityQueue<WorldState, long>(new[] {(startingWorldState, 0L)});
    //    var seen = new HashSet<WorldState>();
    //    var shortestPath = int.MaxValue;
    //    //var goalWorlds = new HashSet<WorldState>();

    //    while (explore.Count > 0)
    //    {
    //        var world = explore.Dequeue();

    //        if (isGoal(world))
    //        {
    //            shortestPath = Math.Min(shortestPath, world.MinuteNumber);
    //            //goalWorlds.Add(world);
    //        }
    //        else if (!seen.Contains(world))
    //        {
    //            foreach (var nextWorld in world.GetSuccessors())
    //            {
    //                if (world.MinuteNumber < shortestPath)
    //                {
    //                    explore.Enqueue(nextWorld, nextWorld.MinuteNumber);
    //                }
    //            }

    //            seen.Add(world);
    //        }
    //    }

    //    //return new ShortestPathBasic(shortestPath, goalWorlds);

    //    //Logger($"BP {startingWorldState.Blueprint.Id} -- {goalWorlds.Count(w => w.MinuteNumber == shortestPath)}");
    //    return shortestPath;

    //    //return shortestPath == int.MaxValue ? null : shortestPath;

    //    //return goalWorlds.MinBy(x => x.MinuteNumber)?.MinuteNumber ?? throw new InvalidOperationException($"{nameof(FindShortestPathBasic)} failed");
    //}

    //public class WorldStateComparer : IEqualityComparer<WorldState>
    //{
    //    public bool Equals(WorldState x, WorldState y)
    //    {
    //        if (ReferenceEquals(x, y)) return true;
    //        if (ReferenceEquals(x, null)) return false;
    //        if (ReferenceEquals(y, null)) return false;
    //        if (x.GetType() != y.GetType()) return false;
    //        return x.NumOreBots == y.NumOreBots && x.NumClayBots == y.NumClayBots && x.NumObsidianBots == y.NumObsidianBots && x.NumGeodeBots == y.NumGeodeBots && x.OreCount == y.OreCount && x.ClayCount == y.ClayCount && x.ObsidianCount == y.ObsidianCount && x.GeodeCount == y.GeodeCount && x.Blueprint.Equals(y.Blueprint);
    //    }

    //    public int GetHashCode(WorldState obj)
    //    {
    //        var hashCode = new HashCode();
    //        hashCode.Add(obj.NumOreBots);
    //        hashCode.Add(obj.NumClayBots);
    //        hashCode.Add(obj.NumObsidianBots);
    //        hashCode.Add(obj.NumGeodeBots);
    //        hashCode.Add(obj.OreCount);
    //        hashCode.Add(obj.ClayCount);
    //        hashCode.Add(obj.ObsidianCount);
    //        hashCode.Add(obj.GeodeCount);
    //        hashCode.Add(obj.Blueprint);
    //        return hashCode.ToHashCode();
    //    }
    //}

    // static inline uint32_t const eta(uint32_t needed, uint32_t got, uint32_t produced)
    //{
    //    return got >= needed ? 1 : 1 + (needed - got + produced - 1) / produced;
    //}

    // static int GetEstimatedCostToCollect(int requiredResourceCount, int currentResourceCount, int currentRobotCount)
    static int GetEstimatedCostToCollect(int requiredResourceCount, int currentResourceCount, int producingRobotCount)
    {
        if (producingRobotCount == 0)
        {
            return int.MaxValue;
        }

        return currentResourceCount >= requiredResourceCount
            ? 1
            : 1 + (requiredResourceCount - currentResourceCount + producingRobotCount - 1) / producingRobotCount;
    }

    static WorldState? FindBestWorldPart1(WorldState startingWorldState)
    {
        var blueprint = startingWorldState.Blueprint;
        var bestGeodeCount = 0;
        //var worlds = new List<WorldState>
        //{
        //    startingWorldState
        //};

        //for (var i = 1; i <= blueprint.FinalMinuteNumber; i++)
        //{
        //    var newWorlds = new List<WorldState>();

        //    foreach (var world in worlds)
        //    {
        //        foreach (var nextWorld in world.GetSuccessors())
        //        {
        //            bestGeodeCount = Math.Max(bestGeodeCount, nextWorld.GeodeCount);
        //            if (nextWorld.GeodeCount >= bestGeodeCount) // || true) // rs-todo: need to work out how to exclude things that aren't looking good!
        //            {
        //                //explore.Enqueue(nextWorld, long.MaxValue - nextWorld.GeodeCount);
        //                //var etc = Math.Max(
        //                //    GetEstimatedCostToCollect(blueprint.GeodeBotOreCost, nextWorld.OreCount, nextWorld.NumOreBots),
        //                //    GetEstimatedCostToCollect(blueprint.GeodeBotObsidianCost, nextWorld.ObsidianCount, nextWorld.NumObsidianBots));
        //                newWorlds.Add(nextWorld);
        //            }
        //        }
        //    }

        //    worlds = newWorlds;
        //}

        //var bestWorld2 = worlds.MaxBy(x => x.GeodeCount);

        //Logger($"Blueprint {startingWorldState.Blueprint.Id}. Best geode count: {bestWorld2?.GeodeCount ?? 0}");

        //return bestWorld2; // ?? throw new InvalidOperationException("Unexpected: No best world found");


        var explore = new PriorityQueue<WorldState, long>(new[] { (startingWorldState, 0L) });
        var seen = new HashSet<WorldState>();
        //var shortestPath = int.MaxValue;
        var finalWorlds = new HashSet<WorldState>();
        bestGeodeCount = 0;

        while (explore.Count > 0)
        {
            var world = explore.Dequeue();

            if (world.MinuteNumber == world.Blueprint.FinalMinuteNumber)
            {
                //shortestPath = Math.Min(shortestPath, world.MinuteNumber);
                finalWorlds.Add(world);
                //if (finalWorlds.Count > 1000000)
                //{
                //    break;
                //}
            }
            else if (!seen.Contains(world))
            {
                foreach (var nextWorld in world.GetSuccessors())
                {
                    bestGeodeCount = Math.Max(bestGeodeCount, nextWorld.GeodeCount);
                    if (nextWorld.GeodeCount >= bestGeodeCount - 12 || true) // rs-todo: need to work out how to exclude things that aren't looking good!
                    {
                        //explore.Enqueue(nextWorld, long.MaxValue - nextWorld.GeodeCount);
                        var etc = Math.Max(
                            GetEstimatedCostToCollect(blueprint.GeodeBotOreCost, nextWorld.OreCount, nextWorld.NumOreBots),
                            GetEstimatedCostToCollect(blueprint.GeodeBotObsidianCost, nextWorld.ObsidianCount, nextWorld.NumObsidianBots));
                        explore.Enqueue(nextWorld, etc);
                    }
                }

                seen.Add(world);
            }
        }

        //return new ShortestPathBasic(shortestPath, goalWorlds);

        //Logger($"BP {startingWorldState.Blueprint.Id} -- {goalWorlds.Count(w => w.MinuteNumber == shortestPath)}");
        var bestWorld = finalWorlds.MaxBy(x => x.GeodeCount);

        Logger($"Blueprint {startingWorldState.Blueprint.Id}. Best geode count: {bestWorld?.GeodeCount ?? 0}");

        return bestWorld; // ?? throw new InvalidOperationException("Unexpected: No best world found");

        //return shortestPath == int.MaxValue ? null : shortestPath;

        //return goalWorlds.MinBy(x => x.MinuteNumber)?.MinuteNumber ?? throw new InvalidOperationException($"{nameof(FindShortestPathBasic)} failed");
    }

    ///// <summary>
    ///// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    ///// </summary>
    //static IReadOnlyCollection<(WorldState, IEnumerable<WorldState>)> FindShortestPath(IEnumerable<WorldState> startingWorldStates, /*int finalMinuteNumber) */ Func<WorldState, bool> isGoal)
    //{
    //    var explore = new PriorityQueue<(WorldState, IEnumerable<WorldState>), long>(
    //        startingWorldStates.Select(x => ((x, new [] { x }.AsEnumerable()), (long)x.Blueprint.GetEstimatedCostToCollect(ResourceType.Geode))));//new[] {(startingWorldState, 0L)});
    //    var seen = new HashSet<WorldState>();
    //    var goalWorlds = new List<(WorldState, IEnumerable<WorldState>)>();
    //    //int? goalWorldMinuteNumber = null;
    //    //int currentMinuteNumber = 0;

    //    while (explore.Count > 0 /*&& (goalWorldMinuteNumber == null || goalWorldMinuteNumber == currentMinuteNumber)*/)
    //    {
    //        var (world, worlds) = explore.Dequeue(); // this takes out the top priority node
    //        //currentMinuteNumber = world.MinuteNumber;

    //        // if node is the goal return the path
    //        if (isGoal(world)) //.MinuteNumber == finalMinuteNumber) /*&& (goalWorldMinuteNumber == null || goalWorldMinuteNumber == currentMinuteNumber)*/
    //        {
    //            //return world;
    //            //goalWorldMinuteNumber = world.MinuteNumber;
    //            goalWorlds.Add((world, worlds));
    //        }
    //        // if we've not already seen the node
    //        else if (!seen.Contains(world))
    //        {
    //            foreach (var nextWorld in world.GetSuccessors())
    //            {
    //                // rs-todo: need to exclude successors that are definitely not valid

    //                explore.Enqueue((nextWorld, worlds.Append(nextWorld)), nextWorld.Blueprint.GetEstimatedCostToCollect(ResourceType.Geode));
    //            }

    //            seen.Add(world);
    //        }
    //    }

    //    return goalWorlds;

    //    //throw new InvalidOperationException("No paths found");
    //}

    public enum ResourceType
    {
        Ore,
        Clay,
        Obsidian,
        Geode
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
        public Blueprint Blueprint { get; } = Blueprint;
        //        public WorldState? PreviousState { get; private set; }

        public long GetQualityLevel() => Blueprint.Id * GeodeCount;

        public IEnumerable<WorldState> GetSuccessors(/*ResourceType? priorityResourceType = null*/)
        {
            if (MinuteNumber >= Blueprint.FinalMinuteNumber)
            {
                return Array.Empty<WorldState>();
            }

            // We could spend our resources building bots, one option is to always never build a bot tho
            var successors = new List<WorldState> { new(this) };

            //if (priorityResourceType == null)
            //{
            //    successors.Add(new WorldState(this));
            //}

            // rs-todo: clay count here was a mistake, but it seems to work!?!??!
            if (Blueprint.OreBotOreCost <= OreCount &&
                OreCount < Blueprint.ClayBotOreCost + Blueprint.ObsidianBotOreCost + Blueprint.GeodeBotOreCost)
                 //(OreCount < Blueprint.ClayBotOreCost ||
                 // OreCount < Blueprint.ObsidianBotOreCost ||
                 // OreCount < Blueprint.GeodeBotOreCost))

            // && (OreCount < Blueprint.ClayBotOreCost ||
            // OreCount < Blueprint.ObsidianBotOreCost ||
            // OreCount < Blueprint.GeodeBotOreCost))
            {
                successors.Add(this with
                {
                    NumOreBots = NumOreBots + 1,
                    OreCount = OreCount - Blueprint.OreBotOreCost
                });
            }

            if (Blueprint.ClayBotOreCost <= OreCount &&
                ClayCount < Blueprint.ObsidianBotClayCost)
            {
                successors.Add(this with
                {
                    NumClayBots = NumClayBots + 1,
                    OreCount = OreCount - Blueprint.ClayBotOreCost
                });
            }

            if (Blueprint.ObsidianBotOreCost <= OreCount &&
                Blueprint.ObsidianBotClayCost <= ClayCount &&
                ObsidianCount < Blueprint.GeodeBotObsidianCost)
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

            //// If we haven't built any bots in this iteration, then just use our self as the next successor
            //if (successors.Count == 0)
            //{
            //    successors.Add(new WorldState(this));
            //}

            // Then, given our number of bots, we can then collect our resources (increment the world's minute number)
            var nextMinuteNumber = MinuteNumber + 1;
            foreach (var successor in successors)
            {
                successor.MinuteNumber = nextMinuteNumber;
                successor.OreCount += NumOreBots;
                successor.ClayCount += NumClayBots;
                successor.ObsidianCount += NumObsidianBots;
                successor.GeodeCount += NumGeodeBots;
                //successor.PreviousState = this;
            }

            // Our new bots then become ready (we don't have to do anything tho, we created them above, but used our current bots to collect new resources)

            return successors.Where(x => x.IsValidSuccessor());
        }

        private bool IsValidSuccessor()
        {
            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneGeodeBot && NumGeodeBots == 0)
            {
                return false;
            }

            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneObsidianBot && NumObsidianBots == 0)
            {
                return false;
            }

            if (MinuteNumber >= Blueprint.CutOffForAtLeastOneClayBot && NumClayBots == 0)
            {
                return false;
            }

            return true;
        }

        public string GetSnapshot()
        {
            return
                $"Minute {MinuteNumber}: OreBots: {NumOreBots}, ClayBots: {NumClayBots}, ObsBots: {NumObsidianBots}, GeoBot: {NumGeodeBots} -- Resources: Ore: {OreCount}, Clay: {ClayCount}, Obs: {ObsidianCount}, Geo: {GeodeCount}";
        }

        public static string GetTimeline(IEnumerable<WorldState> worlds)
        {
            //return "";

            return string.Join(Environment.NewLine, worlds.Select(s => s.GetSnapshot()));
        }
    }

    public sealed class Blueprint
    {
        public Blueprint(
            int id,
            int finalMinuteNumber,
            int oreBotOreCost,
            int clayBotOreCost,
            int obsidianBotOreCost,
            int obsidianBotClayCost,
            int geodeBotOreCost,
            int geodeBotObsidianCost)
        {
            Id = id;
            FinalMinuteNumber = finalMinuteNumber;
            OreBotOreCost = oreBotOreCost;
            ClayBotOreCost = clayBotOreCost;
            ObsidianBotOreCost = obsidianBotOreCost;
            ObsidianBotClayCost = obsidianBotClayCost;
            GeodeBotOreCost = geodeBotOreCost;
            GeodeBotObsidianCost = geodeBotObsidianCost;

            //QuickestTimeToCreateOreBot = OreBotOreCost + 1;
            //QuickestTimeToCreateClayBot = ClayBotOreCost + 1;

            TimeToCreateOreBot = OreBotOreCost + 1; // Note we cannot create one quicker than this
            TimeToCreateClayBot = ClayBotOreCost + 1; // Note we cannot create one quicker than this

            SlowestTimeToCreateObsidianBot = ObsidianBotClayCost + 1 + TimeToCreateClayBot;
            SlowestTimeToCreateGeodeBot = GeodeBotObsidianCost + 1 + SlowestTimeToCreateObsidianBot;

            CutOffForAtLeastOneGeodeBot = finalMinuteNumber - 1;
            CutOffForAtLeastOneObsidianBot = CutOffForAtLeastOneGeodeBot - 2;
            CutOffForAtLeastOneClayBot = CutOffForAtLeastOneObsidianBot - 2;

            //var myQuickTest = FindShortestPathBasic(new WorldState(this), w => w.NumOreBots == 2);
            //if (myQuickTest != TimeToCreateOreBot)
            //{
            //    //throw new InvalidOperationException("oh no!");
            //    throw new InvalidOperationException($"oh no! {id} Ore bot: {myQuickTest} != {TimeToCreateOreBot}");
            //}

            //var myQuickTest2 = FindShortestPathBasic(new WorldState(this), w => w.NumClayBots == 1);
            //if (myQuickTest2 != TimeToCreateClayBot)
            //{
            //    throw new InvalidOperationException($"oh no! {id} Clay bot: {myQuickTest2} != {TimeToCreateClayBot}");
            //}

            //QuickestTimeToCreateObsidianBot = FindShortestPathBasic(new WorldState(this), w => w.NumObsidianBots == 1);
            //QuickestTimeToCreateGeodeBot = FindShortestPathBasic(new WorldState(this), w => w.NumGeodeBots == 1);

            //Logger($"BP {Id}, QuickestTimeToCreateGeodeBot: {QuickestTimeToCreateGeodeBot}, QuickestTimeToCreateObsidianBot: {QuickestTimeToCreateObsidianBot}");

            

            //QuickestTimeToCreateGeodeBot = rs-todo
        }

        public int Id { get; }
        public int FinalMinuteNumber { get; }
        public int OreBotOreCost { get; }
        public int ClayBotOreCost { get; }
        public int ObsidianBotOreCost { get; }
        public int ObsidianBotClayCost { get; }
        public int GeodeBotOreCost { get; }
        public int GeodeBotObsidianCost { get; }
        public int TimeToCreateOreBot { get; }
        public int TimeToCreateClayBot { get; }
        public int SlowestTimeToCreateObsidianBot { get; }
        public int SlowestTimeToCreateGeodeBot { get; }
        public int? QuickestTimeToCreateObsidianBot { get; }
        public int? QuickestTimeToCreateGeodeBot { get; }

        public int CutOffForAtLeastOneClayBot { get; }
        public int CutOffForAtLeastOneObsidianBot { get; }
        public int CutOffForAtLeastOneGeodeBot { get; }

        /// <summary>
        /// Intentionally an over estimate
        /// </summary>
        public int GetEstimatedCostToCollect(ResourceType resourceType) => resourceType switch
        {
            ResourceType.Ore => 1,
            ResourceType.Clay => GetEstimatedCostToCollect(ResourceType.Ore) * ClayBotOreCost,
            ResourceType.Obsidian => GetEstimatedCostToCollect(ResourceType.Ore) * ObsidianBotOreCost +
                                     GetEstimatedCostToCollect(ResourceType.Clay) * ObsidianBotClayCost,
            ResourceType.Geode => GetEstimatedCostToCollect(ResourceType.Ore) * GeodeBotOreCost +
                                  GetEstimatedCostToCollect(ResourceType.Obsidian) * GeodeBotObsidianCost,
            _ => throw new InvalidOperationException("Invalid resource type: " + resourceType)
        };

        public override bool Equals(object? obj) => obj is Blueprint other && Id == other.Id;

        public override int GetHashCode() => Id;

        public override string ToString() =>
            new {Id, OreBotOreCost, ClayBotOreCost, ObsidianBotOreCost, ObsidianBotClayCost, GeodeBotOreCost, GeodeBotObsidianCost}.ToString()!;
    }

    static IReadOnlyList<Blueprint> ParseBlueprints(string input, int finalMinuteNumber)
    {
        return ParseInputRegex.Matches(input).Select(match => new Blueprint(
            int.Parse(match.Groups["blueprintId"].Value),
            finalMinuteNumber,
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

    //public record WorldState(Blueprint Blueprint)
    ////int MinuteNumber,
    ////int NumOreBots,
    ////int NumClayBots = 0,
    ////int NumObsidianBots = 0,
    ////int NumGeodeBots = 0,
    ////int OreCount = 0,
    ////int ClayCount = 0,
    ////int ObsidianCount = 0,
    ////int GeodeCount = 0)
    //{
    //    public int MinuteNumber { get; private set; }
    //    public int NumOreBots { get; private init; } = 1;
    //    public int NumClayBots { get; private init; }
    //    public int NumObsidianBots { get; private init; }
    //    public int NumGeodeBots { get; private init; }
    //    public int OreCount { get; private set; }
    //    public int ClayCount { get; private set; }
    //    public int ObsidianCount { get; private set; }
    //    public int GeodeCount { get; private set; }
    //    public Blueprint Blueprint { get; } = Blueprint;
    //    //        public WorldState? PreviousState { get; private set; }

    //    public long GetQualityLevel() => Blueprint.Id * GeodeCount;

    //    public IEnumerable<WorldState> GetSuccessors(/*ResourceType? priorityResourceType = null*/)
    //    {
    //        if (MinuteNumber >= Blueprint.FinalMinuteNumber)
    //        {
    //            return Array.Empty<WorldState>();
    //        }

    //        // We could spend our resources building bots, one option is to always never build a bot tho
    //        var successors = new List<WorldState> { new(this) };

    //        //if (priorityResourceType == null)
    //        //{
    //        //    successors.Add(new WorldState(this));
    //        //}

    //        if (Blueprint.OreBotOreCost <= OreCount &&
    //            (ClayCount < Blueprint.ClayBotOreCost ||
    //             ClayCount < Blueprint.ObsidianBotOreCost ||
    //             ClayCount < Blueprint.GeodeBotOreCost))
    //        {
    //            successors.Add(this with
    //            {
    //                NumOreBots = NumOreBots + 1,
    //                OreCount = OreCount - Blueprint.OreBotOreCost
    //            });
    //        }

    //        if (Blueprint.ClayBotOreCost <= OreCount &&
    //            ClayCount < Blueprint.ObsidianBotClayCost)
    //        {
    //            successors.Add(this with
    //            {
    //                NumClayBots = NumClayBots + 1,
    //                OreCount = OreCount - Blueprint.ClayBotOreCost
    //            });
    //        }

    //        if (Blueprint.ObsidianBotOreCost <= OreCount &&
    //            Blueprint.ObsidianBotClayCost <= ClayCount
    //            /*&& ClayCount < Blueprint.GeodeBotObsidianCost*/)
    //        {
    //            successors.Add(this with
    //            {
    //                NumObsidianBots = NumObsidianBots + 1,
    //                OreCount = OreCount - Blueprint.ObsidianBotOreCost,
    //                ClayCount = ClayCount - Blueprint.ObsidianBotClayCost,
    //            });
    //        }

    //        if (Blueprint.GeodeBotOreCost <= OreCount &&
    //            Blueprint.GeodeBotObsidianCost <= ObsidianCount)
    //        {
    //            successors.Add(this with
    //            {
    //                NumGeodeBots = NumGeodeBots + 1,
    //                OreCount = OreCount - Blueprint.GeodeBotOreCost,
    //                ObsidianCount = ObsidianCount - Blueprint.GeodeBotObsidianCost,
    //            });
    //        }

    //        //// If we haven't built any bots in this iteration, then just use our self as the next successor
    //        //if (successors.Count == 0)
    //        //{
    //        //    successors.Add(new WorldState(this));
    //        //}

    //        // Then, given our number of bots, we can then collect our resources (increment the world's minute number)
    //        var nextMinuteNumber = MinuteNumber + 1;
    //        foreach (var successor in successors)
    //        {
    //            successor.MinuteNumber = nextMinuteNumber;
    //            successor.OreCount += NumOreBots;
    //            successor.ClayCount += NumClayBots;
    //            successor.ObsidianCount += NumObsidianBots;
    //            successor.GeodeCount += NumGeodeBots;
    //            //successor.PreviousState = this;
    //        }

    //        // Our new bots then become ready (we don't have to do anything tho, we created them above, but used our current bots to collect new resources)

    //        return successors.Where(x => x.IsValidSuccessor());
    //    }

    //    private bool IsValidSuccessor()
    //    {
    //        if (MinuteNumber >= Blueprint.CutOffForAtLeastOneGeodeBot && NumGeodeBots == 0)
    //        {
    //            return false;
    //        }

    //        if (MinuteNumber >= Blueprint.CutOffForAtLeastOneObsidianBot && NumObsidianBots == 0)
    //        {
    //            return false;
    //        }

    //        if (MinuteNumber >= Blueprint.CutOffForAtLeastOneClayBot && NumClayBots == 0)
    //        {
    //            return false;
    //        }

    //        return true;
    //    }

    //    public string GetSnapshot()
    //    {
    //        return
    //            $"Minute {MinuteNumber}: OreBots: {NumOreBots}, ClayBots: {NumClayBots}, ObsBots: {NumObsidianBots}, GeoBot: {NumGeodeBots} -- Resources: Ore: {OreCount}, Clay: {ClayCount}, Obs: {ObsidianCount}, Geo: {GeodeCount}";
    //    }

    //    public static string GetTimeline(IEnumerable<WorldState> worlds)
    //    {
    //        //return "";

    //        return string.Join(Environment.NewLine, worlds.Select(s => s.GetSnapshot()));
    //    }
    //}
}
