using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace TAHCompact
{
    class Program
    {
        static string rescentpath;

        static public void SetRescent(string pathname)
        {
            rescentpath = pathname;
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("TAHCompact <readdirectory> <destination.tah> <version-number>");
                return;
            }
            try
            {
                TAHCompact.Compaction(args[0], args[1], uint.Parse(args[2]));
            }
            catch (Exception e)
            {
                Console.WriteLine(rescentpath);
                Console.WriteLine(e.Message);
            }
        }
    }
}
