using static Crayon.Output;

namespace AoC;

public class InputLoader
{
    private readonly Type _solverType;
    private readonly Lazy<PuzzleInput> _part1;
    private readonly Lazy<PuzzleInput> _part2;

    public PuzzleInput PuzzleInputPart1 => _part1.Value;

    public PuzzleInput PuzzleInputPart2 => _part2.Value;

    public InputLoader(ISolver solver)
    {
        _solverType = solver.GetType();
        _part1 = new Lazy<PuzzleInput>(() => LoadInput(GetInputResourceName($"input-day{solver.DayNumber}.txt")));
        _part2 = new Lazy<PuzzleInput>(() =>
        {
            var part2ResourceName = GetInputResourceName($"input-day{solver.DayNumber}-part-2.txt");

            if (_solverType.Assembly.GetManifestResourceInfo(part2ResourceName) == null)
            {
                return _part1.Value;
            }

            Console.WriteLine(Blue("Part 2 has separate input file"));
            return LoadInput(part2ResourceName);
        });
    }

    private string GetInputResourceName(string fileName) => $"{_solverType.Namespace}.{fileName}";

    private string LoadInput(string resourceName)
    {
        using var resourceStream = _solverType.Assembly.GetManifestResourceStream(resourceName)
                                   ?? throw new InvalidOperationException($"Input file `{resourceName}` not found.");
        using var streamReader = new StreamReader(resourceStream);
        var input = streamReader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Bright.Red($"[WARNING] Input file `{Bright.Cyan(resourceName)}` is empty"));
        }

        return input;
    }
}
