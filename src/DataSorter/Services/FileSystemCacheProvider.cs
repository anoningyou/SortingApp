using System.Collections.Concurrent;

namespace DataSorter.Services;

/// <summary>
/// Provides caching functionality using the file system.
/// </summary>
public class FileSystemCacheProvider(string directoryPath) : ICacheProvider
{
    private readonly BlockingCollection<string> _files = [];
    private readonly CancellationTokenSource _cleanCancelationTokenSource = new();

    /// <summary>
    /// Cleans the cache by deleting all cached files.
    /// </summary>
    public void CleanCache()
    {
        _cleanCancelationTokenSource.Cancel();
        Thread.Sleep(1000);

        foreach (string file in _files)
        {
            File.Delete(file);
        }
    }

    /// <summary>
    /// Asynchronously gets a data provider for the given data.
    /// </summary>
    /// <param name="data">The data to be provided.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the data provider.</returns>
    public async Task<IDataProvider> GetDataProviderAsync(IEnumerable<DataModel> data)
    {
        string filePath = Path.Combine(directoryPath, $"{Guid.CreateVersion7()}.txt");
        FileDataWriterService fileWriter = new(filePath);
        await fileWriter.WriteDataAsync(data);
        _files.Add(filePath);

        return new TextFileDataProvider(filePath, _cleanCancelationTokenSource.Token);
    }
}
