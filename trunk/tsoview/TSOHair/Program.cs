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

        internal static Dictionary<string, TSOTex> texmap;

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

            texmap = new Dictionary<string, TSOTex>();

            foreach (TSOTex tex in tso.textures)
            {
                texmap[tex.Name] = tex;
            }

            foreach (TSOSubScript sub in tso.sub_scripts)
            {
                Console.WriteLine("sub name {0} file {1}", sub.Name, sub.FileName);
                Shader shader = sub.shader;
                if (re_kami.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), @"Cgfx_kami\Cgfxkami_19");
                    sub.Load(sub_path);

                    string tex_path = Path.Combine(GetHairKitPath(), @"image_kami\KIT_BASE_19.bmp");
                    TSOTex colorTex;
                    if (texmap.TryGetValue(shader.ColorTexName, out colorTex))
                    {
                        colorTex.Name = "Kami_Tex";
                        colorTex.Load(tex_path);
                    }
                }
                if (re_housen.IsMatch(sub.Name))
                {
                    string sub_path = Path.Combine(GetHairKitPath(), @"Cgfx_kage\Cgfxkami_19");
                    sub.Load(sub_path);

                    string tex_path = Path.Combine(GetHairKitPath(), @"image_kage\KIT_KAGE_19.bmp");
                    TSOTex colorTex;
                    if (texmap.TryGetValue(shader.ColorTexName, out colorTex))
                    {
                        colorTex.Name = "Housen_Tex";
                        colorTex.Load(tex_path);
                    }
                }
                if (re_ribon.IsMatch(sub.Name))
                {
                    string tex_path = Path.Combine(GetHairKitPath(), @"image_ribon\KIT_RIBON_19.bmp");
                    TSOTex colorTex;
                    if (texmap.TryGetValue(shader.ColorTexName, out colorTex))
                    {
                        colorTex.Load(tex_path);
                    }
                }
                Console.WriteLine("shader shade tex name {0}", shader.ShadeTexName);
                Console.WriteLine("shader color tex name {0}", shader.ColorTexName);
            }

            foreach (TSOTex tex in tso.textures)
            {
                Console.WriteLine("tex name {0} file {1}", tex.Name, tex.FileName);
            }

            tso.Save(string.Format("{0}.tso", basename));
        }
    }
}
