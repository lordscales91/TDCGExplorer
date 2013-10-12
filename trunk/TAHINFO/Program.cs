using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TAHINFO
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(string filename in args)
            {
                Console.WriteLine("tah file:" + filename);
                FileStream fs = File.OpenRead(filename);
                TAHFile tahfiles = new TAHFile(fs);
                tahfiles.LoadEntries();
                foreach (TAHEntry entry in tahfiles.EntrySet.Entries)
                {
                    Console.WriteLine(entry.FileName + " offset:" + entry.DataOffset + " size:" + entry.Length);
                }
                Console.WriteLine("object: " + tahfiles.EntrySet.Count.ToString());
            }
        }
    }
}
