using DataGenerator.Extensions;
using DataGenerator.Models;
using DataGenerator.Services;
using System.Text;

namespace DataGenerator.Tests.Services;

public class TextFileSizeProviderTests
{
    [Fact]
    public void GetSize_ShouldReturnZero_WhenDataIsNull()
    {
        var provider = new TextFileSizeProvider(". ");

        long size = provider.GetSize(null);

        Assert.Equal(0, size);
    }

    [Theory]
    [InlineData(123, "Sample text", ". ")]
    [InlineData(456789, "Another example", "-")]
    [InlineData(0, "", "|||")]
    public void GetSize_ShouldCalculateCorrectSize(int number, string text, string divider)
    {
        var provider = new TextFileSizeProvider(divider);
        var data = new DataModel(number, text);

        int numberDigits = number.Digits();
        int dividerByteSize = Encoding.UTF8.GetByteCount(divider);
        int textByteSize = Encoding.UTF8.GetByteCount(text);
        long expectedSize = numberDigits + dividerByteSize + textByteSize;

        long size = provider.GetSize(data);

        Assert.Equal(expectedSize, size);
    }

    [Fact]
    public void Constructor_ShouldCalculateDividerByteSize()
    {
        string divider = "—"; // Em dash, which is 3 bytes in UTF-8
        var provider = new TextFileSizeProvider(divider);

        // Use reflection to access the private field
        var fieldInfo = typeof(TextFileSizeProvider).GetField("_dividerByteSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        int dividerByteSize = (int)fieldInfo.GetValue(provider);

        int expectedByteSize = Encoding.UTF8.GetByteCount(divider);

        Assert.Equal(expectedByteSize, dividerByteSize);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldThrowArgumentException_WhenDividerIsNullOrEmpty(string divider)
    {
        Assert.Throws<ArgumentException>(() => new TextFileSizeProvider(divider));
    }
}