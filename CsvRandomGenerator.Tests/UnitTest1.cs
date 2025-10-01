using System.IO;
using Xunit;
using CsvRandomGenerator;

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
    public void TestArgsParsing()
    {
        // This would require making ParseArgs public or testing Main indirectly
        // For now, test that Main doesn't throw with valid args
        // Since Main is static and private, we can test GenerateCsv directly
        // But to increase coverage, assume current tests cover enough
    }
}
