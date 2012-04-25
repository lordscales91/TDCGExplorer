using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Tso2MqoGui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            if (args.Length != 1)
            {
                Console.WriteLine("TetraHedron.exe <mqo file>");
                return;
            }
            string source_file = args[0];

            MqoFile mqo = new MqoFile();
            mqo.Load(source_file);
            //mqo.Dump();
 
            //TextWriter tw = Console.Out;
            string dest_path = Path.GetDirectoryName(source_file);
            string folder_name = Path.GetFileNameWithoutExtension(source_file);
            dest_path = Path.Combine(dest_path, folder_name + @".tetrahedron.mqo");

            Console.WriteLine("Save File: " + dest_path);
            TextWriter tw = new StreamWriter(File.Create(dest_path));

            tw.WriteLine("Metasequoia Document");
            tw.WriteLine("Format Text Ver 1.0");
            tw.WriteLine("");
            tw.WriteLine("Scene {");
            tw.WriteLine("	pos -7.0446 4.1793 1541.1764");
            tw.WriteLine("	lookat 11.8726 193.8590 0.4676");
            tw.WriteLine("	head 0.8564");
            tw.WriteLine("	pich 0.1708");
            tw.WriteLine("	ortho 0");
            tw.WriteLine("	zoom2 31.8925");
            tw.WriteLine("	amb 0.250 0.250 0.250");
            tw.WriteLine("}");

            foreach (MqoObject obj in mqo.Objects)
            {
                int nverts = obj.vertices.Count;
                int nfaces = obj.faces.Count;
                //Console.WriteLine("name:{0} vertices:{1} faces:{2}", obj.name, nverts, nfaces);

                //面を構成する頂点出現回数
                uint[] vcounts = new uint[nverts];
                for (int i = 0; i < nverts; i++)
                    vcounts[i] = 0;

                foreach (MqoFace face in obj.faces)
                {
                    vcounts[face.a]++;
                    vcounts[face.b]++;
                    vcounts[face.c]++;
                }

                //頂点が3面に含まれるとき真になる頂点配列
                bool[] vfounds = new bool[nverts];
                for (int i = 0; i < nverts; i++)
                    vfounds[i] = false;

                for (int i = 0; i < nverts; i++)
                {
                    if (vcounts[i] == 3)
                    {
                        vfounds[i] = true;
                    }
                }

                //前提: 3面に含まれる頂点が対象
                //頂点idと面リストを対応付ける辞書
                Dictionary<ushort, List<MqoFace>> facemap = new Dictionary<ushort, List<MqoFace>>();

                foreach (MqoFace face in obj.faces)
                {
                    if (vfounds[face.a])
                    {
                        if (!facemap.ContainsKey(face.a))
                            facemap[face.a] = new List<MqoFace>(3);
                        facemap[face.a].Add(face);
                    }
                    if (vfounds[face.b])
                    {
                        if (!facemap.ContainsKey(face.b))
                            facemap[face.b] = new List<MqoFace>(3);
                        facemap[face.b].Add(face);
                    }
                    if (vfounds[face.c])
                    {
                        if (!facemap.ContainsKey(face.c))
                            facemap[face.c] = new List<MqoFace>(3);
                        facemap[face.c].Add(face);
                    }
                }

                //4面体の中心頂点idリスト
                List<ushort> centary = new List<ushort>();

                Dictionary<ushort, ushort> vidxmap = new Dictionary<ushort, ushort>();
                List<ushort> vidxary = new List<ushort>();

                foreach (ushort i in facemap.Keys)
                {
                    List<MqoFace> faces = facemap[i];

                    //前提: 3面に含まれる頂点が対象
                    //頂点idと頂点出現回数を関連付ける辞書
                    Dictionary<ushort, int> vcs = new Dictionary<ushort, int>();

                    foreach (MqoFace face in faces)
                    {
                        if (!vcs.ContainsKey(face.a))
                            vcs[face.a] = 0;
                        vcs[face.a]++;
                        if (!vcs.ContainsKey(face.b))
                            vcs[face.b] = 0;
                        vcs[face.b]++;
                        if (!vcs.ContainsKey(face.c))
                            vcs[face.c] = 0;
                        vcs[face.c]++;
                    }

                    if (vcs.Count == 4)
                    {
                        //Console.WriteLine("v#{0} in tetrahedron.", i);

                        centary.Add(i);
                        foreach (MqoFace face in faces)
                        {
                            if (!vidxmap.ContainsKey(face.a))
                            {
                                vidxmap[face.a] = (ushort)vidxary.Count;
                                vidxary.Add(face.a);
                            }
                            if (!vidxmap.ContainsKey(face.b))
                            {
                                vidxmap[face.b] = (ushort)vidxary.Count;
                                vidxary.Add(face.b);
                            }
                            if (!vidxmap.ContainsKey(face.c))
                            {
                                vidxmap[face.c] = (ushort)vidxary.Count;
                                vidxary.Add(face.c);
                            }
                        }
                    }
                }

                if (vidxary.Count == 0)
                    continue;

                Console.WriteLine("name:{0} vertices:{1} faces:{2}", "tetrahedron-" + obj.name, vidxary.Count, centary.Count * 3);

                tw.WriteLine("Object \"{0}\" {{", "tetrahedron-" + obj.name);
                tw.WriteLine("	visible {0}", 15);
                tw.WriteLine("	locking {0}", 0);
                tw.WriteLine("	shading {0}", 1);
                tw.WriteLine("	facet {0}", 59.5);
                tw.WriteLine("	color {0:F3} {1:F3} {2:F3}", 0.898f, 0.498f, 0.698f);
                tw.WriteLine("	color_type {0}", 0);

                tw.WriteLine("	vertex {0} {{", vidxary.Count);

                foreach (ushort i in vidxary)
                {
                    Point3 po= obj.vertices[i];
                    WriteVert(tw, ref po);
                }

                tw.WriteLine("	}");
                tw.WriteLine("	face {0} {{", centary.Count * 3);

                foreach (ushort i in centary)
                {
                    List<MqoFace> faces = facemap[i];

                    foreach (MqoFace face in faces)
                    {
                        ushort a = vidxmap[face.a];
                        ushort b = vidxmap[face.b];
                        ushort c = vidxmap[face.c];
                        WriteFace(tw, a, b, c);
                    }
                }

                tw.WriteLine("	}");
                tw.WriteLine("}");
            }
            tw.Close();
        }

        static void WriteVert(TextWriter tw, ref Point3 po)
        {
            tw.WriteLine("  {0:F4} {1:F4} {2:F4}", po.X, po.Y, po.Z);
        }
        static void WriteFace(TextWriter tw, ushort a, ushort b, ushort c)
        {
            tw.WriteLine("  3 V({0} {1} {2})", a, b, c);
        }
    }
}
