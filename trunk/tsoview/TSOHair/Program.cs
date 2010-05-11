using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOHair
{
    public static class Program
    {
        static Regex re_kami = new Regex(@"kami", RegexOptions.IgnoreCase);
        static Regex re_housen = new Regex(@"housen", RegexOptions.IgnoreCase);
        static Regex re_ribon = new Regex(@"ribon", RegexOptions.IgnoreCase);

        public static string GetHairKitPath()
        {
            return Path.Combine(Application.StartupPath, @"HAIR_KIT");
        }

        public static string GetKamiTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_kami\KIT_BASE_{0}.bmp", col));
        }

        public static string GetKageTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_kage\KIT_KAGE_{0}.bmp", col));
        }

        public static string GetRibonTexPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"image_ribon\KIT_RIBON_{0}.bmp", col));
        }

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("TSOHair.exe <tso file> <basename>");
                return;
            }
            string source_file = args[0];

            string basename = "N000BHEA_C00";
            if (args.Length > 1)
            {
                basename = args[1];
            }

            TSOFile tso = new TSOFile();
            tso.Load(source_file);

            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            string col = basename.Substring(10,2);

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("  sub name {0} file {1}", sub.Name, sub.FileName);

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                // Housen ÇÕ Kami ÇÊÇËóDêÊÇ∑ÇÈÅB
                // ex. KamiHousen ÇÃèÍçáÇÕ Housen Ç∆ÇµÇƒèàóù
                if (re_housen.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkage_{0}", col));
                    sub.Load(sub_path);

                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetKageTexPath(col));
                    }
                }
                else
                if (re_ribon.IsMatch(sub.Name))
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetRibonTexPath(col));
                    }
                }
                else
                if (re_kami.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
                    sub.Load(sub_path);

                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(GetKamiTexPath(col));
                    }
                }
                else
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        string file = colorTex.FileName.Trim('"');

                        if (re_housen.IsMatch(file))
                        {
                            string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkage_{0}", col));
                            sub.Load(sub_path);

                            colorTex.Load(GetKageTexPath(col));
                        }
                        else
                        if (re_ribon.IsMatch(file))
                        {
                            colorTex.Load(GetRibonTexPath(col));
                        }
                        else
                        if (re_kami.IsMatch(file))
                        {
                            string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
                            sub.Load(sub_path);

                            colorTex.Load(GetKamiTexPath(col));
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
                Console.WriteLine("tex name {0} file {1}", tex.Name, tex.FileName);
            }

            tso.Save(string.Format("{0}.tso", basename));
        }
    }
}
