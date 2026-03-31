# Philiprehberger.CsvKit

[![CI](https://github.com/philiprehberger/dotnet-csv-kit/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-csv-kit/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.CsvKit.svg)](https://www.nuget.org/packages/Philiprehberger.CsvKit)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-csv-kit)](https://github.com/philiprehberger/dotnet-csv-kit/commits/main)

Lightweight CSV reader and writer with header mapping, type conversion, and streaming support.

## Installation

```bash
dotnet add package Philiprehberger.CsvKit
```

## Usage

```csharp
using Philiprehberger.CsvKit;

// Read CSV into strongly-typed records
var people = Csv.Read<Person>("Name,Age\nAlice,30\nBob,25");

// Write records to CSV
string csv = Csv.Write(people);
```

### Reading Raw Rows

```csharp
// Read as string arrays
foreach (var row in Csv.ReadRows("a,b,c\n1,2,3"))
{
    Console.WriteLine(string.Join(" | ", row));
}
```

### Custom Options

```csharp
var options = new CsvOptions { Delimiter = ';', HasHeader = false };
var rows = Csv.ReadRows("1;2;3\n4;5;6", options);
```

### Column Projection

```csharp
// Only map specific columns by header name
var options = new CsvOptions { Columns = ["Name", "City"] };
var people = Csv.Read<Person>("Name,Age,City\nAlice,30,Berlin", options);
// people[0].Age is 0 (default) because Age was not projected
```

### Automatic Delimiter Detection

```csharp
var text = "Name;Age;City\nAlice;30;Berlin";
char delimiter = CsvReader.DetectDelimiter(text);
// delimiter is ';'

var options = new CsvOptions { Delimiter = delimiter };
var rows = Csv.ReadRows(text, options);
```

### Comment Line Support

```csharp
var csv = "Name,Age\n# This is a comment\nAlice,30";
var options = new CsvOptions { CommentChar = '#' };
var people = Csv.Read<Person>(csv, options);
// Comment lines are skipped
```

### Row Filtering

```csharp
var options = new CsvOptions
{
    SkipRow = row => int.Parse(row[1]) < 18
};
var adults = Csv.Read<Person>("Name,Age\nAlice,30\nBob,12", options);
// Only Alice is returned
```

### Streaming Reader

```csharp
using var reader = new CsvReader(new StringReader(csvText));
while (reader.ReadRow() is { } row)
{
    Console.WriteLine(string.Join(", ", row));
}
```

### Streaming Writer

```csharp
using var writer = new CsvWriter(new StringWriter());
writer.WriteRow(["Name", "Age"]);
writer.WriteRow(["Alice", "30"]);
```

## API

| Method | Description |
|--------|-------------|
| `Csv.Read<T>(string, CsvOptions?)` | Parse CSV string into typed records |
| `Csv.Write<T>(IEnumerable<T>, CsvOptions?)` | Write typed records to CSV string |
| `Csv.ReadRows(string, CsvOptions?)` | Parse CSV string into string arrays |
| `Csv.ReadWithHeaders(string, CsvOptions?)` | Parse CSV and return headers and data rows separately |
| `CsvReader.ReadRow()` | Read the next row as a string array |
| `CsvReader.ReadAllRows()` | Read all remaining rows |
| `CsvReader.DetectDelimiter(string, int)` | Detect the most likely delimiter in CSV text |
| `CsvWriter.WriteRow(string[])` | Write a single row |
| `CsvMapper.Map<T>(string[], string[])` | Map header/values to a typed object |

### `CsvOptions`

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Delimiter` | `char` | `,` | Field delimiter character |
| `QuoteChar` | `char` | `"` | Quote character for enclosing fields |
| `HasHeader` | `bool` | `true` | Whether the first row is a header row |
| `CultureInfo` | `CultureInfo` | `InvariantCulture` | Culture for type conversions |
| `Columns` | `IReadOnlyList<string>?` | `null` | Column names to include; null means all |
| `CommentChar` | `char?` | `null` | Character that marks comment lines |
| `SkipRow` | `Func<string[], bool>?` | `null` | Predicate to skip rows; true means skip |

## Development

```bash
dotnet build src/Philiprehberger.CsvKit.csproj --configuration Release
```

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-csv-kit)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-csv-kit/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-csv-kit/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
