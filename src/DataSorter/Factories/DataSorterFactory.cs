using DataSorter.Comparers;
using DataSorter.Configurations;
using DataSorter.Services;

namespace DataSorter.Factories;
public class DataSorterFactory(DataSorterConfig config) : IDataSorterFactory
{
    public IDataSorter CreateDataSorter(string srceFilePath, string dstFilePath)
    {
        if (string.IsNullOrWhiteSpace(srceFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(srceFilePath));
        }
        if (string.IsNullOrWhiteSpace(dstFilePath))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(dstFilePath));
        }
        TextFileDataProvider dataProvider = new(srceFilePath);
        FileDataWriterService dataWriter = new (dstFilePath);
        FileSystemCacheProvider cacheProvider = new(config.TempDirectoryPath);
        IHeapEntryComparer heapEntryComparer = new HeapEntryComparer(new DataModelComparer(config.StringDivider));

        return new DataSorter(dataProvider, dataWriter, cacheProvider, heapEntryComparer, config.SortingParams);
    }
}
