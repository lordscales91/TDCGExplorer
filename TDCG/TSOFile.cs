using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG.Extensions;

namespace TDCG
{
    /// <summary>
    /// サブメッシュ
    /// </summary>
    public class TSOSubMesh : IDisposable
    {
        /// <summary>
        /// シェーダ設定番号
        /// </summary>
        public int spec;
        /// <summary>
        /// ボーン参照リスト
        /// </summary>
        public int[] bone_indices;
        /// <summary>
        /// ボーン参照リスト
        /// </summary>
        public List<TSONode> bones;
        /// <summary>
        /// 頂点配列
        /// </summary>
        public Vertex[] vertices;

        internal Mesh dm = null;

        /// <summary>
        /// パレット長さ
        /// </summary>
        public int maxPalettes;

        /// <summary>
        /// ボーン数
        /// </summary>
        public int NumberBones
        {
            get { return bone_indices.Length; }
        }

        /// <summary>
        /// 指定indexにあるボーンを得ます。
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>ボーン</returns>
        public TSONode GetBone(int i)
        {
            return bones[i];
        }

        /// <summary>
        /// サブメッシュを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.spec = reader.ReadInt32();
            int bone_indices_count = reader.ReadInt32(); //numbones
            this.maxPalettes = 16;
            if (this.maxPalettes > bone_indices_count)
                this.maxPalettes = bone_indices_count;

            this.bone_indices = new int[bone_indices_count];
            for (int i = 0; i < bone_indices_count; i++)
            {
                this.bone_indices[i] = reader.ReadInt32();
            }

            int vertices_count = reader.ReadInt32(); //numvertices
            this.vertices = new Vertex[vertices_count];
            for (int i = 0; i < vertices_count; i++)
            {
                Vertex v = new Vertex();
                v.Read(reader);
                this.vertices[i] = v;
            }
        }

        /// <summary>
        /// サブメッシュを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.Write(this.spec);

            bw.Write(this.bone_indices.Length);
            foreach (int bone_index in this.bone_indices)
                bw.Write(bone_index);

