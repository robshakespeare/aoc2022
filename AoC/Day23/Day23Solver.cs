using static AoC.Day23.Directions;

namespace AoC.Day23;

public class Day23Solver : ISolver
{
    public string DayName => "Unstable Diffusion";

    public long? SolvePart1(PuzzleInput input)
    {
        var elves = ParseElves(input);
        var elfGrid = Simulate(elves).ElfGrid;

        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);

        foreach (var (p, _) in elfGrid)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        max += Vector2.One;

        var area = (long)(max.X - min.X) * (long)(max.Y - min.Y);

        return area - elves.Length;
    }

    public long? SolvePart2(PuzzleInput input) => Simulate(ParseElves(input), numOfRounds: int.MaxValue).RoundNumberReached;

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public static (Dictionary<Vector2, Elf> ElfGrid, int RoundNumberReached) Simulate(Elf[] elves, int numOfRounds = 10)
    {
        var elfGrid = elves.ToDictionary(elf => elf.Position);
        var candidateMovements = new Queue<CandidateMovement>(CandidateMovementsTemplate);
        bool elvesMoved;
        var roundNumber = 0;

        do
        {
            roundNumber++;
            elvesMoved = false;

            Dictionary<Vector2, long> proposedPositions = new();

            // First half of round, all Elves decide their proposed position
            foreach (var proposalResult in elves.Select(elf => elf.UpdateProposedPosition(elfGrid, candidateMovements)))
            {
                if (proposalResult != null)
                {
                    proposedPositions.AddOrIncrement(proposalResult.Value.ProposedPosition);
                }
            }

            // Second half of round, move elves who were the only one to propose a distinct position
            foreach (var elf in elves)
            {
                if (elf.ProposedPosition != null)
                {
                    var elfProposedPosition = elf.ProposedPosition.Value;

                    if (proposedPositions[elfProposedPosition] == 1)
                    {
                        elfGrid.Remove(elf.Position);
                        elfGrid.Add(elfProposedPosition, elf);
                        elf.Position = elfProposedPosition;
                        elvesMoved = true;
                    }
                }
            }

            //  Finally, at the end of the round, the first direction the Elves considered is moved to the end of the list of directions
            candidateMovements.Enqueue(candidateMovements.Dequeue());
        } while (elvesMoved && roundNumber < numOfRounds);

        return (elfGrid, roundNumber);
    }

    public record Elf(int ElfId, Vector2 Position)
    {
        public Vector2 Position { get; set; } = Position;

        public Vector2? ProposedPosition { get; private set; }

        public (Vector2 ProposedPosition, CandidateMovement Move)? UpdateProposedPosition(
            IReadOnlyDictionary<Vector2, Elf> elvesGrid,
            IEnumerable<CandidateMovement> candidateMovements)
        {
            ProposedPosition = null;

            // If no other Elves are in one of those eight positions, the Elf does not do anything during this round
            if (!AllDirections.Select(dir => Position + dir).Any(elvesGrid.ContainsKey))
            {
                return null;
            }

            // Otherwise, the Elf looks in each of four directions in the following order and proposes moving one step in the first valid direction:
            var chosenMove = candidateMovements
                .FirstOrDefault(move => !move.RequiredFreeAdjacentDirections.Select(dir => Position + dir).Any(elvesGrid.ContainsKey));

            if (chosenMove == null)
            {
                return null;
            }

            ProposedPosition = Position + chosenMove.Direction;
            return (ProposedPosition.Value, chosenMove);
        }
    }

    public record CandidateMovement(Vector2 Direction, Vector2[] RequiredFreeAdjacentDirections);

    static readonly IReadOnlyCollection<CandidateMovement> CandidateMovementsTemplate = new[]
    {
        new CandidateMovement(GridUtils.North, new[] { N, NE, NW }),
        new CandidateMovement(GridUtils.South, new[] { S, SE, SW }),
        new CandidateMovement(GridUtils.West, new[] { W, NW, SW }),
        new CandidateMovement(GridUtils.East, new[] { E, NE, SE }),
    };

    private static readonly IReadOnlyCollection<Vector2> AllDirections = new[] { N, E, S, W, NE, NW, SE, SW };

    public static Elf[] ParseElves(PuzzleInput input)
    {
        var elfId = 0;
        var elves = input.ToString().ReadLines()
            .SelectMany((line, y) =>
                line.Select((c, x) => (c, x)).Where(p => p.c == '#').Select(p => new Elf(++elfId, new Vector2(p.x, y))))
            .ToArray();
        return elves;
    }
}
