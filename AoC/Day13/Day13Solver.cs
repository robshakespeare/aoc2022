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
            if (pair.LeftList.CompareTo(pair.RightList) == true)
            {
                correctPairs.Add(pair);
            }
        }

        return correctPairs.Sum(x => x.Index);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

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

        public abstract bool? CompareTo(Element right);
    }

    public class ListElement : Element
    {
        private readonly List<Element> _elements = new();

        public IReadOnlyList<Element> Elements => _elements;

        public void AddChild(Element child)
        {
            _elements.Add(child);
            child.SetParent(this);
        }

        public override string ToString() => $"[{string.Join(",", Elements)}]";

        public override bool? CompareTo(Element right)
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

                var compare = left[i].CompareTo(right[i]);
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

        public override bool? CompareTo(Element right)
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
