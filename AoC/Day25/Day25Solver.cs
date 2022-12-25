// .:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:.
// .                                                                                                     .
// .       *         __  __                         _____ _          _     _                       _     .
// .      /.\       |  \/  |                       / ____| |        (_)   | |                     | |    .
// .     /..'\      | \  / | ___ _ __ _ __ _   _  | |    | |__  _ __ _ ___| |_ _ __ ___   __ _ ___| |    .
// .     /'.'\      | |\/| |/ _ \ '__| '__| | | | | |    | '_ \| '__| / __| __| '_ ` _ \ / _` / __| |    .
// .    /.''.'\     | |  | |  __/ |  | |  | |_| | | |____| | | | |  | \__ \ |_| | | | | | (_| \__ \_|    .
// .    /.'.'.\     |_|  |_|\___|_|  |_|   \__, |  \_____|_| |_|_|  |_|___/\__|_| |_| |_|\__,_|___(_)    .
// .   /'.''.'.\                            __/ |                                                        .
// .   ^^^[_]^^^                           |___/                                                         .
// .                                                                                                     .
// .:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:.

namespace AoC.Day25;

public class Day25Solver : ISolver<string, string>
{
    public string DayName => "Full of Hot Air";

    /// <summary>
    /// What SNAFU number do you supply to Bob's console?
    /// </summary>
    public string? SolvePart1(PuzzleInput input)
    {
        var sumOfFuelRequirements = input.ReadLines().Select(SnafuToNormalNumber).Sum();

        Console.WriteLine($"Sum of the fuel requirements: {sumOfFuelRequirements}");

        return null;
    }

    public string? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static long SnafuToNormalNumber(string snafuInput) => snafuInput.Reverse().Select((chr, n) =>
    {
        var b = (long)Math.Pow(5, n);
        long u = chr switch
        {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => throw new InvalidOperationException("Invalid snafu char: " + chr)
        };
        return b * u;
    }).Sum();

    public static string NormalNumberToSnafu(long number)
    {
        return "rs-todo";
    }
}
