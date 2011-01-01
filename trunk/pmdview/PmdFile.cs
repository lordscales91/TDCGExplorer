using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Quaternionを読みとります。
        /// </summary>
        /// <param name="reader">BinaryReader</param>
        /// <param name="q">Quaternion</param>
        public static void ReadQuaternion(this BinaryReader reader, ref Quaternion q)
        {
            q.X = reader.ReadSingle();
            q.Y = reader.ReadSingle();
            q.Z = reader.ReadSingle();
            q.W = reader.ReadSingle();
        }
    }

    public struct VertexPosition
    {
        public Vector3 position;
        public Vector3 normal;
    }

    public struct VertexTexcoord
    {
        public float u;
        public float v;
    }

    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public float u;
        public float v;
        public ushort node_id_0;
        public ushort node_id_1;
        public float weight;
        public byte edge;

        public void Read(BinaryReader reader)
        {
            reader.ReadVector3(ref this.position);
            reader.ReadVector3(ref this.normal);
            this.u = reader.ReadSingle();
            this.v = reader.ReadSingle();
            this.node_id_0 = reader.ReadUInt16();
            this.node_id_1 = reader.ReadUInt16();
            this.weight = reader.ReadByte() * 0.01f;
            this.edge = reader.ReadByte();
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
        public int face_vertex_start;
        public int face_vertex_count;
        public string texture_file;

        public int FaceStart
        {
            get { return face_vertex_start / 3; }
        }
        public int FaceCount
        {
            get { return face_vertex_count / 3; }
        }

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
        }

        public int InjectFaceVertexStart(int value)
        {
            face_vertex_start = value;
            return face_vertex_count + value;
        }
    }

    /// <summary>
    /// boneを扱います。
    /// </summary>
    public class PmdNode
    {
        public ushort id;
        public string name;
        public ushort parent_node_id;
        public ushort tail_node_id;
        public byte type;
        public ushort ik_parent_node_id;
        public Vector3 position;

        public List<PmdNode> children = new List<PmdNode>();
        public PmdNode parent;

        /// <summary>
        /// オフセット行列。これはワールド座標系をboneローカル座標系に変換します。
        /// </summary>
        public Matrix offset_matrix;

        public Vector3 local_translation = Vector3.Empty;

        private Quaternion rotation = Quaternion.Identity;
        private Vector3 translation = Vector3.Empty;

        private Matrix transformation_matrix;
        private bool need_update_transformation;

        /// <summary>
        /// 回転変位
        /// </summary>
        public Quaternion Rotation
        {
            get {
                return rotation;
            }
            set {
                rotation = value;
                need_update_transformation = true;
            }
        }

        /// <summary>
        /// 位置変位
        /// </summary>
        public Vector3 Translation
        {
            get {
                return translation;
            }
            set {
                translation = value;
                need_update_transformation = true;
            }
        }

        /// <summary>
        /// ワールド座標系での位置を得ます。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition()
        {
            PmdNode node = this;
            Vector3 v = Vector3.Empty;
            while (node != null)
            {
                v = Vector3.TransformCoordinate(v, node.TransformationMatrix);
                node = node.parent;
            }
            return v;
        }

        /// <summary>
        /// ワールド座標系での位置と向きを得ます。
        /// </summary>
        /// <returns></returns>
        public Matrix GetWorldCoordinate()
        {
            PmdNode node = this;
            Matrix m = Matrix.Identity;
            while (node != null)
            {
                m.Multiply(node.TransformationMatrix);
                node = node.parent;
            }
            return m;
        }

        /// <summary>
        /// 回転行列
        /// </summary>
        public Matrix RotationMatrix
        {
            get {
                return Matrix.RotationQuaternion(rotation);
            }
        }

        /// <summary>
        /// 位置行列
        /// </summary>
        public Matrix TranslationMatrix
        {
            get {
                return Matrix.Translation(translation);
            }
        }

        /// <summary>
        /// 変形行列。これは 回転行列 x 位置行列 です。
        /// </summary>
        public Matrix TransformationMatrix
        {
            get {
                if (need_update_transformation)
                {
                    transformation_matrix = RotationMatrix * TranslationMatrix;
                    need_update_transformation = false;
                }
                return transformation_matrix;
            }
            set {
                transformation_matrix = value;
                Matrix m = transformation_matrix;
                translation = DecomposeMatrix(ref m);
                rotation = Quaternion.RotationMatrix(m);
            }
        }

        /// <summary>
        /// 回転行列と位置ベクトルに分割します。
        /// </summary>
        /// <param name="m">元の行列（戻り値は回転行列）</param>
        /// <returns>位置ベクトル</returns>
        public static Vector3 DecomposeMatrix(ref Matrix m)
        {
            Vector3 t = new Vector3(m.M41, m.M42, m.M43);
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            return t;
        }

        /// <summary>
        /// ワールド座標系での位置と向きを表します。これはviewerから更新されます。
        /// </summary>
        public Matrix combined_matrix;

        public void ComputeOffsetMatrix()
        {
            offset_matrix = Matrix.Invert(Matrix.Translation(position));
        }

        /// <summary>
        /// PmdNodeを生成します。
        /// </summary>
        public PmdNode(ushort id)
        {
            this.id = id;
        }

        static Regex re_knee = new Regex(@"ひざ\z");
        bool knee_p;
        public bool IsKnee { get { return knee_p; } }

        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString(20);
            this.parent_node_id = reader.ReadUInt16();
            this.tail_node_id = reader.ReadUInt16();
            this.type = reader.ReadByte();
            this.ik_parent_node_id = reader.ReadUInt16();
            reader.ReadVector3(ref this.position);
            this.knee_p = re_knee.IsMatch(name);
        }
    }

    public class PmdIK
    {
        public ushort target_node_id;
        public ushort effector_node_id;
        public byte chain_length;
        public ushort niteration;
        public float weight;
        public ushort[] chain_node_ids;

        public PmdNode target = null;
        public PmdNode effector = null;
        public List<PmdNode> chain_nodes = new List<PmdNode>();

        public void Read(BinaryReader reader)
        {
            this.target_node_id = reader.ReadUInt16();
            this.effector_node_id = reader.ReadUInt16();
            this.chain_length = reader.ReadByte();
            this.niteration = reader.ReadUInt16();
            this.weight = reader.ReadSingle();
            this.chain_node_ids = new ushort[chain_length];
            for (int i = 0; i < chain_length; i++)
            {
                chain_node_ids[i] = reader.ReadUInt16();
            }
        }
    }

    /// <summary>
    /// pmdファイルを扱います。
    /// </summary>
    public class PmdFile : IDisposable
    {
        public Vertex[] vertices;
        public ushort[] indices;
        public PmdMaterial[] materials;
        /// <summary>
        /// bone配列
        /// </summary>
        public PmdNode[] nodes;
        public PmdIK[] iks;

        public VertexBuffer vb_position = null;
        public VertexBuffer vb_texcoord = null;
        public IndexBuffer ib = null;

        /// <summary>
        /// 頂点をDirect3Dバッファに書き込みます。
        /// </summary>
        /// <param name="device">device</param>
        public void WriteVertexBuffer(Device device)
        {
            if (vb_position != null)
                vb_position.Dispose();
            vb_position = new VertexBuffer(typeof(VertexPosition), vertices.Length, device, Usage.Dynamic | Usage.WriteOnly, VertexFormats.None, Pool.Default);

            vb_position.Created += new EventHandler(vb_position_Created);
            ClipBoneMatrices();
            vb_position_Created(vb_position, null);

            if (vb_texcoord != null)
                vb_texcoord.Dispose();
            vb_texcoord = new VertexBuffer(typeof(VertexTexcoord), vertices.Length, device, Usage.Dynamic | Usage.WriteOnly, VertexFormats.None, Pool.Default);

            vb_texcoord.Created += new EventHandler(vb_texcoord_Created);
            vb_texcoord_Created(vb_texcoord, null);
        }

        public void RewriteVertexBuffer(Device device)
        {
            ClipBoneMatrices();
            vb_position_Created(vb_position, null);
        }

        Matrix[] bone_matrices;

        void ClipBoneMatrices()
        {
            for (int i = 0; i < nodes.Length; i++)
                bone_matrices[i] = nodes[i].offset_matrix * nodes[i].combined_matrix;
        }

        void CalcSkindeform(ref Vertex v, out Vector3 position, out Vector3 normal)
        {
            Vector3 pos = Vector3.Empty;
            {
                Matrix m = bone_matrices[v.node_id_0];
                float w = v.weight;
                pos += Vector3.TransformCoordinate(v.position, m) * w;
            }
            {
                Matrix m = bone_matrices[v.node_id_1];
                float w = 1 - v.weight;
                pos += Vector3.TransformCoordinate(v.position, m) * w;
            }
            position = pos;

            Vector3 nor = Vector3.Empty;
            {
                Matrix m = bone_matrices[v.node_id_0];
                m.M41 = 0;
                m.M42 = 0;
                m.M43 = 0;
                float w = v.weight;
                nor += Vector3.TransformCoordinate(v.normal, m) * w;
            }
            {
                Matrix m = bone_matrices[v.node_id_1];
                m.M41 = 0;
                m.M42 = 0;
                m.M43 = 0;
                float w = 1 - v.weight;
                nor += Vector3.TransformCoordinate(v.normal, m) * w;
            }
            normal = Vector3.Normalize(nor);
        }

        void vb_position_Created(object sender, EventArgs e)
        {
            VertexBuffer vb = (VertexBuffer)sender;
            //
            // rewrite vertex position buffer
            //
            {
                GraphicsStream gs = vb.Lock(0, 0, LockFlags.None);
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vertex v = vertices[i];

                        Vector3 position, normal;
                        CalcSkindeform(ref v, out position, out normal);

                        gs.Write(position);
                        gs.Write(normal);
                    }
                }
                vb.Unlock();
            }
            //Console.WriteLine("rewrite vertex position buffer");
        }

        void vb_texcoord_Created(object sender, EventArgs e)
        {
            VertexBuffer vb = (VertexBuffer)sender;
            //
            // rewrite vertex texcoord buffer
            //
            {
                GraphicsStream gs = vb.Lock(0, 0, LockFlags.None);
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vertex v = vertices[i];

                        gs.Write(v.u);
                        gs.Write(v.v);
                    }
                }
                vb.Unlock();
            }
            //Console.WriteLine("rewrite vertex texcoord buffer");
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
            ib.Created += new EventHandler(ib_Created);
            ib_Created(ib, null);
        }

        void ib_Created(object sender, EventArgs e)
        {
            IndexBuffer ib = (IndexBuffer)sender;
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
            //Console.WriteLine("rewrite index buffer");
        }

        public static VertexElement[] ve = new VertexElement[]
        {
            new VertexElement(0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            new VertexElement(1,  0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
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

            int face_vertex_start = 0;
            for (int i = 0; i < material_count; i++)
            {
                face_vertex_start = materials[i].InjectFaceVertexStart(face_vertex_start);
            }

            ushort node_count = reader.ReadUInt16();
            Debug.WriteLine("node_count:" + node_count);
            nodes = new PmdNode[node_count];

            for (ushort i = 0; i < node_count; i++)
            {
                nodes[i] = new PmdNode(i);
                nodes[i].Read(reader);
            }
            for (ushort i = 0; i < node_count; i++)
                nodes[i].ComputeOffsetMatrix();

            GenerateNodemapAndTree();
            bone_matrices = new Matrix[node_count];

            ushort ik_count = reader.ReadUInt16();
            Debug.WriteLine("ik_count:" + ik_count);
            iks = new PmdIK[ik_count];

            for (ushort i = 0; i < ik_count; i++)
            {
                iks[i] = new PmdIK();
                iks[i].Read(reader);
            }
            UpdateNodeIKs();
        }

        public Dictionary<string, PmdNode> nodemap = new Dictionary<string, PmdNode>();

        public List<PmdNode> root_nodes = new List<PmdNode>();

        public void GenerateNodemapAndTree()
        {
            nodemap.Clear();
            foreach (PmdNode node in nodes)
                nodemap[node.name] = node;

            foreach (PmdNode node in nodes)
            {
                node.parent = null;
                node.children.Clear();
            }
            foreach (PmdNode node in nodes)
            {
                if (node.parent_node_id == ushort.MaxValue)
                {
                    root_nodes.Add(node);
                    continue;
                }
                node.parent = nodes[node.parent_node_id];
                node.parent.children.Add(node);
            }

            foreach (PmdNode node in nodes)
            {
                if (node.parent == null)
                    node.local_translation = node.position;
                else
                    node.local_translation = node.position - node.parent.position;
            }
        }

        public void UpdateNodeIKs()
        {
            foreach (PmdIK ik in iks)
            {
                ik.effector = nodes[ik.effector_node_id];
                ik.target = nodes[ik.target_node_id];
                ik.chain_nodes.Clear();
                foreach (ushort node_id in ik.chain_node_ids)
                {
                    ik.chain_nodes.Add(nodes[node_id]);
                }
            }
        }

        public VmdFile GenerateVmd()
        {
            VmdFile vmd = new VmdFile();

            int node_count = nodes.Length;
            vmd.nodes = new VmdNode[node_count];

            for (ushort i = 0; i < node_count; i++)
            {
                vmd.nodes[i] = new VmdNode(i);
                vmd.nodes[i].name = nodes[i].name;
                vmd.nodes[i].parent_node_id = nodes[i].parent_node_id;
                VmdMat mat = new VmdMat();
                vmd.nodes[i].matrices = new VmdMat[] { mat };
            }

            vmd.GenerateNodemapAndTree();

            return vmd;
        }

        internal Dictionary<string, Texture> texmap;

        /// <summary>
        /// 指定device上で開きます。
        /// </summary>
        /// <param name="device">device</param>
        /// <param name="effect">effect</param>
        public void Open(Device device, Effect effect)
        {
            if (texmap != null)
            {
                foreach (Texture tex in texmap.Values)
                    tex.Dispose();
            }
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
            if (vb_texcoord != null)
                vb_texcoord.Dispose();
            if (vb_position != null)
                vb_position.Dispose();
            foreach (Texture tex in texmap.Values)
                tex.Dispose();
        }
    }
}
