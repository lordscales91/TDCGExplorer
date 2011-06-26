using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace tso2pmx
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: tso2pmx <source tso>");
                return;
            }

            string source_file = args[0];

            Program program = new Program();
            program.Process(source_file);
        }

        public bool Process(string source_file)
        {
            TSOFile tso;
            tso = new TSOFile();
            tso.Load(source_file);

            PmxFile pmx = new PmxFile();

            pmx.vertices = new PmxVertex[4];

            pmx.vertices[0] = new PmxVertex();
            pmx.vertices[0].position = new Vector3(0, 0, 0);
            pmx.vertices[0].u = 0.0f;
            pmx.vertices[0].v = 0.0f;

            pmx.vertices[1] = new PmxVertex();
            pmx.vertices[1].position = new Vector3(0, 5, 0);
            pmx.vertices[1].u = 0.0f;
            pmx.vertices[1].v = 1.0f;

            pmx.vertices[2] = new PmxVertex();
            pmx.vertices[2].position = new Vector3(5, 0, 0);
            pmx.vertices[2].u = 1.0f;
            pmx.vertices[2].v = 0.0f;

            pmx.vertices[3] = new PmxVertex();
            pmx.vertices[3].position = new Vector3(5, 5, 0);
            pmx.vertices[3].u = 1.0f;
            pmx.vertices[3].v = 1.0f;

            pmx.vindices = new int[6];

            pmx.vindices[0] = 0;
            pmx.vindices[1] = 1;
            pmx.vindices[2] = 2;

            pmx.vindices[3] = 1;
            pmx.vindices[4] = 3;
            pmx.vindices[5] = 2;

            pmx.materials = new PmxMaterial[1];

            pmx.materials[0] = new PmxMaterial();
            pmx.materials[0].tex_id = 0;
            pmx.materials[0].vertices_count = 6;

            string dest_file = "out.pmx";
            Console.WriteLine("Save File: " + dest_file);
            pmx.Save(dest_file);

            return true;
        }
    }
}
