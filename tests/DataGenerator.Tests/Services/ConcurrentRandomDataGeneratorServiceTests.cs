using DataGenerator.Abstractions;
using DataGenerator.Configurations;
using DataGenerator.Models;
using DataGenerator.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataGenerator.Tests.Services;

public class ConcurrentRandomDataGeneratorServiceTests
{
    private readonly Mock<ISizeProvider> _mockSizeProvider;
    private readonly Mock<IRandomTextGenerator> _mockTextGenerator;
    private readonly RandomDataGeneratorServiceParams _params;
    private readonly ConcurrentRandomDataGeneratorService _service;

    public ConcurrentRandomDataGeneratorServiceTests()
    {
        _mockSizeProvider = new Mock<ISizeProvider>();
        _mockTextGenerator = new Mock<IRandomTextGenerator>();
        _params = new RandomDataGeneratorServiceParams
        {
            MinNumber = 1,
            MaxNumber = 100,
            RepeatingLinesPercent = 5,
            MaxLineRepeats = 10,
            RepeatingLinesPercentBufferSize = 100,
            TextGenerationQueueSize = 1000
        };
        _service = new ConcurrentRandomDataGeneratorService(_mockSizeProvider.Object, _mockTextGenerator.Object, _params);
    }

    [Fact]
    public async Task GetDataAsync_ShouldReturnDataModels()
    {
        // Arrange
        long size = 1000;
        _mockSizeProvider.Setup(sp => sp.GetSize(It.IsAny<DataModel>())).Returns(10);
        _mockTextGenerator.Setup(tg => tg.GenerateText()).Returns("Sample text");

        // Act
        var dataModels = _service.GetDataAsync(size);

        // Assert
        await foreach (var dataModel in dataModels)
        {
            Assert.NotNull(dataModel);
            Assert.InRange(dataModel.Number, _params.MinNumber, _params.MaxNumber);
            Assert.Equal("Sample text", dataModel.Text);
        }
    }

    [Fact]
    public async Task GetDataAsync_ShouldCancelWhenTokenIsCancelled()
    {
        long size = 1000;
        _mockSizeProvider.Setup(sp => sp.GetSize(It.IsAny<DataModel>())).Returns(10);
        _mockTextGenerator.Setup(tg => tg.GenerateText()).Returns("Sample text");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await foreach (var _ in _service.GetDataAsync(size, cts.Token))
        {
            Assert.Fail("The method should have been cancelled.");
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetDataAsync_ShouldGenerateRepeatingLines(long size)
    {
        // Arrange
        _mockSizeProvider.Setup(sp => sp.GetSize(It.IsAny<DataModel>())).Returns(1);
        _mockTextGenerator.Setup(tg => tg.GenerateText()).Returns(Guid.NewGuid().ToString());

        // Act
        var dataModels = _service.GetDataAsync(size);

        // Assert
        HashSet<string> strings = new();
        bool hasRepeats = false;
        await foreach (var dataModel in dataModels)
        {
            if (strings.Contains(dataModel.Text))
            {
                hasRepeats = true;
                break;
            }
            strings.Add(dataModel.Text);
        }
        Assert.True(hasRepeats);
    }
}