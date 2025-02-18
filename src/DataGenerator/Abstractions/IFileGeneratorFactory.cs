namespace DataGenerator.Abstractions;

public interface IFileGeneratorFactory
{
    IDataGenerator GetDataGenerator(string filePath);
}
