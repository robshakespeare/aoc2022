using static System.Environment;

namespace AoC.Day01;

public class Day1Solver : ISolver
{
    public string DayName => "Calorie Counting";

    private static IEnumerable<long> GetEachElfTotalCalories(PuzzleInput input) =>
        input.ToString().Split(NewLine + NewLine)
            .Select(elfInventory => elfInventory.Split(NewLine).Select(long.Parse))
            .Select(elfCalories => elfCalories.Sum());

    public long? SolvePart1(PuzzleInput input) => GetEachElfTotalCalories(input).Max();

    public long? SolvePart2(PuzzleInput input) => GetEachElfTotalCalories(input).OrderDescending().Take(3).Sum();
}
