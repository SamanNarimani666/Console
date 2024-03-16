using System;

new Editor().Run();


class Editor
{
    Buffer _buffer;
    Cursor _cursor;
    Stack<(Cursor, Buffer)> _history;

    public Editor()
    {
        var lines = File.ReadAllLines("test.txt").Where(x => x != Environment.NewLine).ToArray();
        _buffer = new Buffer(lines);
        _cursor = new Cursor();
        _history = new Stack<(Cursor, Buffer)>();
    }

    public void Run()
    {
        while (true)
        {
            Render();
            var character = Console.ReadKey(true);
            HandleInput(character);
        }
    }
    private void HandleInput(ConsoleKeyInfo character)
    {
        if ((character.Modifiers & ConsoleModifiers.Control) != 0)
        {
            if (character.Key == ConsoleKey.Q)
            {
                Environment.Exit(0);
            }
            if (character.Key == ConsoleKey.S)
            {
                File.WriteAllLines("test.txt", _buffer.GetLines());
                Environment.Exit(0);
            }

            else if (character.Key == ConsoleKey.Z)
            {
                RestoreSnapshot();
            }
        }
        else
        {
            if (character.Key == ConsoleKey.UpArrow)
            {
                MoveCursorUp();
            }
            else if (character.Key == ConsoleKey.DownArrow)
            {
                MoveCursorDown();
            }
            else if (character.Key == ConsoleKey.LeftArrow)
            {
                MoveCursorLeft();
            }
            else if (character.Key == ConsoleKey.RightArrow)
            {
                MoveCursorRight();
            }
            else if (character.Key == ConsoleKey.Backspace)
            {
                DeleteCharacter();
            }
            else if (character.Key == ConsoleKey.Enter)
            {
                InsertNewLine();
            }
            else if (IsTextChar(character))
            {
                InsertCharacter(character.KeyChar);
            }
        }
    }

    private bool IsTextChar(ConsoleKeyInfo character)
    {
        return !Char.IsControl(character.KeyChar);
    }

    private void Render()
    {
        Console.Clear();
        _buffer.Render();
        Console.SetCursorPosition(_cursor.Col, _cursor.Row);
    }

    private void SaveSnapshot()
    {
        _history.Push((_cursor.Clone(), _buffer.Clone()));
    }

    private void RestoreSnapshot()
    {
        if (_history.Count > 0)
        {
            var (cursor, buffer) = _history.Pop();
            _cursor = cursor;
            _buffer = buffer;
        }
    }

    private void MoveCursorUp()
    {
        SaveSnapshot();
        _cursor = _cursor.Up(_buffer);
    }

    private void MoveCursorDown()
    {
        SaveSnapshot();
        _cursor = _cursor.Down(_buffer);
    }

    private void MoveCursorLeft()
    {
        SaveSnapshot();
        _cursor = _cursor.Left(_buffer);
    }

    private void MoveCursorRight()
    {
        SaveSnapshot();
        _cursor = _cursor.Right(_buffer);
    }

    private void DeleteCharacter()
    {
        if (_cursor.Col > 0)
        {
            SaveSnapshot();
            _buffer = _buffer.Delete(_cursor.Row, _cursor.Col - 1);
            _cursor = _cursor.Left(_buffer);
        }
        else if (_cursor.Col == 0)
        {
            SaveSnapshot();
            _buffer = _buffer.DeleteLine(_cursor.Row, _cursor.Col);
            _cursor = _cursor.Up(_buffer).MoveToRow(_cursor.Row - 1);
        }
    }

    private void InsertNewLine()
    {
        SaveSnapshot();
        _buffer = _buffer.SplitLine(_cursor.Row, _cursor.Col);
        _cursor = _cursor.Down(_buffer).MoveToCol(0);
    }

    private void InsertCharacter(char character)
    {
        SaveSnapshot();
        _buffer = _buffer.Insert(character.ToString(), _cursor.Row, _cursor.Col);
        _cursor = _cursor.Right(_buffer);
    }


}

class Buffer
{
    string[] _lines;

    public Buffer(IEnumerable<string> lines)
    {
        _lines = lines.ToArray();
    }

    public IEnumerable<string> GetLines()
    {
        var result = _lines;
        return result;
    }

    public void Render()
    {
        foreach (var line in _lines)
        {
            Console.WriteLine(line);
        }
    }

    public int LineCount()
    {
        return _lines.Length;
    }

    public int LineLength(int row)
    {
        return _lines[row].Length;
    }

