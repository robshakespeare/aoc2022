namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "Tuning Trouble";

    public override long? SolvePart1(PuzzleInput input) => FindMarkerInDataStream(input, 4, "start-of-packet");

    public override long? SolvePart2(PuzzleInput input) => FindMarkerInDataStream(input, 14, "start-of-message");

    static int FindMarkerInDataStream(string dataStream, int markerLength, string markerName)
    {
        for (var i = 0; i < dataStream.Length; i++)
        {
            var candidatePosition = i + markerLength;
            var candidate = dataStream[i..candidatePosition];

            if (candidate.Distinct().Count() == markerLength)
            {
                return candidatePosition;
            }
        }

        throw new InvalidOperationException($"Failed to find {markerName} marker");
    }
}
