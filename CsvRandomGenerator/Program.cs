using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace CsvRandomGenerator
{
    public enum DataType { Int, Double, String, DateTime }

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
            {
                PrintHelp();
                return;
            }

            var options = ArgumentParser.ParseArgs(args);

            int rows = ArgumentParser.GetOption(options, "rows", 10);
            int cols = ArgumentParser.GetOption(options, "cols", 5);
            string outputPath = ArgumentParser.GetOption(options, "output", "output.csv");
            string folder = Path.GetDirectoryName(outputPath) ?? ".";
            string output = Path.GetFileName(outputPath);
            string baseName = Path.GetFileNameWithoutExtension(output);
            string extension = Path.GetExtension(output);
            int? sortColumn = ArgumentParser.GetOptionNullable(options, "sort-column");
            int duration = ArgumentParser.GetOption(options, "duration", 0);
            int maxFiles = ArgumentParser.GetOption(options, "max-files", 0);
            Dictionary<int, DataType>? columnTypes = null;
            if (options.TryGetValue("column-types", out var typesStr))
            {
                columnTypes = ArgumentParser.ParseColumnTypes(typesStr);
            }

            var generator = new CsvGenerator();

            if (duration > 0)
            {
                Console.WriteLine($"Generating CSV every {duration} seconds. Press Ctrl+C to stop.");
                while (true)
                {
                    if (maxFiles == 0 || Directory.GetFiles(folder).Length < maxFiles)
                    {
                        string timestampedOutput = baseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;
                        generator.GenerateCsv(rows, cols, folder, timestampedOutput, null, append: false, columnTypes);
                    }
                    Thread.Sleep(duration * 1000);
                }
            }
            else
            {
                string timestampedOutput = baseName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;
                generator.GenerateCsv(rows, cols, folder, timestampedOutput, sortColumn, append: false, columnTypes);
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
            Console.WriteLine("  --column-types <types> Specify data types for columns (e.g., '0:int,1:string,2:double')");
            Console.WriteLine("  --help, -h             Show this help message");
            Console.WriteLine();
            Console.WriteLine("Data Types:");
            Console.WriteLine("  Columns have fixed random types: int, double, string, or DateTime.");
        }
    }
}
