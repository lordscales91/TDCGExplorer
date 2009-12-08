using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PSDTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("PSDTest.exe <psd file>");
                return;
            }
            string source_file = args[0];
            Bitmap psd = TDCGMan.PSDFile.LoadImage(source_file);
            psd.Save(@"sample.png");
        }
    }
}