            bw.Write(this.vertices.Length);
            for (int i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i].Write(bw);
            }
        }

        /// <summary>
        /// ボーン参照リストを生成します。
        /// </summary>
        public void LinkBones(TSONode[] nodes)
        {
            this.bones = new List<TSONode>();

            foreach (int bone_index in bone_indices)
                this.bones.Add(nodes[bone_index]);
        }

        /// 頂点数
        public int VerticesCount
        {
            get { return vertices.Length; }
        }

        static VertexElement[] ve = new VertexElement[]
        {
            new VertexElement(0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
            new VertexElement(0, 28, DeclarationType.Ubyte4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 4),
            new VertexElement(0, 32, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            new VertexElement(0, 44, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
        };

        static AttributeRange ar = new AttributeRange();
        /*
        ar.AttributeId = 0;
        ar.FaceStart = 0;
        ar.FaceCount = 0;
        ar.VertexStart = 0;
        ar.VertexCount = 0;
        */

        /// <summary>
        /// 頂点をDirect3Dバッファに書き込みます。
        /// </summary>
        /// <param name="device">device</param>
        public void WriteBuffer(Device device)
        {
            int numVertices = vertices.Length;
            int numFaces = numVertices - 2;

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
                        for (int j = 0; j < 4; j++)
                            gs.Write(v.skin_weights[j].weight);
                        gs.Write(v.bone_indices);
                        gs.Write(v.normal);
                        gs.Write(v.u);
                        gs.Write(v.v);
                    }
                }
                dm.UnlockVertexBuffer();
            }

            //
            // rewrite index buffer
            //
            {
                GraphicsStream gs = dm.LockIndexBuffer(LockFlags.None);
                {
                    for (int i = 2; i < vertices.Length; i++)
                    {
                        if (i % 2 != 0)
                        {
                            gs.Write((short)(i-0));
                            gs.Write((short)(i-1));
                            gs.Write((short)(i-2));
                        }
                        else
                        {
                            gs.Write((short)(i-2));
                            gs.Write((short)(i-1));
                            gs.Write((short)(i-0));
                        }
                    }
                }
                dm.UnlockIndexBuffer();
            }

            //
            // rewrite attribute buffer
            //
            {
                dm.SetAttributeTable(new AttributeRange[] { ar }); 
            }
        }

        /// <summary>
        /// Direct3Dサブメッシュを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (dm != null)
                dm.Dispose();
        }
    }

    /// <summary>
    /// メッシュ
    /// </summary>
    public class TSOMesh : IDisposable
    {
        string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// 変形行列
        /// </summary>
        public Matrix transform_matrix;
        /// <summary>
        /// unknown1
        /// </summary>
        public UInt32 unknown1;
        /// <summary>
        /// サブメッシュ配列
        /// </summary>
        public TSOSubMesh[] sub_meshes;

        /// <summary>
        /// メッシュを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString().Replace(":", "_colon_").Replace("#", "_sharp_"); //should be compatible with directx naming conventions 
            reader.ReadMatrix(ref this.transform_matrix);
            this.unknown1 = reader.ReadUInt32();
            UInt32 mesh_count = reader.ReadUInt32();
            this.sub_meshes = new TSOSubMesh[mesh_count];
            for (int i = 0; i < mesh_count; i++)
            {
                TSOSubMesh sub_mesh = new TSOSubMesh();
                sub_mesh.Read(reader);
                this.sub_meshes[i] = sub_mesh;
            }
        }

        /// <summary>
        /// メッシュを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.name);
            Matrix m = this.transform_matrix;
            bw.Write(ref m);
            bw.Write(this.unknown1);
            bw.Write(this.sub_meshes.Length);

            foreach (TSOSubMesh sub_mesh in this.sub_meshes)
                sub_mesh.Write(bw);
        }

        /// <summary>
        /// ボーン参照リストを生成します。
        /// </summary>
        public void LinkBones(TSONode[] nodes)
        {
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sub_mesh.LinkBones(nodes);
        }

        /// 頂点数の合計を得ます。
        public int SumVerticesCount()
        {
            int sum = 0;
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sum += sub_mesh.VerticesCount;
            return sum;
        }

        /// <summary>
        /// 内部objectを破棄します。
        /// </summary>
        public void Dispose()
        {
            if (sub_meshes != null)
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sub_mesh.Dispose();
        }
    }

    /// <summary>
    /// 頂点
    /// </summary>
    public class Vertex
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
        /// ボーンインデックス
        /// </summary>
        public UInt32 bone_indices;

        /// <summary>
        /// 頂点を読みとります。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            reader.ReadVector3(ref this.position);
            reader.ReadVector3(ref this.normal);
            this.u = reader.ReadSingle();
            this.v = reader.ReadSingle();
            int skin_weights_count = reader.ReadInt32();
            this.skin_weights = new SkinWeight[skin_weights_count];
            for (int i = 0; i < skin_weights_count; i++)
            {
                int bone_index = reader.ReadInt32();
                float weight = reader.ReadSingle();
                this.skin_weights[i] = new SkinWeight(bone_index, weight);
            }

            FillSkinWeights();
            GenerateBoneIndices();
        }

        /// <summary>
        /// スキンウェイト配列を充填します。
        /// </summary>
        public void FillSkinWeights()
        {
            Array.Sort(this.skin_weights);
            int len = skin_weights.Length;
            Array.Resize(ref this.skin_weights, 4);
            for (int i = len; i < 4; i++)
                this.skin_weights[i] = new SkinWeight(0, 0.0f);
        }

        /// <summary>
        /// ボーンインデックスを生成します。
        /// </summary>
        public void GenerateBoneIndices()
        {
            byte[] idx = new byte[4];
            for (int i = 0; i < 4; i++)
                idx[i] = (byte)this.skin_weights[i].bone_index;

            this.bone_indices = BitConverter.ToUInt32(idx, 0);
        }

        /// <summary>
        /// 頂点を書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.Write(ref this.position);
            bw.Write(ref this.normal);
            bw.Write(this.u);
            bw.Write(this.v);

            int skin_weights_count = 0;
            SkinWeight[] skin_weights = new SkinWeight[4];
            foreach (SkinWeight skin_weight in this.skin_weights)
            {
                if (skin_weight.weight == 0.0f)
                    continue;

                skin_weights[skin_weights_count++] = skin_weight;
            }
            bw.Write(skin_weights_count);

            for (int i = 0; i < skin_weights_count; i++)
            {
                bw.Write(skin_weights[i].bone_index);
                bw.Write(skin_weights[i].weight);
            }
        }
    }

    /// <summary>
    /// スキンウェイト
    /// </summary>
    public class SkinWeight : IComparable
    {
        /// <summary>
        /// ボーンインデックス
        /// </summary>
        public int bone_index;

        /// <summary>
        /// ウェイト
        /// </summary>
        public float weight;

        /// <summary>
        /// スキンウェイトを生成します。
        /// </summary>
        /// <param name="bone_index">ボーンインデックス</param>
        /// <param name="weight">ウェイト</param>
        public SkinWeight(int bone_index, float weight)
        {
            this.bone_index = bone_index;
            this.weight = weight;
        }

        /// <summary>
        /// 比較関数
        /// </summary>
        /// <param name="obj">比較対象スキンウェイト</param>
        /// <returns>比較結果</returns>
        public int CompareTo(object obj)
        {
            return -weight.CompareTo(((SkinWeight)obj).weight);
        }
    }

    /// <summary>
    /// スクリプト
    /// </summary>
    public class TSOScript
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name;
        /// <summary>
        /// テキスト行配列
        /// </summary>
        public string[] lines;

        /// <summary>
        /// スクリプトを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString();
            UInt32 line_count = reader.ReadUInt32();
            this.lines = new string[line_count];
            for (int i = 0; i < line_count; i++)
            {
                lines[i] = reader.ReadCString();
            }
        }

        /// <summary>
        /// スクリプトを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.name);
            bw.Write(this.lines.Length);

            foreach (string line in this.lines)
                bw.WriteCString(line);
        }
    }

    /// <summary>
    /// サブスクリプト
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TSOSubScript
    {
        internal string name;
        internal string file;
        /// <summary>
        /// テキスト行配列
        /// </summary>
        public string[] lines;
        /// <summary>
        /// シェーダ設定
        /// </summary>
        public Shader shader = null;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// ファイル名
        /// </summary>
        public string File { get { return file; } set { file = value; } }

        /// <summary>
        /// サブスクリプトを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString();
            this.file = reader.ReadCString();
            UInt32 line_count = reader.ReadUInt32();
            this.lines = new string[line_count];
            for (int i = 0; i < line_count; i++)
            {
                this.lines[i] = reader.ReadCString();
            }

            //Console.WriteLine("name {0} file {1}", this.name, this.file);
        }

        /// <summary>
        /// サブスクリプトを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.name);
            bw.WriteCString(this.file);
            bw.Write(this.lines.Length);

            foreach (string line in this.lines)
                bw.WriteCString(line);
        }

        /// <summary>
        /// シェーダ設定を生成します。
        /// </summary>
        public void GenerateShader()
        {
            this.shader = new Shader();
            this.shader.Load(this.lines);
        }
    }

    /// <summary>
    /// テクスチャ
    /// </summary>
    public class TSOTex : IDisposable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name;
        /// <summary>
        /// ファイル名
        /// </summary>
        public string file;
        /// <summary>
        /// 幅
        /// </summary>
        public int width;
        /// <summary>
        /// 高さ
        /// </summary>
        public int height;
        /// <summary>
        /// 色深度
        /// </summary>
        public int depth;
        /// <summary>
        /// ビットマップ配列
        /// </summary>
        public byte[] data;

        internal Texture tex;

        /// <summary>
        /// テクスチャを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.name = reader.ReadCString();
            this.file = reader.ReadCString();
            this.width = reader.ReadInt32();
            this.height = reader.ReadInt32();
            this.depth = reader.ReadInt32();
            this.data = reader.ReadBytes( this.width * this.height * this.depth );

            for(int j = 0; j < this.data.Length; j += 4)
            {
                byte tmp = this.data[j+2];
                this.data[j+2] = this.data[j+0];
                this.data[j+0] = tmp;
            }
        }

        /// <summary>
        /// テクスチャを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.name);
            bw.WriteCString(this.file);
            bw.Write(this.width);
            bw.Write(this.height);
            bw.Write(this.depth);

            byte[] buf = new byte[this.data.Length];
            Array.Copy(this.data, 0, buf, 0, buf.Length);

            for(int j = 0; j < buf.Length; j += 4)
            {
                byte tmp = buf[j+2];
                buf[j+2] = buf[j+0];
                buf[j+0] = tmp;
            }
            bw.Write(buf);
        }

        /// <summary>
        /// 指定deviceで開きます。
        /// </summary>
        /// <param name="device">device</param>
        public void Open(Device device)
        {
            if (file.Trim('"') == "")
                return;
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                {
                    bw.Write((byte)'B');
                    bw.Write((byte)'M');
                    bw.Write((int)(54 + data.Length));
                    bw.Write((int)0);
                    bw.Write((int)54);
                    bw.Write((int)40);
                    bw.Write((int)width);
                    bw.Write((int)height);
                    bw.Write((short)1);
                    bw.Write((short)(depth*8));
                    bw.Write((int)0);
                    bw.Write((int)data.Length);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                    bw.Write((int)0);
                }

                int count = width * depth;
                int index = width * height * depth - count;
                for (int y = 0; y < height; y++)
                {
                    bw.Write(data, index, count);
                    index -= count;
                }
                bw.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                tex = TextureLoader.FromStream(device, ms);
            }
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

    /// <summary>
    /// node (bone)
    /// </summary>
    public class TSONode
    {
        private int id;
        private string path;
        private string name;

        private Quaternion rotation;
        private Vector3 translation;

        private Matrix transformation_matrix;
        private bool need_update_transformation;

        /// <summary>
        /// TSONodeを生成します。
        /// </summary>
        public TSONode(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// TSONodeを読み込みます。
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.Path = reader.ReadCString();
        }

        /// <summary>
        /// TSONodeを書き出します。
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.Path);
        }

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
        /// 子nodeリスト
        /// </summary>
        public List<TSONode> children = new List<TSONode>();

        /// <summary>
        /// 親node
        /// </summary>
        public TSONode parent;

        /// <summary>
        /// オフセット行列。これはワールド座標系をboneローカル座標系に変換します。
        /// </summary>
        public Matrix offset_matrix;

        /// <summary>
        /// ワールド座標系での位置と向きを表します。これはviewerから更新されます。
        /// </summary>
        public Matrix combined_matrix;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get { return id; } }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                name = path.Substring(path.LastIndexOf('|') + 1);
            }
        }

        /// <summary>
        /// 名称の短い形式。これはTSOFile中で重複する可能性があります。
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// 指定nodeに対するオフセット行列を計算します。
        /// </summary>
        /// <param name="node">node</param>
        public static Matrix GetOffsetMatrix(TSONode node)
        {
            Matrix m = Matrix.Identity;
            while (node != null)
            {
                m.Multiply(node.TransformationMatrix);
                node = node.parent;
            }
            return Matrix.Invert(m);
        }

        /// <summary>
        /// オフセット行列を計算します。
        /// </summary>
        public void ComputeOffsetMatrix()
        {
            offset_matrix = TSONode.GetOffsetMatrix(this);
        }

        /// <summary>
        /// オフセット行列を得ます。
        /// </summary>
        /// <returns></returns>
        public Matrix OffsetMatrix
        {
            get { return offset_matrix; }
        }

        /// <summary>
        /// ワールド座標系での位置を得ます。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition()
        {
            TSONode node = this;
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
            TSONode node = this;
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
                translation = TMOMat.DecomposeMatrix(ref m);
                rotation = Quaternion.RotationMatrix(m);
            }
        }
    }

    /// <summary>
    /// TSOファイルを扱います。
    /// </summary>
    public class TSOFile : IDisposable
    {
        /// <summary>
        /// バイナリ値として読み取ります。
        /// </summary>
        protected BinaryReader reader;

        /// <summary>
        /// bone配列
        /// </summary>
        public TSONode[] nodes;
        /// <summary>
        /// テクスチャ配列
        /// </summary>
        public TSOTex[] textures;
        /// <summary>
        /// スクリプト配列
        /// </summary>
        public TSOScript[] scripts;
        /// <summary>
        /// サブスクリプト配列
        /// </summary>
        public TSOSubScript[] sub_scripts;
        /// <summary>
        /// メッシュ配列
        /// </summary>
        public TSOMesh[] meshes;

        internal Dictionary<string, TSONode> nodemap;

        /// <summary>
        /// 指定パスに保存します。
        /// </summary>
        /// <param name="dest_file">パス</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// 指定ストリームに保存します。
        /// </summary>
        /// <param name="dest_stream">ストリーム</param>
        public void Save(Stream dest_stream)
        {
            BinaryWriter bw = new BinaryWriter(dest_stream);

            WriteMagic(bw);

            bw.Write(nodes.Length);
            foreach (TSONode node in nodes)
                node.Write(bw);

            bw.Write(nodes.Length);
            Matrix m = Matrix.Identity;
            foreach (TSONode node in nodes)
            {
                m = node.TransformationMatrix;
                bw.Write(ref m);
            }

            bw.Write(textures.Length);
            foreach (TSOTex tex in textures)
                tex.Write(bw);

            bw.Write(scripts.Length);
            foreach (TSOScript script in scripts)
                script.Write(bw);

            bw.Write(sub_scripts.Length);
            foreach (TSOSubScript sub_script in sub_scripts)
                sub_script.Write(bw);

            bw.Write(meshes.Length);
            foreach (TSOMesh mesh in meshes)
                mesh.Write(bw);
        }

        /// <summary>
        /// 'TSO1' を書き出します。
        /// </summary>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F5354);
        }

        /// <summary>
        /// 指定パスから読み込みます。
        /// </summary>
        /// <param name="source_file">パス</param>
        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        /// <summary>
        /// 指定ストリームから読み込みます。
        /// </summary>
        /// <param name="source_stream">ストリーム</param>
        public void Load(Stream source_stream)
        {
            reader = new BinaryReader(source_stream, System.Text.Encoding.Default);

            byte[] magic = reader.ReadBytes(4);

            if(magic[0] != (byte)'T'
            || magic[1] != (byte)'S'
            || magic[2] != (byte)'O'
            || magic[3] != (byte)'1')
                throw new Exception("File is not TSO");

            int node_count = reader.ReadInt32();
            nodes = new TSONode[node_count];
            for (int i = 0; i < node_count; i++)
            {
                nodes[i] = new TSONode(i);
                nodes[i].Read(reader);
            }

            GenerateNodemapAndTree();

            int node_matrix_count = reader.ReadInt32();
            Matrix m = Matrix.Identity;
            for (int i = 0; i < node_matrix_count; i++)
            {
                reader.ReadMatrix(ref m);
                nodes[i].TransformationMatrix = m;
            }
            for (int i = 0; i < node_matrix_count; i++)
            {
                nodes[i].ComputeOffsetMatrix();
            }

            UInt32 texture_count = reader.ReadUInt32();
            textures = new TSOTex[texture_count];
            for (int i = 0; i < texture_count; i++)
            {
                textures[i] = new TSOTex();
                textures[i].Read(reader);
            }

            UInt32 script_count = reader.ReadUInt32();
            scripts = new TSOScript[script_count];
            for (int i = 0; i < script_count; i++)
            {
                scripts[i] = new TSOScript();
                scripts[i].Read(reader);
            }

            UInt32 sub_script_count = reader.ReadUInt32();
            sub_scripts = new TSOSubScript[sub_script_count];
            for (int i = 0; i < sub_script_count; i++)
            {
                sub_scripts[i] = new TSOSubScript();
                sub_scripts[i].Read(reader);
                sub_scripts[i].GenerateShader();
            }

            UInt32 mesh_count = reader.ReadUInt32();
            meshes = new TSOMesh[mesh_count];
            for (int i = 0; i < mesh_count; i++)
            {
                meshes[i] = new TSOMesh();
                meshes[i].Read(reader);
                meshes[i].LinkBones(nodes);

                //Console.WriteLine("mesh name {0} len {1}", mesh.name, mesh.sub_meshes.Length);
            }
        }

        internal void GenerateNodemapAndTree()
        {
            nodemap = new Dictionary<string, TSONode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                nodemap.Add(nodes[i].Path, nodes[i]);
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                int index = nodes[i].Path.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string path = nodes[i].Path.Substring(0, index);
                nodes[i].parent = nodemap[path];
                nodes[i].parent.children.Add(nodes[i]);
            }
        }

        /// <summary>
        /// tmoを生成します。
        /// </summary>
        public TMOFile GenerateTMO()
        {
            TMOFile tmo = new TMOFile();
            tmo.header = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            tmo.opt0 = 1;
            tmo.opt1 = 0;

            int node_count = nodes.Length;
            tmo.nodes = new TMONode[node_count];

            for (int i = 0; i < node_count; i++)
            {
                tmo.nodes[i] = new TMONode(i);
                tmo.nodes[i].Path = nodes[i].Path;
            }

            tmo.GenerateNodemapAndTree();

            int frame_count = 1;
            tmo.frames = new TMOFrame[frame_count];

            for (int i = 0; i < frame_count; i++)
            {
                tmo.frames[i] = new TMOFrame(i);
                int matrix_count = node_count;
                tmo.frames[i].matrices = new TMOMat[matrix_count];
                for (int j = 0; j < matrix_count; j++)
                {
                    Matrix m = nodes[j].TransformationMatrix;
                    tmo.frames[i].matrices[j] = new TMOMat(ref m);
                }
            }
            foreach (TMONode node in tmo.nodes)
                node.LinkMatrices(tmo.frames);

            tmo.footer = new byte[4] { 0, 0, 0, 0 };

            return tmo;
        }

        /// 頂点数の合計を得ます。
        public int SumVerticesCount()
        {
            int sum = 0;
            foreach (TSOMesh mesh in meshes)
                sum += mesh.SumVerticesCount();
            return sum;
        }

        internal Device device;
        internal Effect effect;

        EffectHandle[] techniques;
        internal Dictionary<string, EffectHandle> techmap;

        private EffectHandle handle_ShadeTex_texture;
        private EffectHandle handle_ColorTex_texture;
        internal Dictionary<string, TSOTex> texmap;

        private EffectHandle handle_LightDir;
        private EffectHandle handle_LightDirForced;
        private EffectHandle handle_UVSCR;

        /// <summary>
        /// 指定device上で開きます。
        /// </summary>
        /// <param name="device">device</param>
        /// <param name="effect">effect</param>
        public void Open(Device device, Effect effect)
        {
            this.device = device;
            this.effect = effect;

            foreach (TSOMesh mesh in meshes)
            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
                sub_mesh.WriteBuffer(device);

            texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in textures)
            {
                tex.Open(device);
                texmap[tex.name] = tex;
            }

            handle_ShadeTex_texture = effect.GetParameter(null, "ShadeTex_texture");
            handle_ColorTex_texture = effect.GetParameter(null, "ColorTex_texture");

            handle_LightDir = effect.GetParameter(null, "LightDir");
            handle_LightDirForced = effect.GetParameter(null, "LightDirForced");
            handle_UVSCR = effect.GetParameter(null, "UVSCR");

            techmap = new Dictionary<string, EffectHandle>();

            int ntech = effect.Description.Techniques;
            techniques = new EffectHandle[ntech];

            //Console.WriteLine("Techniques:");

            for (int i = 0; i < ntech; i++)
            {
                techniques[i] = effect.GetTechnique(i);
                string tech_name = effect.GetTechniqueDescription(techniques[i]).Name;
                techmap[tech_name] = techniques[i];

                //Console.WriteLine(i + " " + tech_name);
            }
        }

        internal Shader current_shader = null;
        internal Vector3 lightDir = new Vector3(0.0f, 0.0f, -1.0f);

        /// <summary>
        /// 光源方向ベクトルを得ます。
        /// </summary>
        /// <returns></returns>
        public Vector4 LightDirForced()
        {
            return new Vector4(lightDir.X, lightDir.Y, lightDir.Z, 0.0f);
        }

        /// <summary>
        /// UVSCR値を得ます。
        /// </summary>
        /// <returns></returns>
        public Vector4 UVSCR()
        {
            float x = Environment.TickCount * 0.000002f;
            return new Vector4(x, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// レンダリング開始時に呼びます。
        /// </summary>
        public void BeginRender()
        {
            current_shader = null;
        }

        /// <summary>
        /// シェーダ設定を切り替えます。
        /// </summary>
        /// <param name="shader">シェーダ設定</param>
        public void SwitchShader(Shader shader)
        {
            if (shader == current_shader)
                return;
            current_shader = shader;

            if (! techmap.ContainsKey(shader.technique))
            {
                Console.WriteLine("Error: shader technique not found. " + shader.technique);
                return;
            }

            foreach (ShaderParameter p in shader.shader_parameters)
            {
                switch (p.type)
                {
                case ShaderParameter.Type.String:
                    effect.SetValue(p.name, p.GetString());
                    break;
                case ShaderParameter.Type.Float:
                case ShaderParameter.Type.Float3:
                case ShaderParameter.Type.Float4:
                    effect.SetValue(p.name, new float[]{ p.F1, p.F2, p.F3, p.F4 });
                    break;
                    /*
                case ShaderParameter.Type.Texture:
                    effect.SetValue(p.name, p.GetTexture());
                    break;
                    */
                }
            }
            effect.SetValue(handle_LightDir, shader.LightDir);
            effect.SetValue(handle_LightDirForced, LightDirForced());
            effect.SetValue(handle_UVSCR, UVSCR());

            TSOTex shadeTex;
            if (shader.shadeTex != null && texmap.TryGetValue(shader.shadeTex, out shadeTex))
                effect.SetValue(handle_ShadeTex_texture, shadeTex.tex);

            TSOTex colorTex;
            if (shader.colorTex != null && texmap.TryGetValue(shader.colorTex, out colorTex))
                effect.SetValue(handle_ColorTex_texture, colorTex.tex);

            effect.Technique = techmap[shader.technique];
            effect.ValidateTechnique(effect.Technique);
        }

        /// <summary>
        /// シェーダ設定を切り替えます。
        /// </summary>
        /// <param name="sub_mesh">切り替え対象となるサブメッシュ</param>
        public void SwitchShader(TSOSubMesh sub_mesh)
        {
            Debug.Assert(sub_mesh.spec >= 0 && sub_mesh.spec < sub_scripts.Length, string.Format("mesh.spec out of range: {0}", sub_mesh.spec));
            SwitchShader(sub_scripts[sub_mesh.spec].shader);
        }

        /// <summary>
        /// レンダリング終了時に呼びます。
        /// </summary>
        public void EndRender()
        {
            current_shader = null;
        }

        /// <summary>
        /// 内部objectを破棄します。
        /// </summary>
        public void Dispose()
        {
            foreach (TSOMesh mesh in meshes)
                mesh.Dispose();
            foreach (TSOTex tex in textures)
                tex.Dispose();
        }
    }
}
