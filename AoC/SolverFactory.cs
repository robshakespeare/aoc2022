namespace AoC;

public class SolverFactory
{
    private SolverFactory()
    {
        AddSolver<Day00.Day0Solver>();
        AddSolver<Day01.Day1Solver>();
        AddSolver<Day02.Day2Solver>();
        AddSolver<Day03.Day3Solver>();
        AddSolver<Day04.Day4Solver>();
        AddSolver<Day05.Day5Solver>();
        AddSolver<Day06.Day6Solver>();
        AddSolver<Day07.Day7Solver>();
        AddSolver<Day08.Day8Solver>();
        AddSolver<Day09.Day9Solver>();
        AddSolver<Day10.Day10Solver>();
        AddSolver<Day11.Day11Solver>();
        AddSolver<Day12.Day12Solver>();
        AddSolver<Day13.Day13Solver>();
        AddSolver<Day14.Day14Solver>();
        AddSolver<Day15.Day15Solver>();
        AddSolver<Day16.Day16Solver>();
        AddSolver<Day17.Day17Solver>();
        AddSolver<Day18.Day18Solver>();
        AddSolver<Day19.Day19Solver>();
        AddSolver<Day20.Day20Solver>();
        AddSolver<Day21.Day21Solver>();
        AddSolver<Day22.Day22Solver>();
        AddSolver<Day23.Day23Solver>();
        AddSolver<Day24.Day24Solver>();
        AddSolver<Day25.Day25Solver>();
    }

    private static readonly Lazy<SolverFactory> LazyInstance = new(() => new SolverFactory());

    public static SolverFactory Instance => LazyInstance.Value;

    private readonly Dictionary<string, Type> _solvers = new();

    public string DefaultDay => GetDefaultDay(DateTime.Now).ToString();

    internal static int GetDefaultDay(DateTime date) =>
        date.Month switch
        {
            1 or 2 => 25,
            12 => Math.Min(date.Day, 25),
            _ => 1
        };

    public ISolver? TryCreateSolver(string? dayNumber) => _solvers.TryGetValue((dayNumber ?? ""), out var solverType)
        ? (ISolver?) Activator.CreateInstance(solverType)
        : null;

    private void AddSolver<TSolver>() where TSolver : ISolver => _solvers.Add(GetDayNumber(typeof(TSolver)).ToString(), typeof(TSolver));

    private static readonly Regex DayNumRegex = new(@"Day(?<dayNum>\d+)", RegexOptions.Compiled);

    private static int GetDayNumber(Type solverType)
    {
        var fullName = solverType.FullName;
        Match match;

        if (fullName != null &&
            (match = DayNumRegex.Match(fullName)).Success &&
            match.Groups["dayNum"].Success)
        {
            return int.Parse(match.Groups["dayNum"].Value);
        }

        throw new InvalidOperationException("Unable to get day number from type name: " + fullName);
    }

    internal static int GetDayNumber(ISolver solver) => GetDayNumber(solver.GetType());
}
