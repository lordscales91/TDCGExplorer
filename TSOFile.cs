using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace tso2mqo
{
    public class TSOFile : TDCGFile
    {
        internal Dictionary<string, TSONode>    nodemap;
        internal Dictionary<string, TSOTex>     texturemap;
        internal TSONode[]          nodes;
        internal TSOTex[]           textures;
        internal TSOEffect[]        effects;
        internal TSOMaterial[]      materials;
        internal TSOMesh[]          meshes;

        public TSOFile(string file)    : base(file) {}
        public TSOFile(Stream s)       : base(s)    {}
        public TSOFile(BinaryReader r) : base(r)    {}

        public void SaveTo(string file)
        {
        }

        [System.Diagnostics.Conditional("DEBUG_DETAIL")]
        public static void WriteLine(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

        public void ReadAll()
        {
            byte[]  magic                   = r.ReadBytes(4);

            if(magic[0] != (byte)'T'
            || magic[1] != (byte)'S'
            || magic[2] != (byte)'O'
            || magic[3] != (byte)'1')
                throw new Exception("File is not TSO");

            //----- ノード -------------------------------------------------
            nodemap                         = new Dictionary<string, TSONode>();
            int     count                   = r.ReadInt32();
            nodes                           = new TSONode[count];

            for(int i= 0; i < count; ++i)
            {
                nodes[i]                    = new TSONode();
                nodes[i].id                 = i;
                nodes[i].name               = ReadString();
                nodes[i].sname              = nodes[i].name.Substring(nodes[i].name.LastIndexOf('|')+1);
                nodemap.Add(nodes[i].name, nodes[i]);

                WriteLine(i+ ": " + nodes[i].name);
            }

            for(int i= 0; i < count; ++i)
            {
                int     index   = nodes[i].name.LastIndexOf('|');

                if(index <= 0)
                    continue;

                string  pname   = nodes[i].name.Substring(0, index);
                WriteLine(pname);
                nodes[i].parent = nodemap[pname];
                nodes[i].parent.children.Add(nodes[i]);
            }

            WriteLine(r.BaseStream.Position.ToString("X"));

            count                           = r.ReadInt32();

            // Node Matrix
            for(int i= 0; i < count; ++i)
            {
                nodes[i].matrix             = ReadMatrix();
            }

            WriteLine(r.BaseStream.Position.ToString("X"));

            //----- テクスチャ ---------------------------------------------
            count                           = r.ReadInt32();
            textures                        = new TSOTex[count];
            texturemap                      = new Dictionary<string, TSOTex>();

            for(int i= 0; i < count; ++i)
            {
                textures[i]                 = new TSOTex();
                textures[i].id              = i;
                textures[i].name            = ReadString();
                textures[i].file            = ReadString();
                textures[i].width           = r.ReadInt32();
                textures[i].height          = r.ReadInt32();
                textures[i].depth           = r.ReadInt32();
                textures[i].data            = r.ReadBytes(textures[i].width * textures[i].height * textures[i].depth);
                texturemap.Add(textures[i].name, textures[i]);

                for(int j= 0; j < textures[i].data.Length; j+=4)
                {
                    byte    tmp          = textures[i].data[j+2];
                    textures[i].data[j+2]= textures[i].data[j+0];
                    textures[i].data[j+0]= tmp;
                }

                WriteLine(r.BaseStream.Position.ToString("X"));
            }

            //----- エフェクト ---------------------------------------------
            count                   = r.ReadInt32();
            effects                 = new TSOEffect[count];

            for(int i= 0; i < count; ++i)
            {
                StringBuilder   sb          = new StringBuilder();
                effects[i]                  = new TSOEffect();
                effects[i].name             = ReadString();
                effects[i].line             = r.ReadInt32();

                for(int j= 0; j < effects[i].line; ++j)
                    sb.Append(ReadString()).Append('\n');

                effects[i].code             = sb.ToString();

                WriteLine(r.BaseStream.Position.ToString("X"));
            }

            //----- マテリアル ---------------------------------------------
            count                           = r.ReadInt32();
            materials                       = new TSOMaterial[count];

            for(int i= 0; i < count; ++i)
            {
                StringBuilder   sb          = new StringBuilder();
                materials[i]                = new TSOMaterial();
                materials[i].id             = i;
                materials[i].name           = ReadString();
                materials[i].file           = ReadString();
                materials[i].line           = r.ReadInt32();

                for(int j= 0; j < materials[i].line; ++j)
                    sb.Append(ReadString()).Append('\n');

                materials[i].code           = sb.ToString();
                materials[i].ParseParameters();

                WriteLine(r.BaseStream.Position.ToString("X"));
            }

            //----- メッシュ -----------------------------------------------
            count                           = r.ReadInt32();
            meshes                          = new TSOMesh[count];
            int             check           = 0;
#if true
            bool    debug   = false;
#endif
            for(int i= 0; i < count; ++i)
            {
                meshes[i]                   = new TSOMesh();
                meshes[i].file              = this;
                meshes[i].name              = ReadString();
                meshes[i].matrix            = ReadMatrix();
                meshes[i].effect            = r.ReadInt32();
                meshes[i].numsubs           = r.ReadInt32();
                meshes[i].sub               = new TSOSubMesh[meshes[i].numsubs];

                for(int j= 0; j < meshes[i].numsubs; ++j)
                {
                    meshes[i].sub[j]            = new TSOSubMesh();
                    meshes[i].sub[j].owner      = meshes[i];
                    meshes[i].sub[j].spec       = r.ReadInt32();
                    meshes[i].sub[j].numbones   = r.ReadInt32();
                    meshes[i].sub[j].bones      = new int[meshes[i].sub[j].numbones];

                    meshes[i].sub[j].ink        = materials[meshes[i].sub[j].spec].technique.ToUpper().IndexOf("INKOFF") < 0;
                  //meshes[i].sub[j].shadow     = specs[meshes[i].sub[j].spec].technique.ToUpper().IndexOf(Shadow

                    for(int k= 0; k < meshes[i].sub[j].numbones; ++k)
                        meshes[i].sub[j].bones[k]   = r.ReadInt32();

                    meshes[i].sub[j].numvertices= r.ReadInt32();
                    Vertex[]    v               =  new Vertex[meshes[i].sub[j].numvertices];
                    meshes[i].sub[j].vertices= v;

                    for(int k= 0; k < meshes[i].sub[j].numvertices; ++k)
                    {
                        if(debug)
                        {
                            WriteLine(r.BaseStream.Position.ToString("X"));
                            ReadVertexDebug(ref v[k]);
                        } else
                        {
                            ReadVertex(ref v[k]);
                        }
                    }

                    WriteLine(r.BaseStream.Position.ToString("X"));
                    System.Diagnostics.Debug.WriteLine(r.BaseStream.Position.ToString("X"));
#if DEBUG
                    if(r.BaseStream.Position == 0x61F94)
                        debug   = true;
#endif
                }
            }

            WriteLine(r.BaseStream.Position.ToString("X"));
            WriteLine(check.ToString("X"));

            r.BaseStream.Dispose();
        }
    }

    public class TSONode
    {
        internal int            id;
        internal string         name;
        internal string         sname;
        internal Matrix matrix;
        internal Matrix world;
        internal List<TSONode>  children    = new List<TSONode>();
        internal TSONode        parent;

        [Category("General")] public int      ID        { get { return id; } }
        [Category("General")] public string   Name      { get { return name; } }
        [Category("General")] public string   ShortName { get { return sname; } }
        [Category("Detail")]
        public Matrix Matrix { get { return matrix; } set { matrix = value; } }
        [Category("Detail")]
        public Matrix World { get { return world; } set { world = value; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Name:           ").AppendLine(name);
            sb.Append("Matrix:         ").AppendLine(matrix.ToString());
            sb.Append("Children.Count: ").AppendLine(children.Count.ToString());
            return sb.ToString();
        }
    }

    public class TSOTex
    {
        internal int            id;
        internal string         name;
        internal string         file;
        internal int            width;
        internal int            height;
        internal int            depth;
        internal byte[]         data;

        [Category("General")] public int      ID      { get { return id; } }
        [Category("General")] public string   Name    { get { return name; } }
        [Category("Detail")]  public string   File    { get { return file; } set { file= value; } }
        [Category("Detail")]  public int      Width   { get { return width; } }
        [Category("Detail")]  public int      Height  { get { return height; } }
        [Category("Detail")]  public int      Depth   { get { return depth; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Name:        ").AppendLine(name);
            sb.Append("File:        ").AppendLine(file);
            sb.Append("Width:       ").AppendLine(width.ToString());
            sb.Append("Height:      ").AppendLine(height.ToString());
            sb.Append("Depth:       ").AppendLine(depth.ToString());
            sb.Append("Data.Length: ").AppendLine(data.Length.ToString());
            return sb.ToString();
        }
    }

    public class TSOEffect
    {
        internal string         name;
        internal int            line;
        internal string         code;

        [Category("General")] public string   Name    { get { return name; } }
        [Category("Detail")]  public string   Code    { get { return code; } set { code= value; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Name:           ").AppendLine(name);
            sb.Append("Line:           ").AppendLine(line.ToString());
            sb.AppendLine("Code:").AppendLine(code);
            return sb.ToString();
        }
    }

    public class TSOParameter
    {
        public string   Name;
        public string   Type;
        public string   Value;

        public TSOParameter(string type, string name, string value)
        {
            Name    = name;
            Type    = type;
            Value   = value;
        }

        public override string ToString()
        {
#if true
            return Type + " " + Name + " = "  + Value;
#else
            switch(Type)
            {
            case "string":  return Type + " " + Name + " = \"" + Value + "\"";
            case "float":   return Type + " " + Name + " = ["  + Value + "]";
            case "float4":  return Type + " " + Name + " = ["  + Value + "]";
            default:        return Type + " " + Name + " = "  + Value;
            }
#endif
        }
    }

    public class TSOMaterialCode : Dictionary<string, TSOParameter>
    {

        public TSOMaterialCode(string code)
            : this(code.Split('\r', '\n'))
        {
        }

	    public string GetValue(string index)
	    {
            return this[index].Value;
	    }

	    public void SetValue(string index, string value)
	    {
            TSOParameter    p   = this[index];
            p.Value             = value;
	    }
	
        public TSOMaterialCode(string[] code)
        {
            foreach(string i in code)
            {
                try
                {
                    int n1, n2;

                    if((n1= i.IndexOf(' ')) < 0)        continue;
                    if((n2= i.IndexOf('=', n1+1)) < 0)  continue;

                    TSOParameter    p   = new TSOParameter(
                        i.Substring(0, n1)    .Trim(),
                        i.Substring(n1, n2-n1).Trim(),
                        i.Substring(n2+1)     .Trim());
                    TSOFile.WriteLine(p.ToString());
                    Add(p.Name, p);
                } catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }

        public static TSOMaterialCode GenerateFromFile(string filename)
        {
            return new TSOMaterialCode(File.ReadAllLines(filename));
        }
    }

    public class TSOMaterial
    {
        internal int            id;
        internal string         name;
        internal string         file;
        internal int            line;
        internal string         code;
        internal TSOMaterialCode    codedata;

        internal string     description;     // = "TA ToonShader v0.50"
        internal string     shader;          // = "TAToonshade_050.cgfx"
        internal string     technique;       // = "ShadowOn"
        internal float      lightDirX;       // = [-0.00155681]
        internal float      lightDirY;       // = [-0.0582338]
        internal float      lightDirZ;       // = [-0.998302]
        internal float      lightDirW;       // = [0]
        internal Vector4 shadowColor;     // = [0, 0, 0, 1]
        internal string     shadeTex;        // = Ninjya_Ribbon_Toon_Tex
        internal float      highLight;       // = [0]
        internal float      colorBlend;      // = [10]
        internal float      highLightBlend;  // = [10]
        internal Vector4 penColor;        // = [0.166, 0.166, 0.166, 1]
        internal float      ambient;         // = [38]
        internal string     colorTex;        // = file24
        internal float      thickness;       // = [0.018]
        internal float      shadeBlend;      // = [10]
        internal float      highLightPower;  // = [100]

        [Category("General")]    public int         ID              { get { return id; } }
        [Category("General")]    public string      Name            { get { return name; } }
        [Category("Detail")]     public string      File            { get { return file; } }
        [Category("Detail")]     public string      Code            { get { return code; } set { code= value; } }

        [Category("Parameters")] public string      Description     { get { return description;    } set { description   = value; } }
        [Category("Parameters")] public string      Shader			{ get { return shader;         } set { shader        = value; } }
        [Category("Parameters")] public string      Technique		{ get { return technique;      } set { technique     = value; } }
        [Category("Parameters")] public float       LightDirX		{ get { return lightDirX;      } set { lightDirX     = value; } }
        [Category("Parameters")] public float       LightDirY		{ get { return lightDirY;      } set { lightDirY     = value; } }
        [Category("Parameters")] public float       LightDirZ		{ get { return lightDirZ;      } set { lightDirZ     = value; } }
        [Category("Parameters")] public float       LightDirW		{ get { return lightDirW;      } set { lightDirW     = value; } }
        [Category("Parameters")]
        public Vector4 ShadowColor { get { return shadowColor; } set { shadowColor = value; } }
        [Category("Parameters")] public string      ShadeTex		{ get { return shadeTex;       } set { shadeTex      = value; } }
        [Category("Parameters")] public float       HighLight		{ get { return highLight;      } set { highLight     = value; } }
        [Category("Parameters")] public float       ColorBlend		{ get { return colorBlend;     } set { colorBlend    = value; } }
        [Category("Parameters")] public float       HighLightBlend	{ get { return highLightBlend; } set { highLightBlend= value; } }
        [Category("Parameters")]
        public Vector4 PenColor { get { return penColor; } set { penColor = value; } }
        [Category("Parameters")] public float       Ambient		    { get { return ambient;        } set { ambient       = value; } }
        [Category("Parameters")] public string      ColorTex		{ get { return colorTex;       } set { colorTex      = value; } }
        [Category("Parameters")] public float       Thickness		{ get { return thickness;      } set { thickness     = value; } }
        [Category("Parameters")] public float       ShadeBlend		{ get { return shadeBlend;     } set { shadeBlend    = value; } }
        [Category("Parameters")] public float       HighLightPower	{ get { return highLightPower; } set { highLightPower= value; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Name:           ").AppendLine(name);
            sb.Append("File:           ").AppendLine(file);
            sb.Append("Line:           ").AppendLine(line.ToString());
            sb.AppendLine("Code:").AppendLine(code);

            return sb.ToString();
        }

        public void ParseParameters()
        {
            codedata    = new TSOMaterialCode(code);

            foreach(TSOParameter i in codedata.Values)
                SetValue(i.Type, i.Name, i.Value);
        }

        public void SetValue(string type, string name, string value)
        {
            switch(name)
            {
            case "description":     description     = GetString (value); break;  // = "TA ToonShader v0.50"
            case "shader":          shader          = GetString (value); break;  // = "TAToonshade_050.cgfx"
            case "technique":       technique       = GetString (value); break;  // = "ShadowOn"
            case "LightDirX":       lightDirX       = GetFloat  (value); break;  // = [-0.00155681]
            case "LightDirY":       lightDirY       = GetFloat  (value); break;  // = [-0.0582338]
            case "LightDirZ":       lightDirZ       = GetFloat  (value); break;  // = [-0.998302]
            case "LightDirW":       lightDirW       = GetFloat  (value); break;  // = [0]
            case "ShadowColor":     shadowColor     = GetPoint4 (value); break;  // = [0, 0, 0, 1]
            case "ShadeTex":        shadeTex        = GetTexture(value); break;  // = Ninjya_Ribbon_Toon_Tex
            case "HighLight":       highLight       = GetFloat  (value); break;  // = [0]
            case "ColorBlend":      colorBlend      = GetFloat  (value); break;  // = [10]
            case "HighLightBlend":  highLightBlend  = GetFloat  (value); break;  // = [10]
            case "PenColor":        penColor        = GetPoint4 (value); break;  // = [0.166, 0.166, 0.166, 1]
            case "Ambient":         ambient         = GetFloat  (value); break;  // = [38]
            case "ColorTex":        colorTex        = GetTexture(value); break;  // = file24
            case "Thickness":       thickness       = GetFloat  (value); break;  // = [0.018]
            case "ShadeBlend":      shadeBlend      = GetFloat  (value); break;  // = [10]
            case "HighLightPower":  highLightPower  = GetFloat  (value); break;  // = [100]
            default:
                TSOFile.WriteLine("Unknown parameter. type=" + type + ", name=" + name + ", value=" + value);
                break;
            }
        }

        public string   GetTexture(string value)
        {
            return value;
        }

        public string   GetString(string value)
        {
            return value.Trim('"');
        }

        public float    GetFloat(string value)
        {
            return float.Parse(value.Trim('[', ']', ' '));
        }

        public Vector4 GetPoint4(string value)
        {
            string[]    token   = value.Trim('[', ']', ' ').Split(',');
            Vector4 p;
            p.X                 = float.Parse(token[0].Trim());
            p.Y                 = float.Parse(token[1].Trim());
            p.Z                 = float.Parse(token[2].Trim());
            p.W                 = float.Parse(token[3].Trim());
            return p;
        }
    }

    public class TSOMesh
    {
        internal TSOFile        file;
        internal string         name;
        internal Matrix matrix;
        internal int            effect;
        internal int            numsubs;
        internal TSOSubMesh[]   sub;

        [Category("General")]   public string   Name    { get { return name; } set { name= value; } }
      //[Category("Detail")]    public int      Effect  { get { return name; } set { name= value; } }
        [Category("Detail")]
        public Matrix Matrix { get { return matrix; } set { matrix = value; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Name:           ").AppendLine(name);
            sb.Append("Matrix:         ").AppendLine(matrix.ToString());
            sb.Append("Effect?:        ").AppendLine(effect.ToString());
            sb.Append("NumSubs:        ").AppendLine(numsubs.ToString());
            sb.Append("SubMesh.Count:  ").AppendLine(sub.Length.ToString());
            return sb.ToString();
        }
    }

    public class TSOSubMesh
    {
        internal int            spec;
        internal int            numbones;
        internal int[]          bones;
        internal int            numvertices;
        internal Vertex[]       vertices;
        internal TSOMesh        owner;
      //internal bool           shadow;
        internal bool           ink;

        [Category("Detail")]    public int      Spec    { get { return spec; } set { spec= value; } }
      //[Category("Detail")]    public int      Effect  { get { return name; } set { name= value; } }

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.Append("Spec:           ").AppendLine(spec.ToString());
            sb.Append("NumBones:       ").AppendLine(numbones.ToString());
            sb.Append("NumVertices:    ").AppendLine(numvertices.ToString());
            return sb.ToString();
        }
    }

    public partial struct Vertex : IComparable<Vertex>
    {
        public Vector3 Pos;
        public Vector4 Wgt;
        public UInt32       Idx;
        public Vector3 Nrm;
        public Vector2 Tex;
      //public int          Count;
      //public Weights[]    Weights;

        public Vertex(Vector3 pos, Vector4 wgt, UInt32 idx, Vector3 nrm, Vector2 tex)
        {
            Pos = pos;
            Wgt = wgt;
            Idx = idx;
            Nrm = nrm;
            Tex = tex;
        }

        public int CompareTo(Vertex o)
        {
            if(Pos.X < o.Pos.X) return -1; if(Pos.X > o.Pos.X) return 1;
            if(Pos.Y < o.Pos.Y) return -1; if(Pos.Y > o.Pos.Y) return 1;
            if(Pos.Z < o.Pos.Z) return -1; if(Pos.Z > o.Pos.Z) return 1;
            if(Nrm.X < o.Nrm.X) return -1; if(Nrm.X > o.Nrm.X) return 1;
            if(Nrm.Y < o.Nrm.Y) return -1; if(Nrm.Y > o.Nrm.Y) return 1;
            if(Nrm.Z < o.Nrm.Z) return -1; if(Nrm.Z > o.Nrm.Z) return 1;
            if(Tex.X < o.Tex.X) return -1; if(Tex.X > o.Tex.X) return 1;
            if(Tex.Y < o.Tex.Y) return -1; if(Tex.Y > o.Tex.Y) return 1;
            if(Wgt.X < o.Wgt.X) return -1; if(Wgt.X > o.Wgt.X) return 1;
            if(Wgt.Y < o.Wgt.Y) return -1; if(Wgt.Y > o.Wgt.Y) return 1;
            if(Wgt.Z < o.Wgt.Z) return -1; if(Wgt.Z > o.Wgt.Z) return 1;
            if(Wgt.W < o.Wgt.W) return -1; if(Wgt.W > o.Wgt.W) return 1;
            if(Idx   < o.Idx)   return -1; if(Idx   > o.Idx)   return 1;
            return 0;
        }

        public override int GetHashCode()
        {
            return Pos.X.GetHashCode() ^ Pos.Y.GetHashCode() ^ Pos.Z.GetHashCode()
                 ^ Nrm.X.GetHashCode() ^ Nrm.Y.GetHashCode() ^ Nrm.Z.GetHashCode()
                 ^ Tex.X.GetHashCode() ^ Tex.Y.GetHashCode() ^ Wgt.W.GetHashCode()
                 ^ Wgt.X.GetHashCode() ^ Wgt.Y.GetHashCode() ^ Wgt.Z.GetHashCode()
                 - Idx.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Vertex  o   = (Vertex)obj;

            return Pos.X==o.Pos.X && Pos.Y==o.Pos.Y && Pos.Z==o.Pos.Z
                && Nrm.X==o.Nrm.X && Nrm.Y==o.Nrm.Y && Nrm.Z==o.Nrm.Z
                && Tex.X==o.Tex.X && Tex.Y==o.Tex.Y && Wgt.W==o.Wgt.W
                && Wgt.X==o.Wgt.X && Wgt.Y==o.Wgt.Y && Wgt.Z==o.Wgt.Z
                && Idx  ==o.Idx;
        }
    }

    /*
    public struct Weights
    {
        public int          Index;
        public float        Weight;
    }
    */
}
