
using DataGenerator.Configurations;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace DataGenerator.Services;

public class ConcurrentRandomDataGeneratorService : IDataProvider
{
    private readonly ISizeProvider _sizeProvider;
    private readonly IRandomTextGenerator _textGenerator;
    private readonly RandomDataGeneratorServiceParams _param;

    public ConcurrentRandomDataGeneratorService(ISizeProvider sizeProvider, IRandomTextGenerator textGenerator, RandomDataGeneratorServiceParams? param = null)
    {
        _sizeProvider = sizeProvider;
        _textGenerator = textGenerator;
        _param = param ?? new RandomDataGeneratorServiceParams();
    }

    /// <summary>
    /// Generates random data asynchronously.
    /// </summary>
    /// <param name="size">The size of the data to generate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async enumerable of DataModel.</returns>
    public async IAsyncEnumerable<DataModel> GetDataAsync(long size, [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        Random random = new();
        long dataSize = 0;
        CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        DataModel firstData = new(random.Next(_param.MinNumber, _param.MaxNumber), _textGenerator.GenerateText());
        dataSize += _sizeProvider.GetSize(firstData);

        List<(string value, int count)> repeatingLines = [(firstData.Text, 1)];

        long approxDataCount = (long)(size / dataSize);
        int repeatingMaxRandom = (int)Math.Min(100 / _param.RepeatingLinesPercent, approxDataCount);
        bool hasRepeats = false;

        if (!cancellationToken.IsCancellationRequested)
            yield return firstData;

        Channel<(DataModel model, bool isRepeat)> dataModelChannel = GetDataChannel(random, repeatingLines, repeatingMaxRandom, cts.Token);

        while (dataSize < size)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            (DataModel data, bool isRepeat) = await dataModelChannel.Reader.ReadAsync(cancellationToken);
            hasRepeats |= isRepeat;

            dataSize += _sizeProvider.GetSize(data);

            if (dataSize >= size && !hasRepeats)
            {
                data = data with { Text = repeatingLines[random.Next(0, repeatingLines.Count - 1)].value };
            }

            yield return data;
        }

        cts.Cancel();
    }

    private Channel<(DataModel model, bool isRepeat)> GetDataChannel(Random random, List<(string value, int count)> repeatingLines, int repeatingMaxRandom, CancellationToken cancellationToken)
    {
        Channel<(DataModel model, bool isRepeat)> dataModelChannel
            = Channel.CreateBounded<(DataModel model, bool isRepeat)>(new BoundedChannelOptions(_param.TextGenerationQueueSize));

        for (int i = 0; i < Environment.ProcessorCount - 1; i++)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    (DataModel data, bool isRepeat) dataResult = GenerateData(random, dataModelChannel, repeatingLines, repeatingMaxRandom);
                    await dataModelChannel.Writer.WriteAsync(dataResult, cancellationToken);
                }
            });
        }

        return dataModelChannel;
    }

    private (DataModel data, bool isRepeat) GenerateData(
        Random random,
        Channel<(DataModel model, bool isRepeat)> dataModelChannel,
        List<(string value, int count)> repeatingLines,
        int repeatingMaxRandom)
    {
        int number = random.Next(_param.MinNumber, _param.MaxNumber);
        string? text;
        int repeatingRandomNext = random.Next(1, repeatingMaxRandom);
        bool isRepeat = false;
        if (repeatingRandomNext == 1)
        {
            text = GetRepeatingLine(random, repeatingLines);
            isRepeat = true;
        }
        else
        {
            text = _textGenerator.GenerateText();
            if (repeatingRandomNext == repeatingMaxRandom)
            {
                AddRepeatingLineToBuffer(repeatingLines, text);
            }
        }

         return (new DataModel(number, text), isRepeat);  
    }

    private void AddRepeatingLineToBuffer(List<(string value, int count)> repeatingLines, string text)
    {
        lock (repeatingLines)
        {
            repeatingLines.Add((text, 1));
            if (repeatingLines.Count > _param.RepeatingLinesPercentBufferSize)
            {
                repeatingLines.RemoveAt(0);
            }
        }
    }

    private string GetRepeatingLine(Random random, List<(string value, int count)> repeatingLines)
    {
        lock(repeatingLines)
        {
            (string value, int count) repeatingLine = repeatingLines[random.Next(0, repeatingLines.Count - 1)];
            string text = repeatingLine.value;
            repeatingLine.count++;
            if (repeatingLine.count == _param.MaxLineRepeats && repeatingLines.Count > 1)
            {
                repeatingLines.Remove(repeatingLine);
            }

            return text;
        }
    }
}
