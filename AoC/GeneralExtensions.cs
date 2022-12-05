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

    // rs-todo: comments, and tests
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> buildValue)
        where TKey : notnull => dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = buildValue();
}
