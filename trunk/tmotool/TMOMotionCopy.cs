using System;
using System.IO;

namespace TDCG
{
    class TMOMotionCopy
    {
        static void Main(string[] args) 
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: TMOMotionCopy <source tmo> <motion tmo>");
                return;
            }

            string source_file = args[0];
            string motion_file = args[1];

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

            try
            {
                source.CopyMotionFrom(motion);

                Console.WriteLine("source nodes Length {0}", source.nodes.Length);
                Console.WriteLine("motion nodes Length {0}", motion.nodes.Length);
                Console.WriteLine("source frames Length {0}", source.frames.Length);
                Console.WriteLine("motion frames Length {0}", motion.frames.Length);

                Console.WriteLine("Save File: " + source_file);
                source.Save(source_file);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
