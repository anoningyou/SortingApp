using ConsoleApp.Services;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Starting");
IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

Console.WriteLine("Application started");

if (args.Length == 0)
{
    Console.WriteLine("Please, provide parameters.");
}
else
{
    switch (args[0])
    {
        case "generate":
        case "g":
            {
                Console.WriteLine("File generation start.");
                await GenerationService.GenerateFileAsync(config, args);
                break;
            }
        case "sort":
        case "s":
            {
                Console.WriteLine("File sorting start.");
                await SortingService.SortFileAsync(config, args);
                break;
            }

        default:
            {
                Console.WriteLine($"Unknown cummand {args[0]}");
                break;
            }
    }
}

Console.WriteLine("Done. Press any key to close app.");

Console.ReadLine();
