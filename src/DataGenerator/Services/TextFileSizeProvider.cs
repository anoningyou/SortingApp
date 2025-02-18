using DataGenerator.Extensions;

namespace DataGenerator.Services;

public class TextFileSizeProvider : ISizeProvider
{
    private readonly int _dividerByteSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextFileSizeProvider"/> class.
    /// </summary>
    /// <param name="divider">The divider string used in the data model.</param>
    /// <exception cref="ArgumentException">Thrown when the divider is null or empty.</exception>
    public TextFileSizeProvider(string divider)
    {
        if (string.IsNullOrEmpty(divider))
            throw new ArgumentException("Divider cannot be null or empty.", nameof(divider));

        _dividerByteSize = System.Text.Encoding.UTF8.GetByteCount(divider);
    }

    /// <summary>
    /// Gets the size of the specified data model.
    /// </summary>
    /// <param name="data">The data model.</param>
    /// <returns>The size of the data model in bytes.</returns>
    public long GetSize(DataModel data)
    {
        if (data == null)
            return 0;

        return data.Number.Digits()
            + _dividerByteSize
            + System.Text.Encoding.UTF8.GetByteCount(data.Text);
    }
}
