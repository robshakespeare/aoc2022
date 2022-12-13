using static System.Environment;

namespace AoC.Day13;

public class Day13Solver : ISolver
{
    public string DayName => "Distress Signal";

    public long? SolvePart1(PuzzleInput input)
    {
        var pairs = ParsePairs(input);

        // Process the pairs!!
        var correctPairs = new List<Pair>();

        foreach (var pair in pairs)
        {
            if (pair.LeftList.CompareToElement(pair.RightList) == true)
            {
                correctPairs.Add(pair);
            }
        }

        return correctPairs.Sum(x => x.Index);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var dividers = """
            [[2]]
            [[6]]
            """.ReadLines().Select(ParseLine).ToArray();
        dividers[0].IsDivider = dividers[1].IsDivider = true; // rs-todo: do better!!!

        var listElements = input.ReadLines()
            .Where(line => line != "")
            .Select(ParseLine)
            .Concat(dividers)
            .ToArray();

        Array.Sort(listElements);

        return listElements
            .Select((list, i) => (list, index: i + 1))
            .Where(x => x.list.IsDivider)
            .Aggregate(1, (agg, cur) => agg * cur.index);
    }

    //public class ListElementComparer : IComparer<ListElement>
    //{
        
    //}

    // Packet
    // Packet data consists of lists and integers

    public record Pair(int Index, ListElement LeftList, ListElement RightList);

    // Element can either be List or Integer

    public abstract class Element
    {
        public int Level { get; private set; }
        public ListElement? Parent { get; private set; }

        public virtual void SetParent(ListElement parent)
        {
            Parent = parent;
            Level = parent.Level + 1;
        }

        public abstract bool? CompareToElement(Element right);
    }

    public class ListElement : Element, IComparable<ListElement>
    {
        public bool IsDivider { get; set; }

        private readonly List<Element> _elements = new();

        public IReadOnlyList<Element> Elements => _elements;

        public void AddChild(Element child)
        {
            _elements.Add(child);
            child.SetParent(this);
        }

        public int CompareTo(ListElement? other)
        {
            // Less than zero	The current instance precedes the object specified by the CompareTo method in the sort order.
            // Zero This current instance occurs in the same position in the sort order as the object specified by the CompareTo method.
            // Greater than zero This current instance follows the object specified by the CompareTo method in the sort order.

            var compare = CompareToElement(other);

            return compare switch
            {
                true => -1,
                false => 1,
                _ => 0
            };
        }

        public override string ToString() => $"[{string.Join(",", Elements)}]";

        public override bool? CompareToElement(Element right)
        {
            return right switch
            {
                ListElement rightList => CompareLists(Elements, rightList.Elements),
                IntegerElement rightInteger => CompareLists(Elements, new[] {rightInteger}),
                _ => throw new InvalidOperationException("Unexpected list state")
            };
        }

        public static bool? CompareLists(IReadOnlyList<Element> left, IReadOnlyList<Element> right)
        {
            for (var i = 0; i < Math.Max(left.Count, right.Count); i++)
            {
                //if (i >= left.Count && left.Count == right.Count)
                //{
                //    return null;
                //}

                if (i >= left.Count) // && left.Count != right.Count)
                {
                    return true;
                }

                if (i >= right.Count) // && left.Count != right.Count)
                {
                    return false;
                }

                var compare = left[i].CompareToElement(right[i]);
                if (compare != null)
                {
                    return compare;
                }
            }

            return null;
        }
    }

    public class IntegerElement : Element
    {
        public int Value { get; }

        public IntegerElement(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();

        public override bool? CompareToElement(Element right)
        {
            return right switch
            {
                IntegerElement rightInteger when Value < rightInteger.Value => true,
                IntegerElement rightInteger when Value > rightInteger.Value => false,
                IntegerElement => null,
                ListElement rightList => ListElement.CompareLists(new[] {this}, rightList.Elements),
                _ => throw new InvalidOperationException("Unexpected integer state")
            };
        }
    }

    static IReadOnlyList<Pair> ParsePairs(string input) => input.Split($"{NewLine}{NewLine}")
        .Select(chunk => chunk.Split(NewLine))
        .Select((pair, index) => new Pair(index + 1, ParseLine(pair[0]), ParseLine(pair[1])))
        .ToArray();

    // Read a line, [ means new list, \d means an int, comma means next item, ] means end current list

    static ListElement ParseLine(string line)
    {
        var parts = PartsRegex.Matches(line[1..^1]).Select(match => match.Value);
        var rootList = new ListElement();
        var currentList = rootList;

        foreach (var part in parts)
        {
            switch (part)
            {
                case "[":
                {
                    // new list
                    var newList = new ListElement();
                    newList.SetParent(currentList);
                    currentList.AddChild(newList); // rs-todo: yuk, needs tidy!
                    currentList = newList;
                    break;
                }
                case ",":
                    // next item, we don't need to do anything
                    break;
                case "]":
                    // end current list
                    currentList = currentList.Parent ?? throw new InvalidOperationException("Unexpected end current list when there is no parent");
                    break;
                default:
                    // assume this must be an integer
                    currentList.AddChild(new IntegerElement(int.Parse(part)));
                    break;
            }
        }

        return rootList;
    }

    static readonly Regex PartsRegex = new(@"\[|\d+|,|\]", RegexOptions.Compiled);
}
