using System.Collections.Generic;
using System.IO;

namespace RadencyTaskETL
{
    public class Logger
    {
        public int ParsedFiles, ParsedLines, FoundErrors;
        public readonly HashSet<string> InvalidFiles;
        public readonly HashSet<string> InvalidLines;

        public Logger()
        {
            InvalidFiles = new HashSet<string>();
            InvalidLines = new HashSet<string>();
        }
        public Logger(string path) : this()
        {
            using var reader = File.OpenText(path);
            string text = reader.ReadToEnd();
            var lines = text.Split('\n');

            ParsedFiles = int.Parse(lines[0].Substring(lines[0].LastIndexOf(' ') + 1));
            ParsedLines = int.Parse(lines[1].Substring(lines[1].LastIndexOf(' ') + 1));
            FoundErrors = int.Parse(lines[2].Substring(lines[2].LastIndexOf(' ') + 1));

            PopulateSet(InvalidFiles);
            PopulateSet(InvalidLines);

            void PopulateSet(HashSet<string> set)
            {
                int a = text.IndexOf('[');
                int b = text.IndexOf(']');
                var items = text.Substring(a + 1, b - a - 1).Trim().Split(",\n\t");
                foreach (string item in items) set.Add(item);
                text = text.Remove(a, b - a + 1);
            }
        }

        public void LogData(string path)
        {
            string result =
                "parsed_files: " + ParsedFiles +
                "\nparsed_lines: " + ParsedLines +
                "\nfound_errors: " + FoundErrors +
                "\ninvalid_files:\n[\n\t";
            result += string.Join(",\n\t", InvalidFiles);
            result += "\n]\ninvalid_lines:\n[\n\t";
            result += string.Join(",\n\t", InvalidLines);
            result += "\n]";
            
            using var writer = File.CreateText(path);
            writer.Write(result);
        }

        public void ResetData()
        {
            ParsedFiles = 0;
            ParsedLines = 0;
            FoundErrors = 0;
            
            InvalidFiles.Clear();
            InvalidLines.Clear();
        }
    }
}