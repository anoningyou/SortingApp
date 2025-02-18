namespace DataGenerator.Abstractions;

public interface IDataWriter
{
    Task WriteDataAsync(IAsyncEnumerable<DataModel> data, CancellationToken cancellationToken = default);
}
