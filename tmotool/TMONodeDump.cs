using System;
using System.IO;

namespace TDCG
{
    class TMONodeDump
    {
        static void Main(string[] args) 
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TMONodeDump <tmo file>");
                return;
            }

            string tmo_file = args[0];

            Console.WriteLine("Load File: " + tmo_file);
            TMOFile tmo = new TMOFile();
            try
            {
                tmo.Load(tmo_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            foreach (TMONode node in tmo.nodes)
                Console.WriteLine(node.Name);
        }
    }
}
