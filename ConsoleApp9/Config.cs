using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileManager
{
    public static class Config
    {
        // Can be changed
        public static ConsoleColor  BackgroundColor                 = ConsoleColor.Black;
        public static ConsoleColor  ForegroundColor                 = ConsoleColor.White;
        public static ConsoleColor  AdditionalBackgroundColor       = ConsoleColor.Red;
        public static ConsoleColor  MsgBoxBackgroundColor           = ConsoleColor.White;
        public static ConsoleColor  MsgBoxForegroundColor           = ConsoleColor.Black;
        public static ConsoleColor  AdditionalMsgBoxBackgroundColor = ConsoleColor.Red;
        public static ConsoleColor  TextBoxBackgroundColor          = ConsoleColor.Gray;
        public static string        LeftSectionPath                 = @".";
        public static string        RightSectionPath                = @".";
        public static int           StartupWindowWidth              = /*Console.LargestWindowWidth;*/120; // Default - 120
        public static int           StartupWindowHeight             = /*Console.LargestWindowHeight;*/30; // Default - 30

        // Can not be changed
        public static int           WindowWidth                     = StartupWindowWidth - 3;
        public static int           QuarterWindowWidth              = WindowWidth / 4;
        public static int           HalfWindowWidth                 = WindowWidth / 2;
        public static int           WindowHeight                    = StartupWindowHeight - 3;
        public static int           FilesCountOneSection            = StartupWindowHeight - 11;
        public static int           FilesCountTwoSections           = FilesCountOneSection * 2;
        public static string        ProgramLocation                 = Directory.GetCurrentDirectory();

        public static void ChangeColorScheme(ConsoleColor[] colors)
        {
            BackgroundColor = colors[0];
            ForegroundColor = colors[1];
            AdditionalBackgroundColor = colors[2];
            MsgBoxBackgroundColor = colors[3];
            MsgBoxForegroundColor = colors[4];
            AdditionalMsgBoxBackgroundColor = colors[5];
            TextBoxBackgroundColor = colors[6];
        }

        public static void ChangeColorScheme(string str)
        {
            if (str.Length > 150)
                return;

            string[] colors;
            try
            {
                colors = str.Split(",.!-/?\\_ ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception) { throw; }

            Enum.TryParse(colors[0], out BackgroundColor);
            Enum.TryParse(colors[1], out ForegroundColor);
            Enum.TryParse(colors[2], out AdditionalBackgroundColor);
            Enum.TryParse(colors[3], out MsgBoxBackgroundColor);
            Enum.TryParse(colors[4], out MsgBoxForegroundColor);
            Enum.TryParse(colors[5], out AdditionalMsgBoxBackgroundColor);
            Enum.TryParse(colors[6], out TextBoxBackgroundColor);
        }

        public static void ChangeResolution(int w, int h)
        {
            try
            {
                StartupWindowWidth = w;
                StartupWindowHeight = h;

                WindowWidth = StartupWindowWidth - 3;
                QuarterWindowWidth = WindowWidth / 4;
                HalfWindowWidth = WindowWidth / 2;
                WindowHeight = StartupWindowHeight - 3;
                FilesCountOneSection = StartupWindowHeight - 11;
                FilesCountTwoSections = FilesCountOneSection * 2;
            }
            catch(Exception) { }
        }

        public static void SaveConfig()
        {
            try
            {
                using (var sw = File.CreateText($"{ProgramLocation}\\config.txt"))
                {
                    sw.Write(
                        BackgroundColor + " " +
                        ForegroundColor + " " +
                        AdditionalBackgroundColor + " " +
                        MsgBoxBackgroundColor + " " +
                        MsgBoxForegroundColor + " " +
                        AdditionalMsgBoxBackgroundColor + " " +
                        TextBoxBackgroundColor + " "
                        );
                    sw.Write(StartupWindowWidth + " " + StartupWindowHeight);
                }
            }
            catch (Exception) { }
        }
    }
}
