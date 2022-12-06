namespace AoC;

public class PuzzleInput
{
    private readonly string _input;

    public PuzzleInput(string? input)
    {
        // Normalizes the line endings in the puzzle input string, so that all the line endings match the current environment's line endings;
        // and remove all trailing white-space (including trailing line endings)
        _input = (input ?? "").ReplaceLineEndings().TrimEnd();
    }

    /// <summary>
    /// Parses and returns each line in the puzzle input string.
    /// </summary>
    public IEnumerable<string> ReadLines() => _input.ReadLines();

    /// <summary>
    /// Parses and returns each line in the puzzle input string as an Int64.
    /// </summary>
    public IEnumerable<long> ReadLinesAsLongs() => _input.ReadLinesAsLongs();

    /// <summary>
    /// Returns the puzzle input as a string.
    /// </summary>
    public override string ToString() => _input;

    public static implicit operator PuzzleInput(string? input) => new(input);

    public static implicit operator string(PuzzleInput input) => input.ToString();
}
