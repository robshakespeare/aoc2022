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

using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public static int SnafuDigitsRequired(long number)
    {
        var n = 1;

        while (Math.Pow(5, n) / 2 <= number)
        {
            n++;
        }

        return n;
    }

    private static char[] baseChars = { '2', '1', '0', '-', '=' };

    public static string NormalNumberToSnafu(long value)
    {
        // we can only go back -2

        return "rs-todo!";

        //string result = string.Empty;
        //int targetBase = baseChars.Length;

        //do
        //{
        //    var prev = value;
        //    var rem = value % targetBase;
        //    result = baseChars[rem] + result;
        //    value = value / targetBase;

        //    Console.WriteLine(new { prev, value, rem, result });
        //}
        //while (value > 0);

        //return result;

        //var snafuChars = new List<char>();
        //const int targetBase = 5;

        //do
        //{
        //    var prev = number;

        //    (number, var rem) = Math.DivRem(number, targetBase);

        //    Console.WriteLine(new { prev, number, rem });

        //    snafuChars.Add(BaseChars[(int)rem]);

        //    //result = baseChars[value % targetBase] + result;
        //    //value = value / targetBase;
        //}
        //while (number > 0);

        //return string.Concat(snafuChars);
    }

    //public static string NormalNumberToSnafu(long number) => string.Concat(number.ToString().Reverse().Select((chr, n) =>
    //{
    //    return $"todo: (c:{chr} n:{n})!";
    //}).Reverse());
}
