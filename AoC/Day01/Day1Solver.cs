using static System.Environment;

namespace AoC.Day01;

public class Day1Solver : SolverBase
{
    public override string DayName => "Calorie Counting";

    private static IEnumerable<long> GetEachElfTotalCalories(PuzzleInput input) =>
        input.ToString().Split(NewLine + NewLine)
            .Select(elfInventory => elfInventory.Split(NewLine).Select(long.Parse))
            .Select(elfCalories => elfCalories.Sum());

    public override long? SolvePart1(PuzzleInput input) => GetEachElfTotalCalories(input).Max();

    public override long? SolvePart2(PuzzleInput input) => GetEachElfTotalCalories(input).OrderDescending().Take(3).Sum();
}
