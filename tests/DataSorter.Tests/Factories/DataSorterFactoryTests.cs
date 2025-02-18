using DataSorter.Abstractions;
using DataSorter.Configurations;
using DataSorter.Factories;

namespace DataSorter.Tests.Factories;
public class DataSorterFactoryTests
{
    private readonly DataSorterConfig _config;
    private readonly DataSorterFactory _factory;

    public DataSorterFactoryTests()
    {
        _config = new DataSorterConfig
        {
            StringDivider = ". ",
            TempDirectoryPath = "temp",
            SortingParams = new SortingParams
            {
                SortParallelCount = 4,
                WriteParallelCount = 2,
                LinesPerChunk = 1000,
                SplitChannelSize = 10,
                SortChannelSize = 10
            }
        };
        _factory = new DataSorterFactory(_config);
    }

    [Fact]
    public void CreateDataSorter_ShouldReturnDataSorterInstance()
    {
        // Arrange
        string srcFilePath = "source.txt";
        string dstFilePath = "destination.txt";

        // Act
        IDataSorter dataSorter = _factory.CreateDataSorter(srcFilePath, dstFilePath);

        // Assert
        Assert.NotNull(dataSorter);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateDataSorter_ShouldThrowArgumentException_WhenSrcFilePathIsNullOrEmpty(string srcFilePath)
    {
        // Arrange
        string dstFilePath = "destination.txt";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateDataSorter(srcFilePath, dstFilePath));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateDataSorter_ShouldThrowArgumentException_WhenDstFilePathIsNullOrEmpty(string dstFilePath)
    {
        // Arrange
        string srcFilePath = "source.txt";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateDataSorter(srcFilePath, dstFilePath));
    }
}
