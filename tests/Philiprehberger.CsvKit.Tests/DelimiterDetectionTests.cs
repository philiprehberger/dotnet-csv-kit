using Xunit;
using Philiprehberger.CsvKit;

namespace Philiprehberger.CsvKit.Tests;

public class DelimiterDetectionTests
{
    [Fact]
    public void DetectDelimiter_CommaSeparated_ReturnsComma()
    {
        var text = "Name,Age,City\nAlice,30,Berlin\nBob,25,Vienna";

        var result = CsvReader.DetectDelimiter(text);

        Assert.Equal(',', result);
    }

    [Fact]
    public void DetectDelimiter_SemicolonSeparated_ReturnsSemicolon()
    {
        var text = "Name;Age;City\nAlice;30;Berlin\nBob;25;Vienna";

        var result = CsvReader.DetectDelimiter(text);

        Assert.Equal(';', result);
    }

    [Fact]
    public void DetectDelimiter_TabSeparated_ReturnsTab()
    {
        var text = "Name\tAge\tCity\nAlice\t30\tBerlin\nBob\t25\tVienna";

        var result = CsvReader.DetectDelimiter(text);

        Assert.Equal('\t', result);
    }

    [Fact]
    public void DetectDelimiter_PipeSeparated_ReturnsPipe()
    {
        var text = "Name|Age|City\nAlice|30|Berlin\nBob|25|Vienna";

        var result = CsvReader.DetectDelimiter(text);

        Assert.Equal('|', result);
    }

    [Fact]
    public void DetectDelimiter_WithSampleLines_OnlySamplesSpecifiedLines()
    {
        var text = "a;b;c\n1;2;3\n4;5;6\n7;8;9\n10;11;12";

        var result = CsvReader.DetectDelimiter(text, sampleLines: 2);

        Assert.Equal(';', result);
    }

    [Fact]
    public void DetectDelimiter_EmptyText_ReturnsComma()
    {
        var result = CsvReader.DetectDelimiter("");

        Assert.Equal(',', result);
    }
}
