using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;
using TDCG;

namespace TSODeform
{
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("TSODeform.exe <tso file> <tmo file>");
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
        Vector3 pos = Vector3.Zero;
        for (int i = 0; i < 4; i++)
        {
            Matrix m = boneMatrices[v.skin_weights[i].bone_index];
            float w = v.skin_weights[i].weight;
            pos += Vector3.TransformCoordinate(v.position, m) * w;
        }
        v.position = pos;

        Vector3 nor = Vector3.Zero;
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
