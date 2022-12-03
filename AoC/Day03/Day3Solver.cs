namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "Rucksack Reorganization";

    static int GetPriority(char c) => char.IsLower(c) ? 1 + c - 'a' : 27 + c - 'A';

    static char GetCommonCharacter(IReadOnlyCollection<string> group) => group
        .SelectMany(sack => sack.Select(c => (c, sack)))
        .GroupBy(x => x.c)
        .First(g => g.Select(x => x.sack).Distinct().Count() == group.Count)
        .Key;

    public override long? SolvePart1(PuzzleInput input) =>
        input.ReadLines()
            .Select(line => new[] {line[..(line.Length / 2)], line[(line.Length / 2)..]})
            .Select(GetCommonCharacter)
            .Sum(GetPriority);

    public override long? SolvePart2(PuzzleInput input) =>
        input.ReadLines()
            .Select((sack, idx) => (sack, idx))
            .GroupBy(x => x.idx / 3)
            .Select(g => g.Select(x => x.sack).ToArray())
            .Select(GetCommonCharacter)
            .Sum(GetPriority);
}
