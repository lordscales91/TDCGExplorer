using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TDCG;

namespace TBNTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TBNTest.exe <tbn file>");
                return;
            }
            string source_file = args[0];

            TBNFile tbn = new TBNFile();
            tbn.Load(source_file);
            tbn.Dump();
            List<string> strings = tbn.GetStringList();
            Regex re_tsofile = new Regex(@"\.tso$");
            foreach (string str in strings)
            {
                if (re_tsofile.IsMatch(str))
                    Console.WriteLine(str);
            }

            tbn.SetString(0x08A6, "data/model/N327BHEA_C01.tso");
            tbn.Save(@"out.tbn");
        }
    }
}
