using DataGenerator.Abstractions;
using DataGenerator.Configurations;
using DataGenerator.Factories;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ConsoleApp.Services;
internal static class GenerationService
{
    public static async Task GenerateFileAsync(IConfiguration config, string[] args)
    {
        if (args.Length < 3)
            Console.WriteLine("Please provide file path and size parameters.");
        else if (long.TryParse(args[2], out long length))
        {
            DataGeneratorConfig dataGeneratorConfig
                = config.GetSection(nameof(DataGeneratorConfig)).Get<DataGeneratorConfig>() ?? new DataGeneratorConfig();
            IFileGeneratorFactory fileGeneratorFactory = new FileGeneratorFactory(dataGeneratorConfig);
            IDataGenerator fileGenerator = fileGeneratorFactory.GetDataGenerator(args[1]);

            Console.WriteLine("Start file generation");
            long startTime = Stopwatch.GetTimestamp();

            await fileGenerator.GenerateDataAsync(length);

            TimeSpan delta = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"File generation completed by {delta}");
        }
        else
            Console.WriteLine("Please provide correct size parameter.");
    }
}
