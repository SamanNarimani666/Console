
string text = "";
int cursorPosition = 0;

while (true)
{
    Console.Clear();
    Console.WriteLine(text);
    Console.SetCursorPosition(cursorPosition, 0);

    var input = Console.ReadKey(true).Key;

    switch (input)
    {
        case ConsoleKey.UpArrow:
            if (cursorPosition > 0)
                cursorPosition--;
            break;
        case ConsoleKey.DownArrow:
            if (cursorPosition < text.Length)
                cursorPosition++;
            break;
        case ConsoleKey.LeftArrow:
            if (cursorPosition > 0)
                cursorPosition--;
            break;
        case ConsoleKey.RightArrow:
            if (cursorPosition < text.Length)
                cursorPosition++;
            break;
        case ConsoleKey.Backspace:
            if (cursorPosition > 0)
            {
                text = text.Remove(cursorPosition - 1, 1);
                cursorPosition--;
            }
            break;
        case ConsoleKey.Enter:
            text += "\n";
            cursorPosition++;
            break;
        case ConsoleKey.Escape:
            return;
    }
}


