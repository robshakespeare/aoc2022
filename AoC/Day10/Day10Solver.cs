namespace AoC.Day10;

public class Day10Solver : ISolver<long, string>
{
    public string DayName => "Cathode-Ray Tube";

    public long SolvePart1(PuzzleInput input) => EnumerateOutputs(input)
        .Where(output => (output.CycleNumber + 20) % 40 == 0)
        .Aggregate(0, (agg, cur) => agg + cur.CycleNumber * cur.RegisterX);

    public string SolvePart2(PuzzleInput input)
    {
        const int width = 40;
        const int height = 6;

        var grid = Enumerable.Range(0, height).Select(_ => new char[width]).ToArray();

        // Process the commands. Assume all inputs produce 240 cycles.
        foreach (var (cycleNumber, registerX) in EnumerateOutputs(input))
        {
            var x = (cycleNumber - 1) % width;
            var y = (cycleNumber - 1) / width;
            var isLit = x >= registerX - 1 && x <= registerX + 1;
            grid[y][x] = isLit ? '#' : ' ';
        }

        return string.Join(Environment.NewLine, grid.Select(line => string.Concat(line)));
    }

    static IEnumerable<(int CycleNumber, int RegisterX)> EnumerateOutputs(PuzzleInput input)
    {
        var registerX = 1;
        var cycleNumber = 0;
        foreach (var (cycleLength, registerXDelta) in ParseInstructions(input))
        {
            for (var tick = 0; tick < cycleLength; tick++)
            {
                yield return (++cycleNumber, registerX);
            }

            registerX += registerXDelta;
        }
    }

    static IEnumerable<(int CycleLength, int RegisterXDelta)> ParseInstructions(PuzzleInput input) => input.ReadLines()
        .Select(line => line.Split(" "))
        .Select(parts => parts switch
        {
            ["addx", var amount] => (2, int.Parse(amount)),
            _ => (1, 0)
        });
}
