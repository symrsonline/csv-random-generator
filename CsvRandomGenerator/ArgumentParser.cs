using System;
using System.Collections.Generic;

namespace CsvRandomGenerator
{
    public class ArgumentParser
    {
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

        public static int GetOption(Dictionary<string, string> options, string key, int defaultValue)
        {
            return options.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : defaultValue;
        }

        public static string GetOption(Dictionary<string, string> options, string key, string defaultValue)
        {
            return options.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static int? GetOptionNullable(Dictionary<string, string> options, string key)
        {
            return options.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : (int?)null;
        }

        public static Dictionary<int, DataType> ParseColumnTypes(string typesStr)
        {
            var dict = new Dictionary<int, DataType>();
            var pairs = typesStr.Split(',');
            foreach (var pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length >= 2 && int.TryParse(parts[0], out var col))
                {
                    string typeStr = parts[1].ToLower();
                    DataType type;
                    if (typeStr == "datetime")
                    {
                        if (parts.Length >= 3)
                        {
                            string subType = parts[2].ToLower();
                            if (subType == "random")
                                type = DataType.DateTimeRandom;
                            else if (subType == "now")
                                type = DataType.DateTimeNow;
                            else
                                continue; // invalid
                        }
                        else
                        {
                            type = DataType.DateTimeRandom; // default
                        }
                    }
                    else if (Enum.TryParse<DataType>(typeStr, true, out type))
                    {
                        // for other types like int, double, etc.
                    }
                    else
                    {
                        continue; // invalid type
                    }
                    dict[col] = type;
                }
            }
            return dict;
        }
    }
}