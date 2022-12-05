using static System.Environment;

namespace AoC.Day05;

public partial class Day5Solver : SolverBase<string, long?>
{
    public override string DayName => "Supply Stacks";

    public override string SolvePart1(PuzzleInput input)
    {
        var (stacks, moves) = ParsePuzzleInput(input);

        foreach (var (amount, from, to) in moves)
        {
            var sourceStack = stacks[from];

            var toMove = string.Concat(sourceStack[..amount].Reverse());

            stacks[from] = sourceStack[amount..];
            stacks[to] = toMove + stacks[to];
        }

        return string.Concat(stacks.Select(stack => stack[0]));
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    record Move(int Amount, int From, int To);

    static (string[] Stacks, IEnumerable<Move> Moves) ParsePuzzleInput(PuzzleInput input)
    {
        var parts = input.ToString().Split(NewLine + NewLine);
        return (ParseStacks(parts[0]), ParseMoves(parts[1]));
    }

    private static string[] ParseStacks(string input)
    {
        var lines = input.Split(NewLine);
        var indexes =
            lines.Last().Select((chr, charIndex) => (chr, charIndex))
                .Where(x => x.chr != ' ')
                .Select(x => (stackIndex: int.Parse(x.chr.ToString()) - 1, x.charIndex))
                .ToArray();

        var stacks = Enumerable.Range(0, indexes.Length).Select(_ => new List<char>()).ToArray();

        foreach (var line in lines[..^1])
        {
            foreach (var (stackIndex, charIndex) in indexes)
            {
                var crate = line[charIndex];
                if (crate !=  ' ')
                {
                    stacks[stackIndex].Add(crate);
                }
            }
        }

        return stacks.Select(stack => string.Concat(stack)).ToArray();
    }

    static IEnumerable<Move> ParseMoves(string input) =>
        MovesRegex().Matches(input).Select(match => new Move(
            int.Parse(match.Groups["amount"].Value),
            int.Parse(match.Groups["from"].Value) - 1,
            int.Parse(match.Groups["to"].Value) - 1));

    [GeneratedRegex(@"move (?<amount>\d+) from (?<from>\d+) to (?<to>\d+)", RegexOptions.Compiled)]
    private static partial Regex MovesRegex();
}
