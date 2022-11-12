namespace AoC;

public class PuzzleInput
{
    private readonly string _input;

    public PuzzleInput(string? input)
    {
        // Normalizes the line endings in the input string, so that all the line endings match the current environment's line endings;
        // and remove all trailing white-space (including trailing line endings)
        _input = input.NormalizeLineEndings().TrimEnd();
    }

    /// <summary>
    /// Parses and returns each line in the puzzle input string.
    /// </summary>
    public IEnumerable<string> ReadLines()
    {
        using var sr = new StringReader(_input);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            yield return line;
        }
    }

    /// <summary>
    /// Parses and returns each line in the input string as an Int64.
    /// </summary>
    public IEnumerable<long> ReadLinesAsLongs() => ReadLines().Select(long.Parse);

    /// <summary>
    /// Returns the puzzle input as a string.
    /// </summary>
    public override string ToString() => _input;

    public static implicit operator PuzzleInput(string? input) => new(input);
}
