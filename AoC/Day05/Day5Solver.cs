using static System.Environment;

namespace AoC.Day05;

public partial class Day5Solver : ISolver<string, string>
{
    public string DayName => "Supply Stacks";

    public string SolvePart1(PuzzleInput input) => MoveCrates(input, false);

    public string SolvePart2(PuzzleInput input) => MoveCrates(input, true);

    static string MoveCrates(PuzzleInput input, bool moveCratesInOneGo)
    {
        var (stacks, moves) = ParsePuzzleInput(input);

        foreach (var (amount, from, to) in moves)
        {
            var sourceStack = stacks[from];

            var toMove = sourceStack[..amount];
            if (!moveCratesInOneGo)
            {
                toMove = string.Concat(toMove.Reverse());
            }

            stacks[from] = sourceStack[amount..];
            stacks[to] = toMove + stacks[to];
        }

        return string.Concat(stacks.Select(stack => stack[0]));
    }

    record Move(int Amount, int From, int To);

    static (string[] Stacks, IEnumerable<Move> Moves) ParsePuzzleInput(PuzzleInput input)
    {
        var parts = input.ToString().Split(NewLine + NewLine);
        return (ParseStacks(parts[0]), ParseMoves(parts[1]));
    }

    static string[] ParseStacks(string input)
    {
        var lines = input.Split(NewLine);
        var indexes = lines.Last().Select((chr, index) => (chr, index)).Where(x => x.chr != ' ').Select(x => x.index).ToArray();

        return indexes.Select(index => string.Concat(lines[..^1]
            .Select(line => line[index])
            .Where(crate => crate != ' ')))
            .ToArray();
    }

    static IEnumerable<Move> ParseMoves(string input) =>
        MovesRegex().Matches(input).Select(match => new Move(
            int.Parse(match.Groups["amount"].Value),
            int.Parse(match.Groups["from"].Value) - 1,
            int.Parse(match.Groups["to"].Value) - 1));

    [GeneratedRegex(@"move (?<amount>\d+) from (?<from>\d+) to (?<to>\d+)", RegexOptions.Compiled)]
    private static partial Regex MovesRegex();
}
