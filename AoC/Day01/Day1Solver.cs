using static System.Environment;

namespace AoC.Day01;

public class Day1Solver : SolverBase
{
    public override string DayName => "Calorie Counting";

    private static IEnumerable<long> ParseElves(PuzzleInput input) =>
        input.ToString()
            .Split(NewLine + NewLine)
            .Select(block => block.Split(NewLine).Select(long.Parse))
            .Select(elfCalories => elfCalories.Sum());

    public override long? SolvePart1(PuzzleInput input) => ParseElves(input).Max(totalCalories => totalCalories);

    public override long? SolvePart2(PuzzleInput input) => ParseElves(input).OrderByDescending(totalCalories => totalCalories).Take(3).Sum();
}
