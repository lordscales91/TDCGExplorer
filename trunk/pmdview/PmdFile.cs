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
        public int face_vertex_start;

        public Vector4 Diffuse
        {
            get { return new Vector4(diffuse.X, diffuse.Y, diffuse.Z, 1); }
        }
        public Vector4 Ambient
        {
            get { return new Vector4(ambient.X, ambient.Y, ambient.Z, 1); }
        }
        public Vector4 Emmisive
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
            face_vertex_start = whole_face_vertex_start;

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

    public class PmdSubMesh : IDisposable
    {
        public int id;
        public Vertex[] vertices;
        public ushort[] indices;
        public PmdMaterial material;

        public Mesh dm = null;

        static VertexElement[] ve = new VertexElement[]
        {
            new VertexElement(0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
        };

        public PmdSubMesh(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// 頂点をDirect3Dバッファに書き込みます。
        /// </summary>
        /// <param name="device">device</param>
        public void WriteBuffer(Device device)
        {
            int numVertices = vertices.Length;
            int numFaces = indices.Length / 3; //faces.Length;

            /*
            List<ushort> indices = new List<ushort>(numFaces * 3);
            foreach (PmdFace face in faces)
            {
                indices.Add(face.a);
                indices.Add(face.b);
                indices.Add(face.c);
            }
            ushort[] optimized_indices = NvTriStrip.Optimize(indices.ToArray());
            */

            if (dm != null)
            {
                dm.Dispose();
                dm = null;
            }
            dm = new Mesh(numFaces, numVertices, MeshFlags.Managed | MeshFlags.WriteOnly, ve, device);

            //
            // rewrite vertex buffer
            //
            {
                GraphicsStream gs = dm.LockVertexBuffer(LockFlags.None);
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
                dm.UnlockVertexBuffer();
            }
            Console.WriteLine("rewrite vertex buffer");

            //
            // rewrite index buffer
            //
            {
                GraphicsStream gs = dm.LockIndexBuffer(LockFlags.None);
                {
                    foreach (ushort idx in indices)
                    {
                        gs.Write(idx);
                    }
                }
                dm.UnlockIndexBuffer();
            }
            Console.WriteLine("rewrite index buffer");

            /*
            //
            // rewrite attribute table
            //
            {
                AttributeRange[] ars = new AttributeRange[materials.Length];
                int i = 0;
                foreach (PmdMaterial material in materials)
                {
#if false
                    ars[i].AttributeId = material.id;
                    ars[i].FaceStart = material.FaceStart;
                    ars[i].FaceCount = material.FaceCount;
                    ars[i].VertexStart = material.face_vertex_start;
                    ars[i].VertexCount = material.face_vertex_count;
#endif
                    ars[i].AttributeId = material.id;
                    ars[i].FaceStart = material.FaceStart;
                    ars[i].FaceCount = material.FaceCount;
                    ars[i].VertexStart = 0;
                    ars[i].VertexCount = vertices.Length;
                    Console.WriteLine("attributerange:{0}", ars[i]);
                }

                dm.SetAttributeTable(ars); 
            }
            Console.WriteLine("rewrite attribute table");
            */
        }

        public void Dispose()
        {
            if (dm != null)
                dm.Dispose();
        }
    }

    /// <summary>
    /// pmdファイルを扱います。
    /// </summary>
    public class PmdFile : IDisposable
    {
        public Vertex[] vertices;
        public PmdSubMesh[] sub_meshes;
        public ushort[] indices;
        //public PmdFace[] faces;
        public PmdMaterial[] materials;
        /// <summary>
        /// bone配列
        /// </summary>
        public PmdNode[] nodes;

        public void Load(string source_file)
        {
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
            /*
            faces = new PmdFace[face_count];

            for (int i = 0; i < face_count; i++)
            {
                faces[i] = new PmdFace();
                faces[i].Read(reader);
            }
             */

            int material_count = reader.ReadInt32();
            Debug.WriteLine("material_count:" + material_count);
            materials = new PmdMaterial[material_count];

            for (int i = 0; i < material_count; i++)
            {
                materials[i] = new PmdMaterial(i);
                materials[i].Read(reader);
            }

            sub_meshes = new PmdSubMesh[material_count];

            for (int i = 0; i < material_count; i++)
            {
                sub_meshes[i] = new PmdSubMesh(i);
                sub_meshes[i].vertices = vertices;
                sub_meshes[i].indices = new ushort[materials[i].face_vertex_count];
                Array.Copy(indices, materials[i].face_vertex_start, sub_meshes[i].indices, 0, materials[i].face_vertex_count);
                sub_meshes[i].material = materials[i];
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
                string path = Path.Combine(@"C:\TechArts3D\MikuMikuDance_v704\UserFile\Model", material.texture_file);
                Console.WriteLine("loading {0}", path);
                Texture tex = TextureLoader.FromFile(device, path);
                texmap[material.texture_file] = tex;
            }
        }

        public void Dispose()
        {
            foreach (PmdSubMesh sub in sub_meshes)
                sub.Dispose();
            foreach (Texture tex in texmap.Values)
                tex.Dispose();
        }
    }

    /// <summary>
    /// テクスチャ
    /// </summary>
    public class PmdTex : IDisposable
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        internal string file;

        internal Texture tex = null;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get { return file; } set { file = value; } }

        /// <summary>
        /// テクスチャを読み込みます。
        /// </summary>
        public void Load(string source_file)
        {
            file = source_file;
        }

        /// <summary>
        /// 指定deviceで開きます。
        /// </summary>
        /// <param name="device">device</param>
        public void Open(Device device)
        {
            if (string.IsNullOrEmpty(file))
                return;
            string path = Path.Combine(@"D:\TechArts3D\MikuMikuDance_v704\UserFile\Model", file);
            Console.WriteLine("loading {0}", path);
            tex = TextureLoader.FromFile(device, path);
        }

        /// <summary>
        /// Direct3Dテクスチャを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (tex != null)
                tex.Dispose();
        }
    }
}
