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
    public string SolvePart1(PuzzleInput input)
    {
        var sumOfFuelRequirements = input.ReadLines().Select(SnafuToNormalNumber).Sum();

        Console.WriteLine($"Sum of the fuel requirements: {sumOfFuelRequirements}");

        return NormalNumberToSnafu(sumOfFuelRequirements);
    }

    public string SolvePart2(PuzzleInput input) => """
        | The hot air balloons quickly carry you to the North Pole. As soon as you land, most of the expedition is escorted directly to a small building attached to the reindeer stables.
        | 
        | The head smoothie chef has just finished warming up the industrial-grade smoothie blender as you arrive. It will take 50 stars to fill the blender. The expedition Elves turn their attention to you, and you begin emptying the fruit from your pack onto the table.
        | 
        | As you do, a very young Elf - one you recognize from the expedition team - approaches the table and holds up a single star fruit he found. The head smoothie chef places it in the blender.

        Day 25 Part 2 was free after having collected all previous 49 stars! :)

        Advent of Code 2022 is complete.

        Merry Christmas & Happy New Year!";
        """;

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

    static readonly char[] BaseChars = { '0', '1', '2', '=', '-' };

    /// <remarks>
    /// With help from: https://stackoverflow.com/a/923814
    /// </remarks>
    public static string NormalNumberToSnafu(long number)
    {
        var result = new StringBuilder();
        var targetBase = BaseChars.Length;

        do
        {
            var rem = number % targetBase;
            result.Insert(0, BaseChars[rem]);

            if (rem > targetBase / 2)
            {
                number += targetBase;
            }

            number /= targetBase;
        }
        while (number > 0);

        return result.ToString();
    }
}
