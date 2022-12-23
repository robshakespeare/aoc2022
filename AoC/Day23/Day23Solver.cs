using static AoC.Day23.Directions;

namespace AoC.Day23;

public class Day23Solver : ISolver
{
    public string DayName => "Unstable Diffusion";

    public long? SolvePart1(PuzzleInput input)
    {
        var elves = ParseElves(input);
        var elvesGrid = Simulate(elves);

        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);

        foreach (var (p, _) in elvesGrid)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        max += Vector2.One;

        var area = (long)(max.X - min.X) * (long)(max.Y - min.Y);

        return area - elves.Length;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public static Dictionary<Vector2, Elf> Simulate(Elf[] elves, int numOfRounds = 10)
    {
        var elvesGrid = elves.ToDictionary(elf => elf.Position);

        var candidateMovements = CandidateMovementsTemplate.ToList();

        //Logger("== Initial State ==");
        //Logger(elvesGrid.ToStringGrid(x => x.Key, _ => '#', '.').RenderGridToString());
        //Logger("");

        bool elvesMoved = true;

        for (var roundNumber = 1; roundNumber <= numOfRounds && elvesMoved; roundNumber++)
        {
            Dictionary<Vector2, long> proposedPositions = new();
            //CandidateMovement? firstChosenMove = null;

            // First half of round, all Elves decide their proposed position
            foreach (var proposalResult in elves.Select(elf => elf.UpdateProposedPosition(elvesGrid, candidateMovements)))
            {
                if (proposalResult != null)
                {
                    proposedPositions.AddOrIncrement(proposalResult.Value.ProposedPosition);

                    //firstChosenMove ??= proposalResult.Value.Move;
                }
            }

            // Second half of round, move elves who were the only one to propose a distinct position
            elvesMoved = false;
            foreach (var elf in elves)
            {
                if (elf.ProposedPosition != null)
                {
                    var elfProposedPosition = elf.ProposedPosition.Value;

                    if (proposedPositions[elfProposedPosition] == 1)
                    {
                        elvesGrid.Remove(elf.Position);
                        elvesGrid.Add(elfProposedPosition, elf);
                        elf.Position = elfProposedPosition;
                        elvesMoved = true;
                    }
                }
            }

            //  Finally, at the end of the round, the first direction the Elves considered is moved to the end of the list of directions
            var firstChosenMove = candidateMovements[0];
            if (!candidateMovements.Remove(firstChosenMove))
            {
                throw new InvalidOperationException("Error, expected to remove move.");
            }

            candidateMovements.Add(firstChosenMove);

            // rs-todo: rem all temp logging
            //Logger($"== End of Round {roundNumber} ==");
            //Logger(elvesGrid.ToStringGrid(x => x.Key, _ => '#', '.').RenderGridToString());
            //Logger("");
        }

        return elvesGrid;
    }

    public record Elf(int ElfId, Vector2 Position)
    {
        public Vector2 Position { get; set; } = Position;

        public Vector2? ProposedPosition { get; private set; }

        //public List<CandidateMovement> CandidateMovements { get; } = CandidateMovementsTemplate.ToList();

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

            if (chosenMove != null)
            {
                ProposedPosition = Position + chosenMove.Direction;
                return (ProposedPosition.Value, chosenMove);
            }

            return null;
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
