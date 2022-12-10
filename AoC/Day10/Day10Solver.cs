namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Cathode-Ray Tube";

    public long? SolvePart1(PuzzleInput input)
    {
        foreach (var output in ParseAndProcessInstructions(input))
        {
            Console.WriteLine(output);
        }

        return ParseAndProcessInstructions(input).Aggregate(0L, (agg, cur) => agg + cur.CycleNumber * cur.RegisterX);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    static IEnumerable<(long CycleNumber, long RegisterX)> ParseAndProcessInstructions(PuzzleInput input)
    {
        var registerX = 1;
        var cycleNumber = 0;
        foreach (var instruction in ParseInstructions(input))
        {
            for (var i = 0; i < instruction.CycleLength; i++)
            {
                cycleNumber++;

                if ((cycleNumber + 20) % 40 == 0)
                {
                    yield return (cycleNumber, registerX);
                }
            }

            registerX += instruction.RegisterXDelta;
        }
    }

    public record Instruction(string Command, int RegisterXDelta, int CycleLength);

    static IEnumerable<Instruction> ParseInstructions(PuzzleInput input) =>
        input.ReadLines()
            .Select(line => line.Split(" "))
            .Select(parts => parts switch
            {
                ["addx", var amount] => new Instruction("addx", int.Parse(amount), 2),
                ["noop"] => new Instruction("noop", 0, 1),
                _ => throw new InvalidOperationException("Invalid command: " + parts[0])
            });
}
