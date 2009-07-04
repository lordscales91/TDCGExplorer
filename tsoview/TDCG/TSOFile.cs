using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    public class TSOSubMesh : IDisposable
    {
        public string name;
        public int spec;
        public List<UInt32> bone_index_LUT;
        public List<TSONode> bone_LUT;
        public vertex_field[] vertices;

        internal Mesh dm = null;

        public int maxPalettes;
        public int boneID_OFFSET;

        public int NumberBones
        {
            get { return bone_index_LUT.Count; }
        }

        public TSONode GetBone(int i)
        {
            return bone_LUT[i];
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
                        vertex_field tv = vertices[i];

                        gs.Write(tv.position);
                        for (int j = 0; j < 4; j++)
                            gs.Write(tv.skin_weights[j].weight);
                        gs.Write(tv.skin_weight_indices);
                        gs.Write(tv.normal);
                        gs.Write(tv.u);
                        gs.Write(tv.v);
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

        public void Dispose()
        {
            if (dm != null)
                dm.Dispose();
        }
    }

    public class TSOMesh : IDisposable
    {
        public string name;
        public Matrix transform_matrix;
        public UInt32 unknown1;
        public UInt32 sub_mesh_count;
        public TSOSubMesh[] sub_meshes;

        public void Dispose()
        {
            if (sub_meshes != null)
            foreach (TSOSubMesh tm_sub in sub_meshes)
                tm_sub.Dispose();
        }
    }

    public struct vertex_field
    {
        public Vector3 position;
        public Vector3 normal;
        public Single u;
        public Single v;
        public SkinWeight[] skin_weights;
        public UInt32 skin_weight_indices;
    }

    public class SkinWeight : IComparable
    {
        public Single weight;
        public UInt32 index;
        public SkinWeight(Single weight, UInt32 index)
        {
            this.weight = weight;
            this.index = index;
        }
        public int CompareTo(object obj)
        {
            return -weight.CompareTo(((SkinWeight)obj).weight);
        }
    }

    public class TSOScript
    {
        public string name;
        public string[] script_data;
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TSOSubScript
    {
        internal string name;
        internal string file;
        public string[] script_data;
        internal Shader shader = null;

        public string Name { get { return name; } set { name = value; } }
        public string File { get { return file; } set { file = value; } }
    }

    public class TSOTex : IDisposable
    {
        public string name;
        public string file;
        public int width;
        public int height;
        public int depth;
        public byte[] data;

        internal Texture tex;

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

        public void Dispose()
        {
            if (tex != null)
                tex.Dispose();
        }
    }

    public class TSONode
    {
        internal int id;
        internal string name;
        internal string sname;
        public Matrix transformation_matrix;
        internal List<TSONode> child_nodes = new List<TSONode>();
        public TSONode parent;
        public Matrix offset_matrix;
        public Matrix combined_matrix;

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public string ShortName { get { return sname; } }

        public static Matrix GetBoneOffsetMatrix(TSONode bone)
        {
            Matrix mat = Matrix.Identity;
            while (bone != null)
            {
                mat.Multiply(bone.transformation_matrix);
                bone = bone.parent;
            }
            return Matrix.Invert(mat);
        }
        public void ComputeOffsetMatrix()
        {
            offset_matrix = TSONode.GetBoneOffsetMatrix(this);
        }
        public Matrix GetOffsetMatrix()
        {
            return offset_matrix;
        }
    }

    public class TSOFile : IDisposable
    {
        protected BinaryReader reader;

        public TSONode[] nodes;
        public TSOTex[] textures;
        public TSOScript[] scripts;
        public TSOSubScript[] sub_scripts;
        public TSOMesh[] meshes;

        internal Dictionary<string, TSONode> nodemap;

        public string ReadString()
        {
            StringBuilder string_builder = new StringBuilder();
            while ( true ) {
                char c = reader.ReadChar();
                if (c == 0) break;
                string_builder.Append(c);
            }
            return string_builder.ToString();
        }

        public TSOMesh read_mesh()
        {
            TSOMesh mesh = new TSOMesh();

            mesh.name = ReadString();
            mesh.name = mesh.name.Replace(":", "_colon_").Replace("#", "_sharp_"); //should be compatible with directx naming conventions 
            mesh.transform_matrix = ReadMatrix();
            mesh.unknown1 = reader.ReadUInt32();
            mesh.sub_mesh_count = reader.ReadUInt32();
            mesh.sub_meshes = new TSOSubMesh[mesh.sub_mesh_count];

            for (int a = 0; a < mesh.sub_mesh_count; a++)
            {
                TSOSubMesh act_mesh = new TSOSubMesh();
                mesh.sub_meshes[a] = act_mesh;

                act_mesh.name = mesh.name + "_sub_" + a.ToString();

                act_mesh.spec = reader.ReadInt32();
                int bone_index_LUT_entry_count = reader.ReadInt32(); //numbones
                act_mesh.maxPalettes = 16;
                if (act_mesh.maxPalettes > bone_index_LUT_entry_count)
                    act_mesh.maxPalettes = bone_index_LUT_entry_count;
                act_mesh.bone_index_LUT = new List<UInt32>();
                act_mesh.bone_LUT = new List<TSONode>();
                for (int i = 0; i < bone_index_LUT_entry_count; i++)
                {
                    act_mesh.bone_index_LUT.Add(reader.ReadUInt32());
                }
                int vertex_count = reader.ReadInt32(); //numvertices
                act_mesh.vertices = new vertex_field[vertex_count];
                for (int i = 0; i < vertex_count; i++)
                {
                    ReadVertex(ref act_mesh.vertices[i]);
                }
            }

            for (int a = 0; a < mesh.sub_mesh_count; a++)
            {
                TSOSubMesh sub_mesh = mesh.sub_meshes[a];

                for (int i = 0; i < sub_mesh.bone_index_LUT.Count; i++)
                {
                    UInt32 bone_index = sub_mesh.bone_index_LUT[i];
                    TSONode bone = nodes[bone_index];
                    sub_mesh.bone_LUT.Add(bone);
                }
            }

            return mesh;
        }

        public Matrix ReadMatrix()
        {
            Matrix m;

            m.M11 = reader.ReadSingle();
            m.M12 = reader.ReadSingle();
            m.M13 = reader.ReadSingle();
            m.M14 = reader.ReadSingle();

            m.M21 = reader.ReadSingle();
            m.M22 = reader.ReadSingle();
            m.M23 = reader.ReadSingle();
            m.M24 = reader.ReadSingle();

            m.M31 = reader.ReadSingle();
            m.M32 = reader.ReadSingle();
            m.M33 = reader.ReadSingle();
            m.M34 = reader.ReadSingle();

            m.M41 = reader.ReadSingle();
            m.M42 = reader.ReadSingle();
            m.M43 = reader.ReadSingle();
            m.M44 = reader.ReadSingle();

            return m;
        }

        public Vector3 ReadVector3()
        {
            Vector3 v;

            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();

            return v;
        }

        public void ReadVertex(ref vertex_field v)
        {
            v.position = ReadVector3();
            v.normal = ReadVector3();
            v.u = reader.ReadSingle();
            v.v = reader.ReadSingle();
            int bone_weight_entry_count = reader.ReadInt32();
            v.skin_weights = new SkinWeight[bone_weight_entry_count];
            for (int i = 0; i < bone_weight_entry_count; i++)
            {
                uint index = reader.ReadUInt32();
                float weight = reader.ReadSingle();
                v.skin_weights[i] = new SkinWeight(weight, index);
            }
            Array.Sort(v.skin_weights);
            Array.Resize(ref v.skin_weights, 4);
            for (int i = bone_weight_entry_count; i < 4; i++)
            {
                v.skin_weights[i] = new SkinWeight(0.0f, 0);
            }

            byte[] idx = new byte[4];
            for (int i = 0; i < 4; i++)
                idx[i] = (byte)v.skin_weights[i].index;

            v.skin_weight_indices = BitConverter.ToUInt32(idx, 0);
        }

        public void Load(string source_file)
        {
            using (Stream source_stream = File.OpenRead(source_file))
                Load(source_stream);
        }

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
            nodemap = new Dictionary<string, TSONode>();

            for (int i = 0; i < node_count; i++)
            {
                nodes[i] = new TSONode();
                nodes[i].id = i;
                nodes[i].name = ReadString();
                nodes[i].sname = nodes[i].name.Substring(nodes[i].name.LastIndexOf('|')+1);
                nodemap.Add(nodes[i].name, nodes[i]);

                //Console.WriteLine(i + ": " + nodes[i].sname);
            }

            for (int i = 0; i < node_count; i++)
            {
                int index = nodes[i].name.LastIndexOf('|');
                if (index <= 0)
                    continue;
                string pname = nodes[i].name.Substring(0, index);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.child_nodes.Add(nodes[i]);
            }

            int node_matrix_count = reader.ReadInt32();
            for (int i = 0; i < node_matrix_count; i++)
            {
                nodes[i].transformation_matrix = ReadMatrix();
            }
            for (int i = 0; i < node_matrix_count; i++)
            {
                nodes[i].ComputeOffsetMatrix();
            }

            UInt32 texture_count = reader.ReadUInt32();
            textures = new TSOTex[texture_count];
            for (int i = 0; i < texture_count; i++)
            {
                textures[i] = read_texture();
            }

            UInt32 script_count = reader.ReadUInt32();
            scripts = new TSOScript[script_count];
            for (int i = 0; i < script_count; i++)
            {
                scripts[i] = read_script();
            }

            UInt32 sub_script_count = reader.ReadUInt32();
            sub_scripts = new TSOSubScript[sub_script_count];
            for (int i = 0; i < sub_script_count; i++)
            {
                sub_scripts[i] = read_sub_script();
            }

            UInt32 mesh_count = reader.ReadUInt32();
            meshes = new TSOMesh[mesh_count];
            for (int i = 0; i < mesh_count; i++)
            {
                TSOMesh mesh = read_mesh();
                meshes[i] = mesh;

                //Console.WriteLine("mesh name {0} len {1}", mesh.name, mesh.sub_meshes.Length);
            }
        }

        public TSOScript read_script()
        {
            TSOScript script = new TSOScript();
            script.name = ReadString();
            UInt32 line_count = reader.ReadUInt32();
            string[] read_lines = new string[line_count];
            for (int i = 0; i < line_count; i++)
            {
                read_lines[i] = ReadString();
            }
            script.script_data = read_lines;

            return script;
        }

        public TSOSubScript read_sub_script()
        {
            TSOSubScript sub_script = new TSOSubScript();
            sub_script.name = ReadString();
            sub_script.file = ReadString();
            UInt32 sub_line_counts = reader.ReadUInt32();
            sub_script.script_data = new string[sub_line_counts];
            for (int j = 0; j < sub_line_counts; j++)
            {
                sub_script.script_data[j] = ReadString();
            }

            //Console.WriteLine("name {0} file {1}", sub_script.name, sub_script.file);

            sub_script.shader = new Shader();
            sub_script.shader.Load(sub_script.script_data);

            return sub_script;
        }

        public TSOTex read_texture()
        {
            TSOTex tex = new TSOTex();

            tex.name = ReadString();
            tex.file = ReadString();
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

        public void Open(Device device, Effect effect)
        {
            this.device = device;
            this.effect = effect;

            foreach (TSOMesh tm in meshes)
            foreach (TSOSubMesh tm_sub in tm.sub_meshes)
                tm_sub.LoadMesh(device);

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
        internal Vector3 lightDir = new Vector3(0.0f, 0.0f, 1.0f);

        public Vector4 LightDirForced()
        {
            return new Vector4(lightDir.X, lightDir.Y, -lightDir.Z, 0.0f);
        }

        public Vector4 UVSCR()
        {
            float x = Environment.TickCount * 0.000002f;
            return new Vector4(x, 0.0f, 0.0f, 0.0f);
        }

        public void BeginRender()
        {
            current_shader = null;
        }

        public void SwitchShader(Shader shader)
        {
            if (shader == current_shader)
                return;
            current_shader = shader;

            foreach (ShaderParameter p in shader.shader_parameters)
            {
                switch (p.type)
                {
                case ShaderParameter.Type.String:
                    effect.SetValue(p.name, p.GetString());
                    break;
                case ShaderParameter.Type.Float:
                    effect.SetValue(p.name, p.GetFloat());
                    break;
                case ShaderParameter.Type.Float4:
                    effect.SetValue(p.name, p.GetFloat4());
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

        public void SwitchShader(TSOSubMesh tm_sub)
        {
            SwitchShader(sub_scripts[tm_sub.spec].shader);
        }

        public void EndRender()
        {
            current_shader = null;
        }

        public void Dispose()
        {
            foreach (TSOMesh tm in meshes)
                tm.Dispose();
            foreach (TSOTex tex in textures)
                tex.Dispose();
        }
    }
}
