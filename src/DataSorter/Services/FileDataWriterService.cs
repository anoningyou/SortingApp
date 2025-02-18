using System.Text;

namespace DataSorter.Services;

/// <summary>
/// Service for writing data to a file.
/// </summary>
public class FileDataWriterService : IDataWriter
{
    private readonly string _filePath;
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly FileStreamOptions _fileStreamOptions;
    private readonly CancellationToken _baseCancelationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataWriterService"/> class.
    /// </summary>
    /// <param name="filePath">The path of the file to write to.</param>
    /// <param name="fileStreamOptions">Optional file stream options.</param>
    /// <param name="encoding">Optional encoding for the file.</param>
    /// <param name="baseCancelationToken">Optional base cancellation token.</param>
    /// <exception cref="ArgumentException">Thrown when the file path is null or empty.</exception>
    public FileDataWriterService(string filePath, FileStreamOptions? fileStreamOptions = null, Encoding? encoding = null, CancellationToken baseCancelationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }
        _filePath = filePath;

        _fileStreamOptions = fileStreamOptions ?? new FileStreamOptions()
        {
            Mode = FileMode.OpenOrCreate,
        };

        _fileStreamOptions.Access = FileAccess.Write;

        if (encoding != null)
        {
            _encoding = encoding;
        }
        _baseCancelationToken = baseCancelationToken;
    }

    /// <summary>
    /// Writes data asynchronously to the file.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public async Task WriteDataAsync(IAsyncEnumerable<DataModel> data, CancellationToken cancellationToken = default)
    {
        EnsureFileDirectory();
        using StreamWriter writer = new(_filePath, _encoding, _fileStreamOptions);
        await foreach (DataModel item in data)
        {
            if (cancellationToken.IsCancellationRequested || _baseCancelationToken.IsCancellationRequested)
            {
                break;
            }
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(item.Value);
        }
    }

    /// <summary>
    /// Writes data asynchronously to the file.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public async Task WriteDataAsync(IEnumerable<DataModel> data, CancellationToken cancellationToken = default)
    {
        EnsureFileDirectory();
        using StreamWriter writer = new(_filePath, _encoding, _fileStreamOptions);
        foreach (DataModel item in data)
        {
            if (cancellationToken.IsCancellationRequested || _baseCancelationToken.IsCancellationRequested)
            {
                break;
            }
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(item.Value);
        }
    }

    private void EnsureFileDirectory()
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
