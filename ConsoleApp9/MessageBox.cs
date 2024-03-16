using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public abstract class MessageBox
    {
        public string Action { get; set; }
        public string Text { get; set; }

        protected int w;
        protected int h;

        protected int msgbWidth;
        protected int msgbHeigth;

        protected int x;
        protected int y;

        public MessageBox(string action, string text, double koef1 = 1.8, double koef2 = 2.5)
        {
            Action = action;
            Text = text;

            // -------------------------------------

            w = Config.WindowWidth;
            h = Config.WindowHeight;

            msgbWidth = (int)(w / koef1);
            msgbHeigth = (int)(h / koef2);

            x = (w / 2) - (int)(msgbWidth / 2);
            y = (h / 2) - (int)(msgbHeigth / 2);
        }

        public void GetMessageBox()
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = Config.MsgBoxBackgroundColor;

            string buf = "";
            for (int i = 0; i < msgbHeigth; i++)
            {
                buf = "";
                for (int j = 0; j < msgbWidth; j++)
                {
                    buf += " ";
                }
                Console.Write(buf);
                Console.SetCursorPosition(x, ++Console.CursorTop);
            }

            Console.ForegroundColor = Config.MsgBoxForegroundColor;
            Console.SetCursorPosition(x + 3, y + 1);

            buf = "┌";
            for (int i = 0; i < msgbWidth - 8; i++)
            {
                buf += "─";
            }
            buf += "┐";
            Console.Write(buf);

            buf = "";
            Console.SetCursorPosition(x + 3, y + 2);
            for (int j = 0; j < msgbHeigth - 4; j++)
            {
                buf = "│";
                for (int i = 0; i < msgbWidth - 8; i++)
                {
                    buf += " ";
                }
                buf += "│";
                Console.Write(buf);

                Console.SetCursorPosition(x + 3, ++Console.CursorTop);
            }

            buf = "└";
            for (int i = 0; i < msgbWidth - 8; i++)
            {
                buf += "─";
            }
            buf += "┘";
            Console.WriteLine(buf);
        }

        protected void Draw()
        {
            Console.SetCursorPosition(x + 3 + ((msgbWidth - 6) / 2) - (Action.Length / 2), y + 2);
            Console.Write(Action);

            Console.SetCursorPosition(x + 3 + ((msgbWidth - 6) / 2) - (Text.Length / 2), y + 4);
            Console.Write(Text);

            Console.SetCursorPosition(x + 4, y + 6);
            string buf = "";
            for (int i = 0; i < msgbWidth - 8; i++)
            {
                buf += "─";
            }
            Console.WriteLine(buf);
        }
    }

    public class YesNoMessageBox : MessageBox
    {
        public YesNoMessageBox(string action, string text) : base(action, text) { }

        public new bool GetMessageBox()
        {
            base.GetMessageBox();
            base.Draw();
            bool selectedOK = false;

            while (true)
            {
                Console.SetCursorPosition(x + ((msgbWidth - 6) / 2) - 3, y + msgbHeigth - 3);

                if (selectedOK)
                {
                    Console.BackgroundColor = Config.AdditionalMsgBoxBackgroundColor;
                    Console.Write("OK");
                    Console.BackgroundColor = Config.MsgBoxBackgroundColor;
                }
                else
                    Console.Write("OK");

                Console.Write("      ");

                if (!selectedOK)
                {
                    Console.BackgroundColor = Config.AdditionalMsgBoxBackgroundColor;
                    Console.Write("CANCEL");
                    Console.BackgroundColor = Config.MsgBoxBackgroundColor;
                }
                else
                    Console.Write("CANCEL");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        Console.BackgroundColor = Config.BackgroundColor;
                        Console.ForegroundColor = Config.ForegroundColor;

                        if (selectedOK)
                            return true;
                        return false;
                    case ConsoleKey.LeftArrow:
                        selectedOK = true;
                        break;
                    case ConsoleKey.RightArrow:
                        selectedOK = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class OkMessageBox : MessageBox
    {
        public OkMessageBox(string action, string text) : base(action, text) { }

        public new bool GetMessageBox()
        {
            base.GetMessageBox();
            base.Draw();

            while (true)
            {
                Console.SetCursorPosition(x + ((msgbWidth) / 2), y + msgbHeigth - 3);
                Console.BackgroundColor = Config.AdditionalMsgBoxBackgroundColor;
                Console.Write("OK");
                Console.BackgroundColor = Config.MsgBoxBackgroundColor;

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        Console.BackgroundColor = Config.BackgroundColor;
                        Console.ForegroundColor = Config.ForegroundColor;
                        return true;
                    default:
                        break;
                }
            }
        }
    }

    public class MessageBoxWithSelect : MessageBox
    {
        public MessageBoxWithSelect(Options op) : base(string.Empty, string.Empty, 3.2, 1.2) { _options = op; }
        Options _options;

        public int[] GetMessageBoxWithSelect()
        {
            base.GetMessageBox();
            bool selectedLeft = true;

            _options.Sections[0].Cursor.X = x + 4;
            _options.Sections[0].Cursor.Y = y + 5;
            _options.Sections[1].Cursor.X = x + 4;
            _options.Sections[1].Cursor.Y = y + 5;
           
            Console.SetCursorPosition(x + 4, y + 4);
            string buf = "";
            for (int i = 0; i < msgbWidth - 8; i++)
            {
                buf += "─";
            }
            Console.WriteLine(buf);

            Console.SetCursorPosition(x + 5, y + 5);
            DrawOptions(selectedLeft);
            
            while (true)
            {
                if (selectedLeft)
                    _options.Sections[0].Cursor.UnmaskSelection();
                else
                    _options.Sections[1].Cursor.UnmaskSelection();

                DrawOptionsNames(selectedLeft);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        Console.BackgroundColor = Config.BackgroundColor;
                        Console.ForegroundColor = Config.ForegroundColor;
                        throw new Exception();
                    case ConsoleKey.Enter:
                        int n1, n2;
                        if (selectedLeft)
                            n1 = 0;
                        else
                            n1 = 1;

                        Console.BackgroundColor = Config.BackgroundColor;
                        Console.ForegroundColor = Config.ForegroundColor;

                        n2 = _options.Sections[n1].Cursor.Index;
                        return new int[2] { n1, n2 };
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.Tab:

                        if (selectedLeft)
                            _options.Sections[0].Cursor.HideSelection();
                        else
                            _options.Sections[1].Cursor.HideSelection();

                        selectedLeft = !selectedLeft;
                        Clean();
                        DrawOptions(selectedLeft);
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedLeft)
                            _options.Sections[0].Cursor.MoveDown();
                        else
                            _options.Sections[1].Cursor.MoveDown();
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedLeft)
                            _options.Sections[0].Cursor.MoveUp();
                        else
                            _options.Sections[1].Cursor.MoveUp();
                        break;
                }
            }
        }

        private void Clean()
        {
            Console.SetCursorPosition(x + 4, y + 5);
            for (int i = 0; i < msgbHeigth - 8; i++)
            {
                for (int j = 0; j < msgbWidth - 8; j++)
                {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(x + 4, y + 6 + i);
            }
        }

        private void DrawOptionsNames(bool selectedLeft)
        {
            Console.SetCursorPosition(x + 5, y + 3);
            if (selectedLeft)
            {
                Console.BackgroundColor = Config.AdditionalBackgroundColor;
                Console.Write(_options.Sections[0].Name);
                Console.BackgroundColor = Config.MsgBoxBackgroundColor;
            }
            else
            {
                Console.BackgroundColor = Config.MsgBoxBackgroundColor;
                Console.Write(_options.Sections[0].Name);
            }

            Console.SetCursorPosition(x + msgbWidth - 5 - _options.Sections[1].Name.Length, y + 3);
            if (!selectedLeft)
            {
                Console.BackgroundColor = Config.AdditionalBackgroundColor;
                Console.Write(_options.Sections[1].Name);
                Console.BackgroundColor = Config.MsgBoxBackgroundColor;
            }
            else
            {
                Console.BackgroundColor = Config.MsgBoxBackgroundColor;
                Console.Write(_options.Sections[1].Name);
            }
        }

        private void DrawOptions(bool flag)
        {
            Console.SetCursorPosition(x + 5, y + 5);
            if (flag)
            {
                foreach (string i in _options.Sections[0].Cursor.FileNames)
                {
                    Console.Write(i);
                    Console.SetCursorPosition(x + 5, ++Console.CursorTop);
                }
                return;
            }

            foreach (string i in _options.Sections[1].Cursor.FileNames)
            {
                Console.Write(i);
                Console.SetCursorPosition(x + 5, ++Console.CursorTop);
            }
        }
    }
}
