using System;

namespace OperationJSONGenerator
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var filePath = "";
            var fileName = "";
            while (true) {
                Console.WriteLine("Please enter the file path to read: (base directory - Desktop)");
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + Console.ReadLine();
                fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!File.Exists(filePath)) {
                    Console.WriteLine("File does not exist!");
                    Console.WriteLine();
                    continue;
                }
                break;
            }

            var splitFileName = Regex.Split(fileName, "\\s-\\s");

            if (splitFileName.Length != 2) {
                Console.WriteLine("Error: File name in invalid format!");
                return;
            }

            JObject root = new JObject();
            root.Add("name", splitFileName[1]);
            root.Add("category", splitFileName[0]);

            using (var reader = new StreamReader(filePath)) {
                int lineNumber = 1;
                var image = reader.ReadLine();
                if (image == null) {
                    Console.WriteLine($"Error: Line {lineNumber} - Unexpected EOF");
                    return;
                }
                if (image == string.Empty) {
                    root.Add("image", null);
                } else {
                    root.Add("image", image);
                }

                lineNumber++;
                if (reader.ReadLine() != "") {
                    Console.WriteLine($"Error: Line {lineNumber} - Blank line expected");
                    return;
                }

                string inputLine;
                var jInputs = new JArray();
                var inputNames = new List<string>();
                while ((inputLine = reader.ReadLine()) != "") {
                    lineNumber++;
                    if (inputLine == null) {
                        Console.WriteLine($"Error: Line {lineNumber} - Unexpected EOF");
                        return;
                    }
                    var split = Regex.Split(inputLine, "\\s-\\s");
                    if (split.Length < 2 || split.Length > 3 || split.Any(x => x == "")) {
                        Console.WriteLine($"Error: Line {lineNumber} - Invalid format for operation input");
                        return;
                    }

                    inputNames.Add(split[0]);
                }
        }
    }
}
