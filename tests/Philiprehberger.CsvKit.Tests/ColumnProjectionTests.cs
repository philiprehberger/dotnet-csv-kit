using Xunit;
using Philiprehberger.CsvKit;

namespace Philiprehberger.CsvKit.Tests;

public class ColumnProjectionTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string City { get; set; } = "";
    }

    [Fact]
    public void Read_WithColumns_OnlyMapsSpecifiedColumns()
    {
        var csv = "Name,Age,City\nAlice,30,Berlin\nBob,25,Vienna";
        var options = new CsvOptions { Columns = ["Name", "City"] };

        var results = Csv.Read<Person>(csv, options);

        Assert.Equal(2, results.Count);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal("Berlin", results[0].City);
        Assert.Equal(0, results[0].Age); // Not mapped, default value
    }

    [Fact]
    public void Read_WithColumns_CaseInsensitiveMatch()
    {
        var csv = "name,age,city\nAlice,30,Berlin";
        var options = new CsvOptions { Columns = ["NAME", "CITY"] };

        var results = Csv.Read<Person>(csv, options);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal("Berlin", results[0].City);
        Assert.Equal(0, results[0].Age);
    }

    [Fact]
    public void Read_WithNullColumns_MapsAllColumns()
    {
        var csv = "Name,Age,City\nAlice,30,Berlin";
        var options = new CsvOptions { Columns = null };

        var results = Csv.Read<Person>(csv, options);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
        Assert.Equal("Berlin", results[0].City);
    }

    [Fact]
    public void Read_WithColumns_NonExistentColumnIgnored()
    {
        var csv = "Name,Age,City\nAlice,30,Berlin";
        var options = new CsvOptions { Columns = ["Name", "NonExistent"] };

        var results = Csv.Read<Person>(csv, options);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(0, results[0].Age);
        Assert.Equal("", results[0].City);
    }
}
