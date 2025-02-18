using DataGenerator.Models;
using DataGenerator.Services;
using System.Text;

namespace DataGenerator.Tests.Services;

public class FileDataWriterServiceTests
{
    private readonly string _filePath = "test.txt";
    private readonly string _divider = ". ";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrowArgumentException_WhenFilePathIsNullOrEmpty(string filePath)
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriterService(filePath, _divider));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldThrowArgumentException_WhenDividerIsNullOrEmpty(string divider)
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriterService(_filePath, divider));
    }

    [Fact]
    public async Task WriteDataAsync_ShouldWriteDataToFile()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath, _divider);

        await service.WriteDataAsync(dataModels);

        Assert.True(File.Exists(_filePath));
        var writtenLines = await File.ReadAllLinesAsync(_filePath);
        Assert.Equal(2, writtenLines.Length);
        Assert.Contains("1. Sample text 1", writtenLines);
        Assert.Contains("2. Sample text 2", writtenLines);

        File.Delete(_filePath);
    }

    [Fact]
    public async Task WriteDataAsync_ShouldCancelWhenTokenIsCancelled()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath, _divider);

        await service.WriteDataAsync(dataModels, cts.Token);
        Assert.True(!File.Exists(_filePath) || string.IsNullOrEmpty(File.ReadAllText(_filePath)));

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public async Task WriteDataAsync_ShouldUseCustomEncoding()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        var customEncoding = Encoding.ASCII;
        var dataModels = GetTestDataModels();
        var service = new FileDataWriterService(_filePath, _divider, encoding: customEncoding);

        await service.WriteDataAsync(dataModels);

        Assert.True(File.Exists(_filePath));
        var writtenText = await File.ReadAllTextAsync(_filePath, customEncoding);
        Assert.Contains("1. Sample text 1", writtenText);
        Assert.Contains("2. Sample text 2", writtenText);

        File.Delete(_filePath);
    }

    private async IAsyncEnumerable<DataModel> GetTestDataModels()
    {
        yield return await Task.FromResult(new DataModel(1, "Sample text 1"));
        yield return await Task.FromResult(new DataModel(2, "Sample text 2"));
    }
}