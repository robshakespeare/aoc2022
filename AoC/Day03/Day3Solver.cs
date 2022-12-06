namespace AoC.Day03;

public class Day3Solver : ISolver
{
    public string DayName => "Rucksack Reorganization";

    static int GetPriority(char c) => (c - '&') % 58;

    static char GetCommonCharacter(IReadOnlyCollection<string> group) => group
        .Aggregate((agg, cur) => string.Concat(agg.Intersect(cur)))
        .Single();

    public long? SolvePart1(PuzzleInput input) =>
        input.ReadLines()
            .Select(sack => new[] {sack[..(sack.Length / 2)], sack[(sack.Length / 2)..]})
            .Select(GetCommonCharacter)
            .Sum(GetPriority);

    public long? SolvePart2(PuzzleInput input) =>
        input.ReadLines()
            .Select((sack, idx) => (sack, idx))
            .GroupBy(x => x.idx / 3)
            .Select(g => g.Select(x => x.sack).ToArray())
            .Select(GetCommonCharacter)
            .Sum(GetPriority);
}
