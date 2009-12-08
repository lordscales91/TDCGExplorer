using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG.Extensions;

namespace TDCG
{
    /// <summary>
    /// ���b�V��
    /// </summary>
    public class TSOMesh : IDisposable
    {
        /*
        /// <summary>
        /// ���O
        /// </summary>
        public string name;
        */
        /// <summary>
        /// �V�F�[�_�ݒ�ԍ�
        /// </summary>
        public int spec;
        /// <summary>
        /// �{�[���Q�ƃ��X�g
        /// </summary>
        public List<UInt32> bone_index_LUT;
        /// <summary>
        /// �{�[���Q�ƃ��X�g
        /// </summary>
        public List<TSONode> bone_LUT;
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
            get { return bone_index_LUT.Count; }
        }

        /// <summary>
        /// �w��index�ɂ���{�[���𓾂܂��B
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>�{�[��</returns>
        public TSONode GetBone(int i)
        {
            return bone_LUT[i];
        }

        /// <summary>
        /// ���b�V����ǂݍ��݂܂��B
        /// </summary>
        public void Read(BinaryReader reader)
        {
            this.spec = reader.ReadInt32();
            int bone_index_LUT_entry_count = reader.ReadInt32(); //numbones
            this.maxPalettes = 16;
            if (this.maxPalettes > bone_index_LUT_entry_count)
                this.maxPalettes = bone_index_LUT_entry_count;
            this.bone_index_LUT = new List<UInt32>();
            this.bone_LUT = new List<TSONode>();
            for (int i = 0; i < bone_index_LUT_entry_count; i++)
            {
                this.bone_index_LUT.Add(reader.ReadUInt32());
            }
            int vertex_count = reader.ReadInt32(); //numvertices
            this.vertices = new Vertex[vertex_count];
            for (int i = 0; i < vertex_count; i++)
            {
                this.vertices[i].Read(reader);
            }
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
        public void LoadMesh(Device device)
        {
            int numVertices = vertices.Length;
            int numFaces = numVertices - 2;

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
                        gs.Write(v.skin_weight_indices);
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
        /// Direct3D���b�V����j�����܂��B
        /// </summary>
        public void Dispose()
        {
            if (dm != null)
                dm.Dispose();
        }
    }

    /// <summary>
    /// �t���[��
    /// </summary>
    public class TSOFrame : IDisposable
    {
        /// <summary>
        /// ����
        /// </summary>
        public string name;
        /// <summary>
        /// �ό`�s��
        /// </summary>
        public Matrix transform_matrix;
        /// <summary>
        /// unknown1
        /// </summary>
        public UInt32 unknown1;
        //public UInt32 sub_mesh_count;
        /// <summary>
        /// ���b�V���z��
        /// </summary>
        public TSOMesh[] meshes;

        /// <summary>
        /// ����object��j�����܂��B
        /// </summary>
        public void Dispose()
        {
            if (meshes != null)
            foreach (TSOMesh tm_sub in meshes)
                tm_sub.Dispose();
        }
    }

    /// <summary>
    /// ���_
    /// </summary>
    public struct Vertex
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
        public UInt32 skin_weight_indices;

        /// <summary>
        /// ���_��ǂ݂Ƃ�܂��B
        /// </summary>
        public void Read(BinaryReader reader)
        {
            reader.ReadVector3(ref this.position);
            reader.ReadVector3(ref this.normal);
            this.u = reader.ReadSingle();
            this.v = reader.ReadSingle();
            int bone_weight_entry_count = reader.ReadInt32();
            this.skin_weights = new SkinWeight[bone_weight_entry_count];
            for (int i = 0; i < bone_weight_entry_count; i++)
            {
                uint index = reader.ReadUInt32();
                float weight = reader.ReadSingle();
                this.skin_weights[i] = new SkinWeight(weight, index);
            }
            Array.Sort(this.skin_weights);
            Array.Resize(ref this.skin_weights, 4);
            for (int i = bone_weight_entry_count; i < 4; i++)
            {
                this.skin_weights[i] = new SkinWeight(0.0f, 0);
            }

            byte[] idx = new byte[4];
            for (int i = 0; i < 4; i++)
                idx[i] = (byte)this.skin_weights[i].index;

            this.skin_weight_indices = BitConverter.ToUInt32(idx, 0);
        }
    }

    /// <summary>
    /// �X�L���E�F�C�g
    /// </summary>
    public class SkinWeight : IComparable
    {
        /// <summary>
        /// �E�F�C�g
        /// </summary>
        public Single weight;
        /// <summary>
        /// �{�[���C���f�b�N�X
        /// </summary>
        public UInt32 index;
        /// <summary>
        /// �X�L���E�F�C�g�𐶐����܂��B
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="index"></param>
        public SkinWeight(Single weight, UInt32 index)
        {
            this.weight = weight;
            this.index = index;
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
        public string[] script_data;
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
        public string[] script_data;
        internal Shader shader = null;

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// �t�@�C����
        /// </summary>
        public string File { get { return file; } set { file = value; } }
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
        private string name;
        private string sname;

        private Quaternion rotation;
        private Vector3 translation;

        private Matrix transformation_matrix;
        private bool need_update_transformation;

        /// <summary>
        /// TSONode�𐶐����܂��B
        /// </summary>
        public TSONode(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.sname = this.name.Substring(this.name.LastIndexOf('|') + 1);
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
        public List<TSONode> child_nodes = new List<TSONode>();

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
        public string Name { get { return name; } }

        /// <summary>
        /// ���̂̒Z���`���B�����TSOFile���ŏd������\��������܂��B
        /// </summary>
        public string ShortName { get { return sname; } }

        /// <summary>
        /// �w��bone�ɑ΂���I�t�Z�b�g�s����v�Z���܂��B
        /// </summary>
        /// <param name="bone">bone</param>
        public static Matrix GetBoneOffsetMatrix(TSONode bone)
        {
            Matrix m = Matrix.Identity;
            while (bone != null)
            {
                m.Multiply(bone.TransformationMatrix);
                bone = bone.parent;
            }
            return Matrix.Invert(m);
        }

        /// <summary>
        /// �I�t�Z�b�g�s����v�Z���܂��B
        /// </summary>
        public void ComputeOffsetMatrix()
        {
            offset_matrix = TSONode.GetBoneOffsetMatrix(this);
        }

        /// <summary>
        /// �I�t�Z�b�g�s��𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Matrix GetOffsetMatrix()
        {
            return offset_matrix;
        }

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Vector3 GetWorldPosition()
        {
            TSONode bone = this;
            Vector3 v = Vector3.Empty;
            while (bone != null)
            {
                v = Vector3.TransformCoordinate(v, bone.TransformationMatrix);
                bone = bone.parent;
            }
            return v;
        }

        /// <summary>
        /// ���[���h���W�n�ł̈ʒu�ƌ����𓾂܂��B
        /// </summary>
        /// <returns></returns>
        public Matrix GetWorldCoordinate()
        {
            TSONode bone = this;
            Matrix m = Matrix.Identity;
            while (bone != null)
            {
                m.Multiply(bone.TransformationMatrix);
                bone = bone.parent;
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
        /// �t���[���z��
        /// </summary>
        public TSOFrame[] frames;

        internal Dictionary<string, TSONode> nodemap;

        /// <summary>
        /// TSOFrame��ǂ݂Ƃ�܂��B
        /// </summary>
        /// <returns>TSOFrame</returns>
        public TSOFrame ReadFrame()
        {
            TSOFrame frame = new TSOFrame();

            frame.name = reader.ReadCString();
            frame.name = frame.name.Replace(":", "_colon_").Replace("#", "_sharp_"); //should be compatible with directx naming conventions 
            reader.ReadMatrix(ref frame.transform_matrix);
            frame.unknown1 = reader.ReadUInt32();
            UInt32 sub_mesh_count = reader.ReadUInt32();
            frame.meshes = new TSOMesh[sub_mesh_count];

            for (int a = 0; a < sub_mesh_count; a++)
            {
                TSOMesh mesh = new TSOMesh();
                mesh.Read(reader);
                frame.meshes[a] = mesh;
            }

            for (int a = 0; a < sub_mesh_count; a++)
            {
                TSOMesh mesh = frame.meshes[a];

                for (int i = 0; i < mesh.bone_index_LUT.Count; i++)
                {
                    UInt32 bone_index = mesh.bone_index_LUT[i];
                    TSONode bone = nodes[bone_index];
                    mesh.bone_LUT.Add(bone);
                }
            }

            return frame;
        }

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

            TSOWriter.WriteMagic(bw);
            TSOWriter.Write(bw, nodes);
            TSOWriter.Write(bw, textures);
            TSOWriter.Write(bw, scripts);
            TSOWriter.Write(bw, sub_scripts);
            TSOWriter.Write(bw, frames);
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
                string name = reader.ReadCString();
                nodes[i] = new TSONode(i, name);
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
                textures[i] = ReadTexture();
            }

            UInt32 script_count = reader.ReadUInt32();
            scripts = new TSOScript[script_count];
            for (int i = 0; i < script_count; i++)
            {
                scripts[i] = ReadScript();
            }

            UInt32 sub_script_count = reader.ReadUInt32();
            sub_scripts = new TSOSubScript[sub_script_count];
            for (int i = 0; i < sub_script_count; i++)
            {
                sub_scripts[i] = ReadSubScript();
            }

            UInt32 mesh_count = reader.ReadUInt32();
            frames = new TSOFrame[mesh_count];
            for (int i = 0; i < mesh_count; i++)
            {
                TSOFrame frame = ReadFrame();
                frames[i] = frame;

                //Console.WriteLine("frame name {0} len {1}", frame.name, frame.meshes.Length);
            }
        }

        internal void GenerateNodemapAndTree()
        {
            nodemap = new Dictionary<string, TSONode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                nodemap.Add(nodes[i].Name, nodes[i]);
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                int index = nodes[i].Name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].Name.Substring(0, index);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.child_nodes.Add(nodes[i]);
            }
        }

        /// <summary>
        /// �X�N���v�g��ǂݍ��݂܂��B
        /// </summary>
        /// <returns></returns>
        public TSOScript ReadScript()
        {
            TSOScript script = new TSOScript();
            script.name = reader.ReadCString();
            UInt32 line_count = reader.ReadUInt32();
            string[] read_lines = new string[line_count];
            for (int i = 0; i < line_count; i++)
            {
                read_lines[i] = reader.ReadCString();
            }
            script.script_data = read_lines;

            return script;
        }

        /// <summary>
        /// �T�u�X�N���v�g��ǂݍ��݂܂��B
        /// </summary>
        /// <returns></returns>
        public TSOSubScript ReadSubScript()
        {
            TSOSubScript sub_script = new TSOSubScript();
            sub_script.name = reader.ReadCString();
            sub_script.file = reader.ReadCString();
            UInt32 sub_line_counts = reader.ReadUInt32();
            sub_script.script_data = new string[sub_line_counts];
            for (int j = 0; j < sub_line_counts; j++)
            {
                sub_script.script_data[j] = reader.ReadCString();
            }

            //Console.WriteLine("name {0} file {1}", sub_script.name, sub_script.file);

            sub_script.shader = new Shader();
            sub_script.shader.Load(sub_script.script_data);

            return sub_script;
        }

        /// <summary>
        /// �e�N�X�`����ǂݍ��݂܂��B
        /// </summary>
        /// <returns></returns>
        public TSOTex ReadTexture()
        {
            TSOTex tex = new TSOTex();

            tex.name = reader.ReadCString();
            tex.file = reader.ReadCString();
            tex.width = reader.ReadInt32();
            tex.height = reader.ReadInt32();
            tex.depth = reader.ReadInt32();
            tex.data = reader.ReadBytes( tex.width * tex.height * tex.depth );

            for(int j = 0; j < tex.data.Length; j += 4)
            {
                byte tmp = tex.data[j+2];
                tex.data[j+2] = tex.data[j+0];
                tex.data[j+0] = tmp;
            }

            return tex;
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

            foreach (TSOFrame frame in frames)
            foreach (TSOMesh mesh in frame.meshes)
                mesh.LoadMesh(device);

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
        /// <param name="mesh">�؂�ւ��ΏۂƂȂ郁�b�V��</param>
        public void SwitchShader(TSOMesh mesh)
        {
            SwitchShader(sub_scripts[mesh.spec].shader);
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
            foreach (TSOFrame frame in frames)
                frame.Dispose();
            foreach (TSOTex tex in textures)
                tex.Dispose();
        }
    }
}
