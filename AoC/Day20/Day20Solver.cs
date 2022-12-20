namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input)
    {
        var list = new LinkedList<long>(input.ReadLinesAsLongs());

        foreach (var node in ToNodeArray(list))
        {
            // Move the node the number of times indicated by its value!
            // Note that we can forwards or backwards!
            var movement = node.Value;
            var moveForwards = movement > 0;

            if (moveForwards)
            {
                for (var counter = 0; counter < movement; counter++)
                {
                    if (node.Next == list.Last || node.Next == null)
                    {
                        list.Remove(node);
                        list.AddFirst(node);
                    }
                    else
                    {
                        var destination = node.Next ?? throw new InvalidOperationException("Unexpected: should cycle, not reach end");
                        list.Remove(node);
                        list.AddAfter(destination, node);
                    }
                }
            }
            else
            {
                for (var counter = movement; counter < 0; counter++)
                {
                    if (node.Previous == list.First || node.Previous == null)
                    {
                        list.Remove(node);
                        list.AddLast(node);
                    }
                    else
                    {
                        var destination = node.Previous ?? throw new InvalidOperationException("Unexpected: should cycle, not reach beginning");
                        list.Remove(node);
                        list.AddBefore(destination, node);
                    }
                }
            }
        }

        var cycledList = list.ToList();

        var indexOfZero = cycledList.IndexOf(0);

        var value1 = cycledList[(indexOfZero + 1000) % cycledList.Count];
        var value2 = cycledList[(indexOfZero + 2000) % cycledList.Count];
        var value3 = cycledList[(indexOfZero + 3000) % cycledList.Count];

        return value1 + value2 + value3;
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    static IReadOnlyList<LinkedListNode<long>> ToNodeArray(LinkedList<long> list)
    {
        IEnumerable<LinkedListNode<long>> EnumerateLinkedList()
        {
            var current = list.First;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        return EnumerateLinkedList().ToArray();
    }
}
