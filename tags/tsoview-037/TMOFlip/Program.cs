using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TDCG;

namespace TMOFlip
{
class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOFlip.exe <tmo file>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();
        tmo.Load(source_file);

        TDCG.TMOFlip.TMOFlipProcessor processor = new TDCG.TMOFlip.TMOFlipProcessor();
        processor.Process(tmo);

        string dest_path = Path.GetDirectoryName(source_file);
        string dest_file = Path.GetFileNameWithoutExtension(source_file) + @".new.tmo";
        dest_path = Path.Combine(dest_path, dest_file);
        Console.WriteLine("Save File: " + dest_path);
        tmo.Save(dest_path);
    }
}
}
