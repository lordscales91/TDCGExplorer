using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{
    public class Dom
    {
        private static float DegreeToRadian(float angle)
        {
           return (float)(Math.PI * angle / 180.0);
        }

        public static int UpdateTmo(string source_file) 
        {
            string dest_file = source_file + ".tmp";

            TMOFile tmo = new TMOFile();
            try
            {
                tmo.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            if (tmo.nodes[0].ShortName != "W_Hips") {
                Console.WriteLine("Passed: root node is not W_Hips");
                return 1;
            }

            Dictionary<string, TMONode> nodes = new Dictionary<string, TMONode>();

            foreach(TMONode node in tmo.nodes)
            try {
                nodes.Add(node.ShortName, node);
            } catch (ArgumentException) {
                Console.WriteLine("node {0} already exists.", node.ShortName);
            }

            try {
                TMONode node;

                node = nodes["W_Hips"];
                node.Scale1(1.3F, 1.0F, 1.4F);

                node = nodes["W_LeftUpLeg"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_LeftUpLegRoll"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_LeftLeg"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_RightUpLeg"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_RightUpLegRoll"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_RightLeg"];
                node.Scale1(1.3F, 1.0F, 1.2F);

                node = nodes["W_Spine_Dummy"];
                node.Scale1(1.3F, 1.0F, 1.3F);
            } catch (KeyNotFoundException) {
                Console.WriteLine("node not found.");
            }

            tmo.Save(dest_file);

            System.IO.File.Delete(source_file);
            System.IO.File.Move(dest_file, source_file);
            Console.WriteLine("Domed: " + source_file);

            return 0;
        }
    }
}
