using System;
using System.IO;

namespace TAHdecrypt
{
    class TMOBoin
    {
        static void Main(string[] args) 
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TMOBoin <tmo file>");
                return;
            }

            string source_file = args[0];

            Boin.UpdateTmo(source_file);
        }
    }
}
