using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace tso2mqo
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("tso2mqo.exe <tso file>");
                return;
            }

            string source_file = args[0];

            if (Path.GetExtension(source_file).ToLower() == ".tso")
            {
                string dest_path = Path.GetFileNameWithoutExtension(source_file);
                Program program = new Program();
                program.Extract(source_file, dest_path);
            }
            else if (Directory.Exists(source_file))
            {
                Program program = new Program();
                program.Compose(source_file);
            }
        }

        public int Extract(string source_file, string dest_path)
        {
            Directory.CreateDirectory(dest_path);

            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            foreach (TSOTex tex in tso.textures)
            {
                string name = tex.Name;
                string file = tex.FileName.Trim('"');
                file = Path.GetFileNameWithoutExtension(file) + ".bmp";
                Console.WriteLine("tex name:{0} file:{1}", name, file);
                string dest_file = Path.Combine(dest_path, file);
                using (BinaryWriter bw = new BinaryWriter(File.Create(dest_file)))
                    tex.SaveBMP(bw);
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                string name = sub.Name;
                string file = sub.FileName;
                Console.WriteLine("sub name:{0} file:{1}", name, file);
                string dest_file = Path.Combine(dest_path, name);
                sub.Save(dest_file);
            }

            {
                string name = dest_path + ".mqo";
                string dest_file = Path.Combine(dest_path, name);
                Console.WriteLine("Save File: " + dest_file);
                using (TextWriter tw = new StreamWriter(File.Create(dest_file)))
                    SaveToMqo(tw, tso);
            }

            return 0;
        }

        public int Compose(string source_file)
        {
            string source_name = Path.GetFileNameWithoutExtension(source_file);
            string dest_file = Path.Combine(source_file, source_name + ".mqo");

            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            MqoFile mqo = new MqoFile();
            mqo.Load(dest_file);
            mqo.Dump();

            return 0;
        }

        public void SaveToMqo(TextWriter tw, TSOFile tso)
        {
            tw.WriteLine("Metasequoia Document");
            tw.WriteLine("Format Text Ver 1.0");
            tw.WriteLine("");
            tw.WriteLine("Scene {");
            tw.WriteLine("\tpos -7.0446 4.1793 1541.1764");
            tw.WriteLine("\tlookat 11.8726 193.8590 0.4676");
            tw.WriteLine("\thead 0.8564");
            tw.WriteLine("\tpich 0.1708");
            tw.WriteLine("\tortho 0");
            tw.WriteLine("\tzoom2 31.8925");
            tw.WriteLine("\tamb 0.250 0.250 0.250");
            tw.WriteLine("}");

            tw.WriteLine("Material {0} {{", tso.sub_scripts.Length);

            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();
            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                TSOTex tex = texmap[sub.shader.ColorTexName];
                string tex_file = tex.FileName.Trim('"');
                tex_file = Path.GetFileNameWithoutExtension(tex_file) + ".bmp";
                tw.WriteLine(
                    "\t\"{0}\" shader(3) col(1.00 1.00 1.00 1.00) dif(0.800) amb(0.600) emi(0.000) spc(0.000) power(5.00) tex(\"{1}\")",
                    sub.Name, tex_file);
            }

            tw.WriteLine("}");

            foreach (TSOMesh mesh in tso.meshes)
            {
                tw.WriteLine("Object \"{0}\" {{", mesh.Name);
                tw.WriteLine("\tvisible {0}", 15);
                tw.WriteLine("\tlocking {0}", 0);
                tw.WriteLine("\tshading {0}", 1);
                tw.WriteLine("\tfacet {0}", 59.5);
                tw.WriteLine("\tcolor {0} {1} {2}", 0.898f, 0.498f, 0.698f);
                tw.WriteLine("\tcolor_type {0}", 0);

                Heap<UnifiedPositionVertex> vh = new Heap<UnifiedPositionVertex>();
                List<MqoFace> faces = new List<MqoFace>();
                foreach (TSOSubMesh sub in mesh.sub_meshes)
                {
                    UnifiedPositionVertex[] vertices = new UnifiedPositionVertex[sub.vertices.Length];
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i] = new UnifiedPositionVertex(sub.vertices[i]);
                    }
                    for (int i = 2; i < vertices.Length; i++)
                    {
                        UnifiedPositionVertex va, vb, vc;
                        if (i % 2 == 0)
                        {
                            va = vertices[i - 2];
                            vb = vertices[i - 0];
                            vc = vertices[i - 1];
                        }
                        else
                        {
                            va = vertices[i - 2];
                            vb = vertices[i - 1];
                            vc = vertices[i - 0];
                        }

                        ushort a, b, c;
                        a = vh.Add(va);
                        b = vh.Add(vb);
                        c = vh.Add(vc);

                        if (a != b && b != c && c != a)
                        {
                            MqoFace f = new MqoFace();
                            f.a = a;
                            f.b = b;
                            f.c = c;
                            f.ta = new Vector2(va.u, 1-va.v);
                            f.tb = new Vector2(vb.u, 1-vb.v);
                            f.tc = new Vector2(vc.u, 1-vc.v);
                            f.mtl = (ushort)sub.spec;
                            faces.Add(f);
                        }
                    }
                }
                tw.WriteLine("\tvertex {0} {{", vh.Count);
                foreach (UnifiedPositionVertex v in vh.ary)
                {
                    tw.WriteLine("\t\t{0:F6} {1:F6} {2:F6}", v.position.X, v.position.Y, v.position.Z);
                }
                tw.WriteLine("\t}");
                tw.WriteLine("\tface {0} {{", faces.Count);
                foreach (MqoFace f in faces)
                {
                    tw.WriteLine("\t\t{0} V({1} {2} {3}) M({10}) UV({4} {5} {6} {7} {8} {9})", 3, f.a, f.b, f.c, f.ta.X, f.ta.Y, f.tb.X, f.tb.Y, f.tc.X, f.tc.Y, f.mtl);
                }
                tw.WriteLine("\t}");
                tw.WriteLine("}");
            }
        }
    }
}
