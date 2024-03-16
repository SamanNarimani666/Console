using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class OptionsSection
    {
        public string Name { get; set; }
        public Cursor Cursor { get; set; }

        public OptionsSection(string name, Cursor cursor)
        {
            Name = name;
            Cursor = cursor;
        }
    }

    public class Options
    {
        public List<OptionsSection> Sections { get; set; }

        public Options()
        {
            Sections = new List<OptionsSection>();
        }

        public void Add(OptionsSection sec)
        {
            Sections.Add(sec);
        }
    }
}
