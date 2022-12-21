namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public long? SolvePart1(PuzzleInput input) => SumGroveCoordinates(Decrypt(input));

    public long? SolvePart2(PuzzleInput input) => SumGroveCoordinates(Decrypt(input, numOfCycles: 10, decryptionKey: 811589153));

    //public class Number
    //{
    //    public long Value { get; }

    //    public Number(long value) => Value = value;

    //    public override string ToString() => Value.ToString();
    //}

    public static IList<long> Decrypt(string input, int numOfCycles = 1, int decryptionKey = 1)
    {
        //var originalOrder = input.ReadLinesAsLongs().Select(n => new Number(n * decryptionKey)).ToReadOnlyArray();
        //var numbers = originalOrder.ToList();

        //var encrypted = new LinkedList<long>(input.ReadLinesAsLongs().Select(n => new Number(n * decryptionKey)));

        var encrypted = new LinkedList<long>(input.ReadLinesAsLongs().Select(n => n * decryptionKey));
        var originalOrder = EnumerateNodes(encrypted).ToReadOnlyArray();

        const bool enableSkip = false;

        if (!enableSkip && numOfCycles > 1)
        {
            throw new InvalidOperationException("not until optimized works!");
        }

        for (var i = 0; i < numOfCycles; i++)
        {
            ApplyMixingCycle(encrypted, originalOrder, enableSkip);
        }

        return encrypted.ToList();
        //return numbers; //.Select(n => n.Value).ToList();
    }

    //static long GetIt1(LinkedList<long> list, long currentMove)
    //{
    //    return (currentMove - 1) % list.Count;
    //}

    //static long GetIt2(LinkedList<long> list, long currentMove)
    //{
    //    return currentMove % list.Count;
    //}

    //static long GetIt3(LinkedList<long> list, long currentMove)
    //{
    //    return ((currentMove - 1) % list.Count) - 1;
    //}

    //static long GetIt4(LinkedList<long> list, long currentMove)
    //{
    //    return currentMove % (list.Count - 1);
    //}

    static void ApplyMixingCycle(LinkedList<long> list, IEnumerable<LinkedListNode<long>> originalOrder, bool enableSkip)
    {
        //Console.WriteLine("Initial");
        //Console.WriteLine(string.Join(", ", numbers));
        //Console.WriteLine();

        foreach (var node in originalOrder)
        //foreach (var number in originalOrder)
        {
            //MoveNumber(number, numbers);

            //Console.WriteLine("After move");
            //Console.WriteLine(string.Join(", ", numbers));
            //Console.WriteLine();

            MoveNumberNode(node);
        }
    }

    public static void MoveNumberNode(LinkedListNode<long> node)
    {
        var enableSkip = false;

        var list = node.List ?? throw new InvalidOperationException("No list");
        //var cycleSize = list.Count; // - 1L;

        var movement = Math.Abs(node.Value);
        var moveForwards = node.Value > 0;

        Console.WriteLine("Move number start");

        if (moveForwards)
        {
            var edge = false;
            var skip = -1L;
            var i = 0;

            //for (long counter = 0; counter < movement; counter++)
            for (var counter = movement; counter > 0; counter--)
            {
                if (node.Next == null) // i.e. we're the last in the list
                {
                    // Move to start plus 1. i.e. one after start
                    list.Remove(node);
                    list.AddAfter(list.First ?? throw new InvalidOperationException("No first"), node);

                    //counter %= cycleSize;

                    edge = false;
                    skip = -1;
                }
                else if (node.Next == list.Last /*|| node.Next == null*/) // i.e. we're the second to last in the list
                {
                    // Move to start
                    list.Remove(node);
                    list.AddFirst(node);

                    ////#######
                    //if (enableSkip)
                    //{
                    //    counter %= cycleSize;
                    //    if (counter == 0)
                    //    {
                    //        counter = cycleSize;
                    //    }
                    //}
                    ////#######

                    //counter %= cycleSize;

                    edge = true;
                    skip = counter % (list.Count - 1); //Math.Clamp(counter % cycleSize, 1, cycleSize); //Math.Max(counter % cycleSize, 1);
                    if (skip == 0)
                    {
                        skip = list.Count - 1;
                    }
                }
                else
                {
                    var source = node.Next ??
                                 throw new InvalidOperationException("Unexpected: should cycle, not reach end");
                    list.Remove(node);
                    list.AddAfter(source, node);

                    edge = false;
                    skip = -1;
                }

                if ((movement - counter) < 20 || i <= 20)
                {
                    Console.WriteLine($"{counter,4}: {i,4} - {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");

                }
                i++;
            }
        }
        else
        {
            //var reachedEdge = false;
            var edge = false;
            var skip = -1L;
            var i = 0;

            //for (long counter = 0; counter < movement; counter++)
            for (var counter = movement; counter > 0; counter--)
            {
                if (node.Previous == null) // i.e. we're the first in the list
                {
                    // Move to end less one. i.e. one before end
                    list.Remove(node);
                    list.AddBefore(list.Last ?? throw new InvalidOperationException("No last"), node);

                    //counter %= cycleSize;

                    //edge = true;
                    //skip = counter % (list.Count - 1); //Math.Clamp(counter % cycleSize, 1, cycleSize); //Math.Max(counter % cycleSize, 1);
                    //if (skip == 0)
                    //{
                    //    skip = list.Count - 1;
                    //}

                    edge = false;
                    skip = -1;
                }
                else if (node.Previous == list.First /*|| node.Previous == null*/) // i.e. we're the second in the list
                {
                    // Move to end.
                    list.Remove(node);
                    list.AddLast(node);

                    //reachedEdge = true;

                    //if (counter > list.Count + 1)
                    //{

                    //}

                    //counter = GetIt2(list, counter);

                    //if (node.Previous != null)
                    //{
                    //    counter = GetIt2(list, counter);
                    //}

                    edge = true;
                    skip = counter % (list.Count - 1); //Math.Clamp(counter % cycleSize, 1, cycleSize); //Math.Max(counter % cycleSize, 1);
                    if (skip == 0)
                    {
                        skip = list.Count - 1;
                    }

                    //counter %= cycleSize;

                    //////#######
                    //if (enableSkip)
                    //{
                    //    counter %= cycleSize;
                    //    if (counter == 0)
                    //    {
                    //        counter = cycleSize;
                    //    }
                    //}
                    ////#######
                }
                else
                {
                    var source = node.Previous ??
                                 throw new InvalidOperationException("Unexpected: should cycle, not reach beginning");
                    list.Remove(node);
                    list.AddBefore(source, node);

                    edge = false;
                    skip = -1;
                }

                if ((movement - counter) < 20 || i <= 20)
                {
                    Console.WriteLine($"{counter,4}: {i,4} - {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");

                }
                //Console.WriteLine($"{counter,4}: {i,4} {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");

                i++;
                //if (reachedEdge || true)
                //{
                //    Console.WriteLine($"{counter,4}: {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");
                //}
            }
        }
    }

    //public static void MoveNumber(Number number, List<Number> numbers)
    //{
    //    // Move the node the number of times indicated by its value!
    //    // Note that we can forwards or backwards!
    //    var numOfMoves = Math.Abs(number.Value);
    //    //var moveForwards = number.Value > 0;
    //    var dir = number.Value > 0 ? 1 : -1;

    //    // Move the number, until we have completed all of our moves
    //    for (var i = 0L; i < numOfMoves; i++)
    //    {
    //        // Get the current index of the number
    //        // rs-todo: ugg, there must be a moe optimal way of doing this!!
    //        var currentIndex = numbers.IndexOf(number);
    //        if (currentIndex == -1)
    //        {
    //            throw new InvalidOperationException("Unexpected: did not find number in list: " + number);
    //        }

    //        // Loop around if needed
    //        if (dir == -1 && currentIndex == 0)
    //        {
    //            // Move to end less one. i.e. one before end
    //            numbers.RemoveAt(currentIndex);
    //            numbers.Insert(numbers.Count - 1, number);
    //        }
    //        else if (dir == -1 && currentIndex == 1)
    //        {
    //            // Move to end.
    //            numbers.RemoveAt(currentIndex);
    //            numbers.Insert(numbers.Count, number);
    //        }
    //        else if (dir == 1 && currentIndex == numbers.Count - 1)
    //        {
    //            // Move to start plus 1. i.e. one after start
    //            numbers.RemoveAt(currentIndex);
    //            numbers.Insert(1, number);
    //        }
    //        else if (dir == 1 && currentIndex == numbers.Count - 2)
    //        {
    //            // Move to start
    //            numbers.RemoveAt(currentIndex);
    //            numbers.Insert(0, number);
    //        }
    //        else
    //        {
    //            var prevIndex = currentIndex;
    //            currentIndex += dir;

    //            // Otherwise, swap
    //            (numbers[currentIndex], numbers[prevIndex]) = (numbers[prevIndex], numbers[currentIndex]);
    //        }

    //        // rs-todo: I think we can apply the "skip" logic here
    //    }
    //}

    static long SumGroveCoordinates(IList<long> decrypted)
    {
        var indexOfZero = decrypted.IndexOf(0);
        var value1 = decrypted[(indexOfZero + 1000) % decrypted.Count];
        var value2 = decrypted[(indexOfZero + 2000) % decrypted.Count];
        var value3 = decrypted[(indexOfZero + 3000) % decrypted.Count];

        return value1 + value2 + value3;
    }

    public static IEnumerable<LinkedListNode<long>> EnumerateNodes(LinkedList<long> list)
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
