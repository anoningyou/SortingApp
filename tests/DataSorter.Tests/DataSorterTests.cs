using DataSorter.Abstractions;
using DataSorter.Comparers;
using DataSorter.Configurations;
using DataSorter.Models;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DataSorter.Tests;

public class DataSorterTests
{
    private readonly Mock<IDataProvider> _mockProvider;
    private readonly Mock<IDataWriter> _mockWriter;
    private readonly Mock<ICacheProvider> _mockCacheProvider;
    private readonly Mock<IHeapEntryComparer> _mockHeapEntryComparer;
    private readonly SortingParams _sortingParams;
    private readonly DataSorter _dataSorter;

    public DataSorterTests()
    {
        _mockProvider = new Mock<IDataProvider>();
        _mockWriter = new Mock<IDataWriter>();
        _mockCacheProvider = new Mock<ICacheProvider>();
        _mockCacheProvider.Setup(p => p.GetDataProviderAsync(It.IsAny<IEnumerable<DataModel>>()))
            .ReturnsAsync((IEnumerable<DataModel> input) =>
            {
                var provider = new Mock<IDataProvider>();
                provider.Setup(p => p.GetData(It.IsAny<CancellationToken>())).Returns(input);
                return provider.Object;
            });

        _mockHeapEntryComparer = new Mock<IHeapEntryComparer>();
        _sortingParams = new SortingParams
        {
            SortParallelCount = 4,
            WriteParallelCount = 2,
            LinesPerChunk = 1000,
            SplitChannelSize = 10,
            SortChannelSize = 10
        };
        _dataSorter = new DataSorter(
            _mockProvider.Object,
            _mockWriter.Object,
            _mockCacheProvider.Object,
            _mockHeapEntryComparer.Object,
            _sortingParams);
    }

    [Fact]
    public async Task SortDataAsync_ShouldCallProviderAndWriter_WithCorrectParameters()
    {
        // Arrange
        var dataModels = GetTestDataModels();
        _mockProvider.Setup(p => p.GetData(It.IsAny<CancellationToken>())).Returns(dataModels);

        // Act
        await _dataSorter.SortDataAsync();

        // Assert
        _mockProvider.Verify(p => p.GetData(It.IsAny<CancellationToken>()), Times.Once);
        _mockWriter.Verify(w => w.WriteDataAsync(It.IsAny<IEnumerable<DataModel>>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheProvider.Verify(c => c.CleanCache(), Times.Once);
    }

    [Fact]
    public async Task SortDataAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var dataModels = GetTestDataModels();
        _mockProvider.Setup(p => p.GetData(It.IsAny<CancellationToken>())).Returns(dataModels);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => _dataSorter.SortDataAsync(cts.Token));

        _mockProvider.Verify(p => p.GetData(It.IsAny<CancellationToken>()), Times.Once);
        _mockWriter.Verify(w => w.WriteDataAsync(It.IsAny<IEnumerable<DataModel>>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockCacheProvider.Verify(c => c.CleanCache(), Times.Never);
    }

    [Fact]
    public async Task SortDataAsync_ShouldSortDataCorrectly()
    {
        // Arrange
        var dataModels = GetTestDataModels();
        var comparer = new DataModelComparer(". ");
        _mockProvider.Setup(p => p.GetData(It.IsAny<CancellationToken>())).Returns(dataModels);
        _mockHeapEntryComparer.Setup(c => c.DataModelComparer).Returns(comparer);

        // Act
        await _dataSorter.SortDataAsync();

        // Assert
        _mockProvider.Verify(p => p.GetData(It.IsAny<CancellationToken>()), Times.Once);
        _mockWriter.Verify(w =>
            w.WriteDataAsync(
                It.Is<IEnumerable<DataModel>>(d =>
                    d.SequenceEqual(dataModels.Order(comparer), new EqualityComparerDataModel(". ")))
                , It.IsAny<CancellationToken>()),
          Times.Once);

        _mockCacheProvider.Verify(c => c.CleanCache(), Times.Once);
    }

    private IEnumerable<DataModel> GetTestDataModels()
    {
        return new List<DataModel>
            {
                new DataModel("2. Sample text 2"),
                new DataModel("1. Sample text 1")
            };
    }

    private class EqualityComparerDataModel : System.Collections.Generic.IEqualityComparer<DataModel>
    {
        private readonly DataModelComparer _comparer;

        public EqualityComparerDataModel(string delimiter)
        {
            _comparer = new DataModelComparer(delimiter);
        }
        public bool Equals(DataModel? x, DataModel? y)
        {
            return _comparer.Compare(x, y) == 0;
        }

        public int GetHashCode([DisallowNull] DataModel obj)
        {
            throw new NotImplementedException();
        }
    }
}
