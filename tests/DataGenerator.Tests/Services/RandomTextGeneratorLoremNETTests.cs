using DataGenerator.Services;

namespace DataGenerator.Tests.Services;
public class RandomTextGeneratorLoremNETTests
{
    [Theory]
    [InlineData(1, 5)]
    [InlineData(3, 10)]
    [InlineData(5, 15)]
    public void GenerateText_ShouldReturnTextWithWordCountWithinRange(int minWords, int maxWords)
    {
        var textGenerator = new RandomTextGeneratorLoremNET(minWords, maxWords);

        string result = textGenerator.GenerateText();

        Assert.NotNull(result);
        var wordCount = result.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        Assert.InRange(wordCount, minWords, maxWords);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenMinWordsIsLessThanOne()
    {
        Assert.Throws<ArgumentException>(() => new RandomTextGeneratorLoremNET(0, 5));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenMaxWordsIsLessThanMinWords()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new RandomTextGeneratorLoremNET(5, 3));
    }
}