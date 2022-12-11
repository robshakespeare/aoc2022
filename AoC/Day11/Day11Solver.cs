namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Monkey in the Middle";

    public long? SolvePart1(PuzzleInput input)
    {
        var monkeys = ParseInputToMonkeys(input);
        var currentWorryLevel = 0; // rs-todo: don't think this is needed!!

        for (var i = 0; i < 20; i++)
        {
            foreach (var monkey in monkeys)
            {
                currentWorryLevel = monkey.TakeTurn(monkeys, currentWorryLevel);
            }

            Console.WriteLine($"After round {i+1}, the monkeys are holding items with these worry levels:");
            Console.WriteLine(string.Join(Environment.NewLine, monkeys.Select(m => $"Monkey {m.Index}: {string.Join(", ", m.Items)}")));
            Console.WriteLine();
        }

        var mostActive = monkeys.Select(x => x.InspectedItemsCount).OrderDescending().Take(2).ToArray();
        return mostActive[0] * mostActive[1];
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Monkey
    {
        private readonly Func<int, int> _operation;
        private readonly Queue<int> _items;

        public int Index { get; }
        public IEnumerable<int> Items => _items;
        public int TestDivisor { get; }
        public int ThrowToIndexIfTrue { get; }
        public int ThrowToIndexIfFalse { get; }
        public int InspectedItemsCount { get; private set; }

        public Monkey(int index, IEnumerable<int> startingItems, Func<int, int> operation, int testDivisor, int throwToIndexIfTrue, int throwToIndexIfFalse)
        {
            Index = index;
            _items =  new Queue<int>(startingItems);
            _operation = operation;
            TestDivisor = testDivisor;
            ThrowToIndexIfTrue = throwToIndexIfTrue;
            ThrowToIndexIfFalse = throwToIndexIfFalse;
        }

        public int TakeTurn(IReadOnlyList<Monkey> monkeys, int currentWorryLevel)
        {
            while (_items.TryDequeue(out var itemWorryLevel))
            {
                InspectedItemsCount++;
                itemWorryLevel = _operation(itemWorryLevel) / 3;

                //currentWorryLevel += itemWorryLevel;

                var throwToIndex = itemWorryLevel % TestDivisor == 0 ? ThrowToIndexIfTrue : ThrowToIndexIfFalse;
                monkeys[throwToIndex]._items.Enqueue(itemWorryLevel);
            }

            return currentWorryLevel;
        }
    }

    static IReadOnlyList<Monkey> ParseInputToMonkeys(string input) => ParseInputRegex.Matches(input).Select(match =>
    {
        static int? ParseOperand(string operand) => operand == "old" ? null : int.Parse(operand);
        var left = ParseOperand(match.Groups["left"].Value);
        var right = ParseOperand(match.Groups["right"].Value);

        return new Monkey(
            int.Parse(match.Groups["monkeyIndex"].Value),
            match.Groups["startingItems"].Value.Split(", ").Select(int.Parse),
            match.Groups["operator"].Value switch
            {
                "*" => old => (left ?? old) * (right ?? old),
                "+" => old => (left ?? old) + (right ?? old),
                _ => throw new InvalidOperationException("Invalid operator: " + match.Groups["operator"].Value)
            },
            int.Parse(match.Groups["testDivisor"].Value),
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
