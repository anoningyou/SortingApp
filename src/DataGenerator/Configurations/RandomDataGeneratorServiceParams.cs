namespace DataGenerator.Configurations;

public class RandomDataGeneratorServiceParams
{
    public int MinNumber { get; set; } = 1;
    public int MaxNumber { get; set; } = 1000;

    private int _repeatingLinesPercent = 5;
    public int RepeatingLinesPercent
    {
        get => _repeatingLinesPercent;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "RepeatingLinesPercent must be between 0 and 100.");
            _repeatingLinesPercent = value;
        }
    }
    public int MaxLineRepeats { get; set; } = 5;
    public int RepeatingLinesPercentBufferSize { get; set; } = 10;
    public int TextGenerationQueueSize { get; set; } = 1000;
}
