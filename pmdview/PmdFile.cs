using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace pmdview
{
    /// <summary>
    /// BinaryReaderの拡張メソッドを定義します。
    /// </summary>
    public static class BinaryReaderMethods
    {
        static Encoding enc = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// null終端文字列を読みとります。
        /// </summary>
        /// <returns>文字列</returns>
        public static string ReadCString(this BinaryReader reader, int length)
        {
            byte[] buf = reader.ReadBytes(length);
            int len = Array.IndexOf(buf, (byte)0);
            return enc.GetString(buf, 0, len);
        }

        /// <summary>
        /// Vector3を読みとります。
        /// </summary>
        /// <param name="reader">BinaryReader</param>
        /// <param name="v">Vector3</param>
        public static void ReadVector3(this BinaryReader reader, ref Vector3 v)
        {
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
        }
    }

    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public float u;
        public float v;
        public ushort node_id_0;
        public ushort node_id_1;
        public byte weight;
        public byte edge;

        public void Read(BinaryReader reader)
        {
            reader.ReadVector3(ref this.position);
            reader.ReadVector3(ref this.normal);
            this.u = reader.ReadSingle();
            this.v = reader.ReadSingle();
            this.node_id_0 = reader.ReadUInt16();
            this.node_id_1 = reader.ReadUInt16();
            this.weight = reader.ReadByte();
            this.edge = reader.ReadByte();
        }
    }

    public class PmdFace
    {
        public ushort a;
        public ushort b;
        public ushort c;

        public void Read(BinaryReader reader)
        {
            this.a = reader.ReadUInt16();
            this.b = reader.ReadUInt16();
            this.c = reader.ReadUInt16();
        }
    }

    public class PmdMaterial
    {
        public int id;
        public Vector3 diffuse;
        public float alpha;
        public float specularity;
        public Vector3 specular;
        public Vector3 ambient;
        public byte toon_id;
        public byte edge;
        public int face_vertex_count;
        public string texture_file;

        static int whole_face_start = 0;
        static int whole_face_vertex_start = 0;

        public int FaceStart;
        public int FaceCount
        {
            get { return face_vertex_count / 3; }
        }
        public int FaceVertexStart;

        public Vector4 Diffuse
        {
            get { return new Vector4(diffuse.X, diffuse.Y, diffuse.Z, 1); }
        }
        public Vector4 Ambient
        {
            get { return new Vector4(ambient.X, ambient.Y, ambient.Z, 1); }
        }
        public Vector4 Emissive
        {
            get { return new Vector4(0, 0, 0, 0); }
        }
        public Vector4 Specular
        {
            get { return new Vector4(specular.X, specular.Y, specular.Z, 1); }
        }
        public float SpecularPower
        {
            get { return specularity;  }
        }
        public Vector4 MaterialToon
        {
            get { return new Vector4(1, 1, 1, 1); }
        }

        public bool use_texture
        {
            get { return ! string.IsNullOrEmpty(texture_file); }
        }
        public bool use_toon = false;

        public PmdMaterial(int id)
        {
            this.id = id;
        }

        public void Read(BinaryReader reader)
        {
            FaceStart = whole_face_start;
            FaceVertexStart = whole_face_vertex_start;

            reader.ReadVector3(ref this.diffuse);
            this.alpha = reader.ReadSingle();
            this.specularity = reader.ReadSingle();
            reader.ReadVector3(ref this.specular);
            reader.ReadVector3(ref this.ambient);
            this.toon_id = reader.ReadByte();
            this.edge = reader.ReadByte();
            this.face_vertex_count = reader.ReadInt32();
            this.texture_file = reader.ReadCString(20);

            //Console.WriteLine("diffuse:{0}", diffuse);
            //Console.WriteLine("specular:{0}", specular);
            //Console.WriteLine("ambient:{0}", ambient);
            //Console.WriteLine("texture file:{0}", texture_file);

            whole_face_start += FaceCount;
            whole_face_vertex_start += face_vertex_count;
        }
    }

    public class PmdNode
    {
        public ushort id;
        public string name;
        public ushort parent_node_id;
        public ushort tail_node_id;
        public byte type;
        public ushort ik_parent_node_id;
        public Vector3 position;

        /// <summary>
        /// PmdNodeを生成します。
        /// </summary>
        public PmdNode(ushort id)
        {
            this.id = id;
        }

        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString(20);
            this.parent_node_id = reader.ReadUInt16();
            this.tail_node_id = reader.ReadUInt16();
            this.type = reader.ReadByte();
            this.ik_parent_node_id = reader.ReadUInt16();
            reader.ReadVector3(ref this.position);
        }
    }

    /// <summary>
    /// pmdファイルを扱います。
    /// </summary>
    public class PmdFile : IDisposable
    {
        public Vertex[] vertices;
        public ushort[] indices;
        //public PmdFace[] faces;
        public PmdMaterial[] materials;
        /// <summary>
        /// bone配列
        /// </summary>
        public PmdNode[] nodes;

        public VertexBuffer vb = null;
        public IndexBuffer ib = null;

        /// <summary>
        /// 頂点をDirect3Dバッファに書き込みます。
        /// </summary>
        /// <param name="device">device</param>
        public void WriteVertexBuffer(Device device)
        {
            if (vb != null)
                vb.Dispose();
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), vertices.Length, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            //
            // rewrite vertex buffer
            //
            {
                GraphicsStream gs = vb.Lock(0, 0, LockFlags.None);
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vertex v = vertices[i];

                        gs.Write(v.position);
                        //for (int j = 0; j < 4; j++)
                        //    gs.Write(0.0f);
                        //gs.Write(0);
                        gs.Write(v.normal);
                        gs.Write(v.u);
                        gs.Write(v.v);
                    }
                }
                vb.Unlock();
            }
            Console.WriteLine("rewrite vertex buffer");
        }

        /// <summary>
        /// 頂点をDirect3Dバッファに書き込みます。
        /// </summary>
        /// <param name="device">device</param>
        public void WriteIndexBuffer(Device device)
        {
            if (ib != null)
                ib.Dispose();
            ib = new IndexBuffer(typeof(ushort), indices.Length, device, Usage.WriteOnly, Pool.Default);

            //
            // rewrite index buffer
            //
            {
                GraphicsStream gs = ib.Lock(0, 0, LockFlags.None);
                {
                    foreach (ushort idx in indices)
                    {
                        gs.Write(idx);
                    }
                }
                ib.Unlock();
            }
            Console.WriteLine("rewrite index buffer");
        }

        public static VertexElement[] ve = new VertexElement[]
        {
            new VertexElement(0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
        };

        string source_dir;

        public void Load(string source_file)
        {
            source_dir = Path.GetDirectoryName(source_file);
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        public void Load(Stream source_stream)
        {
            BinaryReader reader = new BinaryReader(source_stream, System.Text.Encoding.Default);

            byte[] magic = reader.ReadBytes(3);

            if (magic[0] != (byte)'P' || magic[1] != (byte)'m' || magic[2] != (byte)'d')
                throw new Exception("File is not Pmd");

            float version = reader.ReadSingle();
            Debug.WriteLine("version:" + version);

            string model_name = reader.ReadCString(20);
            Debug.WriteLine("model_name:" + model_name);

            string comment = reader.ReadCString(256);
            Debug.WriteLine("comment:" + comment);

            int vertex_count = reader.ReadInt32();
            Debug.WriteLine("vertex_count:" + vertex_count);
            vertices = new Vertex[vertex_count];

            for (int i = 0; i < vertex_count; i++)
            {
                vertices[i] = new Vertex();
                vertices[i].Read(reader);
            }

            int face_vertex_count = reader.ReadInt32();
            Debug.WriteLine("face_vertex_count:" + face_vertex_count);
            indices = new ushort[face_vertex_count];

            for (int i = 0; i < face_vertex_count; i++)
            {
                indices[i] = reader.ReadUInt16();
            }

            int face_count = face_vertex_count / 3;
            Debug.WriteLine("face_count:" + face_count);

            int material_count = reader.ReadInt32();
            Debug.WriteLine("material_count:" + material_count);
            materials = new PmdMaterial[material_count];

            for (int i = 0; i < material_count; i++)
            {
                materials[i] = new PmdMaterial(i);
                materials[i].Read(reader);
            }

            ushort node_count = reader.ReadUInt16();
            Debug.WriteLine("node_count:" + node_count);
            nodes = new PmdNode[node_count];

            for (ushort i = 0; i < node_count; i++)
            {
                nodes[i] = new PmdNode(i);
                nodes[i].Read(reader);
            }
        }

        internal Dictionary<string, Texture> texmap;

        /// <summary>
        /// 指定device上で開きます。
        /// </summary>
        /// <param name="device">device</param>
        /// <param name="effect">effect</param>
        public void Open(Device device, Effect effect)
        {
            texmap = new Dictionary<string, Texture>();

            foreach (PmdMaterial material in materials)
            {
                if (string.IsNullOrEmpty(material.texture_file))
                    continue;
                if (texmap.ContainsKey(material.texture_file))
                    continue;
                string path = Path.Combine(source_dir, material.texture_file);
                Console.WriteLine("loading {0}", path);
                Texture tex = TextureLoader.FromFile(device, path);
                texmap[material.texture_file] = tex;
            }
        }

        public void Dispose()
        {
            if (ib != null)
                ib.Dispose();
            if (vb != null)
                vb.Dispose();
            foreach (Texture tex in texmap.Values)
                tex.Dispose();
        }
    }
}
