namespace AoC.Day10;

public class Day10Solver : ISolver<long, string>
{
    public string DayName => "Cathode-Ray Tube";

    public long SolvePart1(PuzzleInput input)
    {
        foreach (var output in ParseAndProcessInstructions(input))
        {
            Console.WriteLine(output);
        }

        return ParseAndProcessInstructions(input)
            .Where(output => (output.CycleNumber + 20) % 40 == 0)
            .Aggregate(0L, (agg, cur) => agg + cur.CycleNumber * cur.RegisterX);
    }

    public string SolvePart2(PuzzleInput input)
    {
        const int width = 40;
        const int height = 6;

        var grid = Enumerable.Range(0, height).Select(_ => new char[width]).ToArray();

        // Process the commands. Assume inputs produce 240 cycles.
        foreach (var (cycleNumber, registerX) in ParseAndProcessInstructions(input))
        {
            var x = (cycleNumber - 1) % width;
            var y = (cycleNumber - 1) / width;
            var isLit = x >= registerX - 1 && x <= registerX + 1;
            grid[y][x] = isLit ? '#' : '.';
        }

        return string.Join(Environment.NewLine, grid.Select(line => string.Concat(line)));
    }

    static IEnumerable<(long CycleNumber, long RegisterX)> ParseAndProcessInstructions(PuzzleInput input)
    {
        var registerX = 1;
        var cycleNumber = 0;
        foreach (var instruction in ParseInstructions(input))
        {
            for (var tick = 0; tick < instruction.CycleLength; tick++)
            {
                cycleNumber++;
                yield return (cycleNumber, registerX);
            }

            registerX += instruction.RegisterXDelta;
        }
    }

    public record Instruction(string Command, int RegisterXDelta, int CycleLength);

    static IEnumerable<Instruction> ParseInstructions(PuzzleInput input) => input.ReadLines()
        .Select(line => line.Split(" "))
        .Select(parts => parts switch
        {
            ["addx", var amount] => new Instruction("addx", int.Parse(amount), 2),
            ["noop"] => new Instruction("noop", 0, 1),
            _ => throw new InvalidOperationException("Invalid command: " + parts[0])
        });
}
