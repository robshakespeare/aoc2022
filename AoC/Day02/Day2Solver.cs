namespace AoC.Day02;

public class Day2Solver : SolverBase
{
    //private const char TheirRock = 'A';
    //private const char TheirPaper = 'B';
    //private const char TheirScissors = 'C';

    //private const char OurRock = 'X';
    //private const char OurPaper = 'Y';
    //private const char OurScissors = 'Z';

    public override string DayName => "Rock Paper Scissors";

    public enum Shape
    {
        Rock,
        Paper,
        Scissors
    }

    //private enum Outcome
    //{
    //    Draw,
    //    Win,
    //    Loss
    //}

    //private static Lookup<Outcome2, Round3> Test = new Lookup<Outcome2,Round3>()

    /// <summary>
    /// A winner for a round is selected:
    ///  * Rock defeats Scissors
    ///  * Paper defeats Rock
    ///  * Scissors defeats Paper
    ///  * If both players choose the same shape, the round instead ends in a draw.
    /// </summary>
    private static readonly IReadOnlyDictionary<Shape, Shape> ShapeWinners = new Dictionary<Shape, Shape>
    {
        {Shape.Rock, Shape.Scissors},
        {Shape.Paper, Shape.Rock},
        {Shape.Scissors, Shape.Paper}
    };

    private static readonly IReadOnlyDictionary<Shape, Shape> ShapeLosers = ShapeWinners.ToDictionary(x => x.Value, x => x.Key);

    /// <summary>
    /// Score for the shape you selected: 1 for Rock, 2 for Paper, and 3 for Scissors
    /// </summary>
    private static readonly IReadOnlyDictionary<Shape, long> ShapeScores = new Dictionary<Shape, long>
    {
        {Shape.Rock, 1},
        {Shape.Paper, 2},
        {Shape.Scissors, 3}
    };

    /// <summary>
    /// Score for the outcome of the round: 0 if you lost, 3 if the round was a draw, and 6 if you won
    /// </summary>
    public enum OutcomeScore
    {
        Loss = 0,
        Draw = 3,
        Win = 6
    }

    /// <summary>
    /// First Column - Opponent: A for Rock, B for Paper, and C for Scissors
    /// </summary>
    private static readonly IReadOnlyDictionary<char, Shape> TheirLetterMap = new Dictionary<char, Shape>
    {
        {'A', Shape.Rock},
        {'B', Shape.Paper},
        {'C', Shape.Scissors}
    };

    /// <summary>
    /// Second Column - Us: X for Rock, Y for Paper, and Z for Scissors
    /// </summary>
    private static readonly IReadOnlyDictionary<char, Shape> OurLetterMap = new Dictionary<char, Shape>
    {
        {'X', Shape.Rock},
        {'Y', Shape.Paper},
        {'Z', Shape.Scissors}
    };

    public readonly record struct Round3(char TheirLetter, char OurLetter, Shape TheirShape, Shape OurShape)
    {
        public Round3(char theirLetter, char ourLetter) : this(theirLetter, ourLetter, TheirLetterMap[theirLetter], OurLetterMap[ourLetter])
        {
        }

        public long GetOurScore() => ShapeScores[OurShape] + (long) GetOurOutcomeScore();

        private OutcomeScore GetOurOutcomeScore() =>
            ShapeWinners[OurShape] == TheirShape
                ? OutcomeScore.Win
                : ShapeWinners[TheirShape] == OurShape
                    ? OutcomeScore.Loss
                    : OutcomeScore.Draw;
    }

    //private const long LossScore = 0;
    //private const long DrawScore = 3;
    //private const long WinScore = 6;



    //private readonly record struct Round2(char TheirLetter, char OurLetter);

    //private readonly record struct Outcome2(long Score);

    //private static readonly Outcome2 Win = new(6);
    //private static readonly Outcome2 Draw = new(3);
    //private static readonly Outcome2 Loss = new(0);

    

    private static IEnumerable<Round3> ParseRounds3(PuzzleInput input) =>
        input.ReadLines().Select(line => new Round3(line[0], line[2]));

    public override long? SolvePart1(PuzzleInput input) => ParseRounds3(input).Sum(round => round.GetOurScore());

    public override long? SolvePart2(PuzzleInput input) =>
        ParseRounds3(input)
            // X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win
            .Select(round => round with {OurShape = round.OurLetter switch
            {
                'X' => ShapeWinners[round.TheirShape],
                'Y' => round.TheirShape,
                'Z' => ShapeLosers[round.TheirShape],
                _ => throw new InvalidOperationException("Unexpected letter " + round.OurLetter)
            }})
            .Sum(round => round.GetOurScore());

    //public readonly record struct Round(char OpponentShape, char OurShape);

    //private static IReadOnlyCollection<Round> ParseRounds(PuzzleInput input) =>
    //    input.ReadLines().Select(line => new Round(line[0], line[2])).ToReadOnlyArray();

    //private static long GetOurShapeScore(Round round) => round.OurShape switch
    //{
    //    OurRock => 1, // Rock
    //    OurPaper => 2, // Paper
    //    OurScissors => 3, // Scissors
    //    _ => throw new InvalidOperationException("Invalid OurShape: " + round.OurShape)
    //};

    //private static long GetOurOutcomeScore(Round round)
    //{
    //    var outcome = GetOutcome(round.OpponentShape, round.OurShape);
    //    return outcome switch
    //    {
    //        Outcome.Loss => 0,
    //        Outcome.Draw => 3,
    //        Outcome.Win => 6,
    //        _ => throw new InvalidOperationException("Invalid outcome: " + outcome)
    //    };
    //}

    ///// <summary>
    ///// A winner for a round is selected:
    /////  * Rock defeats Scissors
    /////  * Scissors defeats Paper
    /////  * Paper defeats Rock
    /////  * If both players choose the same shape, the round instead ends in a draw.
    ///// </summary>
    //private static Outcome GetOutcome(char theirShape, char ourShape)
    //{
    //    switch (theirShape)
    //    {
    //        case TheirRock when ourShape == OurScissors:
    //        case TheirScissors when ourShape == OurPaper:
    //        case TheirPaper when ourShape == OurRock:
    //            return Outcome.Loss;
    //    }


    //    switch (ourShape)
    //    {
    //        case OurRock when theirShape == TheirScissors:
    //        case OurScissors when theirShape == TheirPaper:
    //        case OurPaper when theirShape == TheirRock:
    //            return Outcome.Win;
    //        default:
    //            return Outcome.Draw;
    //    }
    //}
}
