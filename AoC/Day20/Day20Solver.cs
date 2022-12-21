namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input) => SumGroveCoordinates(Decrypt(input));

    public long? SolvePart2(PuzzleInput input) => SumGroveCoordinates(Decrypt(input, numOfCycles: 10, decryptionKey: 811589153));

    public static IList<long> Decrypt(string input, int numOfCycles = 1, int decryptionKey = 1)
    {
        var workingList = new LinkedList<long>(input.ReadLinesAsLongs().Select(n => n * decryptionKey));
        var originalList = EnumerateNodes(workingList).ToReadOnlyArray();

        for (var i = 1; i <= numOfCycles; i++)
        {
            Logger($"Started Cycle {i} @ {DateTime.Now:s}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            ApplyMixingCycle(originalList);

            Logger($"Completed Cycle {i}, time taken: {stopwatch.Elapsed}{Environment.NewLine}");
        }

        return workingList.ToList();
    }

    static void ApplyMixingCycle(IEnumerable<LinkedListNode<long>> originalList)
    {
        foreach (var node in originalList)
        {
            MoveNumberNode(node);
        }
    }

    public static void MoveNumberNode(LinkedListNode<long> node)
    {
        var list = node.List ?? throw new InvalidOperationException("No list");
        var cycleSize = list.Count - 1L;

        void ShortcutCounter(ref long counter)
        {
            counter %= cycleSize;
            if (counter == 0)
            {
                counter = cycleSize;
            }
        }

        var movement = Math.Abs(node.Value);
        var moveForwards = node.Value > 0;

        if (moveForwards)
        {
            for (var counter = movement; counter > 0; counter--)
            {
                if (node.Next == null) // i.e. we're the last in the list
                {
                    // Move to start plus 1. i.e. one after start
                    list.Remove(node);
                    list.AddAfter(list.First ?? throw new InvalidOperationException("No first"), node);
                }
                else if (node.Next == list.Last) // i.e. we're the second to last in the list
                {
                    // Move to start, and shortcut the counter past all the repeating movement ranges
                    list.Remove(node);
                    list.AddFirst(node);
                    ShortcutCounter(ref counter);
                }
                else
                {
                    var source = node.Next ?? throw new InvalidOperationException("No next");
                    list.Remove(node);
                    list.AddAfter(source, node);
                }
            }
        }
        else
        {
            for (var counter = movement; counter > 0; counter--)
            {
                if (node.Previous == null) // i.e. we're the first in the list
                {
                    // Move to end less one. i.e. one before end
                    list.Remove(node);
                    list.AddBefore(list.Last ?? throw new InvalidOperationException("No last"), node);
                }
                else if (node.Previous == list.First) // i.e. we're the second in the list
                {
                    // Move to end, and shortcut the counter past all the repeating movement ranges
                    list.Remove(node);
                    list.AddLast(node);
                    ShortcutCounter(ref counter);
                }
                else
                {
                    var source = node.Previous ?? throw new InvalidOperationException("No previous");
                    list.Remove(node);
                    list.AddBefore(source, node);
                }
            }
        }
    }

    static long SumGroveCoordinates(IList<long> decrypted)
    {
        var indexOfZero = decrypted.IndexOf(0);
        var value1 = decrypted[(indexOfZero + 1000) % decrypted.Count];
        var value2 = decrypted[(indexOfZero + 2000) % decrypted.Count];
        var value3 = decrypted[(indexOfZero + 3000) % decrypted.Count];

        return value1 + value2 + value3;
    }

    internal static IEnumerable<LinkedListNode<long>> EnumerateNodes(LinkedList<long> list)
    {
        var current = list.First;
        while (current != null)
        {
            yield return current;
            current = current.Next;
        }
    }
}
