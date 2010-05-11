using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TAHTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TAHTest.exe <tah file>");
                return;
            }
            string source_file = args[0];

            TDCG.TAHTool.Decrypter decrypter = new TDCG.TAHTool.Decrypter();
            decrypter.Load(source_file);
            decrypter.Close();

            foreach (TDCG.TAHTool.TAHEntry entry in decrypter.Entries)
            {
                string dirname = Path.GetDirectoryName(entry.file_name);
                string basename = Path.GetFileNameWithoutExtension(entry.file_name);
                string extname = Path.GetExtension(entry.file_name).ToLower();
                if (extname == ".tbn")
                    Console.WriteLine("{0}\t{1}\t{2}", dirname, basename, extname);
                else
                if (extname == ".psd")
                    Console.WriteLine("{0}\t{1}\t{2}", dirname, basename, extname);
            }
        }
    }
}
