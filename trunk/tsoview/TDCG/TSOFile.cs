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
    /// �T�u���b�V��
    /// </summary>
    public class TSOSubMesh : IDisposable
    {
        /// <summary>
        /// �V�F�[�_�ݒ�ԍ�
        /// </summary>
        public int spec;
        /// <summary>
        /// �{�[���Q�ƃ��X�g
        /// </summary>
        public int[] bone_indices;
        /// <summary>
        /// �{�[���Q�ƃ��X�g
        /// </summary>
        public List<TSONode> bones;
        /// <summary>
        /// ���_�z��
        /// </summary>
        public Vertex[] vertices;

        internal Mesh dm = null;

        /// <summary>
        /// �p���b�g����
        /// </summary>
        public int maxPalettes;

        /// <summary>
        /// �{�[����
        /// </summary>
        public int NumberBones
        {
            get { return bone_indices.Length; }
        }

        /// <summary>
        /// �w��index�ɂ���{�[���𓾂܂��B
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>�{�[��</returns>
        public TSONode GetBone(int i)
        {
            return bones[i];
        }

        /// <summary>
        /// �T�u���b�V����ǂݍ��݂܂��B
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
        /// �T�u���b�V���������o���܂��B
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
        /// �{�[���Q�ƃ��X�g�𐶐����܂��B
        /// </summary>
        public void LinkBones(TSONode[] nodes)
        {
            this.bones = new List<TSONode>();

            foreach (int bone_index in bone_indices)
                this.bones.Add(nodes[bone_index]);
        }

        /// ���_��
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
        /// ���_��Direct3D�o�b�t�@�ɏ������݂܂��B
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
        /// Direct3D�T�u���b�V����j�����܂��B
        /// </summary>
        public void Dispose()
        {
            if (dm != null)
                dm.Dispose();
        }
    }

    /// <summary>
    /// ���b�V��
    /// </summary>
    public class TSOMesh : IDisposable
    {
        string name;
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// �ό`�s��
        /// </summary>
        public Matrix transform_matrix;
        /// <summary>
        /// unknown1
        /// </summary>
        public UInt32 unknown1;
        /// <summary>
        /// �T�u���b�V���z��
        /// </summary>
        public TSOSubMesh[] sub_meshes;

        /// <summary>
        /// ���b�V����ǂݍ��݂܂��B
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
        /// ���b�V���������o���܂��B
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
        /// �{�[���Q�ƃ��X�g�𐶐����܂��B
        /// </summary>
        public void LinkBones(TSONode[] nodes)
        {
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sub_mesh.LinkBones(nodes);
        }

        /// ���_���̍��v�𓾂܂��B
        public int SumVerticesCount()
        {
            int sum = 0;
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sum += sub_mesh.VerticesCount;
            return sum;
        }

        /// <summary>
        /// ����object��j�����܂��B
        /// </summary>
        public void Dispose()
        {
            if (sub_meshes != null)
            foreach (TSOSubMesh sub_mesh in sub_meshes)
                sub_mesh.Dispose();
        }
    }

    /// <summary>
    /// ���_
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// �ʒu
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// �@��
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// �e�N�X�`��U���W
        /// </summary>
        public Single u;
        /// <summary>
        /// �e�N�X�`��V���W
        /// </summary>
        public Single v;
        /// <summary>
        /// �X�L���E�F�C�g�z��
        /// </summary>
        public SkinWeight[] skin_weights;
        /// <summary>
        /// �{�[���C���f�b�N�X
        /// </summary>
        public UInt32 bone_indices;

        /// <summary>
        /// ���_��ǂ݂Ƃ�܂��B
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
        /// �X�L���E�F�C�g�z����[�U���܂��B
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
        /// �{�[���C���f�b�N�X�𐶐����܂��B
        /// </summary>
        public void GenerateBoneIndices()
        {
            byte[] idx = new byte[4];
            for (int i = 0; i < 4; i++)
                idx[i] = (byte)this.skin_weights[i].bone_index;

            this.bone_indices = BitConverter.ToUInt32(idx, 0);
        }

        /// <summary>
        /// ���_�������o���܂��B
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
    /// �X�L���E�F�C�g
    /// </summary>
    public class SkinWeight : IComparable
    {
        /// <summary>
        /// �{�[���C���f�b�N�X
        /// </summary>
        public int bone_index;

        /// <summary>
        /// �E�F�C�g
        /// </summary>
        public float weight;

        /// <summary>
        /// �X�L���E�F�C�g�𐶐����܂��B
        /// </summary>
        /// <param name="bone_index">�{�[���C���f�b�N�X</param>
        /// <param name="weight">�E�F�C�g</param>
        public SkinWeight(int bone_index, float weight)
        {
            this.bone_index = bone_index;
            this.weight = weight;
        }

        /// <summary>
        /// ��r�֐�
        /// </summary>
        /// <param name="obj">��r�ΏۃX�L���E�F�C�g</param>
        /// <returns>��r����</returns>
        public int CompareTo(object obj)
        {
            return -weight.CompareTo(((SkinWeight)obj).weight);
        }
    }

    /// <summary>
    /// �X�N���v�g
    /// </summary>
    public class TSOScript
    {
        /// <summary>
        /// ����
        /// </summary>
        public string name;
        /// <summary>
        /// �e�L�X�g�s�z��
        /// </summary>
        public string[] lines;

        /// <summary>
        /// �X�N���v�g��ǂݍ��݂܂��B
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
        /// �X�N���v�g�������o���܂��B
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
    /// �T�u�X�N���v�g
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TSOSubScript
    {
        internal string name;
        internal string file;
        /// <summary>
        /// �e�L�X�g�s�z��
        /// </summary>
        public string[] lines;
        /// <summary>
        /// �V�F�[�_�ݒ�
        /// </summary>
        public Shader shader = null;

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// �t�@�C����
        /// </summary>
        public string File { get { return file; } set { file = value; } }

        /// <summary>
        /// �T�u�X�N���v�g��ǂݍ��݂܂��B
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
        /// �T�u�X�N���v�g�������o���܂��B
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
        /// �V�F�[�_�ݒ�𐶐����܂��B
        /// </summary>
        public void GenerateShader()
        {
            this.shader = new Shader();
            this.shader.Load(this.lines);
        }
    }

    /// <summary>
    /// �e�N�X�`��
    /// </summary>
    public class TSOTex : IDisposable
    {
        /// <summary>
        /// ����
        /// </summary>
        public string name;
        /// <summary>
        /// �t�@�C����
        /// </summary>
        public string file;
        /// <summary>
        /// ��
        /// </summary>
        public int width;
        /// <summary>
        /// ����
        /// </summary>
        public int height;
        /// <summary>
        /// �F�[�x
        /// </summary>
        public int depth;
        /// <summary>
        /// �r�b�g�}�b�v�z��
        /// </summary>
        public byte[] data;

        internal Texture tex;

        /// <summary>
        /// �e�N�X�`����ǂݍ��݂܂��B
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
        /// �e�N�X�`���������o���܂��B
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
        /// �w��device�ŊJ���܂��B
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
        /// Direct3D�e�N�X�`����j�����܂��B
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
        /// TSONode�𐶐����܂��B
        /// </summary>
        public TSONode(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// TSONode��ǂݍ��݂܂��B
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.Path = reader.ReadCString();
        }

        /// <summary>
        /// TSONode�������o���܂��B
        /// </summary>
        public void Write(BinaryWriter bw)
        {
            bw.WriteCString(this.Path);
        }

        /// <summary>
        /// ��]�ψ�
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
        /// �ʒu�ψ�
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
        /// �qnode���X�g
        /// </summary>
        public List<TSONode> children = new List<TSONode>();

        /// <summary>
        /// �enode
        /// </summary>
        public TSONode parent;

        /// <summary>
        /// �I�t�Z�b�g�s��B����̓��[���h���W�n��bone���[�J�����W�n�ɕϊ����܂��B
        /// </summary>
        public Matrix offset_matrix;

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�ƌ�����\���܂��B�����viewer����X�V����܂��B
        /// </summary>
        public Matrix combined_matrix;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get { return id; } }
        
        /// <summary>
        /// ����
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
        /// ���̂̒Z���`���B�����TSOFile���ŏd������\��������܂��B
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// �w��node�ɑ΂���I�t�Z�b�g�s����v�Z���܂��B
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
        /// �I�t�Z�b�g�s����v�Z���܂��B
        /// </summary>
        public void ComputeOffsetMatrix()
        {
            offset_matrix = TSONode.GetOffsetMatrix(this);
        }

        /// <summary>
        /// �I�t�Z�b�g�s��𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Matrix OffsetMatrix
        {
            get { return offset_matrix; }
        }

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�𓾂܂��B
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
        /// ���[���h���W�n�ł̈ʒu�ƌ����𓾂܂��B
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
        /// ��]�s��
        /// </summary>
        public Matrix RotationMatrix
        {
            get {
                return Matrix.RotationQuaternion(rotation);
            }
        }

        /// <summary>
        /// �ʒu�s��
        /// </summary>
        public Matrix TranslationMatrix
        {
            get {
                return Matrix.Translation(translation);
            }
        }

        /// <summary>
        /// �ό`�s��B����� ��]�s�� x �ʒu�s�� �ł��B
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
    /// TSO�t�@�C���������܂��B
    /// </summary>
    public class TSOFile : IDisposable
    {
        /// <summary>
        /// �o�C�i���l�Ƃ��ēǂݎ��܂��B
        /// </summary>
        protected BinaryReader reader;

        /// <summary>
        /// bone�z��
        /// </summary>
        public TSONode[] nodes;
        /// <summary>
        /// �e�N�X�`���z��
        /// </summary>
        public TSOTex[] textures;
        /// <summary>
        /// �X�N���v�g�z��
        /// </summary>
        public TSOScript[] scripts;
        /// <summary>
        /// �T�u�X�N���v�g�z��
        /// </summary>
        public TSOSubScript[] sub_scripts;
        /// <summary>
        /// ���b�V���z��
        /// </summary>
        public TSOMesh[] meshes;

        internal Dictionary<string, TSONode> nodemap;

        /// <summary>
        /// �w��p�X�ɕۑ����܂��B
        /// </summary>
        /// <param name="dest_file">�p�X</param>
        public void Save(string dest_file)
        {
            using (Stream dest_stream = File.Create(dest_file))
                Save(dest_stream);
        }

        /// <summary>
        /// �w��X�g���[���ɕۑ����܂��B
        /// </summary>
        /// <param name="dest_stream">�X�g���[��</param>
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
        /// 'TSO1' �������o���܂��B
        /// </summary>
        public static void WriteMagic(BinaryWriter bw)
        {
            bw.Write(0x314F5354);
        }

        /// <summary>
        /// �w��p�X����ǂݍ��݂܂��B
        /// </summary>
        /// <param name="source_file">�p�X</param>
        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

        /// <summary>
        /// �w��X�g���[������ǂݍ��݂܂��B
        /// </summary>
        /// <param name="source_stream">�X�g���[��</param>
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
        /// tmo�𐶐����܂��B
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

        /// ���_���̍��v�𓾂܂��B
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
        /// �w��device��ŊJ���܂��B
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
        /// ���������x�N�g���𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Vector4 LightDirForced()
        {
            return new Vector4(lightDir.X, lightDir.Y, lightDir.Z, 0.0f);
        }

        /// <summary>
        /// UVSCR�l�𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Vector4 UVSCR()
        {
            float x = Environment.TickCount * 0.000002f;
            return new Vector4(x, 0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// �����_�����O�J�n���ɌĂт܂��B
        /// </summary>
        public void BeginRender()
        {
            current_shader = null;
        }

        /// <summary>
        /// �V�F�[�_�ݒ��؂�ւ��܂��B
        /// </summary>
        /// <param name="shader">�V�F�[�_�ݒ�</param>
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
        /// �V�F�[�_�ݒ��؂�ւ��܂��B
        /// </summary>
        /// <param name="sub_mesh">�؂�ւ��ΏۂƂȂ�T�u���b�V��</param>
        public void SwitchShader(TSOSubMesh sub_mesh)
        {
            Debug.Assert(sub_mesh.spec >= 0 && sub_mesh.spec < sub_scripts.Length, string.Format("mesh.spec out of range: {0}", sub_mesh.spec));
            SwitchShader(sub_scripts[sub_mesh.spec].shader);
        }

        /// <summary>
        /// �����_�����O�I�����ɌĂт܂��B
        /// </summary>
        public void EndRender()
        {
            current_shader = null;
        }

        /// <summary>
        /// ����object��j�����܂��B
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
