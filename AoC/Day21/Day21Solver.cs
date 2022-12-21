namespace AoC.Day21;

public class Day21Solver : ISolver
{
    public string DayName => "Monkey Math";

    public long? SolvePart1(PuzzleInput input)
    {
        var monkeys = ParseMonkeys(input);

        return monkeys["root"].Evaluate(monkeys);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    static IReadOnlyDictionary<string, Monkey> ParseMonkeys(string input) => input.ReadLines().Select(line =>
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
        private readonly long _value;

        public YellingMonkey(long value) => _value = value;

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys) => _value;
    }

    sealed class MathMonkey : Monkey
    {
        readonly char _operator;
        readonly string _left;
        readonly string _right;

        public MathMonkey(char @operator, string left, string right)
        {
            _operator = @operator;
            _left = left;
            _right = right;
        }

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys)
        {
            var left = monkeys[_left];
            var right = monkeys[_right];

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
