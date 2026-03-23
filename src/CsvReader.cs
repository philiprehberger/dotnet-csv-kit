namespace Philiprehberger.CsvKit;

/// <summary>
/// A streaming RFC 4180 compliant CSV parser. Handles quoted fields, embedded quotes,
/// and newlines within quoted fields.
/// </summary>
public sealed class CsvReader : IDisposable
{
    private readonly TextReader _reader;
    private readonly char _delimiter;
    private readonly char _quote;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvReader"/> with the specified text reader.
    /// </summary>
    /// <param name="reader">The text reader to read CSV data from.</param>
    /// <param name="options">Optional CSV parsing options.</param>
    public CsvReader(TextReader reader, CsvOptions? options = null)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        var opts = options ?? new CsvOptions();
        _delimiter = opts.Delimiter;
        _quote = opts.QuoteChar;
    }

    /// <summary>
    /// Reads the next row from the CSV stream.
    /// </summary>
    /// <returns>An array of field values, or null if there are no more rows.</returns>
    public string[]? ReadRow()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        bool inQuotes = false;
        bool fieldStarted = false;
        bool hasData = false;

        while (true)
        {
            int c = _reader.Read();

            if (c == -1)
            {
                if (!hasData && fields.Count == 0 && field.Length == 0)
                    return null;

                fields.Add(field.ToString());
                break;
            }

            hasData = true;
            char ch = (char)c;

            if (inQuotes)
            {
                if (ch == _quote)
                {
                    int next = _reader.Peek();
                    if (next == _quote)
                    {
                        // Escaped quote
                        _reader.Read();
                        field.Append(_quote);
                    }
                    else
                    {
                        // End of quoted field
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(ch);
                }
            }
            else
            {
                if (ch == _quote && !fieldStarted)
                {
                    inQuotes = true;
                    fieldStarted = true;
                }
                else if (ch == _delimiter)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                    fieldStarted = false;
                }
                else if (ch == '\r')
                {
                    int next = _reader.Peek();
                    if (next == '\n')
                        _reader.Read();

                    fields.Add(field.ToString());
                    break;
                }
                else if (ch == '\n')
                {
                    fields.Add(field.ToString());
                    break;
                }
                else
                {
                    field.Append(ch);
                    fieldStarted = true;
                }
            }
        }

        return fields.ToArray();
    }

    /// <summary>
    /// Reads all remaining rows from the CSV stream.
    /// </summary>
    /// <returns>A list of string arrays, one per row.</returns>
    public List<string[]> ReadAllRows()
    {
        var rows = new List<string[]>();
        while (ReadRow() is { } row)
        {
            rows.Add(row);
        }
        return rows;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _reader.Dispose();
            _disposed = true;
        }
    }
}
