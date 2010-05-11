using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TDCG;

namespace TDCG.TSOHair
{
    public class TSOHairProcessor
    {
        public enum Type { Unknown, Kami, Housen, Ribbon };

        static Regex re_kami = new Regex(@"kami", RegexOptions.IgnoreCase);
        static Regex re_housen = new Regex(@"housen|w_faceparts", RegexOptions.IgnoreCase);
        static Regex re_ribbon = new Regex(@"ribb?on", RegexOptions.IgnoreCase);

        public static string GetHairKitPath()
        {
            return Path.Combine(Application.StartupPath, @"HAIR_KIT");
        }

        public static string GetKamiSubPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kami\Cgfxkami_{0}", col));
        }

        public static string GetKageSubPath(string col)
        {
            return Path.Combine(GetHairKitPath(), string.Format(@"Cgfx_kage\Cgfxkage_{0}", col));
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
                Type type = Type.Unknown;

                Shader shader = sub.shader;
                string color_tex_name = shader.ColorTexName;
                string shade_tex_name = shader.ShadeTexName;

                // Housen ÇÕ Kami ÇÊÇËóDêÊÇ∑ÇÈÅB
                // ex. KamiHousen ÇÃèÍçáÇÕ Housen Ç∆ÇµÇƒèàóù
                if (re_housen.IsMatch(sub.Name))
                {
                    type = Type.Housen;
                }
                else
                if (re_ribbon.IsMatch(sub.Name))
                {
                    type = Type.Ribbon;
                }
                else
                if (re_kami.IsMatch(sub.Name))
                {
                    type = Type.Kami;
                }
                else
                {
                    TSOTex colorTex;
                    if (texmap.TryGetValue(color_tex_name, out colorTex))
                    {
                        string file = colorTex.FileName.Trim('"');

                        if (re_housen.IsMatch(file))
                        {
                            type = Type.Housen;
                        }
                        else
                        if (re_ribbon.IsMatch(file))
                        {
                            type = Type.Ribbon;
                        }
                        else
                        if (re_kami.IsMatch(file))
                        {
                            type = Type.Kami;
                        }
                    }
                }
                Console.WriteLine("    : type {0}", type);

                switch (type)
                {
                    case Type.Kami:
                        {
                            sub.Load(GetKamiSubPath(col));

                            TSOTex colorTex;
                            if (texmap.TryGetValue(color_tex_name, out colorTex))
                            {
                                colorTex.Load(GetKamiTexPath(col));
                            }
                        }
                        break;
                    case Type.Housen:
                        {
                            sub.Load(GetKageSubPath(col));

                            TSOTex colorTex;
                            if (texmap.TryGetValue(color_tex_name, out colorTex))
                            {
                                colorTex.Load(GetKageTexPath(col));
                            }
                        }
                        break;
                    case Type.Ribbon:
                        {
                            TSOTex colorTex;
                            if (texmap.TryGetValue(color_tex_name, out colorTex))
                            {
                                colorTex.Load(GetRibonTexPath(col));
                            }
                        }
                        break;
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
        }
    }
}
