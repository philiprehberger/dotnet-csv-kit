namespace Philiprehberger.CsvKit;

/// <summary>
/// A streaming CSV writer that produces RFC 4180 compliant output.
/// Fields containing the delimiter, quote character, or newlines are automatically quoted.
/// </summary>
public sealed class CsvWriter : IDisposable
{
    private readonly TextWriter _writer;
    private readonly char _delimiter;
    private readonly char _quote;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvWriter"/> with the specified text writer.
    /// </summary>
    /// <param name="writer">The text writer to write CSV data to.</param>
    /// <param name="options">Optional CSV writing options.</param>
    public CsvWriter(TextWriter writer, CsvOptions? options = null)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        var opts = options ?? new CsvOptions();
        _delimiter = opts.Delimiter;
        _quote = opts.QuoteChar;
    }

    /// <summary>
    /// Writes a single row of fields to the output.
    /// </summary>
    /// <param name="fields">The field values to write.</param>
    public void WriteRow(string[] fields)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(fields);

        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0)
                _writer.Write(_delimiter);

            WriteField(fields[i]);
        }

        _writer.WriteLine();
    }

    /// <summary>
    /// Writes a single row of fields to the output.
    /// </summary>
    /// <param name="fields">The field values to write.</param>
    public void WriteRow(IReadOnlyList<string> fields)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(fields);

        for (int i = 0; i < fields.Count; i++)
        {
            if (i > 0)
                _writer.Write(_delimiter);

            WriteField(fields[i]);
        }

        _writer.WriteLine();
    }

    /// <summary>
    /// Flushes the underlying text writer.
    /// </summary>
    public void Flush() => _writer.Flush();

    private void WriteField(string value)
    {
        bool needsQuoting = value.Contains(_delimiter)
            || value.Contains(_quote)
            || value.Contains('\n')
            || value.Contains('\r');

        if (needsQuoting)
        {
            _writer.Write(_quote);
            foreach (var ch in value)
            {
                if (ch == _quote)
                {
                    _writer.Write(_quote);
                    _writer.Write(_quote);
                }
                else
                {
                    _writer.Write(ch);
                }
            }
            _writer.Write(_quote);
        }
        else
        {
            _writer.Write(value);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _writer.Flush();
            _writer.Dispose();
            _disposed = true;
        }
    }
}
