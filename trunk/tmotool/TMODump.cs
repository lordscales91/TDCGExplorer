using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public static class TMODump
{
    static Regex re;

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
                re = new Regex(@"\A" + Regex.Escape(Path.GetDirectoryName(source_file)) + @"\\?");
                DumpTMOEntries(source_file);
            }
            else if (Directory.Exists(source_file))
            {
                re = new Regex(@"\A" + Regex.Escape(source_file) + @"\\?");
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
        string dest_file = re.Replace(source_file, @"");
        Console.WriteLine("# TMO " + dest_file);
    }
}
