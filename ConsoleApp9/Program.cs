using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FileManager
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string settings;
                using (var sr = File.OpenText("config.txt"))
                {
                    settings = sr.ReadToEnd();
                }
                Config.ChangeColorScheme(settings);

                string[] res = settings.Split(",./! ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int _w = Convert.ToInt32(res[res.Length - 2]);
                int _h = Convert.ToInt32(res[res.Length - 1]);

                Config.ChangeResolution(_w, _h);
            }
            catch (Exception) { }

            while (true)
            {
                // -------------------------------------------------

                Options options = new Options();
                OptionsSection themeSelector = new OptionsSection("Theme", new Cursor(0, 0, new ArrayList { "Dark theme", "White theme", "Blue theme", "Open theme" }));
                OptionsSection screenResSelector = new OptionsSection("Resolution", new Cursor(0, 0, new ArrayList { "Fullscreen", "140/40", "175/45", "Default" }));

                options.Sections.Add(themeSelector);
                options.Sections.Add(screenResSelector);

                MessageBoxWithSelect optionsMessageBox = new MessageBoxWithSelect(options);

                // -------------------------------------------------

                bool FullRestart = false;
                GC.Collect();

                FileManager manager = new FileManager("File Manager");
                Console.Clear();

                GUI.DrawMenu(manager.FileManagerName);
                GUI.DrawName();
                GUI.DrawBottomPanel();
                Console.CursorVisible = false;

                manager.DisplayFilesFromTwoSections();
                YesNoMessageBox MsgBox = new YesNoMessageBox("none", "none");
                OkMessageBox InfMsgBox = new OkMessageBox("Information!", "none");
                TextBox TextBox = new TextBox("Please, write name", "");
                Searcher FileSearcher = new Searcher();

                while (!FullRestart)
                {
                    GUI.DrawPathes(manager.Esection, manager.LPath, manager.RPath);

                    if (manager.Esection == ESection.Left)
                        manager.LeftSection.Cursor.UnmaskSelection();
                    else
                        manager.RightSection.Cursor.UnmaskSelection();

                    bool canReturn = false;
                    while (!canReturn)
                    {
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.LeftArrow:
                                manager.MoveUp();
                                break;
                            case ConsoleKey.UpArrow:
                                manager.MoveUp();
                                break;
                            case ConsoleKey.RightArrow:
                                manager.MoveDown();
                                break;
                            case ConsoleKey.DownArrow:
                                manager.MoveDown();
                                break;
                            case ConsoleKey.Tab:
                                manager.ChangeSection();
                                break;
                            case ConsoleKey.Enter:
                                {
                                    if (manager.GetCurrentFile() is FileInfo)
                                    {
                                        try
                                        { Process.Start(manager.GetSelectedPath()); }
                                        catch (Exception) { }
                                        break;
                                    }
                                    try
                                    {
                                        manager.ChangeDirectory(manager.GetSelectedPath());
                                        manager.DisplayFiles();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is UnauthorizedAccessException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "You don't have permissions enough.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        manager.Reload();
                                    }

                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.Escape:
                            case ConsoleKey.Backspace:
                                {
                                    manager.ChangeDirectory("..");
                                    manager.DisplayFiles();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F1:
                                {
                                    InfMsgBox.Action = "Information";
                                    try
                                    { InfMsgBox.Text = manager.GetFileInfo(); }
                                    catch (Exception) { break; }

                                    InfMsgBox.GetMessageBox();

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F2:
                                {
                                    if (!manager.CanCreate())
                                        break;

                                    string fileName;
                                    try
                                    { fileName = TextBox.GetMessageBox(); }
                                    catch (Exception)
                                    {
                                        manager.Reload();
                                        canReturn = true;
                                        break;
                                    }

                                    try
                                    { manager.CreateFile(fileName); }
                                    catch (Exception ex)
                                    {
                                        if (ex is UnauthorizedAccessException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "You don't have permissions enough.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        else
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "Name contains incorrect symbols.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F3:
                                {
                                    if (!manager.CanCreate())
                                        break;

                                    string name;
                                    try
                                    { name = TextBox.GetMessageBox(); }
                                    catch (Exception)
                                    {
                                        manager.Reload();
                                        canReturn = true;
                                        break;
                                    }

                                    try
                                    { manager.CreateFolder(name); }
                                    catch (Exception ex)
                                    {
                                        if (ex is UnauthorizedAccessException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "You don't have permissions enough.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        else
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "Name contains incorrect symbols.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F4:
                                {
                                    InfMsgBox.Action = "Attributes";
                                    try
                                    { InfMsgBox.Text = manager.GetFileAttributes(); }
                                    catch (Exception) { break; }

                                    InfMsgBox.GetMessageBox();

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F5:
                                {
                                    if (manager.GetCurrentFile() is FolderUp || manager.GetCurrentFile() is DriveInfo)
                                        break;

                                    MsgBox.Action = "Copy";
                                    MsgBox.Text = "Do you really want to copy?";

                                    try
                                    {
                                        if (MsgBox.GetMessageBox())
                                            manager.CopyCurrentFile();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is IOException)
                                        {
                                            MsgBox.Action = "Copy";
                                            MsgBox.Text = "File alredy exists. Replace?";

                                            if (MsgBox.GetMessageBox())
                                                manager.CopyCurrentFile(true);
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F6:
                                {
                                    if (manager.GetCurrentFile() is FolderUp || manager.GetCurrentFile() is DriveInfo)
                                        break;

                                    MsgBox.Action = "Move";
                                    MsgBox.Text = "Do you really want to move?";

                                    try
                                    {
                                        if (MsgBox.GetMessageBox())
                                            manager.MoveCurrentFile();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is IOException)
                                        {
                                            MsgBox.Action = "Move";
                                            MsgBox.Text = "File alredy exists. Replace?";

                                            if (MsgBox.GetMessageBox())
                                                manager.MoveCurrentFile(true);
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F7:
                                {
                                    string search;
                                    try
                                    { search = TextBox.GetMessageBox(); }
                                    catch (Exception)
                                    {
                                        manager.Reload();
                                        canReturn = true;
                                        break;
                                    }

                                    try
                                    {
                                        manager.SearchFile(search);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is NoResultException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "No results.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        else if (ex is InvalidOperationException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "Search is already running. First close working search.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        else
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "Name contains incorrect symbols.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.Delete:
                            case ConsoleKey.F8:
                                {
                                    if (manager.GetCurrentFile() is FolderUp || manager.GetCurrentFile() is DriveInfo)
                                        break;

                                    MsgBox.Action = "Delete";
                                    MsgBox.Text = "Do you really want to delete?";

                                    try
                                    {
                                        if (MsgBox.GetMessageBox())
                                            manager.DeleteCurrentFile();
                                    }
                                    catch (Exception) { }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F9:
                                {
                                    manager.DisplayDisks();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F10:
                                {
                                    if (manager.GetCurrentFile() is FolderUp || manager.GetCurrentFile() is DriveInfo)
                                        break;

                                    string name;
                                    try
                                    { name = TextBox.GetMessageBox(); }
                                    catch (Exception)
                                    {
                                        manager.Reload();
                                        canReturn = true;
                                        break;
                                    }

                                    try
                                    { manager.RenameFile(name); }
                                    catch (Exception ex)
                                    {
                                        if (ex is UnauthorizedAccessException)
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "You don't have permissions enough.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                        else
                                        {
                                            InfMsgBox.Action = "Error";
                                            InfMsgBox.Text = "Name contains incorrect symbols.";
                                            InfMsgBox.GetMessageBox();
                                        }
                                    }

                                    manager.Reload();
                                    canReturn = true;
                                }
                                break;
                            case ConsoleKey.F12:
                                {
                                    int[] arr = new int[2];
                                    try
                                    { arr = optionsMessageBox.GetMessageBoxWithSelect(); }
                                    catch (Exception)
                                    {
                                        manager.Reload();
                                        canReturn = true;
                                        break;
                                    }

                                    int op = arr[0];
                                    int num = arr[1];
                                    if (op == 0)
                                    {
                                        switch (num)
                                        {
                                            case 0:
                                                Config.ChangeColorScheme(new ConsoleColor[7]
                                                { ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Red, ConsoleColor.White,
                                            ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Gray });
                                                break;
                                            case 1:
                                                Config.ChangeColorScheme(new ConsoleColor[7]
                                                { ConsoleColor.White, ConsoleColor.Black, ConsoleColor.DarkGray, ConsoleColor.Gray,
                                            ConsoleColor.Black, ConsoleColor.DarkGray, ConsoleColor.White });
                                                break;
                                            case 2:
                                                Config.ChangeColorScheme(new ConsoleColor[7]
                                                { ConsoleColor.DarkBlue, ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Gray,
                                            ConsoleColor.Black, ConsoleColor.Cyan, ConsoleColor.White });
                                                break;
                                            case 3:
                                                {
                                                    var fileContent = string.Empty;
                                                    var filePath = string.Empty;

                                                    try
                                                    {   
                                                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                                                        {
                                                            openFileDialog.InitialDirectory = ".";
                                                            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                                                            openFileDialog.FilterIndex = 2;
                                                            openFileDialog.RestoreDirectory = true;

                                                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                                                            {
                                                                //Get the path of specified file
                                                                filePath = openFileDialog.FileName;

                                                                //Read the contents of the file into a stream
                                                                var fileStream = openFileDialog.OpenFile();

                                                                using (StreamReader reader = new StreamReader(fileStream))
                                                                {
                                                                    fileContent = reader.ReadToEnd();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception) { break; }

                                                    try
                                                    {
                                                        Config.ChangeColorScheme(fileContent);
                                                    }
                                                    catch (Exception) { }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    if (op == 1)
                                    {
                                        switch (num)
                                        {
                                            case 0:
                                                Config.ChangeResolution(Console.LargestWindowWidth, Console.LargestWindowHeight);
                                                break;
                                            case 1:
                                                Config.ChangeResolution(140, 40);
                                                break;
                                            case 2:
                                                Config.ChangeResolution(175, 45);
                                                break;
                                            case 3:
                                                Config.ChangeResolution(120, 30);
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    Config.SaveConfig();
                                    canReturn = true;
                                    FullRestart = true;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
