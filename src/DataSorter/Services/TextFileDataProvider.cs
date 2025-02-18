namespace DataSorter.Services;

/// <summary>
/// Provides data from a text file.
/// </summary>
/// <param name="filePath">The path to the text file.</param>
/// <param name="baseCancelationToken">The base cancellation token.</param>
public class TextFileDataProvider(string filePath, CancellationToken baseCancelationToken = default) : IDataProvider
{
    /// <summary>
    /// Gets the data from the text file.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An enumerable of DataModel.</returns>
    public IEnumerable<DataModel> GetData(CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            using StreamReader reader = new(filePath);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested && !baseCancelationToken.IsCancellationRequested)
            {
                string? line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    yield return new DataModel(line);
                }
            }
        }
    }
}
