using DataSorter.Models;
using DataSorter.Services;

namespace DataSorter.Tests.Services;

public class FileSystemCacheProviderTests
{
    private readonly string _directoryPath = "temp";
    private readonly FileSystemCacheProvider _cacheProvider;

    public FileSystemCacheProviderTests()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
        _cacheProvider = new FileSystemCacheProvider(_directoryPath);
    }

    [Fact]
    public async Task GetDataProviderAsync_ShouldCreateFileAndReturnDataProvider()
    {
        // Arrange
        var dataModels = GetTestDataModels();

        // Act
        var dataProvider = await _cacheProvider.GetDataProviderAsync(dataModels);

        // Assert
        Assert.NotNull(dataProvider);
        Assert.True(Directory.GetFiles(_directoryPath).Length > 0);

        // Cleanup
        _cacheProvider.CleanCache();
    }

    [Fact]
    public void CleanCache_ShouldDeleteAllFiles()
    {
        // Arrange
        var filePath = Path.Combine(_directoryPath, "test.txt");
        var filesbefore = Directory.GetFiles(_directoryPath).Length;
        File.WriteAllText(filePath, "Sample text");

        // Act
        _cacheProvider.CleanCache();

        // Assert
        Assert.True(Directory.GetFiles(_directoryPath).Length == filesbefore);
    }

    [Fact]
    public async Task CleanCache_ShouldNotReturnData()
    {
        // Arrange
        var dataModels = GetTestDataModels();
        var result = await _cacheProvider.GetDataProviderAsync(dataModels);
        _cacheProvider.CleanCache();

        // Act & Assert
        Assert.False(result.GetData().Any());     
    }

    private IEnumerable<DataModel> GetTestDataModels()
    {
        yield return new DataModel("1. Sample text 1");
        yield return new DataModel("2. Sample text 2");
    }
}
