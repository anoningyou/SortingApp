namespace DataSorter.Abstractions;

public interface ICacheProvider
{
    Task<IDataProvider> GetDataProviderAsync(IEnumerable<DataModel> data);
    void CleanCache();
}
