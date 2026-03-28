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
    private readonly char? _commentChar;
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
        _commentChar = opts.CommentChar;
    }

    /// <summary>
    /// Reads the next row from the CSV stream, skipping comment lines.
    /// </summary>
    /// <returns>An array of field values, or null if there are no more rows.</returns>
    public string[]? ReadRow()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        while (true)
        {
            var row = ReadRowCore();
            if (row is null)
                return null;

            // Check if this is a comment line: single field starting with the comment char
            if (_commentChar.HasValue && row.Length > 0 && row[0].Length > 0 && row[0][0] == _commentChar.Value)
            {
                continue;
            }

            return row;
        }
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

    /// <summary>
    /// Detects the most likely delimiter character in the given CSV text by counting occurrences
    /// of common delimiters in the first N lines.
    /// </summary>
    /// <param name="text">The CSV text to analyze.</param>
    /// <param name="sampleLines">The number of lines to sample. Defaults to 5.</param>
    /// <returns>The most likely delimiter character.</returns>
    public static char DetectDelimiter(string text, int sampleLines = 5)
    {
        ArgumentNullException.ThrowIfNull(text);

        char[] candidates = [',', ';', '\t', '|'];
        var counts = new Dictionary<char, int>();
        foreach (var c in candidates)
            counts[c] = 0;

        using var reader = new StringReader(text);
        int linesRead = 0;
        while (linesRead < sampleLines)
        {
            var line = reader.ReadLine();
            if (line is null)
                break;

            linesRead++;
            foreach (var c in candidates)
            {
                foreach (var ch in line)
                {
                    if (ch == c)
                        counts[c]++;
                }
            }
        }

        char best = ',';
        int bestCount = 0;
        foreach (var kvp in counts)
        {
            if (kvp.Value > bestCount)
            {
                bestCount = kvp.Value;
                best = kvp.Key;
            }
        }

        return best;
    }

    private string[]? ReadRowCore()
    {
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
