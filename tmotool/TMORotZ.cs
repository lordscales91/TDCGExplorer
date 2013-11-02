using System;
using System.IO;

namespace TDCG
{
    class TMORotZ
    {
        static void Main(string[] args) 
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: TMORotZ <tmo file> <angle> [node name]");
                return;
            }

            string source_file = args[0];
            float angle = 0.0f;
            try
            {
                angle = Single.Parse(args[1]);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string node_name = "W_Hips";
            if (args.Length > 2)
                node_name = args[2];

            RotZ.UpdateTmo(source_file, angle, node_name);
        }
    }
}
