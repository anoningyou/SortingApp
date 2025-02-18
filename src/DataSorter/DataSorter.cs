using DataSorter.Configurations;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace DataSorter;

public class DataSorter (
    IDataProvider provider,
    IDataWriter dataWriter,
    ICacheProvider cacheProvider,
    IHeapEntryComparer heapEntryComparer,
    SortingParams sortingParams)
    : IDataSorter
{

    /// <summary>
    /// Sorts data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous sort operation.</returns>
    public async Task SortDataAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<DataModel> data = provider.GetData(cancellationToken);

        ChannelReader<List<DataModel>> chunksDataReader = GetSplitDataChannel(data, sortingParams.LinesPerChunk, cancellationToken);

        ChannelReader<List<DataModel>> sortedChunksReader = GetSortedDataChannel(chunksDataReader, sortingParams.SortParallelCount, cancellationToken);

        List<IDataProvider> cachedDataProviders = await GetCachedChuncks(sortedChunksReader, sortingParams.WriteParallelCount, cancellationToken);

        IEnumerable<DataModel> mergedData = MergeSortedChunks(cachedDataProviders, cancellationToken);

        await dataWriter.WriteDataAsync(mergedData, cancellationToken);

        cacheProvider.CleanCache();
    }

    private ChannelReader<List<DataModel>> GetSplitDataChannel(IEnumerable<DataModel> data, int linesPerChunk, CancellationToken token)
    {
        Channel<List<DataModel>> outChannel = Channel.CreateBounded<List<DataModel>>(new BoundedChannelOptions(sortingParams.SplitChannelSize));
        ChannelWriter<List<DataModel>> resultWriter = outChannel.Writer;
        Task.Run(async () =>
        {
            List<DataModel> linesBuffer = new(linesPerChunk);
            foreach (DataModel model in data)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                linesBuffer.Add(model);
                if (linesBuffer.Count >= linesPerChunk)
                {
                    await resultWriter.WriteAsync(linesBuffer, token);
                    linesBuffer = new(linesPerChunk);

                }
            }
            if (!token.IsCancellationRequested && linesBuffer.Count > 0)
            {
                await resultWriter.WriteAsync(linesBuffer, token);
            }
            resultWriter.Complete();
        });

        return outChannel.Reader;
    }

    private ChannelReader<List<DataModel>> GetSortedDataChannel(ChannelReader<List<DataModel>> srcReader, int parallelCount, CancellationToken token)
    {
        Channel<List<DataModel>> outChannel = Channel.CreateBounded<List<DataModel>>(new BoundedChannelOptions(sortingParams.SortChannelSize));
        ChannelWriter<List<DataModel>> dstWriter = outChannel.Writer;

        if (parallelCount <= 0)
            parallelCount = Math.Max(Environment.ProcessorCount - parallelCount, 1);

        List<Task> tasks = new(parallelCount);

        for (int i = 0; i < parallelCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && await srcReader.WaitToReadAsync(token))
                {
                    if (srcReader.TryRead(out List<DataModel>? data))
                    {
                        data.Sort(heapEntryComparer.DataModelComparer);
                        await dstWriter.WriteAsync(data, token);
                    }
                }
            }, token));
        }

        Task.WhenAll(tasks).ContinueWith(_ =>
        {
            dstWriter.Complete();
        });

        return outChannel.Reader;
    }

    private async Task<List<IDataProvider>> GetCachedChuncks(ChannelReader<List<DataModel>> srcReader, int parallelCount, CancellationToken token)
    {
        if (parallelCount <= 0)
            parallelCount = Math.Max(Environment.ProcessorCount - parallelCount, 1);

        List<Task> tasks = new(parallelCount);
        BlockingCollection<IDataProvider> dataProviders = [];

        for (int i = 0; i < parallelCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && await srcReader.WaitToReadAsync(token))
                {
                    if (srcReader.TryRead(out List<DataModel>? data))
                    {
                        IDataProvider cachedData = await cacheProvider.GetDataProviderAsync(data);

                        dataProviders.Add(cachedData);
                    }
                }
            }, token));
        }

        await Task.WhenAll(tasks);

        return [.. dataProviders];
    }

    private IEnumerable<DataModel> MergeSortedChunks(List<IDataProvider> cachedDataProviders, CancellationToken cancellationToken)
    {
        List<IEnumerator<DataModel>> dataStreamEnumerators = cachedDataProviders.Select(p => p.GetData().GetEnumerator()).ToList();
        try
        {
            SortedSet<HeapEntry> heap = new(heapEntryComparer);

            for (int i = 0; i < cachedDataProviders.Count; i++)
            {
                if (dataStreamEnumerators[i].MoveNext())
                {
                    DataModel data = dataStreamEnumerators[i].Current;
                    heap.Add(new HeapEntry(data, i));
                }
            }

            while (heap.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                HeapEntry minEntry = heap.Min;
                heap.Remove(minEntry);

                yield return minEntry.Model;

                int rIndex = minEntry.ReaderIndex;
                if (dataStreamEnumerators[rIndex].MoveNext())
                {
                    DataModel data = dataStreamEnumerators[rIndex].Current;
                    heap.Add(new HeapEntry(data, rIndex));
                }
            }
        }
        finally
        {
            foreach (IEnumerator<DataModel> enumerator in dataStreamEnumerators)
            {
                enumerator.Dispose();
            }
        }
    }
}

