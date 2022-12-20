namespace AoC.Day20;

public class Day20Solver : ISolver
{
    public string DayName => "Grove Positioning System";

    public static Action<string> Logger { get; set; } = Console.WriteLine;

    public long? SolvePart1(PuzzleInput input)
    {
        //var originalList = input.ReadLinesAsLongs().Select((value, index) => new Number(value, index + 1));
        var list = new LinkedList<long>(input.ReadLinesAsLongs());

        //var listNodes = EnumerateLinkedList(list).ToArray();

        string DebugList() => string.Join(", ", list); //.Select(x => x.Value));
        var debug = DebugList;
        Logger("Initial arrangement:");
        Logger(debug());
        Logger("");

        var move = 1;
        //var first = () => list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

        foreach (var node in ToNodeArray(list))
        {
            // Move the node the number of times indicated by its value!
            // Note that we can forwards or backwards!

            //var destination = node;

            var movement = node.Value;
            var moveForwards = movement > 0;

            Logger($"Move {move}:");
            //Logger(debug());

            if (moveForwards)
            {
                for (var counter = 0; counter < movement; counter++)
                {
                    if (node.Next == list.Last)
                    {
                        list.Remove(node);
                        list.AddFirst(node);
                    }
                    else
                    {
                        //var destination = node.Previous ?? list.Last ?? throw new InvalidOperationException("Unexpected: no end of list");
                        var destination = node.Next ?? throw new InvalidOperationException("Unexpected: should cycle, not reach end");
                        list.Remove(node);
                        list.AddAfter(destination, node);
                    }

                    //var destination = node.Next ?? list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

                    ////var destination = node.Next;

                    //list.Remove(node);
                    //list.AddAfter(destination, node);

                    ////if (destination != null)
                    ////{
                    ////    list.AddAfter(destination, node);
                    ////}
                    ////else
                    ////{
                    ////    list.AddFirst(node);
                    ////}
                }

                //list.Remove(node);
                //list.AddAfter(destination, node);
            }
            else
            {
                for (var counter = movement; counter < 0; counter++)
                {
                    if (node.Previous == list.First)
                    {
                        list.Remove(node);
                        list.AddLast(node);
                    }
                    else
                    {
                        //var destination = node.Previous ?? list.Last ?? throw new InvalidOperationException("Unexpected: no end of list");
                        var destination = node.Previous ?? throw new InvalidOperationException("Unexpected: should cycle, not reach beginning");
                        list.Remove(node);
                        list.AddBefore(destination, node);
                    }

                    //var destination = node.Previous;

                    

                    //if (destination != null)
                    //{
                    //    list.AddBefore(destination, node);
                    //}
                    //else
                    //{
                    //    list.AddLast(node);
                    //}
                }

                //list.Remove(node);
                //list.AddBefore(destination, node);
            }

            //for (var counter = 0; counter < node.Value.OriginalOrder; counter++)
            //{
            //    //destination = (destination ?? first()).Next; // ?? list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

            //    //var newDestination = destination.Next;

            //    //if (newDestination == null)
            //    //{
            //    //    newDestination = list.First ?? throw new InvalidOperationException("Unexpected: no start of list");
            //    //    //counter += 2;
            //    //}

            //    //destination = newDestination;

            //    //destination = destination.Next ?? list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

            //    var destination = node.Next;

            //    list.Remove(node);
            //    if (destination != null)
            //    {
            //        list.AddAfter(destination, node);
            //    }
            //    else
            //    {
            //        list.AddFirst(node);
            //    }
            //}

            //list.AddAfter(destination ?? first(), node);

            //Console.WriteLine($"After move {move}:");
            Logger(debug());
            Logger("");
            move++;
        }

        var cycledList = list.ToList();

        var index = cycledList.IndexOf(0);

        return index;
    }

    //public long? SolvePart1(PuzzleInput input)
    //{
    //    var originalList = input.ReadLinesAsLongs().Select((value, index) => new Number(value, index + 1));
    //    var list = new LinkedList<Number>(originalList);

    //    var listNodes = EnumerateLinkedList(list).ToArray();

    //    string DebugList() => string.Join(", ", list.Select(x => x.Value));

    //    var debug = DebugList;

    //    Console.WriteLine("Initial arrangement:");
    //    Console.WriteLine(debug());
    //    Console.WriteLine();

    //    var move = 1;

    //    var first = () => list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

    //    foreach (var node in listNodes)
    //    {
    //        // Move the node the number of times indicated by the original order
    //        // Note that the original order is always positive, and we should loop around when needed

    //        //var destination = node;

    //        for (var counter = 0; counter < node.Value.OriginalOrder; counter++)
    //        {
    //            //destination = (destination ?? first()).Next; // ?? list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

    //            //var newDestination = destination.Next;

    //            //if (newDestination == null)
    //            //{
    //            //    newDestination = list.First ?? throw new InvalidOperationException("Unexpected: no start of list");
    //            //    //counter += 2;
    //            //}

    //            //destination = newDestination;

    //            //destination = destination.Next ?? list.First ?? throw new InvalidOperationException("Unexpected: no start of list");

    //            var destination = node.Next;

    //            list.Remove(node);
    //            if (destination != null)
    //            {
    //                list.AddAfter(destination, node);
    //            }
    //            else
    //            {
    //                list.AddFirst(node);
    //            }
    //        }

    //        //list.Remove(node);
    //        //list.AddAfter(destination, node);
    //        //list.AddAfter(destination ?? first(), node);

    //        Console.WriteLine($"After move {move}:");
    //        Console.WriteLine(debug());
    //        Console.WriteLine();
    //        move++;
    //    }

    //    return null;
    //}

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    //public record Number(long Value, int OriginalOrder);

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

    //static Number[] ToArray(LinkedList<Number> list)
    //{
    //    var result = new Number[list.Count];
    //    list.CopyTo(result, 0);
    //    return result;
    //}
}
