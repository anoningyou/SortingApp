namespace DataSorter.Comparers;

/// <summary>
/// Compares two DataModel objects based on their string and numeric parts.
/// </summary>
/// <param name="divider">The string used to divide the numeric and string parts of the DataModel.</param>
public class DataModelComparer(string divider) : IDataModelComparer
{
    /// <summary>
    /// Compares two DataModel objects.
    /// </summary>
    /// <param name="x">The first DataModel object to compare.</param>
    /// <param name="y">The second DataModel object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of x and y:
    /// </returns>
    public int Compare(DataModel? x, DataModel? y)
    {
        if (x is null && y is null)
            return 0;
        if (x is null)
            return -1;
        if (y is null)
            return 1;

        SplitSpan(x.Value.AsSpan(), divider, out ReadOnlySpan<char> xNumberPart, out ReadOnlySpan<char> xStringPart);
        SplitSpan(y.Value.AsSpan(), divider, out ReadOnlySpan<char> yNumberPart, out ReadOnlySpan<char> yStringPart);

        int cmp = xStringPart.CompareTo(yStringPart, StringComparison.Ordinal);
        if (cmp != 0)
            return cmp;
        else
            return CompareDigitSpans(ref xNumberPart, ref yNumberPart);
    }

    /// <summary>
    /// Compares two ReadOnlySpan<char> objects representing numeric parts.
    /// </summary>
    /// <param name="part1">The first numeric part to compare.</param>
    /// <param name="part2">The second numeric part to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of part1 and part2:
    /// Less than zero if part1 is less than part2.
    /// Zero if part1 equals part2.
    /// Greater than zero if part1 is greater than part2.
    /// </returns>
    private static int CompareDigitSpans(ref ReadOnlySpan<char> part1, ref ReadOnlySpan<char> part2)
    {
        if (part1.Length < part2.Length)
            return -1;
        else if (part1.Length > part2.Length)
            return 1;
        else
            return part1.CompareTo(part2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Splits a ReadOnlySpan<char> into two parts based on the divider.
    /// </summary>
    /// <param name="span">The span to split.</param>
    /// <param name="divider">The string used to divide the span.</param>
    /// <param name="part1">The first part of the split span.</param>
    /// <param name="part2">The second part of the split span.</param>
    /// <exception cref="FormatException">Thrown when the divider is not found in the span.</exception>
    private static void SplitSpan(ReadOnlySpan<char> span, string divider, out ReadOnlySpan<char> part1, out ReadOnlySpan<char> part2)
    {
        int dividerIndex = span.IndexOf(divider.AsSpan());
        if (dividerIndex < 0)
            throw new FormatException($"Divider '{divider}' not found in span.");

        part1 = span[..dividerIndex];
        part2 = span[(dividerIndex + divider.Length)..];
    }
}
