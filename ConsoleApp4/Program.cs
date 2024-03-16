using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int cursorLeft = 0;
        int cursorTop = 0;
        List<string> text = new List<string>();
        Console.SetCursorPosition(cursorLeft, cursorTop);

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.Escape)
                break;

            Console.SetCursorPosition(cursorLeft, cursorTop);

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    cursorTop = Math.Max(0, cursorTop - 1);
                    break;
                case ConsoleKey.DownArrow:
                    cursorTop = Math.Min(Console.WindowHeight - 1, cursorTop + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    cursorLeft = Math.Max(0, cursorLeft - 1);
                    break;
                case ConsoleKey.RightArrow:
                    cursorLeft = Math.Min(Console.WindowWidth - 1, cursorLeft + 1);
                    break;
                case ConsoleKey.Enter:
                    text.Add(Environment.NewLine);
                    cursorTop++;
                    cursorLeft = 0;
                    break;
                case ConsoleKey.Spacebar:
                    text.Add(" ");
                    cursorLeft++;
                    break;
                case ConsoleKey.Tab:
                    text.Add("\t");
                    cursorLeft += 4; // Adjusting for tab key width
                    break;
                default:
                    if (char.IsDigit(keyInfo.KeyChar) || char.IsLetter(keyInfo.KeyChar))
                    {
                        text.Add(keyInfo.KeyChar.ToString());
                        cursorLeft++;
                    }
                    break;
            }
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }


        Console.Clear();
        Console.WriteLine("==========");
        foreach (var item in text)
        {
            Console.Write(item);
        }
    }
}