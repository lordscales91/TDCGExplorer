using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCG;

namespace TSOHair
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TSOHair.exe <tso file> <basename>");
                return;
            }
            string source_file = args[0];

            string basename = "N000BHEA_C00";
            if (args.Length > 1)
            {
                basename = args[1];
            }

            string col = basename.Substring(10,2);

            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            TDCG.TSOHair.TSOHairProcessor processor = new TDCG.TSOHair.TSOHairProcessor();
            processor.Process(tso, col);

            tso.Save(string.Format("{0}.tso", basename));
        }
    }
}
