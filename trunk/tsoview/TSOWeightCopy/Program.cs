using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOWeightCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            TSOFile tso = new TSOFile();
            tso.Load(@"base/data/model/N001BODY_A00.tso");

            TSOMesh found_mesh = null;
            foreach (TSOMesh mesh in tso.meshes)
            {
                if (mesh.Name == "W_BODY_Nurin_M01")
                {
                    found_mesh = mesh;
                    break;
                }
            }

            Vector3 min = Vector3.Empty;
            Vector3 max = Vector3.Empty;
            int nvertices = 0;

            foreach (TSOSubMesh sub in found_mesh.sub_meshes)
            {
                foreach (Vertex v in sub.vertices)
                {
                    float x = v.position.X;
                    float y = v.position.Y;
                    float z = v.position.Z;

                    if (min.X > x) min.X = x;
                    if (min.Y > y) min.Y = y;
                    if (min.Z > z) min.Z = z;

                    if (max.X < x) max.X = x;
                    if (max.Y < y) max.Y = y;
                    if (max.Z < z) max.Z = z;

                    nvertices++;
                }
            }
            Console.WriteLine("#vertices:{0}", nvertices);
            Console.WriteLine("min:{0}", UniqueVertex.ToString(min));
            Console.WriteLine("max:{0}", UniqueVertex.ToString(max));

            Cluster cluster = new Cluster(min, max);
            foreach (TSOSubMesh sub in found_mesh.sub_meshes)
            {
                foreach (Vertex v in sub.vertices)
                {
                    cluster.Push(v, sub);
                }
            }

            Console.WriteLine("#uniq vertices:{0}", cluster.vertices.Count);
            Console.WriteLine();
            cluster.AssignOppositeCells();
            cluster.AssignOppositeVertices();
            //cluster.Dump();
            cluster.CopyOppositeWeights();
        }
    }
}
