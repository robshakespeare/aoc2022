using System.Linq.Expressions;
using Crayon;
using Sprache;

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
            //1 when jobParts[0] == "root" => new YellingMonkey(long.Parse(jobParts[0])),
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
        private readonly long _value;

        public YellingMonkey(long value) => _value = value;

        public override long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys) => _value;
    }

    public sealed class MathMonkey : Monkey
    {
        private readonly char _operator;
        private readonly string _left;
        private readonly string _right;

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

    public interface IExpression
    {
        long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys);
    }

    public record LiteralExpression(long Value) : IExpression
    {
        public long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys) => Value;
    }

    public record BinaryExpression(char Op, string Left, string Right) : IExpression
    {
        public long Evaluate(IReadOnlyDictionary<string, Monkey> monkeys)
        {
            var left = monkeys[Left];
            var right = monkeys[Right];

            return Op switch
            {
                '+' => left.Evaluate(monkeys) + right.Evaluate(monkeys),
                '-' => left.Evaluate(monkeys) - right.Evaluate(monkeys),
                '*' => left.Evaluate(monkeys) * right.Evaluate(monkeys),
                '/' => left.Evaluate(monkeys) / right.Evaluate(monkeys),
                _ => throw new InvalidOperationException("Invalid operation: " + Op)
            };

            //return Op switch
            //{
            //    '+' => Left.Evaluate() + Right.Evaluate(),
            //    '*' => Left.Evaluate() * Right.Evaluate(),
            //    _ => throw new InvalidOperationException($"Unsupported operator: {Op}")
            //};
        }
    }

    //public class ExpressionEvaluator
    //{
    //    public static ExpressionEvaluator Part1Evaluator { get; } = new("+*");

    //    public static ExpressionEvaluator Part2Evaluator { get; } = new("+", "*");

    //    private readonly string[] _operators;
    //    private readonly Parser<IExpression> _expressionParser;

    //    public ExpressionEvaluator(params string[] operators)
    //    {
    //        _operators = operators;
    //        _expressionParser = Expression;
    //    }

    //    public long Evaluate(string expression, IReadOnlyDictionary<string, Monkey> monkeys) => _expressionParser.Parse(expression).Evaluate(monkeys);

    //    protected Parser<IExpression> Literal =
    //        from literal in Parse.Digit.AtLeastOnce().Text().Token()
    //        select new LiteralExpression(long.Parse(literal));

    //    private Parser<IExpression> SubExpression =>
    //        from lp in Parse.Char('(').Token()
    //        from expr in Expression
    //        from rp in Parse.Char(')').Token()
    //        select expr;

    //    private Parser<IExpression> Term =>
    //        SubExpression.XOr(Literal);

    //    private Parser<IExpression> Expression =>
    //        _operators.Aggregate(
    //            Term,
    //            (term, ops) => Parse.ChainOperator(Parse.Chars(ops).Token(), term, (op, left, right) => new BinaryExpression(op, left, right)));
    //}
}
