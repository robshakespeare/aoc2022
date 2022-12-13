using static System.Environment;

namespace AoC.Day13;

public partial class Day13Solver : ISolver
{
    public string DayName => "Distress Signal";

    public long? SolvePart1(PuzzleInput input) =>
        input.ToString().Split($"{NewLine}{NewLine}")
            .Select(chunk => chunk.Split(NewLine))
            .Select((pair, index) => (index: index + 1, left: ParsePacket(pair[0]), right: ParsePacket(pair[1])))
            .Where(pair => pair.left.CompareToElement(pair.right) == true)
            .Sum(pair => pair.index);

    public long? SolvePart2(PuzzleInput input)
    {
        var packets = input.ReadLines()
            .Where(line => line != "")
            .Select(ParsePacket)
            .Concat("""
                [[2]]
                [[6]]
                """.ReadLines().Select(line => ParsePacket(line, isDivider: true)))
            .ToArray();

        Array.Sort(packets);

        return packets
            .Select((list, i) => (list, index: i + 1))
            .Where(x => x.list.IsDivider)
            .Aggregate(1, (agg, cur) => agg * cur.index);
    }

    public abstract class Element
    {
        public ListElement? Parent { get; set; }

        /// <summary>
        /// Returns true if they are in the right order (this, i.e. left, is before right),
        /// false if they are in the wrong order, or if neither, returns null meaning continue checking.
        /// </summary>
        public abstract bool? CompareToElement(Element right);
    }

    public class ListElement : Element, IComparable<ListElement>
    {
        private readonly List<Element> _elements = new();

        public IReadOnlyList<Element> Elements => _elements;

        public void AddChild(Element child)
        {
            _elements.Add(child);
            child.Parent = this;
        }

        public int CompareTo(ListElement? other) => CompareToElement(other ?? throw new InvalidOperationException("Unexpected null other")) switch
        {
            true => -1,
            false => 1,
            _ => 0
        };

        public override bool? CompareToElement(Element right) => right switch
        {
            ListElement rightList => CompareLists(Elements, rightList.Elements),
            IntegerElement rightInteger => CompareLists(Elements, new[] {rightInteger}),
            _ => throw new InvalidOperationException("Unexpected list state")
        };

        public static bool? CompareLists(IReadOnlyList<Element> left, IReadOnlyList<Element> right)
        {
            for (var i = 0; i < Math.Max(left.Count, right.Count); i++)
            {
                if (i >= left.Count)
                {
                    return true;
                }

                if (i >= right.Count)
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

        public override string ToString() => $"[{string.Join(",", Elements)}]";
    }

    public class Packet : ListElement
    {
        public bool IsDivider { get; init; }
    }

    public class IntegerElement : Element
    {
        public int Value { get; }

        public IntegerElement(int value) => Value = value;

        public override string ToString() => Value.ToString();

        public override bool? CompareToElement(Element right) => right switch
        {
            IntegerElement rightInteger when Value < rightInteger.Value => true,
            IntegerElement rightInteger when Value > rightInteger.Value => false,
            IntegerElement => null,
            ListElement rightList => ListElement.CompareLists(new[] {this}, rightList.Elements),
            _ => throw new InvalidOperationException("Unexpected integer state")
        };
    }

    static Packet ParsePacket(string line) => ParsePacket(line, false);

    static Packet ParsePacket(string line, bool isDivider)
    {
        var tokens = PartsRegex.Matches(line[1..^1]).Select(match => match.Value);
        var root = new Packet {IsDivider = isDivider};
        ListElement currentList = root;

        foreach (var token in tokens)
            switch (token)
            {
                case "[":
                {
                    // new list
                    var newList = new ListElement();
                    currentList.AddChild(newList);
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
                    currentList.AddChild(new IntegerElement(int.Parse(token)));
                    break;
            }

        return root;
    }

    static readonly Regex PartsRegex = BuildPartsRegex();

    // Read a line, [ means new list, \d means an int, comma means next item, ] means end current list
    [GeneratedRegex(@"\[|\d+|,|\]", RegexOptions.Compiled)]
    private static partial Regex BuildPartsRegex();
}
