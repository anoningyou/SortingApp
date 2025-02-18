namespace DataSorter.Abstractions;

public interface IHeapEntryComparer : IComparer<HeapEntry>
{
    public IDataModelComparer DataModelComparer { get; }
}
