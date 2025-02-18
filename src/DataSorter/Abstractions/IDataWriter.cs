namespace DataSorter.Abstractions;

public interface IDataWriter
{
    Task WriteDataAsync(IAsyncEnumerable<DataModel> data, CancellationToken cancellationToken = default);
    Task WriteDataAsync(IEnumerable<DataModel> data, CancellationToken cancellationToken = default);
}
