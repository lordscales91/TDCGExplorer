using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOSkin
{
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("TSOSkin.exe <tso file> <tmo file>");
            return;
        }
        string tso_file = args[0];
        string tmo_file = args[1];

        TSOFile tso = new TSOFile();
        tso.Load(tso_file);

        TMOFile tmo = new TMOFile();
        tmo.Load(tmo_file);

        Figure fig = new Figure();
        fig.TSOList.Add(tso);
        fig.Tmo = tmo;
        fig.UpdateNodeMapAndBoneMatrices();

        foreach (TSOMesh mesh in tso.meshes)
        foreach (TSOSubMesh sub in mesh.sub_meshes)
        {
            Matrix[] clipped_boneMatrices = new Matrix[sub.maxPalettes];

            for (int numPalettes = 0; numPalettes < sub.maxPalettes; numPalettes++)
            {
                TSONode tso_node = sub.GetBone(numPalettes);
                TMONode tmo_node;
                if (fig.nodemap.TryGetValue(tso_node, out tmo_node))
                    clipped_boneMatrices[numPalettes] = tso_node.offset_matrix * tmo_node.combined_matrix;
            }

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
}
}
