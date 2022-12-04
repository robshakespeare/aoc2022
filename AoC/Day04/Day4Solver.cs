namespace AoC.Day04;

public class Day4Solver : SolverBase
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

    static IEnumerable<(Section Section1, Section Section2)> ParseSectionPairs(PuzzleInput input) =>
        input.ReadLines().Select(line => line.Split('-', ',')).Select(parts => (
            new Section(int.Parse(parts[0]), int.Parse(parts[1])),
            new Section(int.Parse(parts[2]), int.Parse(parts[3]))));
}
