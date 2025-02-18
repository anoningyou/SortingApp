namespace DataGenerator;

/// <summary>
/// Represents a data generator that uses a data provider to generate data and a data writer to write the data.
/// </summary>
public class DataGenerator(IDataProvider provider, IDataWriter dataWriter) : IDataGenerator
{
    /// <summary>
    /// Generates data asynchronously.
    /// </summary>
    /// <param name="size">The size of the data to generate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GenerateDataAsync(long size, CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<DataModel> data = provider.GetDataAsync(size, cancellationToken);
        await dataWriter.WriteDataAsync(data, cancellationToken);
    }
}
