namespace AoC;

public static class GeneralExtensions
{
    /// <summary>
    /// Returns the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static int[] ToArray(this Range range) => ToEnumerable(range).ToArray();

    /// <summary>
    /// Enumerates the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static IEnumerable<int> ToEnumerable(this Range range)
    {
        for (var i = range.Start.Value; i < range.End.Value; i++)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Converts the specified enumerable/collection to a read only array.
    /// </summary>
    public static IReadOnlyList<TSource> ToReadOnlyArray<TSource>(this IEnumerable<TSource> source) => Array.AsReadOnly(source.ToArray());

    private static readonly Regex LineEndingsRegex = new(@"\r\n|\n|\r", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes the line endings in the input string, so that all the line endings match the current environment's line endings.
    /// </summary>
    public static string NormalizeLineEndings(this string? value) => LineEndingsRegex.Replace(value ?? "", Environment.NewLine); // rs-todo: replace with inbuilt one

    /// <summary>
    /// Increments the value in the dictionary matching the key by the specified amount, or adds the amount in to the dictionary if no matching key is found.
    /// </summary>
    public static void AddOrIncrement<TKey>(this Dictionary<TKey, long> dictionary, TKey key, long amount) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = 0;
        }

        dictionary[key] += amount;
    }
}
