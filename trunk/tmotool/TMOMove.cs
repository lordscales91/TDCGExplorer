using System;
using System.IO;

namespace TDCG
{
    class TMOMove
    {
        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TMOMove <tmo file> [x y z] [node name]");
                return;
            }

            string source_file = args[0];
            if (args.Length > 3)
            {
                float x = 0.0f;
                float y = 0.0f;
                float z = 0.0f;
                try
                {
                    x = Single.Parse(args[1]);
                    y = Single.Parse(args[2]);
                    z = Single.Parse(args[3]);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                string node_name = "W_Hips";
                if (args.Length > 4)
                    node_name = args[4];

                Move.UpdateTmo(source_file, x, y, z, node_name);
            }
            else
            {
                Move.DisplayTranslation(source_file);
            }
        }
    }
}
