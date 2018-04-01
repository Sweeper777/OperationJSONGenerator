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
                break;
            }
        }
    }
}
