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

using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        while (Math.Pow(5, n) / 2 <= number) // NOTE: we need the extra digit once number becomes greater than half of what can be done in that range
        {
            n++;
        }

        return n;
    }

    private static char[] baseChars = { '0', '1', '2', '=', '-' };

    private static (char snafuDigit, int delta)[] BaseChars =
    {
        ('2', 2),
        ('1', 1),
        ('0', 0),
        ('-', -1),
        ('=', -2)
    };

    public static Action<string> Logger = Console.WriteLine;

    public static string NormalNumberToSnafu(long number)
    {
        var result = new StringBuilder();
        var targetBase = baseChars.Length;

        do
        {
            var prev = number;
            var rem = number % targetBase;

            Logger(rem.ToString());

            var chr = baseChars[rem];

            //result = baseChars[rem] + result;
            result.Insert(0, chr);// + result;

            if (chr is '-' or '=')
            {
                number += targetBase;
            }

            number /= targetBase;

            Console.WriteLine(new { prev, number, rem, result });
        }
        while (number > 0);

        return result.ToString();

        var snafuDigitsRequired = SnafuDigitsRequired(number);

        string snafuDigits = ""; // rs-todo: use string builder

        var remain = (long)Math.Pow(5, snafuDigitsRequired) / 2;

        //Console.WriteLine(remain);

        for (var n = snafuDigitsRequired - 1; n >= 0; n--)
        {
            var b = (long)Math.Pow(5, n); // NOTE: DEFINITELY CORRECT, MATCH (b * u) above.  Jst need to work out u!!!!
            //Console.WriteLine(new { number, n, b, u1 = b * 2, u2 = b * 1, u3 = b * 0, u4 = b * -1, u5 = b * -2 });

            var test = BaseChars.Select(baseChar => number + b * baseChar.delta).First(nextNum => nextNum >= 0);

            // rs-todo: choose the first u that doesn't take next number below zero

            Console.WriteLine(new { number, n, b, test, u1 = b * 2, u2 = b * 1, u3 = b * 0, u4 = b * -1, u5 = b * -2 });


            continue;

            

            //var range = (long)Math.Pow(5, n + 1);

            var actRange = (long)Math.Pow(5, n + 1);

            var range = (long)Math.Pow(5, n + 1);
            var max = range / 2;
            var min = ((long)Math.Pow(5, n) / 2) + 1; // rs-todo: min is correct for everything but first "bracket", so IF NEED min, then MUST FIX

            //if (number / (decimal)range > 0.3m)
            //{
            //    // we need whole of it??
            //    snafuDigits += '2';
            //}
            //else if (number / (decimal)range > 0.1m)
            //{
            //    // we need whole of it??
            //    snafuDigits += '1';
            //}

            // NOTE: number will never be greater than max


            // normal range = (5 ^ n) - 1
            // so, our range = (5 ^ n) - 1 - ()

            //var range = (long)Math.Pow(5, n + 1);

            //var snafuRequired = number > (range / 2); // <= number;

            //Console.WriteLine(new { number, n, b, actRange }); //, range, snafuRequired, keh = (range / 2) });

            //Console.WriteLine(new { number, n, b, range, actRange, min, max, snafuDigits, hmm = number / (decimal)range } ); //, range, snafuRequired, keh = (range / 2) });

            var hmm2 = number / (decimal)b;

            if (hmm2 <= 1)
            {
                snafuDigits += '2';
            }
            else if (hmm2 <= 2)
            {
                snafuDigits += '2';
            }
            else
            {
                //throw new InvalidOperationException("not yet supported!");
            }

            Console.WriteLine(new { snafuDigits, number, n, b, range, min, max, hmm2, hmm = number / (decimal)range }); //, range, snafuRequired, keh = (range / 2) });

            //break;

            //var backBySize = b / 5;
            //var maxBackBySize = backBySize * 2;

            ////var (res, rem) = Math.DivRem(value, b);

            //var res = value / (float)b;
            //var resR = Math.Round(value / (float)b);
            //var need = res - Math.Floor(res) > 0.5;
            ////var rem = value % b;

            //if (value > b)
            //{
            //    snafuDigits += resR;
            //}

            //Console.WriteLine(new { value, n, b, res, resR, need, snafuDigits /*, backBySize, maxBackBySize, snafuDigits*//*, res, rem*/ });



            //break;

            //if (value >= maxBackBySize)
            //{
            //    snafuDigits += '=';
            //    //value -= maxBackBySize;
            //}
            //else if (value >= backBySize)
            //{
            //    snafuDigits += '-';
            //    //value -= maxBackBySize;
            //}

            //Console.WriteLine(new { value, b, backBySize, maxBackBySize, snafuDigits });
        }

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
