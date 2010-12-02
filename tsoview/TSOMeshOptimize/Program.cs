using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOMeshOptimize
{
    class Heap<T>
    {
        public Dictionary<T, ushort> map = new Dictionary<T, ushort>();
        public List<T> ary = new List<T>();

        public void Clear()
        {
            map.Clear();
            ary.Clear();
        }
        public bool ContainsKey(T item)
        {
            return map.ContainsKey(item);
        }
        public int Count
        {
            get { return ary.Count; }
        }
        public void Add(T item)
        {
            map[item] = (ushort)ary.Count;
            ary.Add(item);
        }
        public ushort this[T item]
        {
            get { return map[item]; }
        }
    }

    class UnifiedPositionTexcoordVertex : Vertex, IComparable
    {
        public int CompareTo(object obj)
        {
            UnifiedPositionTexcoordVertex v = obj as UnifiedPositionTexcoordVertex;
            if ((object)v == null)
                throw new ArgumentException("not a UnifiedPositionTexcoordVertex");
            int cmp = this.position.X.CompareTo(v.position.X);
            if (cmp == 0)
                cmp = this.position.Y.CompareTo(v.position.Y);
            if (cmp == 0)
                cmp = this.position.Z.CompareTo(v.position.Z);
            if (cmp == 0)
                cmp = this.u.CompareTo(v.u);
            if (cmp == 0)
                cmp = this.v.CompareTo(v.v);
            return cmp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UnifiedPositionTexcoordVertex v = obj as UnifiedPositionTexcoordVertex;
            if ((object)v == null)
                return false;
            //return this.position == v.position && this.u == v.u && this.v == v.v;
            return position.X == v.position.X && position.Y == v.position.Y && position.Z == v.position.Z && this.u == v.u && this.v == v.v;
        }

        public  bool Equals(UnifiedPositionTexcoordVertex v)
        {
            if ((object)v == null)
                return false;
            //return this.position == v.position && this.u == v.u && this.v == v.v;
            return position.X == v.position.X && position.Y == v.position.Y && position.Z == v.position.Z && this.u == v.u && this.v == v.v;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
        }
    }

    class UnifiedPositionSpecVertex : IComparable
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 法線
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// テクスチャU座標
        /// </summary>
        public Single u;
        /// <summary>
        /// テクスチャV座標
        /// </summary>
        public Single v;
        /// <summary>
        /// スキンウェイト配列
        /// </summary>
        public SkinWeight[] skin_weights;
        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public int spec;

        public UnifiedPositionSpecVertex(Vertex a, TSOSubMesh sub)
        {
            this.position = a.position;
            this.normal = a.normal;
            this.u = a.u;
            this.v = a.v;
            this.skin_weights = new SkinWeight[4];
            for (int i = 0; i < 4; i++)
            {
                skin_weights[i] = new SkinWeight(sub.bone_indices[a.skin_weights[i].bone_index], a.skin_weights[i].weight);
            }
            this.spec = sub.spec;
        }

        public int CompareTo(object obj)
        {
            UnifiedPositionSpecVertex v = obj as UnifiedPositionSpecVertex;
            if ((object)v == null)
                throw new ArgumentException("not a UnifiedPositionSpecVertex");
            int cmp = this.position.X.CompareTo(v.position.X);
            if (cmp == 0)
                cmp = this.position.Y.CompareTo(v.position.Y);
            if (cmp == 0)
                cmp = this.position.Z.CompareTo(v.position.Z);
            if (cmp == 0)
                cmp = this.spec.CompareTo(v.spec);
            return cmp;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            UnifiedPositionSpecVertex v = obj as UnifiedPositionSpecVertex;
            if ((object)v == null)
                return false;
            //return this.position == v.position && this.spec == v.spec;
            return position.X == v.position.X && position.Y == v.position.Y && position.Z == v.position.Z && spec == v.spec;
        }

        public bool Equals(UnifiedPositionSpecVertex v)
        {
            if ((object)v == null)
                return false;
            //return this.position == v.position && this.spec == v.spec;
            return position.X == v.position.X && position.Y == v.position.Y && position.Z == v.position.Z && spec == v.spec;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ spec.GetHashCode();
            //return position.X.GetHashCode() ^ position.Y.GetHashCode() ^ position.Z.GetHashCode() ^ spec.GetHashCode();
        }
    }

    class TSOFace
    {
        public readonly UnifiedPositionSpecVertex a;
        public readonly UnifiedPositionSpecVertex b;
        public readonly UnifiedPositionSpecVertex c;
        public readonly int spec;
        public readonly UnifiedPositionSpecVertex[] vertices;

        public TSOFace(UnifiedPositionSpecVertex a, UnifiedPositionSpecVertex b, UnifiedPositionSpecVertex c, int spec)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.spec = spec;
            vertices = new UnifiedPositionSpecVertex[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }
    }

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
            try
            {
                mesh_idx = int.Parse(Console.ReadLine());
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e);
                return;
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

            RebuildMesh(selected_mesh);

            tso.Save(@"out.tso");
        }

        public static List<TSOFace> BuildFaces(TSOMesh mesh)
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
                        a = vertices[i-2];
                        b = vertices[i-0];
                        c = vertices[i-1];
                    }
                    else
                    {
                        a = vertices[i-2];
                        b = vertices[i-1];
                        c = vertices[i-0];
                    }
                    if (!a.Equals(b) && !b.Equals(c) && !c.Equals(a))
                    {
                        faces.Add(new TSOFace(a, b, c, sub.spec));
                    }
                }
            }
            return faces;
        }

        public static UnifiedPositionTexcoordVertex BuildVertex(UnifiedPositionSpecVertex v, Dictionary<int, ushort> bone_idmap)
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

        public static float WeightEpsilon = float.Epsilon;
        public static int MaxPalettes = 16;

        public static TSOSubMesh[] BuildSubMeshes(List<TSOFace> faces)
        {
            List<TSOFace> faces_1 = faces;
            List<TSOFace> faces_2 = new List<TSOFace>();

            Heap<int> bh = new Heap<int>();
            Heap<UnifiedPositionTexcoordVertex> vh = new Heap<UnifiedPositionTexcoordVertex>();
            
            List<ushort> vert_indices = new List<ushort>();
            Dictionary<int, bool> bone_indices = new Dictionary<int, bool>();
            List<TSOSubMesh> sub_meshes = new List<TSOSubMesh>();

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
                            if (bh.Count == MaxPalettes)
                            {
                                valid = false;
                                break;
                            }
                            bone_indices[sw.bone_index] = true;
                            if (bh.Count + bone_indices.Count > MaxPalettes)
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
                        UnifiedPositionTexcoordVertex a = BuildVertex(v, bh.map);
                        if (!vh.ContainsKey(a))
                        {
                            vh.Add(a);
                        }
                        vert_indices.Add(vh[a]);
                    }
                }
                Console.WriteLine("#vert_indices:{0}", vert_indices.Count);
                ushort[] optimized_indices = NvTriStrip.Optimize(vert_indices.ToArray());
                Console.WriteLine("#optimized_indices:{0}", optimized_indices.Length);

                TSOSubMesh sub = new TSOSubMesh();
                sub.spec = spec;
                Console.WriteLine("#bone_indices:{0}", bh.Count);
                sub.bone_indices = bh.ary.ToArray();

                UnifiedPositionTexcoordVertex[] vertices = new UnifiedPositionTexcoordVertex[optimized_indices.Length];
                for (int i = 0; i < optimized_indices.Length; i++)
                {
                    vertices[i] = vh.ary[optimized_indices[i]];
                }
                sub.vertices = vertices;
                sub_meshes.Add(sub);

                List<TSOFace> faces_tmp = faces_1;
                faces_1 = faces_2;
                faces_2 = faces_tmp;
                faces_tmp.Clear();
            }
            return sub_meshes.ToArray();
        }

        public static TSOSubMesh[] BuildSubMeshes(TSOMesh mesh)
        {
            List<TSOFace> faces = BuildFaces(mesh);
            Console.WriteLine("#uniq faces:{0}", faces.Count);
            return BuildSubMeshes(faces);
        }

        public static void RebuildMesh(TSOMesh mesh)
        {
            mesh.sub_meshes = BuildSubMeshes(mesh);
        }
    }
}
