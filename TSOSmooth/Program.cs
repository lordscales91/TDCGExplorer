using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOSmooth
{
    class Program
    {
        /// <summary>
        /// 指定位置間の距離の平方を返します。
        /// </summary>
        /// <param name="a">位置a</param>
        /// <param name="b">位置b</param>
        /// <returns></returns>
        public static float LengthSq(Vector3 a, Vector3 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float dz = b.Z - a.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        public static string GetSetsRoot()
        {
            return Path.Combine(Application.StartupPath, @"sets");
        }

        public static List<string> GetSetsNames()
        {
            List<string> names = new List<string>();
            string[] files = Directory.GetFiles(GetSetsRoot(), "*.txt");
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                names.Add(name);
            }
            return names;
        }

        static string setsname;

        public static string GetSetsPath()
        {
            return Path.Combine(GetSetsRoot(), setsname + ".txt");
        }

        static List<TSONode> use_nodes = new List<TSONode>();

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TSOSmooth.exe <tso file>");
                return;
            }
            string source_file = args[0];

            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            Dictionary<string, TSONode> nodemap = new Dictionary<string, TSONode>();

            foreach (TSONode node in tso.nodes)
                nodemap.Add(node.Name, node);

            string[] nodesets = GetSetsNames().ToArray();

            Console.WriteLine("nodesets:");
            {
                int i = 0;
                foreach (string name in nodesets)
                {
                    Console.WriteLine("{0} {1}", i, name);
                    i++;
                }
            }
            Console.Write("nodesetsを選択 (0-{0}): ", nodesets.Length - 1);
            int sets_idx = 0;
            {
                string line = Console.ReadLine();
                if (line.Length != 0)
                    try
                    {
                        sets_idx = int.Parse(line);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
            }
            setsname = nodesets[sets_idx];

            char[] delim = { ' ' };
            using (StreamReader source = new StreamReader(File.OpenRead(GetSetsPath())))
            {
                string line;
                while ((line = source.ReadLine()) != null)
                {
                    string[] tokens = line.Split(delim);
                    string op = tokens[0];
                    if (op == "node")
                    {
                        Debug.Assert(tokens.Length == 2, "tokens length should be 2: " + line);
                        string cnode_name = tokens[1];
                        TSONode cnode = nodemap[cnode_name];

                        use_nodes.Add(cnode);
                    }
                }
            }

            Console.WriteLine("メッシュ:");
            {
                int i = 0;
                foreach (TSOMesh mesh in tso.meshes)
                {
                    Console.WriteLine("{0} {1}", i, mesh.Name);
                    i++;
                }
            }

            Console.Write("メッシュを選択 (0-{0}): ", tso.meshes.Length - 1);
            int mesh_idx = 0;
            {
                string line = Console.ReadLine();
                if (line.Length != 0)
                    try
                    {
                        mesh_idx = int.Parse(line);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
            }

            TSOMesh selected_mesh = null;
            try
            {
                selected_mesh = tso.meshes[mesh_idx];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
                return;
            }

            Console.WriteLine("サブメッシュ:");
            Console.WriteLine("  vertices bone_indices");
            Console.WriteLine("  -------- ------------");
            foreach (TSOSubMesh sub in selected_mesh.sub_meshes)
            {
                Console.WriteLine("  {0,8} {1,12}", sub.vertices.Length, sub.bone_indices.Length);
            }

            int max_palettes = 16;

            RebuildMesh(selected_mesh, max_palettes);

            string dest_path = Path.GetDirectoryName(source_file);
            string dest_file = Path.GetFileNameWithoutExtension(source_file) + @".new.tso";
            dest_path = Path.Combine(dest_path, dest_file);
            Console.WriteLine("Save File: " + dest_path);
            tso.Save(dest_path);
        }

        static Heap<UnifiedPositionVertex> unified_position_vert_heap;

        /// <summary>
        /// 面リストを生成します。
        /// </summary>
        public static List<TSOFace> CreateFaces(TSOMesh mesh)
        {
            unified_position_vert_heap = new Heap<UnifiedPositionVertex>();
            List<TSOFace> faces = new List<TSOFace>();

            foreach (TSOSubMesh sub in mesh.sub_meshes)
            {
                ushort[] vert_indices = new ushort[sub.vertices.Length];
                for (int i = 0; i < vert_indices.Length; i++)
                {
                    UnifiedPositionVertex v = new UnifiedPositionVertex(sub.vertices[i], sub.bone_indices, sub.spec);
                    if (!unified_position_vert_heap.ContainsKey(v))
                        unified_position_vert_heap.Add(v);
                    vert_indices[i] = unified_position_vert_heap[v];
                }
                for (int i = 2; i < vert_indices.Length; i++)
                {
                    FaceVertex a, b, c;
                    if (i % 2 != 0)
                    {
                        a.i = vert_indices[i - 2];
                        b.i = vert_indices[i - 0];
                        c.i = vert_indices[i - 1];
                    }
                    else
                    {
                        a.i = vert_indices[i - 2];
                        b.i = vert_indices[i - 1];
                        c.i = vert_indices[i - 0];
                    }
                    if (a.i != b.i && b.i != c.i && c.i != a.i)
                    {
                        if (i % 2 != 0)
                        {
                            a.u = sub.vertices[i - 2].u;
                            a.v = sub.vertices[i - 2].v;
                            b.u = sub.vertices[i - 0].u;
                            b.v = sub.vertices[i - 0].v;
                            c.u = sub.vertices[i - 1].u;
                            c.v = sub.vertices[i - 1].v;
                        }
                        else
                        {
                            a.u = sub.vertices[i - 2].u;
                            a.v = sub.vertices[i - 2].v;
                            b.u = sub.vertices[i - 1].u;
                            b.v = sub.vertices[i - 1].v;
                            c.u = sub.vertices[i - 0].u;
                            c.v = sub.vertices[i - 0].v;
                        }
                        faces.Add(new TSOFace(a, b, c, sub.spec));
                    }
                }
            }
            return faces;
        }

        public static float WeightEpsilon = float.Epsilon;

        public static TSOSubMesh[] CreateSubMeshes(List<TSOFace> faces, int max_palettes)
        {
            List<TSOFace> faces_1 = faces;
            List<TSOFace> faces_2 = new List<TSOFace>();

            Heap<int> bh = new Heap<int>();
            Heap<UnifiedPositionTexcoordVertex> vh = new Heap<UnifiedPositionTexcoordVertex>();

            List<ushort> vert_indices = new List<ushort>();
            Dictionary<int, bool> adding_bone_indices = new Dictionary<int, bool>();
            List<TSOSubMesh> sub_meshes = new List<TSOSubMesh>();

            Console.WriteLine("  vertices bone_indices");
            Console.WriteLine("  -------- ------------");

            while (faces_1.Count != 0)
            {
                int spec = faces_1[0].spec;
                bh.Clear();
                vh.Clear();
                vert_indices.Clear();

                foreach (TSOFace f in faces_1)
                {
                    if (f.spec != spec)
                    {
                        faces_2.Add(f);
                        continue;
                    }
                    adding_bone_indices.Clear();

                    foreach (ushort i in f.vert_indices)
                    {
                        UnifiedPositionVertex v = unified_position_vert_heap.ary[i];
                        foreach (SkinWeight sw in v.skin_weights)
                        {
                            if (sw.weight < WeightEpsilon)
                                continue;
                            if (bh.ContainsKey(sw.bone_index))
                                continue;
                            adding_bone_indices[sw.bone_index] = true;
                        }
                    }

                    if (bh.Count + adding_bone_indices.Count > max_palettes)
                    {
                        faces_2.Add(f);
                        continue;
                    }
                    foreach (int bone_index in adding_bone_indices.Keys)
                    {
                        bh.Add(bone_index);
                    }

                    foreach (FaceVertex fv in f.vertices)
                    {
                        UnifiedPositionVertex v = unified_position_vert_heap.ary[fv.i];
                        UnifiedPositionTexcoordVertex a = new UnifiedPositionTexcoordVertex(v, fv.u, fv.v, bh.map);
                        if (!vh.ContainsKey(a))
                            vh.Add(a);
                        vert_indices.Add(vh[a]);
                    }
                }
                //Console.WriteLine("#vert_indices:{0}", vert_indices.Count);
                ushort[] optimized_indices = NvTriStrip.Optimize(vert_indices.ToArray());
                //Console.WriteLine("#optimized_indices:{0}", optimized_indices.Length);

                TSOSubMesh sub = new TSOSubMesh();
                sub.spec = spec;
                //Console.WriteLine("#bone_indices:{0}", bh.Count);
                sub.bone_indices = bh.ary.ToArray();

                UnifiedPositionTexcoordVertex[] vertices = new UnifiedPositionTexcoordVertex[optimized_indices.Length];
                for (int i = 0; i < optimized_indices.Length; i++)
                {
                    vertices[i] = vh.ary[optimized_indices[i]];
                }
                sub.vertices = vertices;

                Console.WriteLine("  {0,8} {1,12}", sub.vertices.Length, sub.bone_indices.Length);

                sub_meshes.Add(sub);

                List<TSOFace> faces_tmp = faces_1;
                faces_1 = faces_2;
                faces_2 = faces_tmp;
                faces_tmp.Clear();
            }
            return sub_meshes.ToArray();
        }

        public static TSOSubMesh[] CreateSubMeshes(TSOMesh mesh, int max_palettes)
        {
            List<TSOFace> faces = CreateFaces(mesh);
            //Console.WriteLine("#uniq faces:{0}", faces.Count);

            Smooth();

            TSOSubMesh[] sub_meshes = CreateSubMeshes(faces, max_palettes);
            //Console.WriteLine("#subs:{0}", sub_meshes.Length);
            return sub_meshes;
        }

        static void Smooth()
        {
            int len = use_nodes.Count;

            TSONode[] nodes = new TSONode[len];
            Vector3[] node_world_positions = new Vector3[len];

            for (int i = 0; i < len; i++)
            {
                node_world_positions[i] = use_nodes[i].GetWorldPosition();
            }

            float[] distances = new float[len];
            float[] inv_distances = new float[len];

            foreach (UnifiedPositionVertex v in unified_position_vert_heap.ary)
            {
                for (int i = 0; i < len; i++)
                {
                    nodes[i] = use_nodes[i];
                }

                //頂点vに対して距離が近い順に並べたnodesを得る。
                for (int i = 0; i < len; i++)
                {
                    distances[i] = LengthSq(v.position, node_world_positions[i]);
                }
                Array.Sort(distances, nodes);

                for (int i = 0; i < 4; i++)
                {
                    inv_distances[i] += 1.0f / distances[i];
                }

                //平方距離の逆数の合計
                float sum_inv_distances = 0.0f;

                for (int i = 0; i < 4; i++)
                {
                    sum_inv_distances += inv_distances[i];
                }

                //平方距離の逆数の割合をweightとする。
                for (int i = 0; i < 4; i++)
                {
                    v.skin_weights[i].bone_index = nodes[i].ID;
                    v.skin_weights[i].weight = inv_distances[i] / sum_inv_distances;
                }
            }
        }

        public static void RebuildMesh(TSOMesh mesh, int max_palettes)
        {
            mesh.sub_meshes = CreateSubMeshes(mesh, max_palettes);
        }
    }
}
