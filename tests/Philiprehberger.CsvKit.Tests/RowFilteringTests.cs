using Xunit;
using Philiprehberger.CsvKit;

namespace Philiprehberger.CsvKit.Tests;

public class RowFilteringTests
{
    private class Product
    {
        public string Name { get; set; } = "";
        public int Price { get; set; }
    }

    [Fact]
    public void Read_WithSkipRow_FiltersMatchingRows()
    {
        var csv = "Name,Price\nApple,1\nBanana,5\nCherry,2";
        var options = new CsvOptions
        {
            SkipRow = row => int.TryParse(row[1], out var price) && price < 3
        };

        var results = Csv.Read<Product>(csv, options);

        Assert.Single(results);
        Assert.Equal("Banana", results[0].Name);
        Assert.Equal(5, results[0].Price);
    }

    [Fact]
    public void ReadRows_WithSkipRow_FiltersMatchingRows()
    {
        var csv = "a,b\n1,2\n3,4\n5,6";
        var options = new CsvOptions
        {
            SkipRow = row => row[0] == "3"
        };

        var results = Csv.ReadRows(csv, options);

        Assert.Equal(2, results.Count);
        Assert.Equal("1", results[0][0]);
        Assert.Equal("5", results[1][0]);
    }

    [Fact]
    public void ReadWithHeaders_WithSkipRow_FiltersMatchingRows()
    {
        var csv = "Name,Price\nApple,1\nBanana,5\nCherry,2";
        var options = new CsvOptions
        {
            SkipRow = row => row[0] == "Banana"
        };

        var (headers, rows) = Csv.ReadWithHeaders(csv, options);

        Assert.Equal(2, rows.Count);
        Assert.Equal("Apple", rows[0][0]);
        Assert.Equal("Cherry", rows[1][0]);
    }

    [Fact]
    public void Read_WithNullSkipRow_NoRowsFiltered()
    {
        var csv = "Name,Price\nApple,1\nBanana,5";
        var options = new CsvOptions { SkipRow = null };

        var results = Csv.Read<Product>(csv, options);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Read_WithSkipRowAndCommentChar_BothApplied()
    {
        var csv = "Name,Price\n# comment\nApple,1\nBanana,5\nCherry,2";
        var options = new CsvOptions
        {
            CommentChar = '#',
            SkipRow = row => row[0] == "Banana"
        };

        var results = Csv.Read<Product>(csv, options);

        Assert.Equal(2, results.Count);
        Assert.Equal("Apple", results[0].Name);
        Assert.Equal("Cherry", results[1].Name);
    }
}
