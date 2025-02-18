namespace DataSorter.Configurations;

public class SortingParams
{
    public int SortParallelCount { get; set; } = -1;
    public int WriteParallelCount { get; set; } = 1;
    public int LinesPerChunk { get; set; } = 1000000;
    public int SplitChannelSize { get; set; } = 100;
    public int SortChannelSize { get; set; } = 100;
}
