namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Monkey in the Middle";

    public long? SolvePart1(PuzzleInput input)
    {
        var monkeys = ParseInputToMonkeys(input);

        for (var i = 0; i < 20; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.TakeTurn(monkeys, true);
            }

            //Console.WriteLine($"After round {i+1}, the monkeys are holding items with these worry levels:");
            //Console.WriteLine(string.Join(Environment.NewLine, monkeys.Select(m => $"Monkey {m.Index}: {string.Join(", ", m.Items)}")));
            //Console.WriteLine();
        }

        var mostActive = monkeys.Select(x => x.InspectedItemsCount).OrderDescending().Take(2).ToArray();
        return mostActive[0] * mostActive[1];
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var monkeys = ParseInputToMonkeys(input);

        for (var i = 0; i < 10000; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.TakeTurn(monkeys, false);
            }

            //Console.WriteLine($"After round {i+1}, the monkeys are holding items with these worry levels:");
            //Console.WriteLine(string.Join(Environment.NewLine, monkeys.Select(m => $"Monkey {m.Index}: {string.Join(", ", m.Items)}")));
            //Console.WriteLine();
        }

        var mostActive = monkeys.Select(x => x.InspectedItemsCount).OrderDescending().Take(2).ToArray();
        return mostActive[0] * mostActive[1];
    }

    public class Monkey
    {
        private readonly Func<long, long> _operation;
        private readonly Queue<long> _items;

        public long Index { get; }
        public IEnumerable<long> Items => _items;
        public long TestDivisor { get; }
        public int ThrowToIndexIfTrue { get; }
        public int ThrowToIndexIfFalse { get; }
        public long InspectedItemsCount { get; private set; }

        public Monkey(long index, IEnumerable<long> startingItems, Func<long, long> operation, long testDivisor, int throwToIndexIfTrue, int throwToIndexIfFalse)
        {
            Index = index;
            _items =  new Queue<long>(startingItems);
            _operation = operation;
            TestDivisor = testDivisor;
            ThrowToIndexIfTrue = throwToIndexIfTrue;
            ThrowToIndexIfFalse = throwToIndexIfFalse;
        }

        public void TakeTurn(IReadOnlyList<Monkey> monkeys, bool divideBy3)
        {
            while (_items.TryDequeue(out var itemWorryLevel))
            {
                InspectedItemsCount++;
                itemWorryLevel = _operation(itemWorryLevel);
                if (divideBy3)
                {
                    itemWorryLevel /= 3;
                }

                var throwToIndex = itemWorryLevel % TestDivisor == 0 ? ThrowToIndexIfTrue : ThrowToIndexIfFalse;
                monkeys[throwToIndex]._items.Enqueue(itemWorryLevel);
            }
        }
    }

    static IReadOnlyList<Monkey> ParseInputToMonkeys(string input) => ParseInputRegex.Matches(input).Select(match =>
    {
        static long? ParseOperand(string operand) => operand == "old" ? null : long.Parse(operand);
        var left = ParseOperand(match.Groups["left"].Value);
        var right = ParseOperand(match.Groups["right"].Value);

        return new Monkey(
            long.Parse(match.Groups["monkeyIndex"].Value),
            match.Groups["startingItems"].Value.Split(", ").Select(long.Parse),
            match.Groups["operator"].Value switch
            {
                "*" => old => (left ?? old) * (right ?? old),
                "+" => old => (left ?? old) + (right ?? old),
                _ => throw new InvalidOperationException("Invalid operator: " + match.Groups["operator"].Value)
            },
            long.Parse(match.Groups["testDivisor"].Value),
            int.Parse(match.Groups["monkeyIfTrue"].Value),
            int.Parse(match.Groups["monkeyIfFalse"].Value)
        );
    }).ToReadOnlyArray();

    private static readonly Regex ParseInputRegex = new("""
        Monkey (?<monkeyIndex>\d):
          Starting items: (?<startingItems>.+)
          Operation: new = (?<left>[^ ]+) (?<operator>[^ ]+) (?<right>[^ ]+)
          Test: divisible by (?<testDivisor>\d+)
            If true: throw to monkey (?<monkeyIfTrue>\d+)
            If false: throw to monkey (?<monkeyIfFalse>\d+)
        """.ReplaceLineEndings(), RegexOptions.Compiled); // rs-todo: .ReplaceLineEndings() ?
}
