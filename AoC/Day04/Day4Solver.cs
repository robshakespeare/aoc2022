namespace AoC.Day04;

public partial class Day4Solver : SolverBase
{
    public override string DayName => "Camp Cleanup";

    public override long? SolvePart1(PuzzleInput input) =>
        ParseSectionPairs(input).Count(pair => pair.Section1.Contains(pair.Section2) || pair.Section2.Contains(pair.Section1));

    public override long? SolvePart2(PuzzleInput input) =>
        ParseSectionPairs(input).Count(pair => pair.Section1.Intersects(pair.Section2) || pair.Section2.Intersects(pair.Section1));

    record Section(int Start, int End)
    {
        public bool Contains(Section other) => Start <= other.Start && End >= other.End;

        public bool Intersects(Section other) => Start >= other.Start && Start <= other.End || End >= other.Start && End <= other.End;
    }

    record SectionPair(Section Section1, Section Section2);

    [GeneratedRegex(@"(?<start1>\d+)-(?<end1>\d+),(?<start2>\d+)-(?<end2>\d+)", RegexOptions.Compiled)]
    private static partial Regex BuildParsePairsRegex();

    private static readonly Regex ParsePairsRegex = BuildParsePairsRegex();

    static IEnumerable<SectionPair> ParseSectionPairs(PuzzleInput input) =>
        ParsePairsRegex.Matches(input.ToString()).Select(match => new SectionPair(
            new Section(int.Parse(match.Groups["start1"].Value), int.Parse(match.Groups["end1"].Value)),
            new Section(int.Parse(match.Groups["start2"].Value), int.Parse(match.Groups["end2"].Value))));
}
