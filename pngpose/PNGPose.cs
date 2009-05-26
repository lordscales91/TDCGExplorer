using System;
using System.IO;

namespace TAHdecrypt
{
class PNGPose
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("Usage: PNGPose <png file>");
            return;
        }

        string source_file = args[0];
        PNGFile png = new PNGFile();

        if (source_file.Substring(source_file.Length - 3, 3).ToLower().Equals("png"))
        {
            string dest_path = "";
            string[] sep = new string[1];
            sep[0] = "\\";
            string[] file_path = source_file.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
            string file_name = file_path[file_path.Length - 1];
            string folder_name = file_name.Substring(0, file_name.LastIndexOf("."));
            for (int i = 0; i < file_path.Length - 1; i++)
            {
                dest_path += file_path[i] + "\\";
            }
            dest_path += folder_name;
            Console.WriteLine("Extract File: " + source_file);
            png.Extract(source_file, dest_path);
        }
        else if (System.IO.Directory.Exists(source_file))
        {
            Console.WriteLine("Compose File: " + source_file);
            png.Compose(source_file);
        }
    }
}
}
