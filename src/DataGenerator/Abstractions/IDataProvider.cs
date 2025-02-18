namespace DataGenerator.Abstractions;

public interface IDataProvider
{
    IAsyncEnumerable<DataModel> GetDataAsync(long size, CancellationToken cancellationToken = default);
}
