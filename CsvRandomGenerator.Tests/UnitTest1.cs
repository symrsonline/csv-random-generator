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
        string folder = Path.GetTempPath();
        string output = "test.csv";
        string filePath = Path.Combine(folder, output);

        // Act
        CsvRandomGenerator.Program.GenerateCsv(5, 3, folder, output, null);

        // Assert
        Assert.True(File.Exists(filePath));
        var lines = File.ReadAllLines(filePath);
        Assert.Equal(5, lines.Length);
        foreach (var line in lines)
        {
            var columns = line.Split(',');
            Assert.Equal(3, columns.Length);
        }

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void TestCsvSorting()
    {
        // Arrange
        string folder = Path.GetTempPath();
        string output = "test_sorted.csv";
        string filePath = Path.Combine(folder, output);

        // Act
        CsvRandomGenerator.Program.GenerateCsv(10, 2, folder, output, 0); // Sort by first column

        // Assert
        Assert.True(File.Exists(filePath));
        var lines = File.ReadAllLines(filePath);
        Assert.Equal(10, lines.Length);

        // Check if sorted (assuming first column is numeric)
        for (int i = 1; i < lines.Length; i++)
        {
            var prev = double.Parse(lines[i - 1].Split(',')[0]);
            var curr = double.Parse(lines[i].Split(',')[0]);
            Assert.True(prev <= curr);
        }

        // Cleanup
        File.Delete(filePath);
    }
}
