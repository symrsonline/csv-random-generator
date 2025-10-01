using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace CsvRandomGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
            {
                PrintHelp();
                return;
            }

            var options = ParseArgs(args);

            int rows = GetOption(options, "rows", 10);
            int cols = GetOption(options, "cols", 5);
            string outputPath = GetOption(options, "output", "output.csv");
            string folder = Path.GetDirectoryName(outputPath) ?? ".";
            string output = Path.GetFileName(outputPath);
            string baseName = Path.GetFileNameWithoutExtension(output);
            string extension = Path.GetExtension(output);
            int? sortColumn = GetOptionNullable(options, "sort-column");
            int duration = GetOption(options, "duration", 0);
            int maxFiles = GetOption(options, "max-files", 0);

            if (duration > 0)
            {
                Console.WriteLine($"Generating CSV every {duration} seconds. Press Ctrl+C to stop.");
                while (true)
                {
                    if (maxFiles == 0 || Directory.GetFiles(folder).Length < maxFiles)
                    {
                        string timestampedOutput = baseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;
                        GenerateCsv(rows, cols, folder, timestampedOutput, null, append: false);
                    }
                    Thread.Sleep(duration * 1000);
                }
            }
            else
            {
                string timestampedOutput = baseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;
                GenerateCsv(rows, cols, folder, timestampedOutput, sortColumn, append: false);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("CSV Random Generator");
            Console.WriteLine("Generate random CSV files with specified rows and columns.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --rows <number>        Number of rows (default: 10)");
            Console.WriteLine("  --cols <number>        Number of columns (default: 5)");
            Console.WriteLine("  --output <path>        Output file path (default: output.csv)");
            Console.WriteLine("  --sort-column <index>  Column to sort by (0-based index, optional)");
            Console.WriteLine("  --duration <seconds>   Interval to append data (optional, continuous mode)");
            Console.WriteLine("  --max-files <number>   Maximum number of files in output folder (0 for unlimited, default: 0)");
            Console.WriteLine("  --help, -h             Show this help message");
            Console.WriteLine();
            Console.WriteLine("Data Types:");
            Console.WriteLine("  Columns have fixed random types: int, double, string, or DateTime.");
        }

        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            var options = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i].StartsWith("--") && i + 1 < args.Length)
                {
                    options[args[i].Substring(2)] = args[i + 1];
                }
            }
            return options;
        }

        static int GetOption(Dictionary<string, string> options, string key, int defaultValue)
        {
            return options.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : defaultValue;
        }

        static string GetOption(Dictionary<string, string> options, string key, string defaultValue)
        {
            return options.TryGetValue(key, out var value) ? value : defaultValue;
        }

        static int? GetOptionNullable(Dictionary<string, string> options, string key)
        {
            return options.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : (int?)null;
        }

        public static void GenerateCsv(int rows, int cols, string folder, string output, int? sortColumn, bool append = false)
        {
            Directory.CreateDirectory(folder);

            Random random = new Random();
            int[] columnTypes = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                columnTypes[j] = random.Next(4);
            }
            List<string[]> data = new List<string[]>();
            for (int i = 0; i < rows; i++)
            {
                string[] row = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    int type = columnTypes[j];
                    if (type == 0)
                        row[j] = random.Next(0, 101).ToString();
                    else if (type == 1)
                        row[j] = Math.Round(random.NextDouble() * 100, 2).ToString();
                    else if (type == 2)
                        row[j] = new string(Enumerable.Range(0, random.Next(5, 11)).Select(_ => (char)random.Next(65, 91)).ToArray());
                    else
                        row[j] = DateTime.Now.AddDays(random.Next(-365, 365)).AddHours(random.Next(24)).AddMinutes(random.Next(60)).AddSeconds(random.Next(60)).ToString("yyyy/MM/dd HH:mm:ss");
                }
                data.Add(row);
            }

            if (!append && sortColumn.HasValue && sortColumn.Value >= 0 && sortColumn.Value < cols)
            {
                data.Sort((a, b) =>
                {
                    int type = columnTypes[sortColumn.Value];
                    if (type == 0) // int
                        return int.Parse(a[sortColumn.Value]).CompareTo(int.Parse(b[sortColumn.Value]));
                    else if (type == 1) // double
                        return double.Parse(a[sortColumn.Value]).CompareTo(double.Parse(b[sortColumn.Value]));
                    else if (type == 2) // string
                        return string.Compare(a[sortColumn.Value], b[sortColumn.Value]);
                    else // DateTime
                        return DateTime.Parse(a[sortColumn.Value]).CompareTo(DateTime.Parse(b[sortColumn.Value]));
                });
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(folder, output), append))
            {
                foreach (var row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }

            Console.WriteLine($"CSV data appended to {Path.Combine(folder, output)}");
        }
    }
}
