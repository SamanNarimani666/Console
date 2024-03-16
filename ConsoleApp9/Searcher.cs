using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace FileManager
{
    public class Searcher
    {
        public ArrayList Results { get; private set; }

        public Searcher()
        {
            Results = new ArrayList();
        }

        public void Clear()
        {
            Results.Clear();
        }

        public ArrayList GetResults()
        {
            return Results;
        }

        public void Search(DirectoryInfo dir, string fileName)
        {
            try
            {
                ArrayList _tmpFiles = new ArrayList();
                _tmpFiles.AddRange(dir.GetDirectories());
                _tmpFiles.AddRange(dir.GetFiles());

                foreach (var file in _tmpFiles)
                {
                    if (file is DirectoryInfo)
                    {
                        if (Regex.IsMatch((file as DirectoryInfo).Name, fileName))
                            Results.Add(file);

                        Search(file as DirectoryInfo, fileName);
                    }
                    else
                    {
                        if (Regex.IsMatch((file as FileInfo).Name, fileName))
                            Results.Add(file);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
