using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace pmd2tmo
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
        public Vector3 diffuse;
        public float alpha;
        public float specularity;
        public Vector3 specular;
        public Vector3 ambient;
        public byte toon_id;
        public byte edge;
        public int face_vertex_count;
        public string texture_file;

        public void Read(BinaryReader reader)
        {
            reader.ReadVector3(ref this.diffuse);
            this.alpha = reader.ReadSingle();
            this.specularity = reader.ReadSingle();
            reader.ReadVector3(ref this.specular);
            reader.ReadVector3(ref this.ambient);
            this.toon_id = reader.ReadByte();
            this.edge = reader.ReadByte();
            this.face_vertex_count = reader.ReadInt32();
            this.texture_file = reader.ReadCString(20);
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
    public class PmdFile
    {
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

            uint vertex_count = reader.ReadUInt32();
            Debug.WriteLine("vertex_count:" + vertex_count);

            for (uint i = 0; i < vertex_count; i++)
            {
                Vertex v = new Vertex();
                v.Read(reader);
            }

            uint face_vertex_count = reader.ReadUInt32();
            Debug.WriteLine("face_vertex_count:" + face_vertex_count);

            uint face_count = face_vertex_count / 3;
            Debug.WriteLine("face_count:" + face_count);

            for (uint i = 0; i < face_count; i++)
            {
                PmdFace face = new PmdFace();
                face.Read(reader);
            }

            uint material_count = reader.ReadUInt32();
            Debug.WriteLine("material_count:" + material_count);

            for (uint i = 0; i < material_count; i++)
            {
                PmdMaterial material = new PmdMaterial();
                material.Read(reader);
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
    }
}
