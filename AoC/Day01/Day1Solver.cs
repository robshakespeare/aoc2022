using static System.Environment;

namespace AoC.Day01;

public class Day1Solver : SolverBase
{
    public override string DayName => "Calorie Counting";

    public override long? SolvePart1(PuzzleInput input)
    {
        var elves = input.ToString()
            .Split($"{NewLine}{NewLine}")
            .Select(elf => elf.Split(NewLine).Select(long.Parse))
            .ToArray();

        return elves.Select(elf => elf.Sum()).Max();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var elves = input.ToString()
            .Split($"{NewLine}{NewLine}")
            .Select(elf => elf.Split(NewLine).Select(long.Parse))
            .ToArray();

        return elves.Select(elf => elf.Sum())
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
    }
}
