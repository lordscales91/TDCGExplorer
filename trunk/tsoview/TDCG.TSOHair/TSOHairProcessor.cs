using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TDCG;

namespace TDCG.TSOHair
{
    public class TSOHairProcessor
    {
        public List<TSOHairPart> parts;

        public TSOHairProcessor()
        {
        }

        public void CreateParts()
        {
            parts = new List<TSOHairPart>();
            TSOHairPart part;

            // Housen ÇÕ Kami ÇÊÇËóDêÊÇ∑ÇÈÅB
            // ex. KamiHousen ÇÃèÍçáÇÕ Housen Ç∆ÇµÇƒèàóù
            part = new TSOHairPart();
            part.Name = "Housen";
            part.TextPattern = @"housen|w_facepartsa";
            part.SubPath = @"Cgfx_kage\Cgfx_kage_{0}";
            part.TexPath = @"Image_kage\Image_kage_{0}.bmp";
            parts.Add(part);

            part = new TSOHairPart();
            part.Name = "Ribbon";
            part.TextPattern = @"ribb?on";
            part.SubPath = @"Cgfx_ribon\Cgfx_ribon_{0}";
            part.TexPath = @"Image_ribon\Image_ribon_{0}.bmp";
            parts.Add(part);

            part = new TSOHairPart();
            part.Name = "Kami";
            part.TextPattern = @"kami";
            part.SubPath = @"Cgfx_kami\Cgfx_kami_{0}";
            part.TexPath = @"Image_kami\Image_kami_{0}.bmp";
            parts.Add(part);
        }

        public void Dump(string dest_file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TSOHairProcessor));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding("Shift_JIS");
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(dest_file, settings);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static TSOHairProcessor Load(string source_file)
        {
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(TSOHairProcessor));
            TSOHairProcessor program = serializer.Deserialize(reader) as TSOHairProcessor;
            reader.Close();
            return program;
        }

        public static string GetHairKitPath()
        {
            return Path.Combine(Application.StartupPath, @"HAIR_KIT");
        }

        public void Process(TSOFile tso, string col)
        {
            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("  sub name {0} file {1}", sub.Name, sub.FileName);
                TSOHairPart detected_part = null;

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                foreach (TSOHairPart part in parts)
                {
                    if (part.GetTextRegex().IsMatch(sub.Name))
                    {
                        detected_part = part;
                        break;
                    }
                }

                if (detected_part == null)
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        string file = colorTex.FileName.Trim('"');

                        foreach (TSOHairPart part in parts)
                        {
                            if (part.GetTextRegex().IsMatch(file))
                            {
                                detected_part = part;
                                break;
                            }
                        }
                    }
                }

                Console.WriteLine("    : type {0}", (detected_part != null) ? detected_part.Name : "Unknown");

                if (detected_part != null)
                {
                    if (detected_part.SubPath != null)
                    {
                        sub.Load(Path.Combine(GetHairKitPath(), string.Format(detected_part.SubPath, col)));
                    }

                    if (detected_part.TexPath != null)
                    {
                        TSOTex colorTex;
                        if (texmap.TryGetValue(color_tex_name, out colorTex))
                        {
                            colorTex.Load(Path.Combine(GetHairKitPath(), string.Format(detected_part.TexPath, col)));
                        }
                    }
                }

                sub.GenerateShader();
                Shader new_shader = sub.shader;
                new_shader.ColorTexName = color_tex_name;
                new_shader.ShadeTexName = shade_tex_name;
                sub.SaveShader();

                Console.WriteLine("    shader color tex name {0}", color_tex_name);
                Console.WriteLine("    shader shade tex name {0}", shade_tex_name);
            }

            foreach (TSOTex tex in tso.textures)
            {
                Console.WriteLine("  tex name {0} file {1}", tex.Name, tex.FileName);
            }
        }
    }
}
