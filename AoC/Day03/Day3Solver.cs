using System.Linq;

namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "";

    private static int GetPriority(char c) => char.IsLower(c) ? 1 + c - 'a' : 27 + c - 'A';

    public override long? SolvePart1(PuzzleInput input)
    {
        return input.ReadLines()
            .Select(line => new
            {
                compartment1 = line[..(line.Length / 2)],
                compartment2 = line[(line.Length / 2)..]
            })
            .Select(compartments => compartments.compartment1.Join(compartments.compartment2, c => c, c => c, (c, _) => c).First())
            //.Select(line => line.GroupBy(c => c).First(g => g.Count() == 2).Key)
            .Sum(GetPriority);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }
}
