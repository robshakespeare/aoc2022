using static System.Environment;
using static Crayon.Output;

namespace AoC;

public interface ISolverBase
{
    public string DayName { get; }

    public object? SolvePart1AsObject(PuzzleInput input);

    public object? SolvePart2AsObject(PuzzleInput input);
}

public interface ISolver : ISolver<long?, long?>
{
}

public interface ISolvePart1<out TOutputPart1> : ISolverBase
{
    public TOutputPart1? SolvePart1(PuzzleInput input);
}

public interface ISolvePart2<out TOutputPart2> : ISolverBase
{
    public TOutputPart2? SolvePart2(PuzzleInput input);
}

public interface ISolver<out TOutputPart1, out TOutputPart2> : ISolvePart1<TOutputPart1>, ISolvePart2<TOutputPart2>
{
    object? ISolverBase.SolvePart1AsObject(PuzzleInput input) => SolvePart1(input);

    object? ISolverBase.SolvePart2AsObject(PuzzleInput input) => SolvePart2(input);
}

public interface IVisualize : ISolverBase
{
    public IAsyncEnumerable<string> GetVisualizationAsync(PuzzleInput input);

    public IAsyncEnumerable<string> GetVisualizationAsync() => GetVisualizationAsync(this.GetInputLoader().PuzzleInputPart2);
}

public static class SolverBaseExtensions
{
    private static readonly Dictionary<Type, int> DayNumberCache = new();
    private static readonly Dictionary<Type, InputLoader> InputLoaderCache = new();

    public static int GetDayNumber(this ISolverBase solver) =>
        DayNumberCache.GetOrAdd(solver.GetType(), () => SolverFactory.GetDayNumber(solver));

    public static string GetTitle(this ISolverBase solver) =>
        $"--- Day {solver.GetDayNumber()}{(solver.DayName is null or "" ? "" : ": ")}{solver.DayName} ---";

    internal static InputLoader GetInputLoader(this ISolverBase solver) =>
        InputLoaderCache.GetOrAdd(solver.GetType(), () => new InputLoader(solver));

    public static async Task RunAsync(
        this ISolverBase solver,
        Func<Results, Task>? onUpdated = null)
    {
        Console.WriteLine(Yellow(solver.GetTitle() + NewLine));

        var results = new Result[2];
        results.Initialize();

        async Task Updated() => await (onUpdated?.Invoke(new Results(results[0], results[1])) ?? Task.CompletedTask);

        results[0] = Result.Started();
        await Updated();
        solver.SolvePart1(out results[0]);

        results[1] = Result.Started();
        await Updated();
        solver.SolvePart2(out results[1]);

        await Updated();
    }

    public static TOutputPart1? SolvePart1<TOutputPart1>(this ISolvePart1<TOutputPart1> solver) => (TOutputPart1?)solver.SolvePart1(out _);

    public static TOutputPart2? SolvePart2<TOutputPart2>(this ISolvePart2<TOutputPart2> solver) => (TOutputPart2?)solver.SolvePart2(out _);

    public static object? SolvePart1(
        this ISolverBase solver,
        out Result result) =>
        SolvePartTimed(1, solver.GetInputLoader().PuzzleInputPart1, solver.SolvePart1AsObject, out result);

    public static object? SolvePart2(
        this ISolverBase solver,
        out Result result) =>
        SolvePartTimed(2, solver.GetInputLoader().PuzzleInputPart2, solver.SolvePart2AsObject, out result);

    private static object? SolvePartTimed(int partNum, PuzzleInput input, Func<PuzzleInput, object?> solve, out Result result)
    {
        using var timer = new TimingBlock($"Part {partNum}");
        var solution = solve(input);
        var elapsed = timer.Stop();
        Console.WriteLine($"Part {partNum}:{NewLine}{NewLine}{Green($"{solution}")}{NewLine}");
        if (solution == null)
        {
            Console.WriteLine(Bright.Magenta($"Part {partNum} returned null / is not yet implemented"));
        }

        result = Result.Completed(solution, elapsed);
        return solution;
    }
}

public readonly record struct Result(object? Value, TimeSpan? Elapsed, bool IsStarted, bool IsCompleted)
{
    public static Result Started() => default(Result) with { IsStarted = true };

    public static Result Completed(object? value, TimeSpan? elapsed) => new(value, elapsed, true, true);

    public bool IsRunning => IsStarted && !IsCompleted;

    public double ElapsedTotalSeconds => Elapsed?.TotalSeconds ?? 0;

    public bool IsCompletedWithValue => IsCompleted && Value != null;

    public bool IsCompletedWithoutValue => IsCompleted && Value == null;
}

public readonly record struct Results(Result Part1Result, Result Part2Result);
