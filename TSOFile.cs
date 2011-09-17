using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace Tso2MqoGui
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

        public static void ExchangeChannel(byte[] data, int depth)
        {
            for(int j= 0; j < data.Length; j+=depth)
            {
                byte    tmp = data[j+2];
                data[j+2]   = data[j+0];
                data[j+0]   = tmp;
            }
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

                ExchangeChannel(textures[i].data, textures[i].depth);

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
                        ReadVertex(ref v[k]);
                    }

                    WriteLine(r.BaseStream.Position.ToString("X"));
                    System.Diagnostics.Debug.WriteLine(r.BaseStream.Position.ToString("X"));
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
        internal Matrix44       matrix;
        internal Matrix44       world;
        internal List<TSONode>  children    = new List<TSONode>();
        internal TSONode        parent;

        [Category("General")] public int      ID        { get { return id; } }
        [Category("General")] public string   Name      { get { return name; } }
        [Category("General")] public string   ShortName { get { return sname; } }
        [Category("Detail")]  public Matrix44 Matrix    { get { return matrix; } set { matrix= value; } }
        [Category("Detail")]  public Matrix44 World     { get { return world;  } set { world = value; } }

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
        internal Point4     shadowColor;     // = [0, 0, 0, 1]
        internal string     shadeTex;        // = Ninjya_Ribbon_Toon_Tex
        internal float      highLight;       // = [0]
        internal float      colorBlend;      // = [10]
        internal float      highLightBlend;  // = [10]
        internal Point4     penColor;        // = [0.166, 0.166, 0.166, 1]
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
        [Category("Parameters")] public Point4      ShadowColor     { get { return shadowColor;    } set { shadowColor   = value; } }
        [Category("Parameters")] public string      ShadeTex		{ get { return shadeTex;       } set { shadeTex      = value; } }
        [Category("Parameters")] public float       HighLight		{ get { return highLight;      } set { highLight     = value; } }
        [Category("Parameters")] public float       ColorBlend		{ get { return colorBlend;     } set { colorBlend    = value; } }
        [Category("Parameters")] public float       HighLightBlend	{ get { return highLightBlend; } set { highLightBlend= value; } }
        [Category("Parameters")] public Point4      PenColor		{ get { return penColor;       } set { penColor      = value; } }
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

        public Point4   GetPoint4(string value)
        {
            string[]    token   = value.Trim('[', ']', ' ').Split(',');
            Point4      p       = new Point4();
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
        internal Matrix44       matrix;
        internal int            effect;
        internal int            numsubs;
        internal TSOSubMesh[]   sub;

        [Category("General")]   public string   Name    { get { return name; } set { name= value; } }
      //[Category("Detail")]    public int      Effect  { get { return name; } set { name= value; } }
        [Category("Detail")]    public Matrix44 Matrix  { get { return matrix; } set { matrix= value; } }

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

    public struct Matrix44
    {
        public static readonly Matrix44 Identity    = new Matrix44(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1);

        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;

        public float M11 { get { return m11; } set { m11= value; } }
        public float M12 { get { return m12; } set { m12= value; } }
        public float M13 { get { return m13; } set { m13= value; } }
        public float M14 { get { return m14; } set { m14= value; } }
        public float M21 { get { return m21; } set { m21= value; } }
        public float M22 { get { return m22; } set { m22= value; } }
        public float M23 { get { return m23; } set { m23= value; } }
        public float M24 { get { return m24; } set { m24= value; } }
        public float M31 { get { return m31; } set { m31= value; } }
        public float M32 { get { return m32; } set { m32= value; } }
        public float M33 { get { return m33; } set { m33= value; } }
        public float M34 { get { return m34; } set { m34= value; } }
        public float M41 { get { return m41; } set { m41= value; } }
        public float M42 { get { return m42; } set { m42= value; } }
        public float M43 { get { return m43; } set { m43= value; } }
        public float M44 { get { return m44; } set { m44= value; } }

        public Matrix44(
            float a11, float a12, float a13, float a14,
            float a21, float a22, float a23, float a24,
            float a31, float a32, float a33, float a34,
            float a41, float a42, float a43, float a44)
        {
            m11=a11; m12=a12; m13=a13; m14=a14;
            m21=a21; m22=a22; m23=a23; m24=a24;
            m31=a31; m32=a32; m33=a33; m34=a34;
            m41=a41; m42=a42; m43=a43; m44=a44;
        }

        public Point3 Translation   { get { return new Point3(M41, M42, M43); } } 

        public override string ToString()
        {
            StringBuilder   sb  = new StringBuilder();
            sb.AppendFormat("[{0}, {1}, {2}, {3}], ",  M11.ToString("N"), M12.ToString("N"), M13.ToString("N"), M14.ToString("N"))
              .AppendFormat("[{0}, {1}, {2}, {3}], ",  M21.ToString("N"), M22.ToString("N"), M23.ToString("N"), M24.ToString("N"))
              .AppendFormat("[{0}, {1}, {2}, {3}], ",  M31.ToString("N"), M32.ToString("N"), M33.ToString("N"), M34.ToString("N"))
              .AppendFormat("[{0}, {1}, {2}, {3}]",    M41.ToString("N"), M42.ToString("N"), M43.ToString("N"), M44.ToString("N"));
            return sb.ToString();
        }

        public static Matrix44 Mul(Matrix44 a, Matrix44 b)
        {
            Matrix44    m   = new Matrix44();

            m.M11   = a.M11*b.M11 + a.M12*b.M21 + a.M13*b.M31 + a.M14*b.M41;
            m.M12   = a.M11*b.M12 + a.M12*b.M22 + a.M13*b.M32 + a.M14*b.M42;
            m.M13   = a.M11*b.M13 + a.M12*b.M23 + a.M13*b.M33 + a.M14*b.M43;
            m.M14   = a.M11*b.M14 + a.M12*b.M24 + a.M13*b.M34 + a.M14*b.M44;

            m.M21   = a.M21*b.M11 + a.M22*b.M21 + a.M23*b.M31 + a.M24*b.M41;
            m.M22   = a.M21*b.M12 + a.M22*b.M22 + a.M23*b.M32 + a.M24*b.M42;
            m.M23   = a.M21*b.M13 + a.M22*b.M23 + a.M23*b.M33 + a.M24*b.M43;
            m.M24   = a.M21*b.M14 + a.M22*b.M24 + a.M23*b.M34 + a.M24*b.M44;

            m.M31   = a.M31*b.M11 + a.M32*b.M21 + a.M33*b.M31 + a.M34*b.M41;
            m.M32   = a.M31*b.M12 + a.M32*b.M22 + a.M33*b.M32 + a.M34*b.M42;
            m.M33   = a.M31*b.M13 + a.M32*b.M23 + a.M33*b.M33 + a.M34*b.M43;
            m.M34   = a.M31*b.M14 + a.M32*b.M24 + a.M33*b.M34 + a.M34*b.M44;

            m.M41   = a.M41*b.M11 + a.M42*b.M21 + a.M43*b.M31 + a.M44*b.M41;
            m.M42   = a.M41*b.M12 + a.M42*b.M22 + a.M43*b.M32 + a.M44*b.M42;
            m.M43   = a.M41*b.M13 + a.M42*b.M23 + a.M43*b.M33 + a.M44*b.M43;
            m.M44   = a.M41*b.M14 + a.M42*b.M24 + a.M43*b.M34 + a.M44*b.M44;

            return m;
        }
    }

    public partial struct Vertex : IComparable<Vertex>
    {
        public Point3       Pos;
        public Point4       Wgt;
        public UInt32       Idx;
        public Point3       Nrm;
        public Point2       Tex;
      //public int          Count;
      //public Weights[]    Weights;

        public Vertex(Point3 pos, Point4 wgt, UInt32 idx, Point3 nrm, Point2 tex)
        {
            Pos = pos;
            Wgt = wgt;
            Idx = idx;
            Nrm = nrm;
            Tex = tex;
        }

        public int CompareTo(Vertex o)
        {
            if(Pos.x < o.Pos.x) return -1; if(Pos.x > o.Pos.x) return 1;
            if(Pos.y < o.Pos.y) return -1; if(Pos.y > o.Pos.y) return 1;
            if(Pos.z < o.Pos.z) return -1; if(Pos.z > o.Pos.z) return 1;
            if(Nrm.x < o.Nrm.x) return -1; if(Nrm.x > o.Nrm.x) return 1;
            if(Nrm.y < o.Nrm.y) return -1; if(Nrm.y > o.Nrm.y) return 1;
            if(Nrm.z < o.Nrm.z) return -1; if(Nrm.z > o.Nrm.z) return 1;
            if(Tex.x < o.Tex.x) return -1; if(Tex.x > o.Tex.x) return 1;
            if(Tex.y < o.Tex.y) return -1; if(Tex.y > o.Tex.y) return 1;
            if(Wgt.x < o.Wgt.x) return -1; if(Wgt.x > o.Wgt.x) return 1;
            if(Wgt.y < o.Wgt.y) return -1; if(Wgt.y > o.Wgt.y) return 1;
            if(Wgt.z < o.Wgt.z) return -1; if(Wgt.z > o.Wgt.z) return 1;
            if(Wgt.w < o.Wgt.w) return -1; if(Wgt.w > o.Wgt.w) return 1;
            if(Idx   < o.Idx)   return -1; if(Idx   > o.Idx)   return 1;
            return 0;
        }

        public override int GetHashCode()
        {
            return Pos.x.GetHashCode() ^ Pos.y.GetHashCode() ^ Pos.z.GetHashCode()
                 ^ Nrm.x.GetHashCode() ^ Nrm.y.GetHashCode() ^ Nrm.z.GetHashCode()
                 ^ Tex.x.GetHashCode() ^ Tex.y.GetHashCode() ^ Wgt.w.GetHashCode()
                 ^ Wgt.x.GetHashCode() ^ Wgt.y.GetHashCode() ^ Wgt.z.GetHashCode()
                 ^ Idx.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Vertex  o   = (Vertex)obj;

            return Pos.x==o.Pos.x && Pos.y==o.Pos.y && Pos.z==o.Pos.z
                && Nrm.x==o.Nrm.x && Nrm.y==o.Nrm.y && Nrm.z==o.Nrm.z
                && Tex.x==o.Tex.x && Tex.y==o.Tex.y && Wgt.w==o.Wgt.w
                && Wgt.x==o.Wgt.x && Wgt.y==o.Wgt.y && Wgt.z==o.Wgt.z
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
