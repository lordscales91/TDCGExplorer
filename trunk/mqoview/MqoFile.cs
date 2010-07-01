using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace mqoview
{
    public class MqoFile : IDisposable
    {
        private delegate bool SectionHandler(string[] tokens);

        public static char[] delimiters = new char[] { ' ', '\t' };
        public static char[] delimiters2 = new char[] { ' ', '\t', '(', ')' };

        private string file;
        private StreamReader sr;
        private MqoScene scene;
        private List<MqoMaterial> materials;
        private List<MqoObject> objects = new List<MqoObject>();
        private MqoObject current;

        public MqoScene Scene { get { return scene; } }
        public List<MqoMaterial> Materials { get { return materials; } }
        public List<MqoObject> Objects { get { return objects; } }

        public void Load(string file)
        {
            using (FileStream fs = File.OpenRead(file))
            {
                this.file = file;
                sr = new StreamReader(fs, Encoding.Default);
                ReadAll();
            }
        }

        public void Dump()
        {
            System.Diagnostics.Debug.WriteLine(file);
            System.Diagnostics.Debug.WriteLine(scene);

            foreach (MqoMaterial i in materials)
                System.Diagnostics.Debug.WriteLine(i);

            foreach (MqoObject i in objects)
                System.Diagnostics.Debug.WriteLine(i);
        }

        public void ReadAll()
        {
            DoRead(SectionRoot);
        }

        private void DoRead(SectionHandler h)
        {
            for (int no = 1; ; ++no)
            {
                string line = sr.ReadLine();

                if (line == null)
                    break;

                line = line.Trim();
                string[] tokens = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    if (tokens.Length == 0)
                        continue;

                    if (!h(tokens))
                        break;
                }
                catch (Exception e)
                {
                    string msg = string.Format("File format error: {0} \"{1}\"", no, line);
                    throw new Exception(msg, e);
                }
            }
        }

        public void Error(string[] tokens)
        {
            throw new Exception("File Format Error: \"" + string.Concat(tokens) + "\"");
        }

        public static Vector3 ParseColor3(string[] t, int begin)
        {
            return new Vector3(
                float.Parse(t[begin + 0]),
                float.Parse(t[begin + 1]),
                float.Parse(t[begin + 2]));
        }

        public static Vector2 ParsePoint2(string[] t, int begin)
        {
            return new Vector2(
                float.Parse(t[begin + 0]),
                float.Parse(t[begin + 1]));
        }

        public static Vector3 ParsePoint3(string[] t, int begin)
        {
            return new Vector3(
                float.Parse(t[begin + 0]),
                float.Parse(t[begin + 1]),
                float.Parse(t[begin + 2]));
        }

        private bool SectionRoot(string[] tokens)
        {
            switch (tokens[0].ToLower())
            {
                case "metasequoia": ParseMetasequoia(tokens); return true;
                case "format": ParseFormat(tokens); return true;
                case "scene": ParseScene(tokens); return true;
                case "material": ParseMaterial(tokens); return true;
                case "object": ParseObject(tokens); return true;
                case "eof": return false;
                default: return true;
            }
        }

        private bool SectionScene(string[] tokens)
        {
            scene = new MqoScene();

            switch (tokens[0].ToLower())
            {
                case "pos": scene.pos = ParsePoint3(tokens, 1); return true;
                case "lookat": scene.lookat = ParsePoint3(tokens, 1); return true;
                case "head": scene.head = float.Parse(tokens[1]); return true;
                case "pich": scene.pich = float.Parse(tokens[1]); return true;
                case "ortho": scene.ortho = float.Parse(tokens[1]); return true;
                case "zoom2": scene.zoom2 = float.Parse(tokens[1]); return true;
                case "amb": scene.amb = ParseColor3(tokens, 1); return true;
                case "}": return false;
                default: return true;
            }
        }

        private bool SectionMaterial(string[] tokens)
        {
            if (tokens[0] == "}")
                return false;

            StringBuilder sb = new StringBuilder();

            foreach (string i in tokens)
                sb.Append(' ').Append(i);

            string line = sb.ToString().Trim();
            MqoMaterial m = new MqoMaterial(tokens[0].Trim('"'));
            tokens = line.Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
            materials.Add(m);

            for (int i = 1; i < tokens.Length; ++i)
            {
                string t = tokens[i];

                switch (tokens[i].ToLower())
                {
                    case "shader": m.shader = int.Parse(tokens[++i]); break;
                    case "col": m.col = ParseColor3(tokens, i + 1); i += 4; break;
                    case "dif": m.dif = float.Parse(tokens[++i]); break;
                    case "amb": m.amb = float.Parse(tokens[++i]); break;
                    case "emi": m.emi = float.Parse(tokens[++i]); break;
                    case "spc": m.spc = float.Parse(tokens[++i]); break;
                    case "power": m.power = float.Parse(tokens[++i]); break;
                    case "tex": m.tex = tokens[++i].Trim('\"'); break;
                }
            }

            return true;
        }

        private bool SectionObject(string[] tokens)
        {
            switch (tokens[0].ToLower())
            {
                case "visible": current.visible = int.Parse(tokens[1]); return true;
                case "locking": current.locking = int.Parse(tokens[1]); return true;
                case "shading": current.shading = int.Parse(tokens[1]); return true;
                case "facet": current.facet = float.Parse(tokens[1]); return true;
                case "color": current.color = ParseColor3(tokens, 1); return true;
                case "color_type": current.color_type = int.Parse(tokens[1]); return true;
                case "vertex": ParseVertex(tokens); return true;
                case "face": ParseFace(tokens); return true;
                case "}": return false;
                default: return true;
            }
        }

        private bool SectionVertex(string[] tokens)
        {
            if (tokens[0] == "}")
                return false;
            UVertex v = new UVertex();
            v.position = ParsePoint3(tokens, 0);
            current.vertices.Add(v);

            return true;
        }

        private bool SectionFace(string[] tokens)
        {
            if (tokens[0] == "}")
                return false;

            if (3 != int.Parse(tokens[0]))
                return true;

            StringBuilder sb = new StringBuilder();

            foreach (string i in tokens)
                sb.Append(' ').Append(i);

            string line = sb.ToString().Trim();
            MqoFace f = new MqoFace();
            tokens = line.Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
            current.faces.Add(f);

            for (int i = 1; i < tokens.Length; ++i)
            {
                switch (tokens[i].ToLower())
                {
                    case "v":
                        f.a = ushort.Parse(tokens[++i]);
                        f.b = ushort.Parse(tokens[++i]);
                        f.c = ushort.Parse(tokens[++i]);
                        break;
                    case "m":
                        f.mtl = ushort.Parse(tokens[++i]);
                        break;
                    case "uv":
                        f.ta = ParsePoint2(tokens, i + 1); i += 2;
                        f.tb = ParsePoint2(tokens, i + 1); i += 2;
                        f.tc = ParsePoint2(tokens, i + 1); i += 2;
                        break;
                }
            }

            return true;
        }

        //----- Root elements ----------------------------------------------
        private void ParseMetasequoia(string[] tokens)
        {
            if (tokens[1].ToLower() != "document") Error(tokens);
        }

        private void ParseFormat(string[] tokens)
        {
            if (tokens[1].ToLower() != "text") Error(tokens);
            if (tokens[2].ToLower() != "ver") Error(tokens);
            if (tokens[3].ToLower() != "1.0") Error(tokens);
        }

        private void ParseScene(string[] tokens)
        {
            if (tokens[1].ToLower() != "{") Error(tokens);

            DoRead(SectionScene);
        }

        private void ParseMaterial(string[] tokens)
        {
            if (tokens[2].ToLower() != "{") Error(tokens);

            materials = new List<MqoMaterial>(int.Parse(tokens[1]));
            DoRead(SectionMaterial);
        }

        private void ParseObject(string[] tokens)
        {
            if (tokens[2].ToLower() != "{") Error(tokens);

            current = new MqoObject(tokens[1].Trim('"'));
            objects.Add(current);
            DoRead(SectionObject);
        }

        private void ParseVertex(string[] tokens)
        {
            if (tokens[2].ToLower() != "{") Error(tokens);

            current.vertices = new List<UVertex>(int.Parse(tokens[1]));
            DoRead(SectionVertex);
        }

        private void ParseFace(string[] tokens)
        {
            if (tokens[2].ToLower() != "{") Error(tokens);

            current.faces = new List<MqoFace>(int.Parse(tokens[1]));
            DoRead(SectionFace);
        }

        public void Dispose()
        {
            foreach (MqoObject obj in objects)
                obj.Dispose();
        }
    }

    public class MqoScene
    {
        public Vector3 pos;
        public Vector3 lookat;
        public float head;
        public float pich;
        public float ortho;
        public float zoom2;
        public Vector3 amb;

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" pos: ").Append(pos)
                .Append(" lookat: ").Append(lookat)
                .Append(" head: ").Append(head)
                .Append(" pich: ").Append(pich)
                .Append(" ortho: ").Append(ortho)
                .Append(" zoom2: ").Append(zoom2)
                .Append(" amb: ").Append(amb)
                .ToString();
        }
    }

    public class MqoMaterial
    {
        public string name;
        public int shader;
        public Vector3 col;
        public float dif;
        public float amb;
        public float emi;
        public float spc;
        public float power;
        public string tex;

        public MqoMaterial() { }
        public MqoMaterial(string n) { name = n; }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" shader: ").Append(shader)
                .Append(" col: ").Append(col)
                .Append(" dif: ").Append(dif)
                .Append(" amb: ").Append(amb)
                .Append(" emi: ").Append(emi)
                .Append(" spc: ").Append(spc)
                .Append(" power: ").Append(power)
                .Append(" tex: ").Append(tex)
                .Append(" name: ").Append(name)
                .ToString();
        }
    }

    public class MqoObject : IDisposable
    {
        public string name;
        public int visible;
        public int locking;
        public int shading;
        public float facet;
        public Vector3 color;
        public int color_type;
        public List<UVertex> vertices;
        public List<MqoFace> faces;

        public Mesh dm = null;

        public MqoObject() { }
        public MqoObject(string n) { name = n; }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" visible: ").Append(visible)
                .Append(" locking: ").Append(locking)
                .Append(" shading: ").Append(shading)
                .Append(" facet: ").Append(facet)
                .Append(" color: ").Append(color)
                .Append(" color_type: ").Append(color_type)
                .Append(" vertices: ").Append(vertices.Count)
                .Append(" faces: ").Append(faces.Count)
                .Append(" name: ").Append(name)
                .ToString();
        }

        static VertexElement[] ve = new VertexElement[]
        {
            new VertexElement(0,  0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            //new VertexElement(0, 12, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 3),
            //new VertexElement(0, 28, DeclarationType.Ubyte4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 4),
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
            int numVertices = vertices.Count;
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
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        UVertex v = vertices[i];

                        gs.Write(v.position);
                        //for (int j = 0; j < 4; j++)
                        //    gs.Write(v.skin_weights[j].weight);
                        //gs.Write(v.bone_indices);
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
                    foreach (MqoFace face in faces)
                    {
                        gs.Write(face.a);
                        gs.Write(face.b);
                        gs.Write(face.c);
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

    public class MqoFace
    {
        public ushort a, b, c, mtl;
        public Vector2 ta, tb, tc;

        public MqoFace()
        {
        }

        public MqoFace(ushort a, ushort b, ushort c, ushort mtl, Vector2 ta, Vector2 tb, Vector2 tc)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.mtl = mtl;
            this.ta = ta;
            this.tb = tb;
            this.tc = tc;
        }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append("v: ").Append(a).Append(" ").Append(b).Append(" ").Append(c)
                .Append(" mtl: ").Append(mtl)
                .Append(" uv: ").Append(ta).Append(" ").Append(tb).Append(" ").Append(tc)
                .ToString();
        }
    }

    public class UVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public float u, v;
        public int mtl;

        public UVertex()
        {
        }

        public UVertex(Vector3 position, Vector3 normal, float u, float v, int mtl)
        {
            this.position = position;
            this.normal = normal;
            this.u = u;
            this.v = v;
            this.mtl = mtl;
        }
    }
}
