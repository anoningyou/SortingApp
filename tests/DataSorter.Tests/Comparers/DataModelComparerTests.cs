using DataSorter.Comparers;
using DataSorter.Models;

namespace DataSorter.Tests.Comparers;

public class DataModelComparerTests
{
    private readonly string _divider = ". ";
    private readonly DataModelComparer _comparer;

    public DataModelComparerTests()
    {
        _comparer = new DataModelComparer(_divider);
    }

    [Fact]
    public void Compare_BothNull_ShouldReturnZero()
    {
        // Act
        int result = _comparer.Compare(null, null);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_FirstNull_ShouldReturnMinusOne()
    {
        // Arrange
        var dataModel = new DataModel("1. Sample text");

        // Act
        int result = _comparer.Compare(null, dataModel);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Compare_SecondNull_ShouldReturnOne()
    {
        // Arrange
        var dataModel = new DataModel("1. Sample text");

        // Act
        int result = _comparer.Compare(dataModel, null);

        // Assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData("1. Apple", "1. Apple", 0)]
    [InlineData("1. Apple", "2. Apple", -1)]
    [InlineData("1. Apple", "1. Banana", -1)]
    [InlineData("1. Banana", "1. Apple", 1)]
    [InlineData("10. Apple", "2. Apple", 1)]
    [InlineData("2. Apple", "10. Apple", -1)]
    public void Compare_ShouldCompareCorrectly(string value1, string value2, int expected)
    {
        // Arrange
        var dataModel1 = new DataModel(value1);
        var dataModel2 = new DataModel(value2);

        // Act
        int result = _comparer.Compare(dataModel1, dataModel2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compare_InvalidDivider_ShouldThrowFormatException()
    {
        // Arrange
        var invalidComparer = new DataModelComparer("|");
        var dataModel1 = new DataModel("1. Apple");
        var dataModel2 = new DataModel("2. Banana");

        // Act & Assert
        Assert.Throws<FormatException>(() => invalidComparer.Compare(dataModel1, dataModel2));
    }

    [Fact]
    public void Compare_InvalidDataFormat_ShouldThrowFormatException()
    {
        // Arrange
        var dataModel1 = new DataModel("1Apple");
        var dataModel2 = new DataModel("2Banana");

        // Act & Assert
        Assert.Throws<FormatException>(() => _comparer.Compare(dataModel1, dataModel2));
    }
}
