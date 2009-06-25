using System;
using System.IO;

namespace TAHdecrypt
{
    class TMOPedo
    {
        static void Main(string[] args) 
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TMOPedo <tmo file>");
                return;
            }

            string source_file = args[0];

            Pedo.UpdateTmo(source_file);
        }
    }
}
