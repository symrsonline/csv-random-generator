using System.IO;
using Xunit;
using CsvRandomGenerator;
using System.Text;

namespace CsvRandomGenerator.Tests;

public class UnitTest1
{
    [Fact]
    public void TestCsvGeneration()
    {
        // Arrange
        string tempPath = Path.GetTempPath();
        string outputPath = Path.Combine(tempPath, "test.csv");
        string folder = Path.GetDirectoryName(outputPath) ?? tempPath;
        string output = Path.GetFileName(outputPath);

        // Act
        CsvRandomGenerator.Program.GenerateCsv(5, 3, folder, output, null);

        // Assert
        Assert.True(File.Exists(outputPath));
        var lines = File.ReadAllLines(outputPath);
        Assert.Equal(5, lines.Length);
        foreach (var line in lines)
        {
            var columns = line.Split(',');
            Assert.Equal(3, columns.Length);
        }

        // Cleanup
        File.Delete(outputPath);
    }

    [Fact]
    public void TestCsvSorting()
    {
        // Arrange
        string tempPath = Path.GetTempPath();
        string outputPath = Path.Combine(tempPath, "test_sorted.csv");
        string folder = Path.GetDirectoryName(outputPath) ?? tempPath;
        string output = Path.GetFileName(outputPath);

        // Act
        CsvRandomGenerator.Program.GenerateCsv(10, 2, folder, output, 0); // Sort by first column

        // Assert
        Assert.True(File.Exists(outputPath));
        var lines = File.ReadAllLines(outputPath);
        Assert.Equal(10, lines.Length);

        // Check if sorted (assuming first column is numeric or string)
        for (int i = 1; i < lines.Length; i++)
        {
            var prevParts = lines[i - 1].Split(',');
            var currParts = lines[i].Split(',');
            var prev = prevParts[0];
            var curr = currParts[0];
            // Try to parse as double, if not, compare as string
            if (double.TryParse(prev, out var prevNum) && double.TryParse(curr, out var currNum))
            {
                Assert.True(prevNum <= currNum);
            }
            else if (DateTime.TryParse(prev, out var prevDate) && DateTime.TryParse(curr, out var currDate))
            {
                Assert.True(prevDate <= currDate);
            }
            else
            {
                Assert.True(string.Compare(prev, curr) <= 0);
            }
        }

        // Cleanup
        File.Delete(outputPath);
    }

    [Fact]
    public void TestCsvAppend()
    {
        // Arrange
        string tempPath = Path.GetTempPath();
        string outputPath = Path.Combine(tempPath, "test_append.csv");
        string folder = Path.GetDirectoryName(outputPath) ?? tempPath;
        string output = Path.GetFileName(outputPath);

        // Act: First generate
        CsvRandomGenerator.Program.GenerateCsv(3, 2, folder, output, null, append: false);
        // Then append
        CsvRandomGenerator.Program.GenerateCsv(2, 2, folder, output, null, append: true);

        // Assert
        Assert.True(File.Exists(outputPath));
        var lines = File.ReadAllLines(outputPath);
        Assert.Equal(5, lines.Length); // 3 + 2
        foreach (var line in lines)
        {
            var columns = line.Split(',');
            Assert.Equal(2, columns.Length);
        }

        // Cleanup
        File.Delete(outputPath);
    }

    [Fact]
    public void TestInvalidSortColumn()
    {
        // Arrange
        string tempPath = Path.GetTempPath();
        string outputPath = Path.Combine(tempPath, "test_invalid_sort.csv");
        string folder = Path.GetDirectoryName(outputPath) ?? tempPath;
        string output = Path.GetFileName(outputPath);

        // Act: Sort by invalid column (out of range)
        CsvRandomGenerator.Program.GenerateCsv(5, 2, folder, output, 5); // cols=2, so 5 is invalid

        // Assert: Should still generate file without sorting
        Assert.True(File.Exists(outputPath));
        var lines = File.ReadAllLines(outputPath);
        Assert.Equal(5, lines.Length);

        // Cleanup
        File.Delete(outputPath);
    }

    [Fact]
    public void TestHelp()
    {
        // Arrange
        var stringWriter = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stringWriter);

        // Act
        CsvRandomGenerator.Program.Main(new string[] { "--help" });

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("CSV Random Generator", output);
        Assert.Contains("--rows", output);
        Assert.Contains("--cols", output);
        Assert.Contains("--output", output);
        Assert.Contains("--sort-column", output);
        Assert.Contains("--duration", output);
        Assert.Contains("--help", output);

        // Cleanup
        Console.SetOut(originalOut);
        stringWriter.Dispose();
    }

    [Theory]
    [InlineData(new string[] {}, 0)]
    [InlineData(new string[] { "--rows", "10" }, 1)]
    [InlineData(new string[] { "--rows", "10", "--cols", "5" }, 2)]
    [InlineData(new string[] { "--rows", "10", "--cols", "5", "--output", "test.csv" }, 3)]
    [InlineData(new string[] { "--rows", "10", "--cols", "5", "--output", "test.csv", "--sort-column", "0" }, 4)]
    [InlineData(new string[] { "--rows", "10", "--cols", "5", "--output", "test.csv", "--sort-column", "0", "--duration", "30" }, 5)]
    [InlineData(new string[] { "--rows" }, 0)] // Odd number, no value
    [InlineData(new string[] { "rows", "10" }, 0)] // Not starting with --
    [InlineData(new string[] { "--help" }, 0)] // No value
    [InlineData(new string[] { "--rows", "10", "extra" }, 1)] // Extra arg
    public void TestParseArgs(string[] args, int expectedCount)
    {
        // Act
        var result = CsvRandomGenerator.Program.ParseArgs(args);

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void TestParseArgsValues()
    {
        // Arrange
        var args = new string[] { "--rows", "20", "--cols", "3", "--output", "myfile.csv", "--sort-column", "1", "--duration", "60" };

        // Act
        var result = CsvRandomGenerator.Program.ParseArgs(args);

        // Assert
        Assert.Equal("20", result["rows"]);
        Assert.Equal("3", result["cols"]);
        Assert.Equal("myfile.csv", result["output"]);
        Assert.Equal("1", result["sort-column"]);
        Assert.Equal("60", result["duration"]);
    }

    [Fact(Timeout = 2000)]
    public async Task TestDurationMode()
    {
        // Arrange
        var stringWriter = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(stringWriter);

        // Act: Start Main with duration > 0 in a separate task
        var task = Task.Run(() => CsvRandomGenerator.Program.Main(new string[] { "--rows", "5", "--cols", "3", "--duration", "1" }));

        // Wait a bit for the output
        await Task.Delay(500);

        // Assert
        var output = stringWriter.ToString();
        Assert.Contains("Generating CSV every 1 seconds. Press Ctrl+C to stop.", output);

        // Cleanup: Cancel the task (simulate Ctrl+C)
        // Since it's infinite loop, we need to abort the task
        // For simplicity, just check the output and let the test timeout if needed
        // In practice, this test will timeout after 2 seconds due to Timeout attribute

        Console.SetOut(originalOut);
        stringWriter.Dispose();
    }
}
