using DataSorter.Abstractions;
using DataSorter.Configurations;
using DataSorter.Factories;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ConsoleApp.Services;
internal class SortingService
{
    public static async Task SortFileAsync(IConfiguration config, string[] args)
    {
        if (args.Length < 3)
            Console.WriteLine("Please provide file path parameters.");

        DataSorterConfig dataGeneratorConfig
                = config.GetSection(nameof(DataSorterConfig)).Get<DataSorterConfig>() ?? new DataSorterConfig();
        IDataSorterFactory fileGeneratorFactory = new DataSorterFactory(dataGeneratorConfig);
        IDataSorter fileSorter = fileGeneratorFactory.CreateDataSorter(args[1], args[2]);

        Console.WriteLine("Start file sorting");
        long startTime = Stopwatch.GetTimestamp();
        await fileSorter.SortDataAsync();

        TimeSpan delta = Stopwatch.GetElapsedTime(startTime);

        Console.WriteLine($"File sorting completed by {delta}");

    }
}
