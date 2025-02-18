using DataSorter.Abstractions;
using DataSorter.Comparers;
using DataSorter.Models;
using Moq;

namespace DataSorter.Tests.Comparers;
public class HeapEntryComparerTests
{
    private readonly HeapEntryComparer _comparer;

    public HeapEntryComparerTests()
    {
        var dataModelComparer = new DataModelComparer(". ");
        _comparer = new HeapEntryComparer(dataModelComparer);
    }

    [Theory]

    [InlineData(1, 0, 0, 1)]
    [InlineData(1, 0, 1, 1)]
    [InlineData(1, 1, 0, 1)]

    [InlineData(-1, 0, 0, -1)]
    [InlineData(-1, 0, 1, -1)]
    [InlineData(-1, 1, 0, -1)]

    public void Compare_ShouldReturnTheSameResultOfDataModelComparerIfItNotZero(int compareresult, int readerIndex1, int readerIndex2, int expected)
    {
        // Arrange
        var heapEntry1 = new HeapEntry(new DataModel("1. Apple"), readerIndex1);
        var heapEntry2 = new HeapEntry(new DataModel("1. Apple"), readerIndex2);

        var dataModelComparer = new Mock<IDataModelComparer>();
        dataModelComparer
            .Setup(dc => dc.Compare(heapEntry1.Model, heapEntry2.Model))
            .Returns(compareresult);
        var comparer = new HeapEntryComparer(dataModelComparer.Object);

        // Act
        int result = comparer.Compare(heapEntry1, heapEntry2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, 0, 1, -1)]
    [InlineData(0, 1, 0, 1)]
    public void Compare_ShouldCompareByINdexIfDataModelComparerResultIsZero(int compareresult, int readerIndex1, int readerIndex2, int expected)
    {
        // Arrange
        var heapEntry1 = new HeapEntry(new DataModel("1. Apple"), readerIndex1);
        var heapEntry2 = new HeapEntry(new DataModel("1. Apple"), readerIndex2);

        var dataModelComparer = new Mock<IDataModelComparer>();
        dataModelComparer
            .Setup(dc => dc.Compare(heapEntry1.Model, heapEntry2.Model))
            .Returns(compareresult);
        var comparer = new HeapEntryComparer(dataModelComparer.Object);

        // Act
        int result = _comparer.Compare(heapEntry1, heapEntry2);

        // Assert
        Assert.Equal(expected, result);
    }
}
