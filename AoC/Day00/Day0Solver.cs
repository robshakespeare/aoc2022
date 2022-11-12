using System.Runtime.InteropServices;

namespace AoC.Day00;

public class Day0Solver : SolverBase
{
    public override string DayName => "Test Day";

    private static bool IsWebAssembly => RuntimeInformation.RuntimeIdentifier.Contains("browser", StringComparison.OrdinalIgnoreCase) ||
                                         RuntimeInformation.RuntimeIdentifier.Contains("wasm", StringComparison.OrdinalIgnoreCase);

    public override long? SolvePart1(PuzzleInput input)
    {
        if (IsWebAssembly) // Simulate a long-ish task, to test the async wait handling when running in Blazor 
        {
            var i = 0;
            for (; i < 19999999; i++) { }
            return i + 2739581;
        }

        var numbers = input.ReadLinesAsLongs().ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1]);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        if (IsWebAssembly)
        {
            var i = 0;
            for (; i < 29999999; i++) { }
            return i + 364682626;
        }

        var numbers = input.ToString().Split(',').Select(long.Parse).ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1], numbers[2]);
    }
}
