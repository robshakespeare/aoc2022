using static System.Environment;
using static Crayon.Output;

namespace AoC;

public interface ISolver
{
    int DayNumber { get; }

    string DayName { get; }

    string Title => $"--- Day {DayNumber}{(DayName is null or "" ? "" : ": ")}{DayName} ---";

    Task RunAsync(Func<Task>? onUpdated = null);

    Result Part1Result { get; }

    Result Part2Result { get; }
}

public abstract class SolverBase : SolverBase<long?, long?>
{
}

public abstract class SolverBase<TOutputPart1, TOutputPart2> : ISolver
{
    private readonly InputLoader _inputLoader;
    private readonly Result[] _results = new Result[2];

    public int DayNumber { get; }

    public abstract string DayName { get; }

    public Result Part1Result => _results[0];

    public Result Part2Result => _results[1];

    protected SolverBase()
    {
        DayNumber = SolverFactory.GetDayNumber(this);
        _inputLoader = new InputLoader(this);
    }

    public async Task RunAsync(Func<Task>? onUpdated = null)
    {
        async Task Updated() => await (onUpdated?.Invoke() ?? Task.CompletedTask);

        Console.WriteLine(Yellow($"{((ISolver) this).Title}{NewLine}"));

        _results.Initialize();

        _results[0] = Result.Started();
        await Updated();
        SolvePart1();

        _results[1] = Result.Started();
        await Updated();
        SolvePart2();

        await Updated();
    }

    private TOutput? SolvePartTimed<TOutput>(int partNum, PuzzleInput input, Func<PuzzleInput, TOutput?> solve)
    {
        using var timer = new TimingBlock($"Part {partNum}");
        var result = solve(input);
        var elapsed = timer.Stop();
        var resultFormatted = result is string ? $"{NewLine}{NewLine}{result}{NewLine}" : result?.ToString();
        Console.WriteLine($"Part {partNum}: {Green(resultFormatted ?? "")}");
        if (result == null)
        {
            Console.WriteLine(Bright.Magenta($"Part {partNum} returned null / is not yet implemented"));
        }
        _results[partNum - 1] = Result.Completed(result, elapsed);
        return result;
    }

    public TOutputPart1? SolvePart1() => SolvePartTimed(1, _inputLoader.PuzzleInputPart1, SolvePart1);

    public TOutputPart2? SolvePart2() => SolvePartTimed(2, _inputLoader.PuzzleInputPart2, SolvePart2);

    public abstract TOutputPart1? SolvePart1(PuzzleInput input);

    public abstract TOutputPart2? SolvePart2(PuzzleInput input);
}

public readonly record struct Result(object? Value, TimeSpan? Elapsed, bool IsStarted, bool IsCompleted)
{
    public static Result Started() => default(Result) with { IsStarted = true };

    public static Result Completed(object? value, TimeSpan? elapsed) => new(value, elapsed, true, true);
}
