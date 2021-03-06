﻿using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OperationJSONGenerator {
    class MainClass {
        public static void Main(string[] args) {
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
                    var inputObject = new JObject();
                    inputObject.Add("name", split[0]);
                    inputObject.Add("description", split[1]);
                    inputObject.Add("rejectFloatingPoint", split.Length == 3 && split[2].StartsWith("int", StringComparison.Ordinal));
                    jInputs.Add(inputObject);
                }

                lineNumber++;
                root.Add("inputs", jInputs);

                var implementations = new JArray();
                while ((inputLine = reader.ReadLine()) != null) {
                    lineNumber++;
                    var match = Regex.Match(inputLine, "^(.+?)\\s=\\s(.+)$");
                    if (match == null) {
                        Console.WriteLine($"Error: Line {lineNumber} - Invalid format for operation implementation");
                        return;
                    }
                    var implementationObject = new JObject();
                    var processedExpression = ProcessExpression(match.Groups[2].Value, inputNames.Concat(new[] { "pi", "pref90", "pref180" }).ToList());
                    implementationObject.Add("resultName", match.Groups[1].Value);
                    implementationObject.Add("expression", processedExpression);
                    implementations.Add(implementationObject);
                }
                root.Add("implementations", implementations);
            }

            Console.WriteLine("JSON Generated:");
            Console.WriteLine(root.ToString(Formatting.Indented));
        }

        static string ProcessExpression(string expression, List<string> inputs) {
            var regexForInputs = string.Join("|", inputs.Select(x => Regex.Escape(x)));
            var mainRegex = $"(?=({regexForInputs})\\b)";
            return Regex.Replace(expression, mainRegex, "$");
        }
    }
}
