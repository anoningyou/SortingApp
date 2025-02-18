namespace DataSorter.Abstractions;

public interface IDataSorter
{
    Task SortDataAsync(CancellationToken cancellationToken = default);
}
