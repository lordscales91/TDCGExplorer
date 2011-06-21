using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCG;

namespace tso2pmx
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: tso2pmx <source tso>");
                return;
            }

            string source_file = args[0];

            Program program = new Program();
            program.Process(source_file);
        }

        public bool Process(string source_file)
        {
            TSOFile tso;
            tso = new TSOFile();
            tso.Load(source_file);

            PmxFile pmx = new PmxFile();

            string dest_file = "out.pmx";
            Console.WriteLine("Save File: " + dest_file);
            pmx.Save(dest_file);

            return true;
        }
    }
}
