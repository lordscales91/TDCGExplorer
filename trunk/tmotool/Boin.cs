using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{
    public class Boin
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

                node = nodes["Chichi_Right1"];
                node.RotateY(DegreeToRadian(-5));
                node.RotateX(DegreeToRadian(5));

                node = nodes["Chichi_Left1"];
                node.RotateY(DegreeToRadian(10));
                node.RotateX(DegreeToRadian(10));

                node = nodes["Chichi_Right2"];
                node.Scale(1.8F, 1.8F, 2.2F);

                node = nodes["Chichi_Left2"];
                node.Scale(1.8F, 1.8F, 2.2F);

                node = nodes["W_Spine_Dummy"];
                node.Scale1(1.1F, 1.0F, 1.1F);

                node = nodes["W_Spine1"];
                node.Scale1(1.2F, 1.0F, 1.2F);

                node = nodes["W_Spine2"];
                node.Scale1(1.3F, 1.0F, 1.3F);

                node = nodes["W_Spine3"];
                node.Scale1(1.1F, 1.0F, 1.1F);
            } catch (KeyNotFoundException) {
                Console.WriteLine("node not found.");
            }

            tmo.Save(dest_file);

            System.IO.File.Delete(source_file);
            System.IO.File.Move(dest_file, source_file);
            Console.WriteLine("Boined: " + source_file);

            return 0;
        }
    }
}
