using DataGenerator.Abstractions;
using DataGenerator.Configurations;
using DataGenerator.Factories;

namespace DataGenerator.Tests.Factories;

public class FileGeneratorFactoryTests
{
    private readonly DataGeneratorConfig _config;
    private readonly FileGeneratorFactory _factory;

    public FileGeneratorFactoryTests()
    {
        _config = new DataGeneratorConfig
        {
            StringDivider = ". ",
            MinWords = 1,
            MaxWords = 10,
            RandomDataGeneratorServiceParams = new RandomDataGeneratorServiceParams
            {
                MinNumber = 1,
                MaxNumber = 100,
                RepeatingLinesPercent = 5,
                MaxLineRepeats = 10,
                RepeatingLinesPercentBufferSize = 100,
                TextGenerationQueueSize = 1000
            }
        };
        _factory = new FileGeneratorFactory(_config);
    }

    [Fact]
    public void GetDataGenerator_ShouldReturnDataGeneratorInstance()
    {
        string filePath = "test.txt";

        IDataGenerator dataGenerator = _factory.GetDataGenerator(filePath);

        Assert.NotNull(dataGenerator);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GetDataGenerator_ShouldThrowArgumentException_WhenFilePathIsNullOrEmpty(string filePath)
    {
        Assert.Throws<ArgumentException>(() => _factory.GetDataGenerator(filePath));
    }
}