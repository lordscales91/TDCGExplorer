using System;
using System.IO;

namespace TAHdecrypt
{
    class TMOZoom
    {
        static void Main(string[] args) 
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine("Usage: TMOZoom <tmo file> <ratio>");
                return;
            }

            string source_file = args[0];
            float ratio = 1.0f;
            try
            {
                ratio = Single.Parse(args[1]);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Zoom.UpdateTmo(source_file, ratio);
        }
    }
}
