using System;
using System.IO;

namespace TAHdecrypt
{
    class TMODom
    {
        static void Main(string[] args) 
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TMODom <tmo file>");
                return;
            }

            string source_file = args[0];

            Dom.UpdateTmo(source_file);
        }
    }
}
