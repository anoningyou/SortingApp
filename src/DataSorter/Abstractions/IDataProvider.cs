namespace DataSorter.Abstractions;

public interface IDataProvider
{
    IEnumerable<DataModel> GetData(CancellationToken cancellationToken = default);
}
