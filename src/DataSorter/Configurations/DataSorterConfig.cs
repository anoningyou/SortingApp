namespace DataSorter.Configurations;

public class DataSorterConfig
{
    public string StringDivider { get; set; } = ". ";
    public string TempDirectoryPath { get; set; } = "";
    public SortingParams SortingParams { get; set; } = new SortingParams();

}
