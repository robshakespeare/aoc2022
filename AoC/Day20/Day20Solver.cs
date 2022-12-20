using System.Collections.Generic;

namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public long? SolvePart1(PuzzleInput input) => SumGroveCoordinates(Decrypt(input));

    public long? SolvePart2(PuzzleInput input) => SumGroveCoordinates(Decrypt(input, numOfCycles: 10, decryptionKey: 811589153));

    public static IList<long> Decrypt(string input, int numOfCycles = 1, int decryptionKey = 1)
    {
        var encrypted = new LinkedList<long>(input.ReadLinesAsLongs().Select(n => n * decryptionKey));
        var originalOrder = EnumerateNodes(encrypted).ToReadOnlyArray();

        const bool enableSkip = true;

        if (!enableSkip && numOfCycles > 1)
        {
            throw new InvalidOperationException("not until optimized works!");
        }

        for (var i = 0; i < numOfCycles; i++)
        {
            ApplyMixingCycle(encrypted, originalOrder, enableSkip);
        }

        return encrypted.ToList();
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
        var cycleSize = list.Count - 1L;

        foreach (var node in originalOrder)
        {
            // Move the node the number of times indicated by its value!
            // Note that we can forwards or backwards!
            var movement = Math.Abs(node.Value);
            var moveForwards = node.Value > 0;

            if (moveForwards)
            {
                //for (long counter = 0; counter < movement; counter++)
                for (var counter = movement; counter > 0; counter--)
                {
                    if (node.Next == list.Last || node.Next == null)
                    {
                        list.Remove(node);
                        list.AddFirst(node);

                        ////#######
                        if (enableSkip)
                        {
                            counter %= cycleSize;
                            if (counter == 0)
                            {
                                counter = cycleSize;
                            }
                        }
                        ////#######

                        //if (node.Next != null)
                        //{
                        //    counter = GetIt2(list, counter);
                        //}
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
                //var reachedEdge = false;
                //var edge = false;
                //var skip = -1L;

                //for (long counter = 0; counter < movement; counter++)
                for (var counter = movement; counter > 0; counter--)
                {
                    if (node.Previous == list.First || node.Previous == null)
                    {
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

                        //edge = true;
                        //skip = counter % cycleSize; //Math.Clamp(counter % cycleSize, 1, cycleSize); //Math.Max(counter % cycleSize, 1);
                        //if (skip == 0)
                        //{
                        //    skip = cycleSize;
                        //}

                        ////#######
                        if (enableSkip)
                        {
                            counter %= cycleSize;
                            if (counter == 0)
                            {
                                counter = cycleSize;
                            }
                        }
                        ////#######
                    }
                    else
                    {
                        var destination = node.Previous ?? throw new InvalidOperationException("Unexpected: should cycle, not reach beginning");
                        list.Remove(node);
                        list.AddBefore(destination, node);

                        //edge = false;
                        //skip = -1;
                    }

                    //if (reachedEdge || true)
                    //{
                    //    Console.WriteLine($"{counter,4}: {string.Join(", ", list)} -- {edge} {(edge ? skip : null)}");
                    //}
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
