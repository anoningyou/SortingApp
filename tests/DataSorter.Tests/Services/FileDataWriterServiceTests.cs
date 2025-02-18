using DataSorter.Models;
using DataSorter.Services;
using System.Text;

namespace DataSorter.Tests.Services;
public class FileDataWriterServiceTests
{
    private readonly string _filePath = "test.txt";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenFilePathIsNullOrEmpty(string filePath)
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriterService(filePath));
    }

    [Fact]
    public async Task WriteDataAsync_ShouldWriteDataToFile()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath);

        // Act
        await service.WriteDataAsync(dataModels);

        // Assert
        Assert.True(File.Exists(_filePath));
        var writtenLines = await File.ReadAllLinesAsync(_filePath);
        Assert.Equal(2, writtenLines.Length);
        Assert.Contains("1. Sample text 1", writtenLines);
        Assert.Contains("2. Sample text 2", writtenLines);

        // Cleanup
        File.Delete(_filePath);
    }

    [Fact]
    public async Task WriteDataAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath);

        // Act
        await service.WriteDataAsync(dataModels, cts.Token);

        // Assert
        Assert.True(!File.Exists(_filePath) || string.IsNullOrEmpty(File.ReadAllText(_filePath)));

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public async Task WriteDataAsync_ShouldUseCustomEncoding()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var customEncoding = Encoding.ASCII;
        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath, encoding: customEncoding);

        // Act
        await service.WriteDataAsync(dataModels);

        // Assert
        Assert.True(File.Exists(_filePath));
        var writtenText = await File.ReadAllTextAsync(_filePath, customEncoding);
        Assert.Contains("1. Sample text 1", writtenText);
        Assert.Contains("2. Sample text 2", writtenText);

        // Cleanup
        File.Delete(_filePath);
    }

    [Fact]
    public async Task WriteDataAsync_ShouldRespectBaseCancellationToken()
    {
        // Arrange
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var baseCts = new CancellationTokenSource();
        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath, baseCancelationToken: baseCts.Token);

        // Act
        baseCts.Cancel();
        await service.WriteDataAsync(dataModels);

        Assert.True(!File.Exists(_filePath) || string.IsNullOrEmpty(File.ReadAllText(_filePath)));

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    private async IAsyncEnumerable<DataModel> GetTestDataModels()
    {
        yield return await Task.FromResult(new DataModel("1. Sample text 1"));
        yield return await Task.FromResult(new DataModel("2. Sample text 2"));
    }
}
