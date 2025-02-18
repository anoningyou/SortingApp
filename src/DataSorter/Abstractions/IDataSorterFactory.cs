namespace DataSorter.Abstractions;

public interface IDataSorterFactory
{
    IDataSorter CreateDataSorter(string srceFilePath, string dstFilePath);
}
