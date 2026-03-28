using Xunit;
using Philiprehberger.CsvKit;

namespace Philiprehberger.CsvKit.Tests;

public class CommentLineTests
{
    [Fact]
    public void ReadRows_WithCommentChar_SkipsCommentLines()
    {
        var csv = "Name,Age\n# This is a comment\nAlice,30\n# Another comment\nBob,25";
        var options = new CsvOptions { CommentChar = '#' };

        var results = Csv.ReadRows(csv, options);

        Assert.Equal(2, results.Count);
        Assert.Equal("Alice", results[0][0]);
        Assert.Equal("Bob", results[1][0]);
    }

    [Fact]
    public void ReadRows_WithNullCommentChar_NoLinesSkipped()
    {
        var csv = "a,b\n# not a comment\n1,2";
        var options = new CsvOptions { CommentChar = null };

        var results = Csv.ReadRows(csv, options);

        Assert.Equal(2, results.Count);
        Assert.Equal("# not a comment", results[0][0]);
    }

    [Fact]
    public void Read_WithCommentChar_SkipsCommentLinesInTypedRead()
    {
        var csv = "Name,Age\n; comment line\nAlice,30";
        var options = new CsvOptions { CommentChar = ';' };

        var results = Csv.Read<PersonRecord>(csv, options);

        Assert.Single(results);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(30, results[0].Age);
    }

    [Fact]
    public void ReadWithHeaders_WithCommentChar_SkipsCommentLines()
    {
        var csv = "Name,Age\n# comment\nAlice,30\nBob,25";
        var options = new CsvOptions { CommentChar = '#' };

        var (headers, rows) = Csv.ReadWithHeaders(csv, options);

        Assert.Equal(2, headers.Length);
        Assert.Equal(2, rows.Count);
    }

    private class PersonRecord
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
}
