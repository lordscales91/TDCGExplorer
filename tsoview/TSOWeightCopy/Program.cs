using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOWeightCopy
{
    class Program
    {
        public static string GetFlipNodesPath()
        {
            return Path.Combine(Application.StartupPath, @"flipnodes.txt");
        }

        static void Main(string[] args)
        {
            TSOFile tso = new TSOFile();
            tso.Load(@"base/data/model/N001BODY_A00.tso");

            Dictionary<string, TSONode> nodemap = new Dictionary<string, TSONode>();

            foreach (TSONode node in tso.nodes)
                nodemap.Add(node.Name, node);

            Dictionary<int, int> oppnode_idmap = new Dictionary<int, int>();

            char[] delim = { ' ' };
            using (StreamReader source = new StreamReader(File.OpenRead(GetFlipNodesPath())))
            {
                string line;
                while ((line = source.ReadLine()) != null)
                {
                    string[] tokens = line.Split(delim);
                    string op = tokens[0];
                    if (op == "flip")
                    {
                        Debug.Assert(tokens.Length == 2, "tokens length should be 2: " + line);
                        string cnode_name = tokens[1];
                        int cnode_id = nodemap[cnode_name].ID;

                        oppnode_idmap[cnode_id] = cnode_id;
                    }
                    else
                    if (op == "swap")
                    {
                        Debug.Assert(tokens.Length == 3, "tokens length should be 3: " + line);
                        string lnode_name = tokens[1];
                        string rnode_name = tokens[2];
                        int lnode_id = nodemap[lnode_name].ID;
                        int rnode_id = nodemap[rnode_name].ID;

                        oppnode_idmap[lnode_id] = rnode_id;
                        oppnode_idmap[rnode_id] = lnode_id;
                    }
                }
            }

            UniqueVertex.oppnode_idmap = oppnode_idmap;

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

            tso.Save(@"out.tso");
        }
    }
}
