using System.Globalization;

namespace Philiprehberger.CsvKit;

/// <summary>
/// Configuration options for CSV reading and writing.
/// </summary>
public sealed record CsvOptions
{
    /// <summary>
    /// Gets or sets the field delimiter character. Defaults to comma.
    /// </summary>
    public char Delimiter { get; init; } = ',';

    /// <summary>
    /// Gets or sets the quote character for enclosing fields. Defaults to double quote.
    /// </summary>
    public char QuoteChar { get; init; } = '"';

    /// <summary>
    /// Gets or sets whether the first row is a header row. Defaults to true.
    /// </summary>
    public bool HasHeader { get; init; } = true;

    /// <summary>
    /// Gets or sets the culture used for type conversions. Defaults to <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    public CultureInfo CultureInfo { get; init; } = CultureInfo.InvariantCulture;
}
