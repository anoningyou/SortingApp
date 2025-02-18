using DataGenerator.Abstractions;
using DataGenerator.Models;
using Moq;

namespace DataGenerator.Tests;
public class DataGeneratorTests
    {
        private readonly Mock<IDataProvider> _mockProvider;
        private readonly Mock<IDataWriter> _mockWriter;
        private readonly DataGenerator _dataGenerator;

        public DataGeneratorTests()
        {
            _mockProvider = new Mock<IDataProvider>();
            _mockWriter = new Mock<IDataWriter>();
            _dataGenerator = new DataGenerator(_mockProvider.Object, _mockWriter.Object);
        }

        [Fact]
        public async Task GenerateDataAsync_ShouldCallProviderAndWriter_WithCorrectParameters()
        {
            // Arrange
            long size = 1000;
            var cancellationToken = CancellationToken.None;
            var mockData = GetMockData();

            _mockProvider
                .Setup(provider => provider.GetDataAsync(size, cancellationToken))
                .Returns(mockData);

            _mockWriter
                .Setup(writer => writer.WriteDataAsync(mockData, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _dataGenerator.GenerateDataAsync(size, cancellationToken);

            // Assert
            _mockProvider.Verify(provider => provider.GetDataAsync(size, cancellationToken), Times.Once);
            _mockWriter.Verify(writer => writer.WriteDataAsync(mockData, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GenerateDataAsync_ShouldPassCancellationToken()
        {
            // Arrange
            long size = 1000;
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var mockData = GetMockData();

            _mockProvider
                .Setup(provider => provider.GetDataAsync(size, cancellationToken))
                .Returns(mockData);

            _mockWriter
                .Setup(writer => writer.WriteDataAsync(mockData, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _dataGenerator.GenerateDataAsync(size, cancellationToken);

            // Assert
            _mockProvider.Verify(provider => provider.GetDataAsync(size, cancellationToken), Times.Once);
            _mockWriter.Verify(writer => writer.WriteDataAsync(mockData, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GenerateDataAsync_ShouldThrowException_WhenProviderThrowsException()
        {
            // Arrange
            long size = 1000;
            var cancellationToken = CancellationToken.None;

            _mockProvider
                .Setup(provider => provider.GetDataAsync(size, cancellationToken))
                .Throws(new System.Exception("Provider exception"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _dataGenerator.GenerateDataAsync(size, cancellationToken));

            _mockProvider.Verify(provider => provider.GetDataAsync(size, cancellationToken), Times.Once);
            _mockWriter.Verify(writer => writer.WriteDataAsync(It.IsAny<IAsyncEnumerable<DataModel>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateDataAsync_ShouldThrowException_WhenWriterThrowsException()
        {
            // Arrange
            long size = 1000;
            var cancellationToken = CancellationToken.None;
            var mockData = GetMockData();

            _mockProvider
                .Setup(provider => provider.GetDataAsync(size, cancellationToken))
                .Returns(mockData);

            _mockWriter
                .Setup(writer => writer.WriteDataAsync(mockData, cancellationToken))
                .ThrowsAsync(new System.Exception("Writer exception"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _dataGenerator.GenerateDataAsync(size, cancellationToken));

            _mockProvider.Verify(provider => provider.GetDataAsync(size, cancellationToken), Times.Once);
            _mockWriter.Verify(writer => writer.WriteDataAsync(mockData, cancellationToken), Times.Once);
        }

        private async IAsyncEnumerable<DataModel> GetMockData()
        {
            yield return await Task.FromResult(new DataModel(1, "Sample text 1"));
            yield return await Task.FromResult(new DataModel(2, "Sample text 2"));
        }

    }