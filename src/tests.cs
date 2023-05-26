class tests
{   
    static void testEvtxBinary(string evtxFilePath)
    {
        int bufferSize = 4096;

        using (FileStream fileStream = new FileStream(evtxFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (BinaryReader reader = new BinaryReader(fileStream))
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    byte data = buffer[i];

                    Console.WriteLine(data);
                }
            }
        }
    }

    public static void evtxFilesList(){
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "winevt", "Logs");
        if (Directory.Exists(folderPath))
        {
            string[] evtxFiles = Directory.GetFiles(folderPath, "*.evtx");

            Console.WriteLine("evtx file list:");
            for (int i = 0; i < evtxFiles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(evtxFiles[i])}");
            }
            Console.WriteLine("Select file (enter number):.");
            // string input = Console.ReadLine();
            if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= evtxFiles.Length)
            {
                string selectedFilePath = evtxFiles[selectedIndex - 1];
                Console.WriteLine($"selected file: {selectedFilePath}");
            }
            else
            {
                Console.WriteLine("Invalid Number.");
            }

        }
        else
        {
            Console.WriteLine("The specified folder does not exist.");
        }
    }
        
}