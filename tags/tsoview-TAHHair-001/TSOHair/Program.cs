using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOHair
{
    public static class Program
    {
        public static string GetHairKitPath()
        {
            return @"D:\TechArts3D\mod0416\_HAIR_KIT";
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

            Regex re_kami = new Regex(@"Kami");
            Regex re_housen = new Regex(@"Housen");
            Regex re_ribon = new Regex(@"Ribon");

            Dictionary<string, TSOTex> texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            string col = basename.Substring(10,2);

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("sub name {0} file {1}", sub.Name, sub.FileName);

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                if (re_kami.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
                    sub.Load(sub_path);

                    string tex_path = Path.Combine(GetHairKitPath(), string.Format(@"image_kami\KIT_BASE_{0}.bmp", col));
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(tex_path);
                    }
                }
                else
                if (re_housen.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkami_{0}", col));
                    sub.Load(sub_path);

                    string tex_path = Path.Combine(GetHairKitPath(), string.Format(@"image_kage\KIT_KAGE_{0}.bmp", col));
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(tex_path);
                    }
                }
                else
                if (re_ribon.IsMatch(sub.Name))
                {
                    string tex_path = Path.Combine(GetHairKitPath(), string.Format(@"image_ribon\KIT_RIBON_{0}.bmp", col));
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        colorTex.Load(tex_path);
                    }
                }

                sub.GenerateShader();
                Shader new_shader = sub.shader;
                new_shader.ColorTexName = color_tex_name;
                new_shader.ShadeTexName = shade_tex_name;
                sub.SaveShader();

                Console.WriteLine("shader color tex name {0}", color_tex_name);
                Console.WriteLine("shader shade tex name {0}", shade_tex_name);
            }

            foreach (TSOTex tex in tso.textures)
            {
                Console.WriteLine("tex name {0} file {1}", tex.Name, tex.FileName);
            }

            tso.Save(string.Format("{0}.tso", basename));
        }
    }
}
