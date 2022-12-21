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

        var sourceMonkey = monkeys[rootMonkey.Left];
        var targetValue = monkeys[rootMonkey.Right].Evaluate(monkeys);

        var reverse = TryYell(sourceMonkey, 100, monkeys) > targetValue;

        var lower = 0L;
        var upper = 5_000_000_000_000;

        while (lower <= upper)
        {
            var candidateYell = (lower + upper) / 2;

            var result = TryYell(sourceMonkey, candidateYell, monkeys);

            if (result == targetValue)
            {
                return candidateYell;
            }

            var isLower = reverse ? !(result < targetValue) : result < targetValue;

            if (isLower)
            {
                lower = candidateYell + 1;
            }
            else
            {
                upper = candidateYell - 1;
            }

            Logger($"Yell: {candidateYell:#,0}, result: {result:#,0}, lower: {lower:#,0}, upper: {upper:#,0}, target: {targetValue:#,0}");
        }

        throw new InvalidOperationException("No number found to pass root's equality test");
    }

    private static long TryYell(Monkey source, long yell, Dictionary<string, Monkey> monkeys)
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

    abstract class Monkey
    {
        public string Id { get; set; } = "";

        public abstract long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys);
    }

    sealed class YellingMonkey : Monkey
    {
        readonly long _value;

        public YellingMonkey(long value) => _value = value;

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys) => _value;
    }

    sealed class MathMonkey : Monkey
    {
        readonly char _operator;

        public MathMonkey(char @operator, string left, string right)
        {
            _operator = @operator;
            Left = left;
            Right = right;
        }

        public string Left { get; }

        public string Right { get; }

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys)
        {
            var left = monkeys[Left];
            var right = monkeys[Right];

            return _operator switch
            {
                '+' => left.Evaluate(monkeys) + right.Evaluate(monkeys),
                '-' => left.Evaluate(monkeys) - right.Evaluate(monkeys),
                '*' => left.Evaluate(monkeys) * right.Evaluate(monkeys),
                '/' => left.Evaluate(monkeys) / right.Evaluate(monkeys),
                _ => throw new InvalidOperationException("Invalid operation: " + _operator)
            };
        }
    }
}
