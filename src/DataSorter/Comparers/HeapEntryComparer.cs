namespace DataSorter.Comparers;

public class HeapEntryComparer(IDataModelComparer modelComparer) : IHeapEntryComparer
{
    public IDataModelComparer DataModelComparer => modelComparer;

    public int Compare(HeapEntry x, HeapEntry y)
    {
        int cmp = DataModelComparer.Compare(x.Model, y.Model);
        if (cmp == 0)
        {
            return x.ReaderIndex.CompareTo(y.ReaderIndex);
        }
        return cmp;
    }
}
