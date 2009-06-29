using System;
using System.IO;

namespace TAHdecrypt
{
    class TMORotY
    {
        static void Main(string[] args) 
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: TMORotY <tmo file> <angle>");
                return;
            }

            string source_file = args[0];
            float angle = 0.0f;
            try
            {
                angle = Single.Parse(args[1]);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            RotY.UpdateTmo(source_file, angle);
        }
    }
}
