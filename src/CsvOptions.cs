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

    /// <summary>
    /// Gets or sets the list of column names to include when reading CSV data.
    /// When set, only the specified columns are mapped by header name; all other columns are skipped.
    /// When null, all columns are included. Defaults to null.
    /// </summary>
    public IReadOnlyList<string>? Columns { get; init; }

    /// <summary>
    /// Gets or sets the character that marks a line as a comment.
    /// Lines starting with this character are skipped during reading.
    /// When null, no lines are treated as comments. Defaults to null.
    /// </summary>
    public char? CommentChar { get; init; }

    /// <summary>
    /// Gets or sets a predicate that determines whether a parsed row should be skipped.
    /// The predicate receives the parsed field values; if it returns true, the row is excluded from results.
    /// When null, no rows are skipped. Defaults to null.
    /// </summary>
    public Func<string[], bool>? SkipRow { get; init; }
}
