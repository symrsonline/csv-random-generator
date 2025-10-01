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
}
