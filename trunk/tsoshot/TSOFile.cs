using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{
    public class TSOMesh : IDisposable
    {
        public string name;
        public Matrix transform_matrix;
        public UInt32 unknown1;
        public UInt32 sub_mesh_count;
        public int spec;
        public List<UInt32> bone_index_LUT; //to look up bone field entries... bones are not directly assigned to vertices but by the means of this bone index LUT (look up table)... so if there is e.g. a bone field entry with the value 1, this means to look up in the LUT the first entry to retrieve the actual bone index...
        public vertex_field[] vertices;
        public TSOMesh[] sub_meshes;
        public List<TSONode> bone_LUT;

        public Mesh dm = null;

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

        public void Dispose()
        {
            if (sub_meshes != null)
            foreach (TSOMesh tm_sub in sub_meshes)
                tm_sub.Dispose();
            if (dm != null)
                dm.Dispose();
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
        public string file;
        public string[] script_data;
        public TSOScript[] sub_scripts;

        public Shader shader = null;
    }

    public class TSOTex : IDisposable
    {
        public string name;
        public string file;
        public int width;
        public int height;
        public int depth;
        public byte[] data;

        public Texture tex;

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

                bw.Write(data, 0, data.Length);
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
        private Matrix offset_matrix;
        internal int slow = 0;

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

        public void SlowChildren()
        {
            SlowChildren(3);
        }
        public void SlowChildren(int slow)
        {
            this.slow = slow;
            foreach (TSONode child_node in child_nodes)
                child_node.SlowChildren(slow);
        }
    }

    public class TSOFile : IDisposable
    {
        protected BinaryReader reader;

        public TSONode[] nodes;
        public TSOTex[] textures;
        public TSOScript[] scripts;
        public TSOMesh[] meshes;

        internal string source_file;
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
            mesh.sub_meshes = new TSOMesh[mesh.sub_mesh_count];

            mesh.spec = 0;
            mesh.bone_index_LUT = new List<UInt32>();
            mesh.bone_LUT = new List<TSONode>();

            mesh.vertices = new vertex_field[0];

            for (int a = 0; a < mesh.sub_mesh_count; a++)
            {
                TSOMesh act_mesh = new TSOMesh();
                mesh.sub_meshes[a] = act_mesh;

                act_mesh.name = mesh.name + "_sub_" + a.ToString();
                act_mesh.transform_matrix = mesh.transform_matrix;
                act_mesh.unknown1 = mesh.unknown1;
                act_mesh.sub_mesh_count = 0;

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
                TSOMesh sub_mesh = mesh.sub_meshes[a];

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

        public TSONode[] flat_bones()
        {
            return nodes;
        }

        public int Load(string source_file)
        {
            this.source_file = source_file;

            Load(File.OpenRead(source_file));
            return 0;
        }

        public int Load(Stream source_stream)
        {
            try
            {
                reader = new BinaryReader(source_stream, System.Text.Encoding.Default);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: This file cannot be read or does not exist.");
                return -1;
            }

            byte[] file_header = new byte[4];
            file_header = reader.ReadBytes(4);

            if (! System.Text.Encoding.ASCII.GetString(file_header).Contains("TSO1"))
            {
                Console.WriteLine("Error: This seems not to be a TSO file.");
            }

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

            {
                TSONode node;
                if (nodemap.TryGetValue("|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|Chichi_Right1", out node))
                    node.SlowChildren();
                if (nodemap.TryGetValue("|W_Hips|W_Spine_Dummy|W_Spine1|W_Spine2|W_Spine3|Chichi_Left1", out node))
                    node.SlowChildren();
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
            TSOScript[] sub_scripts = new TSOScript[sub_script_count];
            for (int i = 0; i < sub_script_count; i++)
            {
                sub_scripts[i] = read_sub_script();
            }
            scripts[0].sub_scripts = sub_scripts;

            UInt32 mesh_count = reader.ReadUInt32();
            meshes = new TSOMesh[mesh_count];
            for (int i = 0; i < mesh_count; i++)
            {
                TSOMesh mesh = read_mesh();
                meshes[i] = mesh;

                //Console.WriteLine("mesh name {0} len {1}", mesh.name, mesh.sub_meshes.Length);
            }
            reader.Close();
            return 0;
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

        public TSOScript read_sub_script()
        {
            TSOScript sub_script = new TSOScript();
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

    public void LoadMesh()
    {
        foreach (TSOMesh tm in meshes)
        foreach (TSOMesh tm_sub in tm.sub_meshes)
        {
            int numVertices = tm_sub.vertices.Length;
            int numFaces = numVertices - 2;

            tm_sub.dm = new Mesh(numFaces, numVertices, MeshFlags.Managed | MeshFlags.WriteOnly, ve, device);

            //
            // rewrite vertex buffer
            //
            {
                GraphicsStream gs = tm_sub.dm.LockVertexBuffer(LockFlags.None);
                {
                    for (int i = 0; i < tm_sub.vertices.Length; i++)
                    {
                        vertex_field tv = tm_sub.vertices[i];

                        gs.Write(tv.position);
                        for (int j = 0; j < 4; j++)
                            gs.Write(tv.skin_weights[j].weight);
                        gs.Write(tv.skin_weight_indices);
                        gs.Write(tv.normal);
                        gs.Write(tv.u);
                        gs.Write(1-tv.v);
                    }
                }
                tm_sub.dm.UnlockVertexBuffer();
            }

            //
            // rewrite index buffer
            //
            {
                GraphicsStream gs = tm_sub.dm.LockIndexBuffer(LockFlags.None);
                {
                    for (int i = 2; i < tm_sub.vertices.Length; i++)
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
                tm_sub.dm.UnlockIndexBuffer();
            }

            //
            // rewrite attribute buffer
            //
            {
                tm_sub.dm.SetAttributeTable(new AttributeRange[] { ar }); 
            }
        }
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

            LoadMesh();

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

        public void SwitchShader(TSOMesh tm_sub)
        {
            SwitchShader(scripts[0].sub_scripts[tm_sub.spec].shader);
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
