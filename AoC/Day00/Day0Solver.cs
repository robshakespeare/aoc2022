namespace AoC.Day00;

public class Day0Solver : ISolver<long, string>
{
    public string DayName => "Test Day";

    public bool SimulateSlowness { get; set; } = true;

    private readonly Random _random = new();

    private void SimulateLongRunning(int minWaitMilliseconds)
    {
        if (SimulateSlowness)
        {
            Thread.Sleep(minWaitMilliseconds + _random.Next(1, 200));
        }
    }

    public long SolvePart1(PuzzleInput input)
    {
        SimulateLongRunning(800); // Simulate a short-ish task, to test wait handling / spinners

        var numbers = input.ReadLinesAsLongs().ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1]);
    }

    public string SolvePart2(PuzzleInput input)
    {
        SimulateLongRunning(1700); // Simulate a long-ish task, to test wait handling / spinners

        var numbers = input.ToString().Split(',').Select(long.Parse).ToArray();
        var result = MathUtils.LeastCommonMultiple(numbers[0], numbers[1], numbers[2]);

        return $"""
            Hello World!
            This tests that sometimes the answer can be just text.
            The specific answer was: {result}
            """;
    }
}
