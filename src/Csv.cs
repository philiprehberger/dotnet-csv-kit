namespace Philiprehberger.CsvKit;

/// <summary>
/// Provides static entry points for reading and writing CSV data.
/// </summary>
public static class Csv
{
    /// <summary>
    /// Reads a CSV string into a list of strongly-typed records.
    /// The first row is used as headers and matched to properties by name (case-insensitive).
    /// </summary>
    /// <typeparam name="T">The target record type. Must have a parameterless constructor.</typeparam>
    /// <param name="csv">The CSV string to parse.</param>
    /// <param name="options">Optional parsing options.</param>
    /// <returns>A list of parsed records.</returns>
    public static List<T> Read<T>(string csv, CsvOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(csv);

        var opts = options ?? new CsvOptions();
        using var reader = new CsvReader(new StringReader(csv), opts);

        string[]? headers = null;
        if (opts.HasHeader)
        {
            headers = reader.ReadRow();
            if (headers is null)
                return [];
        }

        var results = new List<T>();

        while (reader.ReadRow() is { } row)
        {
            if (headers is not null)
            {
                var obj = CsvMapper.Map<T>(headers, row, opts.CultureInfo);
                results.Add(obj);
            }
        }

        return results;
    }

    /// <summary>
    /// Writes a collection of records to a CSV string.
    /// Property names are used as column headers.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="records">The records to write.</param>
    /// <param name="options">Optional writing options.</param>
    /// <returns>A CSV formatted string.</returns>
    public static string Write<T>(IEnumerable<T> records, CsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(records);

        var opts = options ?? new CsvOptions();
        var sw = new StringWriter();
        using var writer = new CsvWriter(sw, opts);

        if (opts.HasHeader)
        {
            var headers = CsvMapper.GetHeaders<T>();
            writer.WriteRow(headers);
        }

        foreach (var record in records)
        {
            var values = CsvMapper.GetValues(record, opts.CultureInfo);
            writer.WriteRow(values);
        }

        writer.Flush();
        return sw.ToString();
    }

    /// <summary>
    /// Reads a CSV string into a list of string arrays, one per row.
    /// If <see cref="CsvOptions.HasHeader"/> is true, the header row is excluded from results.
    /// </summary>
    /// <param name="csv">The CSV string to parse.</param>
    /// <param name="options">Optional parsing options.</param>
    /// <returns>A list of string arrays representing data rows.</returns>
    public static List<string[]> ReadRows(string csv, CsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(csv);

        var opts = options ?? new CsvOptions();
        using var reader = new CsvReader(new StringReader(csv), opts);

        if (opts.HasHeader)
        {
            reader.ReadRow(); // Skip header
        }

        return reader.ReadAllRows();
    }

    /// <summary>
    /// Reads a CSV string and returns both headers and data rows.
    /// </summary>
    /// <param name="csv">The CSV string to parse.</param>
    /// <param name="options">Optional parsing options. HasHeader is treated as true.</param>
    /// <returns>A tuple of headers and data rows.</returns>
    public static (string[] Headers, List<string[]> Rows) ReadWithHeaders(string csv, CsvOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(csv);

        var opts = options ?? new CsvOptions();
        using var reader = new CsvReader(new StringReader(csv), opts);

        var headers = reader.ReadRow() ?? [];
        var rows = reader.ReadAllRows();

        return (headers, rows);
    }
}
