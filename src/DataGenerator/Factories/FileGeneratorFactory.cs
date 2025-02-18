using DataGenerator.Configurations;
using DataGenerator.Services;

namespace DataGenerator.Factories;

public class FileGeneratorFactory(DataGeneratorConfig config) : IFileGeneratorFactory
{
    public IDataGenerator GetDataGenerator(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value should not be empty", nameof(filePath));

        ISizeProvider sizeProvider = new TextFileSizeProvider(config.StringDivider);
        IRandomTextGenerator textGenerator = new RandomTextGeneratorLoremNET(config.MinWords, config.MaxWords);
        IDataProvider dataProvider = new ConcurrentRandomDataGeneratorService(sizeProvider, textGenerator, config.RandomDataGeneratorServiceParams);
        IDataWriter dataWriter = new FileDataWriterService(filePath, config.StringDivider);

        return new DataGenerator(dataProvider, dataWriter);
    }
}
