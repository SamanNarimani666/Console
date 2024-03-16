
//var result = Directory.GetFileSystemEntries(@"C:\Users\Saman\Desktop\F","*.txt",SearchOption.AllDirectories);

//foreach (var item in result)
//{
//    Console.WriteLine(item);
//}

string currentDirectory = Directory.GetCurrentDirectory();
ExploreDirectory(currentDirectory);


static void ExploreDirectory(string path)
{
    Console.Clear();
    Console.WriteLine("Current Directory: " + path);
    string[] directories = Directory.GetDirectories(path);
    string[] files = Directory.GetFiles(path);

    // Display directories
    Console.WriteLine("Directories:");
    for (int i = 0; i < directories.Length; i++)
    {
        Console.WriteLine($"{i + 1}. {Path.GetFileName(directories[i])}");
    }

    // Display files
    Console.WriteLine("Files:");
    for (int i = 0; i < files.Length; i++)
    {
        Console.WriteLine($"{i + directories.Length + 1}. {Path.GetFileName(files[i])}");
    }

    Console.WriteLine("Use Arrow keys to navigate. Press Enter to open a file or Enter a directory.");
    ConsoleKeyInfo keyInfo = Console.ReadKey();

    if (keyInfo.Key == ConsoleKey.RightArrow && keyInfo.Modifiers == ConsoleModifiers.Alt)
    {
        // Enter directory
        int selectedIndex = Console.CursorTop - directories.Length - 2;
        if (selectedIndex >= 0 && selectedIndex < directories.Length)
        {
            ExploreDirectory(directories[selectedIndex]);
        }
    }
    else if (keyInfo.Key == ConsoleKey.LeftArrow && keyInfo.Modifiers == ConsoleModifiers.Alt)
    {
        // Go back to parent directory
        string parentDirectory = Directory.GetParent(path)?.FullName;
        if (parentDirectory != null)
        {
            ExploreDirectory(parentDirectory);
        }
    }
    else if (keyInfo.Key == ConsoleKey.Enter)
    {
        // Open file
        int selectedIndex = Console.CursorTop - directories.Length - 2;
        if (selectedIndex >= 0 && selectedIndex < files.Length)
        {
            string selectedFile = files[selectedIndex];
            Console.Clear();
            Console.WriteLine("File Content:");
            Console.WriteLine(File.ReadAllText(selectedFile));
            Console.WriteLine("Press any key to go back.");
            Console.ReadKey();
            ExploreDirectory(path);
        }
        else if (selectedIndex >= directories.Length)
        {
            // Enter directory
            selectedIndex -= files.Length;
            ExploreDirectory(directories[selectedIndex]);
        }
    }
    else
    {
        // Invalid key, retry
        ExploreDirectory(path);
    }
}
