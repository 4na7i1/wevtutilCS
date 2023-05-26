using System.Runtime.InteropServices;
using System.Text;
using wevtutilCS;

namespace utils
{
    class OsDetecter
    {
        public static OSPlatform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            throw new Exception("Cannot determine operating system!");
        }
    }

    class ConsoleWriter
    {
        public static void WriteLineWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    class FileWriter
    {
        /*
        public static async Task WriteToFileAsync(string FilePath, string content){
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                await writer.WriteAsync(content);
            }
        }
        */
        public static Task WriteToFileAsync(string FilePath, string content)
        {
            Task task = Task.Run(async () =>
            {
                using (StreamWriter writer = new StreamWriter(FilePath, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(content);
                }
            });
            return task;
            // Program.tasks.Add(task);
        }
        public static void WriteToFile(string filePath, string contents)
        {
            using (var writer = new StreamWriter(filePath, append: false, Encoding.UTF8, bufferSize: 8192))
            {
                writer.AutoFlush = true;
                writer.Write(contents);
            }
        }
        public static void InitializeFile(string filePath) // Initialize File
        {
            // if exist file -> delete
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // create file
            File.Create(filePath).Close();
        }
        public static void filterXML(string filePath) // Remove values that cannot be stored in XML variables from XML files
        {
            // read the file
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Remove strings that cannot be used in XML
            byte[] filteredBytes = fileBytes.Where(b => (int)b >= 0x20 && (int)b <= 0xD7FF || (int)b >= 0xE000 && (int)b <= 0xFFFD).ToArray();

            // Overwrite and save the file
            File.WriteAllBytes(filePath, filteredBytes);

            Console.WriteLine("Done! >> {0}", filePath);
        }
    }

}

