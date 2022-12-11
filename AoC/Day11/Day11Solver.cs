namespace AoC.Day11;

public class Day11Solver : ISolver
{
    public string DayName => "Monkey in the Middle";

    public long? SolvePart1(PuzzleInput input)
    {
        var monkeys = ParseInputToMonkeys(input);
        return CalculateMonkeyBusiness(monkeys, 20, itemWorryLevel => itemWorryLevel / 3);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var monkeys = ParseInputToMonkeys(input);
        // rs-todo: util method
        var leastCommonMultiple = monkeys.Aggregate(1L, (lcm, monkey) => MathUtils.LeastCommonMultiple(lcm, monkey.TestDivisor));
        return CalculateMonkeyBusiness(monkeys, 10000, itemWorryLevel => itemWorryLevel % leastCommonMultiple);
    }

    static long CalculateMonkeyBusiness(IReadOnlyList<Monkey> monkeys, int numberOfRounds, Func<long, long> manageWorryLevel)
    {
        for (var i = 0; i < numberOfRounds; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.TakeTurn(monkeys, manageWorryLevel);
            }
        }

        var mostActive = monkeys.Select(x => x.InspectedItemsCount).OrderDescending().Take(2).ToArray();
        var monkeyBusiness = mostActive[0] * mostActive[1];

        Console.WriteLine($"MonkeyBusiness = {mostActive[0]} * {mostActive[1]} = {monkeyBusiness}");
        return monkeyBusiness;
    }

    public class Monkey
    {
        private readonly Func<long, long> _operation;
        private readonly Queue<long> _items;

        public int Index { get; }
        public IEnumerable<long> Items => _items;
        public long TestDivisor { get; }
        public int ThrowToIndexIfTrue { get; }
        public int ThrowToIndexIfFalse { get; }
        public long InspectedItemsCount { get; private set; }

        public Monkey(int index, IEnumerable<long> startingItems, Func<long, long> operation, long testDivisor, int throwToIndexIfTrue, int throwToIndexIfFalse)
        {
            Index = index;
            _items = new Queue<long>(startingItems);
            _operation = operation;
            TestDivisor = testDivisor;
            ThrowToIndexIfTrue = throwToIndexIfTrue;
            ThrowToIndexIfFalse = throwToIndexIfFalse;
        }

        public void TakeTurn(IReadOnlyList<Monkey> monkeys, Func<long, long> manageWorryLevel)
        {
            while (_items.TryDequeue(out var itemWorryLevel))
            {
                InspectedItemsCount++;
                itemWorryLevel = manageWorryLevel(_operation(itemWorryLevel));
                var throwToIndex = itemWorryLevel % TestDivisor == 0 ? ThrowToIndexIfTrue : ThrowToIndexIfFalse;
                monkeys[throwToIndex]._items.Enqueue(itemWorryLevel);
            }
        }

        public string Debug1() => $"Monkey {Index}: {string.Join(", ", Items)}";

        public string Debug2() => $"Monkey {Index} inspected items {InspectedItemsCount} times.";
    }

    static IReadOnlyList<Monkey> ParseInputToMonkeys(string input) => ParseInputRegex.Matches(input).Select(match =>
    {
        static long? ParseOperand(string operand) => operand == "old" ? null : long.Parse(operand);
        var left = ParseOperand(match.Groups["left"].Value);
        var right = ParseOperand(match.Groups["right"].Value);

        return new Monkey(
            int.Parse(match.Groups["monkeyIndex"].Value),
            match.Groups["startingItems"].Value.Split(", ").Select(long.Parse),
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

    static readonly Regex ParseInputRegex = new("""
        Monkey (?<monkeyIndex>\d):
          Starting items: (?<startingItems>.+)
          Operation: new = (?<left>[^ ]+) (?<operator>[^ ]+) (?<right>[^ ]+)
          Test: divisible by (?<testDivisor>\d+)
            If true: throw to monkey (?<monkeyIfTrue>\d+)
            If false: throw to monkey (?<monkeyIfFalse>\d+)
        """.ReplaceLineEndings(), RegexOptions.Compiled);
}
