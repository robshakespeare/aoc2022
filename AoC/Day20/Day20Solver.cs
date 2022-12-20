using System.Collections.Generic;
using System.Globalization;

namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public long? SolvePart1(PuzzleInput input) => SumGroveCoordinates(Decrypt(input));

    public long? SolvePart2(PuzzleInput input) => SumGroveCoordinates(Decrypt(input, numOfCycles: 10, decryptionKey: 811589153));

    public class Number
    {
        public long Value { get; }

        public Number(long value) => Value = value;

        public override string ToString() => Value.ToString();
    }

    public static List<Number> Decrypt(string input, int numOfCycles = 1, int decryptionKey = 1)
    {
        var originalOrder = input.ReadLinesAsLongs().Select(n => new Number(n * decryptionKey)).ToReadOnlyArray();
        var numbers = originalOrder.ToList();

        //var encrypted = new LinkedList<long>(input.ReadLinesAsLongs().Select(n => new Number(n * decryptionKey)));
        //var originalOrder = EnumerateNodes(encrypted).ToReadOnlyArray();

        const bool enableSkip = false;

        if (!enableSkip && numOfCycles > 1)
        {
            throw new InvalidOperationException("not until optimized works!");
        }

        for (var i = 0; i < numOfCycles; i++)
        {
            ApplyMixingCycle(numbers, originalOrder, enableSkip);
        }

        return numbers; //.Select(n => n.Value).ToList();
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

    static void ApplyMixingCycle(List<Number> numbers, IEnumerable<Number> originalOrder, bool enableSkip)
    {
        var cycleSize = numbers.Count - 1L;

        foreach (var number in originalOrder)
        {
            MoveNumber(number, numbers);

            //if (moveForwards)
            //{
            //    //for (long counter = 0; counter < movement; counter++)
            //    for (var counter = movement; counter > 0; counter--)
            //    {
            //        if (node.Next == list.Last || node.Next == null)
            //        {
            //            list.Remove(node);
            //            list.AddFirst(node);

            //            ////#######
            //            if (enableSkip)
            //            {
            //                counter %= cycleSize;
            //                if (counter == 0)
            //                {
            //                    counter = cycleSize;
            //                }
            //            }
            //            ////#######

            //            //if (node.Next != null)
            //            //{
            //            //    counter = GetIt2(list, counter);
            //            //}
            //        }
            //        else
            //        {
            //            var destination = node.Next ?? throw new InvalidOperationException("Unexpected: should cycle, not reach end");
            //            list.Remove(node);
            //            list.AddAfter(destination, node);
            //        }
            //    }
            //}
            //else
            //{
            //    //var reachedEdge = false;
            //    //var edge = false;
            //    //var skip = -1L;

            //    //for (long counter = 0; counter < movement; counter++)
            //    for (var counter = movement; counter > 0; counter--)
            //    {
            //        if (node.Previous == list.First || node.Previous == null)
            //        {
            //            list.Remove(node);
            //            list.AddLast(node);

            //            //reachedEdge = true;

            //            //if (counter > list.Count + 1)
            //            //{

            //            //}

            //            //counter = GetIt2(list, counter);

            //            //if (node.Previous != null)
            //            //{
            //            //    counter = GetIt2(list, counter);
            //            //}

            //            //edge = true;
            //            //skip = counter % cycleSize; //Math.Clamp(counter % cycleSize, 1, cycleSize); //Math.Max(counter % cycleSize, 1);
            //            //if (skip == 0)
            //            //{
            //            //    skip = cycleSize;
            //            //}

            //            ////#######
            //            if (enableSkip)
            //            {
            //                counter %= cycleSize;
            //                if (counter == 0)
            //                {
            //                    counter = cycleSize;
            //                }
            //            }
            //            ////#######
            //        }
            //        else
            //        {
            //            var destination = node.Previous ?? throw new InvalidOperationException("Unexpected: should cycle, not reach beginning");
            //            list.Remove(node);
            //            list.AddBefore(destination, node);

            //            //edge = false;
            //            //skip = -1;
            //        }

            //        //if (reachedEdge || true)
            //        //{
            //        //    Console.WriteLine($"{counter,4}: {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");
            //        //}
            //    }
            //}
        }
    }

    public static void MoveNumber(Number number, List<Number> numbers)
    {
        // Move the node the number of times indicated by its value!
        // Note that we can forwards or backwards!
        var numOfMoves = Math.Abs(number.Value);
        //var moveForwards = number.Value > 0;
        var dir = number.Value > 0 ? 1 : -1;

        // Get the current index of the number
        var currentIndex = numbers.IndexOf(number);
        if (currentIndex == -1)
        {
            throw new InvalidOperationException("Unexpected: did not find number in list: " + number);
        }

        //Console.WriteLine(string.Join(", ", numbers));

        // Swap the numbers, until we have completed all of our moves
        //var prevIndex = currentIndex;
        for (var i = 0L; i < numOfMoves; i++)
        {
            var prevIndex = currentIndex;
            currentIndex += dir;

            // Loop around if needed
            // rs-todo: can this be done better, is that the trick!?!?
            if (currentIndex == numbers.Count)
            {
                currentIndex = 1;
                numbers.RemoveAt(prevIndex);
                numbers.Insert(currentIndex, number);
            }
            else if (currentIndex == -1)
            {
                currentIndex = numbers.Count - 2;
                numbers.RemoveAt(prevIndex);
                numbers.Insert(currentIndex, number);
            }
            else
            {
                // Swap
                (numbers[currentIndex], numbers[prevIndex]) = (numbers[prevIndex], numbers[currentIndex]);
            }

            

            //Console.WriteLine(string.Join(", ", numbers));

            // rs-todo: I think we can apply the "skip" logic here
        }
    }

    static long SumGroveCoordinates(List<Number> decrypted)
    {
        var indexOfZero = decrypted.FindIndex(n => n.Value == 0);
        var value1 = decrypted[(indexOfZero + 1000) % decrypted.Count].Value;
        var value2 = decrypted[(indexOfZero + 2000) % decrypted.Count].Value;
        var value3 = decrypted[(indexOfZero + 3000) % decrypted.Count].Value;

        return value1 + value2 + value3;
    }

    //static IEnumerable<LinkedListNode<long>> EnumerateNodes(LinkedList<long> list)
    //{
    //    var current = list.First;
    //    while (current != null)
    //    {
    //        yield return current;
    //        current = current.Next;
    //    }
    //}

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
