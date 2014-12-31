using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSODeform
{
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("TSODeform.exe <tso file> <tmo file> [fig file]");
            return;
        }
        string tso_file = args[0];
        string tmo_file = args[1];

        string fig_file = null;
        if (args.Length > 2)
        {
            fig_file = args[2];
        }

        TSOFile tso = new TSOFile();
        tso.Load(tso_file);

        TMOFile tmo = new TMOFile();
        tmo.Load(tmo_file);

        Figure fig = new Figure();
        fig.TSOList.Add(tso);
        fig.Tmo = tmo;

        if (fig_file != null)
        {
            List<float> ratios = ReadFloats(fig_file);
            /*
            ◆FIGU
            スライダの位置。値は float型で 0.0 .. 1.0
                0: 姉妹
                1: うで
                2: あし
                3: 胴まわり
                4: おっぱい
                5: つり目たれ目
                6: やわらか
             */
            fig.slider_matrix.TallRatio = ratios[0];
            fig.slider_matrix.ArmRatio = ratios[1];
            fig.slider_matrix.LegRatio = ratios[2];
            fig.slider_matrix.WaistRatio = ratios[3];
            fig.slider_matrix.BustRatio = ratios[4];
            fig.slider_matrix.EyeRatio = ratios[5];
        }
        fig.UpdateNodeMap();
        if (fig_file != null)
            fig.UpdateBoneMatrices(true);
        else
            fig.UpdateBoneMatricesWithoutSlider(true);

        foreach (TSOMesh mesh in tso.meshes)
        foreach (TSOSubMesh sub in mesh.sub_meshes)
        {
            Matrix[] clipped_boneMatrices = fig.ClipBoneMatrices(sub);

            for (int i = 0; i < sub.vertices.Length; i++)
            {
                CalcSkindeform(ref sub.vertices[i], clipped_boneMatrices);
            }
        }

        foreach (TSONode tso_node in tso.nodes)
        {
            TMONode tmo_node;
            if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                tso_node.TransformationMatrix = tmo_node.matrices[0].m;
        }

        tso.Save(@"out.tso");
    }

    public static void CalcSkindeform(ref Vertex v, Matrix[] boneMatrices)
    {
        Vector3 pos = Vector3.Empty;
        for (int i = 0; i < 4; i++)
        {
            Matrix m = boneMatrices[v.skin_weights[i].bone_index];
            float w = v.skin_weights[i].weight;
            pos += Vector3.TransformCoordinate(v.position, m) * w;
        }
        v.position = pos;

        Vector3 nor = Vector3.Empty;
        for (int i = 0; i < 4; i++)
        {
            Matrix m = boneMatrices[v.skin_weights[i].bone_index];
            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            float w = v.skin_weights[i].weight;
            nor += Vector3.TransformCoordinate(v.normal, m) * w;
        }
        v.normal = Vector3.Normalize(nor);
    } 

    static List<float> ReadFloats(string dest_file)
    {
        List<float> floats = new List<float>();
        string line;
        using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
        while ((line = source.ReadLine()) != null)
        {
            floats.Add(Single.Parse(line));
        }
        return floats;
    }
}
}
