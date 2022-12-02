namespace AoC.Day02;

/*
 * Column 0 - Opponent: A for Rock, B for Paper, and C for Scissors
 * Column 1 - Us: X for Rock, Y for Paper, and Z for Scissors
 *
 * 1 for Rock, 2 for Paper, and 3 for Scissors
 * 0 if you lost, 3 if the round was a draw, and 6 if you won
 */

public class Day2Solver : SolverBase
{
    private const char TheirRock = 'A';
    private const char TheirPaper = 'B';
    private const char TheirScissors = 'C';

    private const char OurRock = 'X';
    private const char OurPaper = 'Y';
    private const char OurScissors = 'Z';

    public override string DayName => "Rock Paper Scissors";

    public override long? SolvePart1(PuzzleInput input)
    {
        return ParseRounds(input)
            .Select(round => GetOurShapeScore(round) + GetOurOutcomeScore(round))
            .Sum();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public readonly record struct Round(char OpponentShape, char OurShape);

    private static IReadOnlyCollection<Round> ParseRounds(PuzzleInput input) =>
        input.ReadLines().Select(line => new Round(line[0], line[2])).ToReadOnlyArray();

    private static long GetOurShapeScore(Round round) => round.OurShape switch
    {
        OurRock => 1, // Rock
        OurPaper => 2, // Paper
        OurScissors => 3, // Scissors
        _ => throw new InvalidOperationException("Invalid OurShape: " + round.OurShape)
    };

    private static long GetOurOutcomeScore(Round round)
    {
        var outcome = GetOutcome(round.OpponentShape, round.OurShape);
        return outcome switch
        {
            Outcome.Loss => 0,
            Outcome.Draw => 3,
            Outcome.Win => 6,
            _ => throw new InvalidOperationException("Invalid outcome: " + outcome)
        };
    }

    /// <summary>
    /// A winner for a round is selected:
    ///  * Rock defeats Scissors
    ///  * Scissors defeats Paper
    ///  * Paper defeats Rock
    ///  * If both players choose the same shape, the round instead ends in a draw.
    /// </summary>
    private static Outcome GetOutcome(char theirScore, char ourScore)
    {
        if (theirScore == TheirRock && ourScore == OurScissors)
        {
            return Outcome.Loss;
        }

        if (theirScore == TheirScissors && ourScore == OurPaper)
        {
            return Outcome.Loss;
        }

        if (theirScore == TheirPaper && ourScore == OurRock)
        {
            return Outcome.Loss;
        }


        if (ourScore == OurRock && theirScore == TheirScissors)
        {
            return Outcome.Win;
        }

        if (ourScore == OurScissors && theirScore == TheirPaper)
        {
            return Outcome.Win;
        }

        if (ourScore == OurPaper && theirScore == TheirRock)
        {
            return Outcome.Win;
        }


        return Outcome.Draw;
    }

    private enum Outcome
    {
        Draw,
        Win,
        Loss
    }
}
