using Crayon;

namespace AoC.Day21;

public class Day21Solver : ISolver
{
    public string DayName => "Monkey Math";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input)
    {
        var monkeys = ParseMonkeys(input);

        return monkeys["root"].Evaluate(monkeys);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var monkeys = ParseMonkeys(input);

        var rootMonkey = (MathMonkey) monkeys["root"];

        //foreach (var yell in new[] { -10000, 0, 1, 301, 1000, 10000, 1_000_000, 1_000_000_000, 1_000_000_000_000, 2_000_000_000_000, 5_000_000_000_000, long.MaxValue })
        foreach (var yell in new[] { 0, 1, 100, 301, 1000, 10000, 1_000_000_000_000, 2_000_000_000_000, 3_000_000_000_000, 4_000_000_000_000, 5_000_000_000_000, long.MaxValue })
        {
            monkeys["humn"] = new YellingMonkey(yell);

            var left = monkeys[rootMonkey.Left];
            var right = monkeys[rootMonkey.Right];

            Logger($"Yelling {yell:#,0}: {left.Evaluate(monkeys):#,0} == {right.Evaluate(monkeys):#,0}");
        }

        Logger("==================");

        // Right number is always the same
        // So, we can use "insertion sort" to find the correct left number.
        // Basically, start with long Max Value
        // Eval 

        var source = monkeys[rootMonkey.Left];
        var target = monkeys[rootMonkey.Right].Evaluate(monkeys);

        var reverse = TryYell(target, source, 100, monkeys) > target;

        Logger("reverse: " + reverse);
        Logger("==================");

        //var lower = 0L;
        ////var upper = monkeys.Count == 15 ? 5000 : long.MaxValue;
        //var upper = 10000L; //long.MaxValue;

        var lower = reverse ? 2_000_000_000_000 : 0L;
        var upper = reverse ? 5_000_000_000_000 : long.MaxValue; //long.MaxValue;

        //const string fmt = "#,0";

        while (lower <= upper)
        {
            var candidateYell = (lower + upper) / 2;

            var result = TryYell(target, source, candidateYell, monkeys);

            if (result == target)
            {
                return candidateYell;
            }

            var isLower = reverse ? !(result < target) : result < target;
            //if (reverse)
            //{
            //    isLower
            //}

            if (isLower)
            {
                lower = candidateYell + 1; //result + 1;

                //upper = candidateYell - 1; //result - 1;
                // example: lower = Math.Max(candidateYell + 1, 0); //result + 1;
            }
            else
            {
                upper = candidateYell - 1; //result - 1;

                //lower = candidateYell + 1; //result + 1;
                // example: upper = candidateYell - 1; //result - 1;
            }

            //Logger($"{new { candidateYell = candidateYell.ToString("N"), result, lower, upper, target, isLower = result < target }}");

            Logger($"yell: {candidateYell:#,0}, result: {result:#,0}, lower: {lower:#,0}, upper: {upper:#,0}, target: {target:#,0}, isLower: {result < target}");
        }

        throw new InvalidOperationException("No number found to pass root's equality test");

        //foreach (var yell in new[] { 0, 1, 301, 1000, 10000 })
        //{
        //    monkeys["humn"] = new YellingMonkey(yell);

        //    var left = monkeys[rootMonkey.Left];
        //    var right = monkeys[rootMonkey.Right];

        //    Logger($"Yelling {yell}: {left.Evaluate(monkeys)} == {right.Evaluate(monkeys)}");
        //}

        //return null;
    }

    private static long TryYell(long target, Monkey source, long yell, Dictionary<string, Monkey> monkeys)
    {
        monkeys["humn"] = new YellingMonkey(yell);
        return source.Evaluate(monkeys);
    }

    static Dictionary<string, Monkey> ParseMonkeys(string input) => input.ReadLines().Select(line =>
    {
        var sections = line.Split(": ");
        var id = sections[0];
        var jobParts = sections[1].Split(' ');
        Monkey monkey = jobParts.Length switch
        {
            1 => new YellingMonkey(long.Parse(jobParts[0])),
            3 => new MathMonkey(jobParts[1].Single(), jobParts[0], jobParts[2]),
            _ => throw new InvalidOperationException("Unexpected job parts length: " + jobParts.Length)
        };
        monkey.Id = id;
        return monkey;
    }).ToDictionary(monkey => monkey.Id);

    public abstract class Monkey
    {
        public string Id { get; set; } = "";

        public abstract long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys);
    }

    public sealed class YellingMonkey : Monkey
    {
        public YellingMonkey(long value) => Value = value;

        public long Value { get; }

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys) => Value;
    }

    public sealed class MathMonkey : Monkey
    {
        public MathMonkey(char @operator, string left, string right)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }

        public char Operator { get; }

        public string Left { get; }

        public string Right { get; }

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys)
        {
            var left = monkeys[Left];
            var right = monkeys[Right];

            return Operator switch
            {
                '+' => left.Evaluate(monkeys) + right.Evaluate(monkeys),
                '-' => left.Evaluate(monkeys) - right.Evaluate(monkeys),
                '*' => left.Evaluate(monkeys) * right.Evaluate(monkeys),
                '/' => left.Evaluate(monkeys) / right.Evaluate(monkeys),
                _ => throw new InvalidOperationException("Invalid operation: " + Operator)
            };
        }
    }
}
