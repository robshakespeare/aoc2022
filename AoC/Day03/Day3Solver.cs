using System.Collections.Concurrent;

namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "Rucksack Reorganization";

    static int GetPriority(char c) => char.IsLower(c) ? 1 + c - 'a' : 27 + c - 'A';

    static char GetCommonCharacter(IReadOnlyCollection<string> group)
    {
        var sacksByChar = new ConcurrentDictionary<char, List<string>>();

        foreach (var sack in group)
        {
            foreach (var c in sack.Distinct())
            {
                sacksByChar.GetOrAdd(c, _ => new List<string>()).Add(sack);
            }
        }

        return sacksByChar.Single(x => x.Value.Count == group.Count).Key;
    }

    public override long? SolvePart1(PuzzleInput input) =>
        input.ReadLines()
            .Select(line => new[] { line[..(line.Length / 2)], line[(line.Length / 2)..] })
            .Select(GetCommonCharacter)
            .Sum(GetPriority);

    public override long? SolvePart2(PuzzleInput input) =>
        input.ReadLines()
            .Select((line, i) => (line, i))
            .GroupBy(x => x.i / 3)
            .Select(g => g.Select(sack => sack.line).ToArray())
            .Select(GetCommonCharacter)
            .Sum(GetPriority);
}
