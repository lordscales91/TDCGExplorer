using System;
using System.IO;

namespace TAHdecrypt
{
    class TMOPose
    {
        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: TMOPose <tmo file> [frame]");
                return;
            }

            string source_file = args[0];
            int frame_index = 0;
            if (args.Length > 1)
            {
                try
                {
                    frame_index = Int32.Parse(args[1]);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }

            Console.WriteLine("Load File: " + source_file);
            TMOFile source = new TMOFile();
            try
            {
                source.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            source.TruncateFrame(frame_index);

            Console.WriteLine("Save File: " + source_file);
            source.Save(source_file);
        }
    }
}
