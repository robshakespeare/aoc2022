namespace AoC.Day02;

public class Day2Solver : ISolver
{
    public string DayName => "Rock Paper Scissors";

    // Possible shapes, and the score for each: 1 for Rock, 2 for Paper, and 3 for Scissors
    enum Shape
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    // Rock defeats Scissors, Paper defeats Rock, Scissors defeats Paper
    static readonly IReadOnlyDictionary<Shape, Shape> ShapeWinners = new Dictionary<Shape, Shape>
    {
        {Shape.Rock, Shape.Scissors},
        {Shape.Paper, Shape.Rock},
        {Shape.Scissors, Shape.Paper}
    };

    static readonly IReadOnlyDictionary<Shape, Shape> ShapeLosers = ShapeWinners.ToDictionary(x => x.Value, x => x.Key);

    // Score for the outcome of the round: 0 if you lost, 3 if the round was a draw, and 6 if you won
    enum OutcomeScore
    {
        Loss = 0,
        Draw = 3,
        Win = 6
    }

    // First Column - Opponent: A for Rock, B for Paper, and C for Scissors
    static readonly IReadOnlyDictionary<char, Shape> TheirLetterMap = new Dictionary<char, Shape>
    {
        {'A', Shape.Rock},
        {'B', Shape.Paper},
        {'C', Shape.Scissors}
    };

    // Second Column - Us: X for Rock, Y for Paper, and Z for Scissors
    static readonly IReadOnlyDictionary<char, Shape> OurLetterMap = new Dictionary<char, Shape>
    {
        {'X', Shape.Rock},
        {'Y', Shape.Paper},
        {'Z', Shape.Scissors}
    };

    record Round(char OurLetter, Shape TheirShape, Shape OurShape)
    {
        public Round(char theirLetter, char ourLetter) : this(ourLetter, TheirLetterMap[theirLetter], OurLetterMap[ourLetter])
        {
        }

        public long GetOurScore() => (long) OurShape + (long) GetOurOutcomeScore();

        OutcomeScore GetOurOutcomeScore() =>
            ShapeWinners[OurShape] == TheirShape
                ? OutcomeScore.Win
                : ShapeWinners[TheirShape] == OurShape
                    ? OutcomeScore.Loss
                    : OutcomeScore.Draw;
    }

    static IEnumerable<Round> ParseRounds(PuzzleInput input) => input.ReadLines().Select(line => new Round(line[0], line[2]));

    public long? SolvePart1(PuzzleInput input) => ParseRounds(input).Sum(round => round.GetOurScore());

    public long? SolvePart2(PuzzleInput input) => ParseRounds(input)
        // X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win
        .Select(round => round with
        {
            OurShape = round.OurLetter switch
            {
                'X' => ShapeWinners[round.TheirShape],
                'Y' => round.TheirShape,
                'Z' => ShapeLosers[round.TheirShape],
                _ => throw new InvalidOperationException("Unexpected letter " + round.OurLetter)
            }
        })
        .Sum(round => round.GetOurScore());
}
