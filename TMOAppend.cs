using System;
using System.IO;

namespace TAHdecrypt
{
    class TMOAppend
    {
        static void Main(string[] args) 
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine("Usage: TMOAppend <source tmo> <motion tmo>");
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
                source.AppendFrameFrom(motion);

                Console.WriteLine("source nodes {0}", source.nodes.Length);
                Console.WriteLine("motion nodes {0}", motion.nodes.Length);
                Console.WriteLine("source frames {0}", source.frames.Length);
                Console.WriteLine("motion frames {0}", motion.frames.Length);

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
