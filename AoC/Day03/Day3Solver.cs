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
            .Sum(GetPriority);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        char GetGroupBadge(IEnumerable<string> group)
        {
            var dict = new Dictionary<char, List<string>>();

            foreach (var sack in group)
            {
                foreach (var c in sack.Distinct())
                {
                    if (!dict.ContainsKey(c))
                    {
                        dict[c] = new List<string>();
                    }

                    dict[c].Add(sack);
                }
            }

            return dict.Single(x => x.Value.Count == 3).Key;
        }

        return input.ReadLines()
            .Select((line, i) => (line, i))
            .GroupBy(x => x.i / 3)
            .Select(g => g.Select(sack => sack.line))
            .Select(GetGroupBadge)
            .Sum(GetPriority);
    }
}
