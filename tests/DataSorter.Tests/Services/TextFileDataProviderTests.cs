using DataSorter.Models;
using DataSorter.Services;
using Xunit.Sdk;

namespace DataSorter.Tests.Services;
public class TextFileDataProviderTests
{
    private readonly string _filePath = "test.txt";

    [Fact]
    public void GetData_ShouldReturnDataModels_WhenFileExists()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var lines = new List<string> { "1. Sample text 1", "2. Sample text 2" };
        File.WriteAllLines(_filePath, lines);

        var provider = new TextFileDataProvider(_filePath);

        // Act
        var dataModels = provider.GetData();

        // Assert
        var dataModelList = new List<DataModel>(dataModels);
        Assert.Equal(2, dataModelList.Count);
        Assert.Equal("1. Sample text 1", dataModelList[0].Value);
        Assert.Equal("2. Sample text 2", dataModelList[1].Value);

        // Cleanup
        File.Delete(_filePath);
    }

    [Fact]
    public void GetData_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var provider = new TextFileDataProvider(_filePath);

        // Act
        var dataModels = provider.GetData();

        // Assert
        Assert.Empty(dataModels);
    }

    [Fact]
    public void GetData_ShouldRespectCancellationToken()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var lines = new List<string> { "1. Sample text 1", "2. Sample text 2" };
        File.WriteAllLines(_filePath, lines);

        var cts = new CancellationTokenSource();
        var provider = new TextFileDataProvider(_filePath);

        // Act
        cts.Cancel();
        var dataModels = provider.GetData(cts.Token);

        // Assert
        Assert.Empty(dataModels);

        // Cleanup
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void GetData_ShouldRespectBaseCancellationToken()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var lines = new List<string> { "1. Sample text 1", "2. Sample text 2" };
        File.WriteAllLines(_filePath, lines);

        var baseCts = new CancellationTokenSource();
        var provider = new TextFileDataProvider(_filePath, baseCts.Token);

        // Act
        baseCts.Cancel();
        var dataModels = provider.GetData();

        // Assert
        Assert.Empty(dataModels);

        // Cleanup
        File.Delete(_filePath);
    }
}
