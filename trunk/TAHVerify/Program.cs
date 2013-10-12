using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace TAHVerify
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
            if (args.Length != 1)
            {
                Console.WriteLine("TAHVerify <readdirectory>");
                return;
            }
            try
            {
                TAHCompact.Verify(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(rescentpath);
                Console.WriteLine(e.Message);
            }
        }
    }
}
