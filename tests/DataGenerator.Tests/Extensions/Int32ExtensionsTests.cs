using DataGenerator.Extensions;

namespace DataGenerator.Tests.Extensions;
public class Int32ExtensionsTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(5, 1)]
    [InlineData(9, 1)]
    [InlineData(10, 2)]
    [InlineData(99, 2)]
    [InlineData(100, 3)]
    [InlineData(999, 3)]
    [InlineData(1000, 4)]
    [InlineData(9999, 4)]
    [InlineData(10000, 5)]
    [InlineData(99999, 5)]
    [InlineData(100000, 6)]
    [InlineData(999999, 6)]
    [InlineData(1000000, 7)]
    [InlineData(9999999, 7)]
    [InlineData(10000000, 8)]
    [InlineData(99999999, 8)]
    [InlineData(100000000, 9)]
    [InlineData(999999999, 9)]
    [InlineData(1000000000, 10)]
    [InlineData(int.MaxValue, 10)] // 2147483647
    [InlineData(-1, 2)]
    [InlineData(-9, 2)]
    [InlineData(-10, 3)]
    [InlineData(-99, 3)]
    [InlineData(-100, 4)]
    [InlineData(-999, 4)]
    [InlineData(-1000, 5)]
    [InlineData(-9999, 5)]
    [InlineData(-10000, 6)]
    [InlineData(-99999, 6)]
    [InlineData(-100000, 7)]
    [InlineData(-999999, 7)]
    [InlineData(-1000000, 8)]
    [InlineData(-9999999, 8)]
    [InlineData(-10000000, 9)]
    [InlineData(-99999999, 9)]
    [InlineData(-100000000, 10)]
    [InlineData(-999999999, 10)]
    [InlineData(-1000000000, 11)]
    [InlineData(int.MinValue, 11)] // -2147483648
    public void Digits_ShouldReturnCorrectDigitCount(int number, int expectedDigits)
    {
        // Act
        int actualDigits = number.Digits();

        // Assert
        Assert.Equal(expectedDigits, actualDigits);
    }
}
