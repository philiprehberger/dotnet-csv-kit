# Philiprehberger.CsvKit

[![CI](https://github.com/philiprehberger/dotnet-csv-kit/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-csv-kit/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.CsvKit.svg)](https://www.nuget.org/packages/Philiprehberger.CsvKit)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-csv-kit)](LICENSE)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Lightweight CSV reader and writer with header mapping, type conversion, and streaming support.

## Installation

```bash
dotnet add package Philiprehberger.CsvKit
```

## Usage

```csharp
using Philiprehberger.CsvKit;

// Read CSV into strongly-typed records
var people = Csv.Read<Person>("name,age\nAlice,30\nBob,25");

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
| `CsvWriter.WriteRow(string[])` | Write a single row |
| `CsvMapper.Map<T>(string[], string[])` | Map header/values to a typed object |

## Development

```bash
dotnet build src/Philiprehberger.CsvKit.csproj --configuration Release
```

## License

[MIT](LICENSE)
