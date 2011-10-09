using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Tso2MqoGui
{
    public class MqoFile
    {
        private delegate bool SectionHandler(string[] tokens);
        
        public static char[]        delimiters  = new char[]{' ', '\t'};
        public static char[]        delimiters2 = new char[]{' ', '\t', '(', ')'};

        private string              file;
        private StreamReader        sr;
        private MqoScene            scene;
        private List<MqoMaterial>   materials;
        private List<MqoObject>     objects     = new List<MqoObject>();
        private MqoObject           current;

        public MqoScene             Scene       { get { return scene;       } }
        public List<MqoMaterial>    Materials   { get { return materials;   } }
        public List<MqoObject>      Objects     { get { return objects;     } }

        public void Load(string file)
        {
            using(FileStream fs= File.OpenRead(file))
            {
                this.file   = file;
                sr          = new StreamReader(fs, Encoding.Default);
                ReadAll();
            }
        }

        public void Dump()
        {
            System.Diagnostics.Debug.WriteLine(file);
            System.Diagnostics.Debug.WriteLine(scene);

            foreach(MqoMaterial i in materials)
                System.Diagnostics.Debug.WriteLine(i);

            foreach(MqoObject i in objects)
                System.Diagnostics.Debug.WriteLine(i);
        }

        public void ReadAll()
        {
            DoRead(SectionRoot);
        }

        private static string[] SplitString(string s)
        {
            List<string>    tokens  = new List<string>();
            StringBuilder   sb     = new StringBuilder(s.Length);
            bool            str    = false;
            bool            escape = false;
            bool            bracket= false;
            s                      = s.Trim(' ', '\t', '\r', '\n');

            foreach(char i in s)
            {
                if(escape)
                {
                    sb.Append(i);
                    escape  = false;
                    continue;
                }


                switch(i)
                {
                case '\\':
                    if(str) sb.Append(i);
                    else    escape  = true;
                    break;
                case ' ':
                case '\t':
                         if(bracket)        {sb.Append(i);                            }
                    else if(str)            {sb.Append(i);                            }
                    else if(sb.Length > 0)  {tokens.Add(sb.ToString()); sb.Length= 0; }
                    break;
                case '(':
                    sb.Append(i);
                    if(!str)
                        bracket= true;
                    break;
                case ')':
                    sb.Append(i);
                    if(!str)
                        bracket= false;
                    break;
                case '\"':
                    sb.Append(i);
                    str = !str;
                    break;
                default:
                    sb.Append(i);
                    break;
                }
            }

            if(sb.Length > 0)
                tokens.Add(sb.ToString());

            return tokens.ToArray();
        }

        private void DoRead(SectionHandler h)
        {
            for(int no= 1;; ++no)
            {
                string  line= sr.ReadLine();

                if(line == null)
                    break;

                line                = line.Trim();
                string[]    tokens  = SplitString(line);

                try
                {
                    if(tokens.Length == 0)
                        continue;

                    if(!h(tokens))
                        break;
                } catch(Exception e)
                {
                    string  msg = string.Format("File format error: {0} \"{1}\"", no, line);
                    throw new Exception(msg, e);
                }
            }
        }

        public void Error(string[] tokens)
        {
            throw new Exception("File Format Error: \"" + string.Concat(tokens) + "\"");
        }

        private bool SectionRoot(string[] tokens)
        {
            switch(tokens[0].ToLower())
            {
            case "metasequoia": ParseMetasequoia(tokens);   return true;
            case "format":      ParseFormat     (tokens);   return true;
            case "scene":       ParseScene      (tokens);   return true;
            case "material":    ParseMaterial   (tokens);   return true;
            case "object":      ParseObject     (tokens);   return true;
            case "eof":                                     return false;
          //default:            Error(tokens);              return false;
            default:                                        return true;
            }
        }

        private bool SectionScene(string[] tokens)
        {
            scene   = new MqoScene();

            switch(tokens[0].ToLower())
            {
            case "pos":         scene.pos           = Point3.Parse(tokens, 1);  return true;
            case "lookat":      scene.lookat        = Point3.Parse(tokens, 1);  return true;
            case "head":        scene.head          = float .Parse(tokens[1]);  return true;
            case "pich":        scene.pich          = float .Parse(tokens[1]);  return true;
            case "ortho":       scene.ortho         = float .Parse(tokens[1]);  return true;
            case "zoom2":       scene.zoom2         = float .Parse(tokens[1]);  return true;
            case "amb":         scene.amb           = Color3.Parse(tokens, 1);  return true;
            case "}":                                                           return false;
          //default:            Error(tokens);                                  return false;
            default:                                                            return true;
            }
        }

        private static string[] SplitParam(string s)
        {
            return s.Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool SectionMaterial(string[] tokens)
        {
            if(tokens[0] == "}")
                return false;

            StringBuilder   sb  = new StringBuilder();

            foreach(string i in tokens)
                sb.Append(' ').Append(i);

            string      line= sb.ToString().Trim();
            MqoMaterial m   = new MqoMaterial(tokens[0].Trim('"'));
            tokens          = SplitString(line);
            materials.Add(m);

            for(int i= 1 ; i < tokens.Length; ++i)
            {
                string      t   = tokens[i];
                string      t2  = t.ToLower();

                     if(t2.StartsWith("shader("))  m.shader   = int   .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("col("))     m.col      = Color3.Parse(SplitParam(t), 1);
                else if(t2.StartsWith("dif("))     m.dif      = float .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("amb("))     m.amb      = float .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("emi("))     m.emi      = float .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("spc("))     m.spc      = float .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("power("))   m.power    = float .Parse(SplitParam(t)[1]);
                else if(t2.StartsWith("tex("))     m.tex      = t.Substring(3).Trim('(', ')', '"');
            }

            return true;
        }

        private bool SectionObject(string[] tokens)
        {
            switch(tokens[0].ToLower())
            {
            case "visible":     current.visible     = int   .Parse(tokens[1]);  return true;
            case "locking":     current.locking     = int   .Parse(tokens[1]);  return true;
            case "shading":     current.shading     = int   .Parse(tokens[1]);  return true;
            case "facet":       current.facet       = float .Parse(tokens[1]);  return true;
            case "color":       current.color       = Color3.Parse(tokens, 1);  return true;
            case "color_type":  current.color_type  = int   .Parse(tokens[1]);  return true;
            case "vertex":      ParseVertex(tokens);                            return true;
            case "face":        ParseFace(tokens);                              return true;
            case "}":                                                           return false;
          //default:            Error(tokens);                                  return false;
            default:                                                            return true;
            }
        }

        private bool SectionVertex(string[] tokens)
        {
            if (tokens[0] == "}")
                return false;

            current.vertices.Add(Point3.Parse(tokens, 0));

            return true;
        }

        private bool SectionFace(string[] tokens)
        {
            if (tokens[0] == "}")
                return false;

            int nface = int.Parse(tokens[0]);

            if (3 != nface)
                return true;

            {
                StringBuilder sb = new StringBuilder();
                foreach (string i in tokens)
                    sb.Append(' ').Append(i);
                string line = sb.ToString().Trim();
                tokens = SplitString(line);
            }
            MqoFace f = new MqoFace();

            for (int i = 1; i < tokens.Length; ++i)
            {
                string t = tokens[i];
                string t2 = t.ToLower();

                if (t2.StartsWith("v("))
                {
                    string[] t3 = SplitParam(t);
                    f.a = ushort.Parse(t3[1]);
                    f.b = ushort.Parse(t3[2]);
                    f.c = ushort.Parse(t3[3]);
                }
                else
                if (t2.StartsWith("m("))
                {
                    string[] t3 = SplitParam(t);
                    f.mtl = ushort.Parse(t3[1]);
                }
                else
                if (t2.StartsWith("uv("))
                {
                    string[] t3 = SplitParam(t);
                    f.ta = Point2.Parse(t3, 1);
                    f.tb = Point2.Parse(t3, 3);
                    f.tc = Point2.Parse(t3, 5);
                }
            }
            current.faces.Add(f);

            return true;
        }

        //----- Root elements ----------------------------------------------
        private void ParseMetasequoia(string[] tokens)
        {
            if(tokens[1].ToLower() != "document")   Error(tokens);
        }

        private void ParseFormat(string[] tokens)
        {
            if(tokens[1].ToLower() != "text")       Error(tokens);
            if(tokens[2].ToLower() != "ver")        Error(tokens);
            if(tokens[3].ToLower() != "1.0")        Error(tokens);
        }

        private void ParseScene(string[] tokens)
        {
            if(tokens[1].ToLower() != "{")          Error(tokens);

            DoRead(SectionScene);
        }

        private void ParseMaterial(string[] tokens)
        {
            if(tokens[2].ToLower() != "{")          Error(tokens);

            materials   = new List<MqoMaterial>(int.Parse(tokens[1]));
            DoRead(SectionMaterial);
        }

        private void ParseObject(string[] tokens)
        {
            if(tokens[2].ToLower() != "{")          Error(tokens);

            current = new MqoObject(tokens[1].Trim('"'));
            objects.Add(current);
            DoRead(SectionObject);
        }

        private void ParseVertex(string[] tokens)
        {
            if(tokens[2].ToLower() != "{")          Error(tokens);

            current.vertices    = new List<Point3>(int.Parse(tokens[1]));
            DoRead(SectionVertex);
        }

        private void ParseFace(string[] tokens)
        {
            if(tokens[2].ToLower() != "{")          Error(tokens);

            current.faces       = new List<MqoFace>(int.Parse(tokens[1]));
            DoRead(SectionFace);
        }
    }

    public class MqoScene
    {
        public Point3           pos;
        public Point3           lookat;
        public float            head;
        public float            pich;
        public float            ortho;
        public float            zoom2;
        public Color3           amb;

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" pos: ")   .Append(pos)
                .Append(" lookat: ").Append(lookat)
                .Append(" head: ")  .Append(head)
                .Append(" pich: ")  .Append(pich)
                .Append(" ortho: ") .Append(ortho)
                .Append(" zoom2: ") .Append(zoom2)
                .Append(" amb: ")   .Append(amb)
                .ToString();
        }
    }

    public class MqoMaterial
    {
        public string           name;
        public int              shader;
        public Color3           col;
        public float            dif;
        public float            amb;
        public float            emi;
        public float            spc;
        public float            power;
        public string           tex;

        public MqoMaterial()            {           }
        public MqoMaterial(string n)    { name= n;  }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" shader: ").Append(shader)
                .Append(" col: ")   .Append(col)
                .Append(" dif: ")   .Append(dif)
                .Append(" amb: ")   .Append(amb)
                .Append(" emi: ")   .Append(emi)
                .Append(" spc: ")   .Append(spc)
                .Append(" power: ") .Append(power)
                .Append(" tex: ")   .Append(tex)
                .Append(" name: ")  .Append(name)
                .ToString();
        }
    }

    public class MqoObject
    {
        public string           name;
        public int              visible;
	    public int              locking;
	    public int              shading;
	    public float            facet;
	    public Color3           color;
	    public int              color_type;
        public List<Point3>     vertices;
        public List<MqoFace>    faces;

        public MqoObject()              {           }
        public MqoObject(string n)      { name= n;  }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append(" visible: ")   .Append(visible)
                .Append(" locking: ")   .Append(locking)
                .Append(" shading: ")   .Append(shading)
                .Append(" facet: ")     .Append(facet)
                .Append(" color: ")     .Append(color)
                .Append(" color_type: ").Append(color_type)
                .Append(" vertices: ")  .Append(vertices.Count)
                .Append(" faces: ")     .Append(faces.Count)
                .Append(" name: ")      .Append(name)
                .ToString();
        }
    }

    public class  MqoFace
    {
        public ushort   a, b, c, mtl;
        public Point2   ta, tb, tc;

        public MqoFace()
        {
        }

        public MqoFace(ushort a, ushort b, ushort c, ushort mtl, Point2 ta, Point2 tb, Point2 tc)
        {
            this.a  = a;
            this.b  = b;
            this.c  = c;
            this.mtl= mtl;
            this.ta = ta;
            this.tb = tb;
            this.tc = tc;
        }

        public override string ToString()
        {
            return (new StringBuilder(256))
                .Append("v: ")   .Append(a).Append(" ").Append(b).Append(" ").Append(c)
                .Append(" mtl: ").Append(mtl)
                .Append(" uv: ") .Append(ta).Append(" ").Append(tb).Append(" ").Append(tc)
                .ToString();
        }
    }
}
