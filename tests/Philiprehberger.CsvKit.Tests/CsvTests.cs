using Xunit;
using Philiprehberger.CsvKit;

namespace Philiprehberger.CsvKit.Tests;

public class CsvTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void Read_WithValidCsv_ReturnsTypedRecords()
    {
        var csv = "Name,Age\nAlice,30\nBob,25";

        var results = Csv.Read<Person>(csv);

        Assert.Equal(2, results.Count);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
    }

    [Fact]
    public void Write_WithRecords_ProducesValidCsv()
    {
        var people = new List<Person>
        {
            new() { Name = "Alice", Age = 30 },
            new() { Name = "Bob", Age = 25 }
        };

        var csv = Csv.Write(people);

        Assert.Contains("Name,Age", csv);
        Assert.Contains("Alice,30", csv);
        Assert.Contains("Bob,25", csv);
    }

    [Fact]
    public void ReadRows_WithValidCsv_ReturnsStringArrays()
    {
        var csv = "a,b,c\n1,2,3\n4,5,6";

        var results = Csv.ReadRows(csv);

        Assert.Equal(2, results.Count);
        Assert.Equal(["1", "2", "3"], results[0]);
    }

    [Fact]
    public void ReadWithHeaders_ReturnsHeadersAndRows()
    {
        var csv = "Name,Age\nAlice,30";

        var (headers, rows) = Csv.ReadWithHeaders(csv);

        Assert.Equal(["Name", "Age"], headers);
        Assert.Single(rows);
    }

    [Fact]
    public void Read_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Csv.Read<Person>(null!));
    }
}
