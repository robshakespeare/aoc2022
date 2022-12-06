namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "";

    public override long? SolvePart1(PuzzleInput input)
    {
        var str = input.ToString();

        for (var i = 3; i < str.Length; i++)
        {
            var candidatePosition = i + 4;
            var candidate = str[i..candidatePosition];

            if (candidate.Distinct().Count() == 4)
            {
                return candidatePosition;
            }
        }

        throw new InvalidOperationException("Failed to find start-of-packet marker");
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var str = input.ToString();

        for (var i = 3; i < str.Length; i++)
        {
            var candidatePosition = i + 14;
            var candidate = str[i..candidatePosition];

            if (candidate.Distinct().Count() == 14)
            {
                return candidatePosition;
            }
        }

        throw new InvalidOperationException("Failed to find start-of-message marker");
    }
}