    internal Buffer Insert(string character, int row, int col)
    {
        var linesDeepCopy = (string[])_lines.Clone();
        linesDeepCopy[row] = linesDeepCopy[row].Insert(col, character);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer Delete(int row, int col)
    {
        var linesDeepCopy = (string[])_lines.Clone();
        linesDeepCopy[row] = linesDeepCopy[row].Remove(col, 1);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer SplitLine(int row, int col)
    {
        var linesDeepCopy = new List<string>(_lines);
        var line = linesDeepCopy[row];
        var newLines = new[] { line.Substring(0, col), line.Substring(col) };
        linesDeepCopy[row] = newLines[0];
        linesDeepCopy.Insert(row + 1, newLines[1]);
        return new Buffer(linesDeepCopy);
    }

    internal Buffer DeleteLine(int row, int col)
    {

        var linesDeepCopy = (string[])_lines.Clone();
        try
        {
            if (linesDeepCopy[row] == "")
            {
                if (row >= 0 && row < linesDeepCopy.Length)
                {
                    for (int i = row; i < linesDeepCopy.Length - 1; i++)
                    {
                        linesDeepCopy[i] = linesDeepCopy[i + 1];
                    }

                    Array.Resize(ref linesDeepCopy, linesDeepCopy.Length - 1);
                }
            }
            else if (linesDeepCopy[row] != string.Empty)
            {
                if (row >= 0 && row < linesDeepCopy.Length && row - 1 < linesDeepCopy.Length)
                {
                    linesDeepCopy[row - 1] = linesDeepCopy[row - 1] + linesDeepCopy[row];


                    for (int i = row; i < linesDeepCopy.Length - 1; i++)
                    {
                        linesDeepCopy[i] = linesDeepCopy[i + 1];
                    }

                    Array.Resize(ref linesDeepCopy, linesDeepCopy.Length - 1);
                }
            }
        }
        catch { }
        return new Buffer(linesDeepCopy);
    }




    internal Buffer Clone()
    {
        return new Buffer(_lines);
    }

}

class Cursor
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    public Cursor(int row = 0, int col = 0)
    {
        Row = row;
        Col = col;
    }

    public Cursor Clone()
    {
        return new Cursor(Row, Col);
    }

    internal Cursor Up(Buffer buffer)
    {
        if (Row >= 0)
        {
            var lines = buffer.GetLines().ToArray();
            if (Col == lines[Row].ToString().Trim().Length)
            {
                if (Row != 0)
                {
                    MoveToRow(Row);
                    Col = lines[Row - 1].ToString().Length + 1;
                    MoveToCol(Col);
                }
            }
        }
        return new Cursor(Row - 1, Col).Clamp(buffer);
    }

    internal Cursor Down(Buffer buffer)
    {
        var lines = buffer.GetLines().ToArray();
        if (Col <= lines[Row].ToString().Trim().Length)
        {
            MoveToRow(Row + 1);
            if (Row + 1 <= lines[Row + 1].ToString().Length + 1)
            {
                Col = lines[Row + 1].ToString().Length + 1;
                MoveToCol(Col);
            }
        }
        return new Cursor(Row + 1, Col).Clamp(buffer);
    }

    internal Cursor Left(Buffer buffer)
    {
        var lines = buffer.GetLines().ToArray();
        if (Col <= lines[Row].ToString().Trim().Length)
        {
            if (Row > 0 && Col == 0)
            {
                Row -= 1;
                MoveToRow(Row);
                Col = lines[Row].ToString().Length + 1;
                MoveToCol(Col);
            }
        }
        return new Cursor(Row, Col - 1).Clamp(buffer);
    }

    internal Cursor Right(Buffer buffer)
    {
        try
        {
            var lines = buffer.GetLines().ToArray();
            if (Col >= lines[Row].ToString().Trim().Length)
            {
                Row += 1;
                MoveToRow(Row);

                Col = 0;
                MoveToCol(0);

            }
        }
        catch { }

        return new Cursor(Row, Col + 1).Clamp(buffer);
    }

    private Cursor Clamp(Buffer buffer)
    {
        Row = Math.Min(buffer.LineCount() - 1, Math.Max(Row, 0));
        Col = Math.Min(buffer.LineLength(Row), Math.Max(Col, 0));
        return new Cursor(Row, Col);
    }

    internal Cursor MoveToCol(int col)
    {
        return new Cursor(Row, col);
    }

    internal Cursor MoveToRow(int row)
    {
        return new Cursor(row, this.Col);
    }
}
