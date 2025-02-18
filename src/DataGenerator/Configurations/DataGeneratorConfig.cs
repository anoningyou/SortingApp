namespace DataGenerator.Configurations;

public class DataGeneratorConfig
{
    public string StringDivider { get; set; } = ". ";
    public int MinWords { get; set; } = 1;
    public int MaxWords { get; set; } = 5;
    public RandomDataGeneratorServiceParams RandomDataGeneratorServiceParams { get; set; } = default!;
}
