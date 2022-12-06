namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "Tuning Trouble";

    public override long? SolvePart1(PuzzleInput input) => FindMarkerInDataStream(input, 4);

    public override long? SolvePart2(PuzzleInput input) => FindMarkerInDataStream(input, 14);

    static int FindMarkerInDataStream(string dataStream, int markerLength) => Enumerable
        .Range(markerLength, dataStream.Length - markerLength)
        .First(candidatePosition => dataStream[(candidatePosition - markerLength)..candidatePosition].Distinct().Count() == markerLength);
}
