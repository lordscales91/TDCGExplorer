using System;
using System.Collections.Generic;
using System.IO;

public static class TMODump
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Usage: TMODump <source file>");
            return;
        }

        string source_file = args[0];
        try
        {
            string ext = Path.GetExtension(source_file).ToUpper();
            if (ext == ".TMO")
            {
                DumpTMOEntries(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                DumpDirEntries(source_file);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }

    public static void DumpDirEntries(string dir)
    {
        string[] tmo_files = Directory.GetFiles(dir, "*.TMO");
        foreach (string file in tmo_files)
        {
            DumpTMOEntries(file);
        }
        string[] entries = Directory.GetDirectories(dir);
        foreach (string entry in entries)
        {
            DumpDirEntries(entry);
        }
    }

    public static void DumpTMOEntries(string source_file)
    {
        Console.WriteLine("# TMO " + source_file);
    }
}
