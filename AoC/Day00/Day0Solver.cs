namespace AoC.Day00;

public class Day0Solver : SolverBase
{
    public override string DayName => "Test Day";

    private readonly Random _random = new();

    public override long? SolvePart1(PuzzleInput input)
    {
        Thread.Sleep(800 + _random.Next(1, 200)); // Simulate a short-ish task, to test wait handling / spinners

        var numbers = input.ReadLinesAsLongs().ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1]);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        Thread.Sleep(1600 + _random.Next(1, 400)); // Simulate a long-ish task, to test wait handling / spinners

        var numbers = input.ToString().Split(',').Select(long.Parse).ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1], numbers[2]);
    }
}
