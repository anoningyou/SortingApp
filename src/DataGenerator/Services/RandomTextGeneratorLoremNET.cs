namespace DataGenerator.Services;

public class RandomTextGeneratorLoremNET : IRandomTextGenerator
{
    private readonly int _minWords;
    private readonly int _maxWords;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomTextGeneratorLoremNET"/> class.
    /// </summary>
    /// <param name="minWords">The minimum number of words to generate.</param>
    /// <param name="maxWords">The maximum number of words to generate.</param>
    /// <exception cref="ArgumentException">Thrown when minWords is less than 1 or maxWords is less than minWords.</exception>
    public RandomTextGeneratorLoremNET(int minWords, int maxWords)
    {
        if (minWords < 1)
        {
            throw new ArgumentException("Min words cannot be less than 1", nameof(minWords));
        }
        if (maxWords < minWords)
        {
            throw new ArgumentException("Max words cannot be less than Min words", nameof(maxWords));
        }

        _minWords = minWords;
        _maxWords = maxWords;
    }

    /// <summary>
    /// Generates random text with a word count between the specified minimum and maximum values.
    /// </summary>
    /// <returns>A string containing the generated text.</returns>
    public string GenerateText() => LoremNET.Lorem.Words(_minWords, _maxWords);
}
