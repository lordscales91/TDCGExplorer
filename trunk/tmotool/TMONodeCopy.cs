using System;
using System.IO;

namespace TDCG
{
    class TMONodeCopy
    {
        static void Main(string[] args) 
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: TMONodeCopy <source tmo> <motion tmo> [node name]");
                return;
            }

            string source_file = args[0];
            string motion_file = args[1];
            string node_name = "W_Hips";

            if (args.Length > 2)
                node_name = args[2];

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

            Console.WriteLine("Load File: " + motion_file);
            TMOFile motion = new TMOFile();
            try
            {
                motion.Load(motion_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            source.CopyNodeFrom(motion, node_name);

            Console.WriteLine("Save File: " + source_file);
            source.Save(source_file);
        }
    }
}
