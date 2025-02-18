using System.Text;

namespace DataGenerator.Services;

public class FileDataWriterService : IDataWriter
{
    private readonly string _filePath;
    private readonly string _divider;
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly FileStreamOptions _fileStreamOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDataWriterService"/> class.
    /// </summary>
    /// <param name="filePath">The file path to write data to.</param>
    /// <param name="divider">The divider to use between data fields.</param>
    /// <param name="fileStreamOptions">The file stream options.</param>
    /// <param name="encoding">The encoding to use for writing data.</param>
    public FileDataWriterService(string filePath, string divider = ". ", FileStreamOptions? fileStreamOptions = null, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }
        if (string.IsNullOrEmpty(divider))
        {
            throw new ArgumentException("Divider cannot be null or empty", nameof(divider));
        }
        _filePath = filePath;
        _divider = divider;

        _fileStreamOptions = fileStreamOptions ?? new FileStreamOptions()
        {
            Mode = FileMode.OpenOrCreate,
        };

        _fileStreamOptions.Access = FileAccess.Write;

        if (encoding != null)
        {
            _encoding = encoding;
        }
    }

    /// <summary>
    /// Writes data asynchronously to the file.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public async Task WriteDataAsync(IAsyncEnumerable<DataModel> data, CancellationToken cancellationToken = default)
    {
        EnsureFileDirectory();
        using StreamWriter writer = new(_filePath, _encoding, _fileStreamOptions);
        await foreach (DataModel item in data)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            await writer.WriteLineAsync($"{item.Number}{_divider}{item.Text}");
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
