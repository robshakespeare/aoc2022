namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public long? SolvePart1(PuzzleInput input) => SumGroveCoordinates(Decrypt(input));

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static IList<long> Decrypt(string input, int numOfCycles = 1)
    {
        var encrypted = new LinkedList<long>(input.ReadLinesAsLongs());
        var originalOrder = EnumerateNodes(encrypted).ToReadOnlyArray();

        for (var i = 0; i < numOfCycles; i++)
        {
            ApplyMixingCycle(encrypted, originalOrder);
        }

        return encrypted.ToList();
    }

    static void ApplyMixingCycle(LinkedList<long> list, IEnumerable<LinkedListNode<long>> originalOrder)
    {
        long GetIt1(long currentMove)
        {
            return (currentMove - 1) % list.Count;
        }

        long GetIt2(long currentMove)
        {
            return currentMove % list.Count;
        }

        long GetIt3(long currentMove)
        {
            return ((currentMove - 1) % list.Count) -1;
        }

        foreach (var node in originalOrder)
        {
            // Move the node the number of times indicated by its value!
            // Note that we can forwards or backwards!
            var movement = Math.Abs(node.Value);
            var moveForwards = node.Value > 0;

            if (moveForwards)
            {
                for (long counter = 0; counter < movement; counter++)
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
                for (long counter = 0; counter < movement; counter++)
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
    }

    static long SumGroveCoordinates(IList<long> decrypted)
    {
        var indexOfZero = decrypted.IndexOf(0);
        var value1 = decrypted[(indexOfZero + 1000) % decrypted.Count];
        var value2 = decrypted[(indexOfZero + 2000) % decrypted.Count];
        var value3 = decrypted[(indexOfZero + 3000) % decrypted.Count];

        return value1 + value2 + value3;
    }

    static IEnumerable<LinkedListNode<long>> EnumerateNodes(LinkedList<long> list)
    {
        var current = list.First;
        while (current != null)
        {
            yield return current;
            current = current.Next;
        }
    }

    //static IReadOnlyList<LinkedListNode<long>> ToNodeArray(LinkedList<long> list)
    //{
    //    IEnumerable<LinkedListNode<long>> EnumerateLinkedList()
    //    {
    //        var current = list.First;
    //        while (current != null)
    //        {
    //            yield return current;
    //            current = current.Next;
    //        }
    //    }

    //    return EnumerateLinkedList().ToArray();
    //}
}
