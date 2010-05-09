using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOHair
{
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TSOHair.exe <tso file>");
            return;
        }
        string source_file = args[0];

        TSOFile tso = new TSOFile();
        tso.Load(source_file);

        foreach (TSOSubScript sub in tso.sub_scripts)
        {
            Console.WriteLine("sub name {0} file {1}", sub.Name, sub.File);
            Shader shader = sub.shader;
            Console.WriteLine("shader shade tex name {0}", shader.ShadeTexName);
            Console.WriteLine("shader color tex name {0}", shader.ColorTexName);
        }

        foreach (TSOTex tex in tso.textures)
        {
            Console.WriteLine("tex name {0} file {1}", tex.Name, tex.File);
        }
    }
}
}
