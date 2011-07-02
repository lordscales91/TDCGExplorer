using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOMeshOptimize
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TSOMeshOptimize.exe <tso file>");
                return;
            }
            string source_file = args[0];

            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            Console.WriteLine("メッシュ:");
            int i = 0;
            foreach (TSOMesh mesh in tso.meshes)
            {
                Console.WriteLine("{0} {1}", i, mesh.Name);
                i++;
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

            Console.Write("最大パレット長: ");
            int max_palettes = 16;
            {
                string line = Console.ReadLine();
                if (line.Length != 0)
                    try
                    {
                        max_palettes = int.Parse(line);
                    }
                    catch (System.FormatException e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
            }

            RebuildMesh(selected_mesh, max_palettes);

            string dest_path = Path.GetDirectoryName(source_file);
            string dest_file = Path.GetFileNameWithoutExtension(source_file) + @".new.tso";
            dest_path = Path.Combine(dest_path, dest_file);
            Console.WriteLine("Save File: " + dest_path);
            tso.Save(dest_path);
        }

        public static UnifiedPositionTexcoordVertex CreateVertex(UnifiedPositionSpecVertex v, Dictionary<int, ushort> bone_idmap)
        {
            UnifiedPositionTexcoordVertex a = new UnifiedPositionTexcoordVertex();
            a.position = v.position;
            a.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                if (v.skin_weights[i].weight < WeightEpsilon)
                    a.skin_weights[i] = new SkinWeight(0, 0.0f);
                else
                    a.skin_weights[i] = new SkinWeight(bone_idmap[v.skin_weights[i].bone_index], v.skin_weights[i].weight);
            }
            a.GenerateBoneIndices();
            a.normal = v.normal;
            a.u = v.u;
            a.v = v.v;
            return a;
        }

        /// <summary>
        /// 面リストを生成します。
        /// </summary>
        /// <returns>面リスト</returns>
        public static List<TSOFace> CreateFaces(TSOMesh mesh)
        {
            List<TSOFace> faces = new List<TSOFace>();
            foreach (TSOSubMesh sub in mesh.sub_meshes)
            {
                UnifiedPositionSpecVertex[] vertices = new UnifiedPositionSpecVertex[sub.vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = new UnifiedPositionSpecVertex(sub.vertices[i], sub);
                }
                for (int i = 2; i < vertices.Length; i++)
                {
                    UnifiedPositionSpecVertex a, b, c;
                    if (i % 2 != 0)
                    {
                        a = vertices[i - 2];
                        b = vertices[i - 0];
                        c = vertices[i - 1];
                    }
                    else
                    {
                        a = vertices[i - 2];
                        b = vertices[i - 1];
                        c = vertices[i - 0];
                    }
                    if (!a.Equals(b) && !b.Equals(c) && !c.Equals(a))
                    {
                        faces.Add(new TSOFace(a, b, c));
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
            Dictionary<int, bool> bone_indices = new Dictionary<int, bool>();
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
                    bool valid = true;
                    bone_indices.Clear();
                    foreach (UnifiedPositionSpecVertex v in f.vertices)
                    {
                        foreach (SkinWeight sw in v.skin_weights)
                        {
                            if (sw.weight < WeightEpsilon)
                                continue;
                            if (bh.ContainsKey(sw.bone_index))
                                continue;
                            if (bh.Count == max_palettes)
                            {
                                valid = false;
                                break;
                            }
                            bone_indices[sw.bone_index] = true;
                            if (bh.Count + bone_indices.Count > max_palettes)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                    if (!valid)
                    {
                        faces_2.Add(f);
                        continue;
                    }
                    foreach (int bone_index in bone_indices.Keys)
                    {
                        bh.Add(bone_index);
                    }
                    foreach (UnifiedPositionSpecVertex v in f.vertices)
                    {
                        UnifiedPositionTexcoordVertex a = CreateVertex(v, bh.map);
                        if (!vh.ContainsKey(a))
                        {
                            vh.Add(a);
                        }
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
            TSOSubMesh[] sub_meshes = CreateSubMeshes(faces, max_palettes);
            //Console.WriteLine("#subs:{0}", sub_meshes.Length);
            return sub_meshes;
        }

        public static void RebuildMesh(TSOMesh mesh, int max_palettes)
        {
            mesh.sub_meshes = CreateSubMeshes(mesh, max_palettes);
        }
    }
}
