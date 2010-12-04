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
                Console.WriteLine("TBNTest.exe <tbn file> <basename>");
                return;
            }
            string source_file = args[0];

            string basename = "N000BHEA_C00";
            if (args.Length > 1)
            {
                basename = args[1];
            }

            TBNFile tbn = new TBNFile();
            tbn.Load(source_file);
            tbn.Dump();
            Dictionary<uint, string> strings = tbn.GetStringDictionary();
            Regex re_tsofile = new Regex(@"\.tso$");
            foreach (uint i in strings.Keys)
            {
                string str = strings[i];
                if (re_tsofile.IsMatch(str))
                {
                    Console.WriteLine("{0:X4}: {1}", i, str);
                    tbn.SetString(i, string.Format("data/model/{0}.tso", basename));
                }
            }

            tbn.Save(string.Format("{0}.tbn", basename));
        }
    }
}
