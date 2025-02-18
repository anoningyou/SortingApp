namespace DataGenerator.Abstractions;

public interface IDataGenerator
{
    Task GenerateDataAsync(long size, CancellationToken cancellationToken = default);
}
