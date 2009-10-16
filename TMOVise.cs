using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

public static class TMOVise
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOVise.exe <tmo file>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();
        tmo.Load(source_file);
        TMOConstraint constraint = TMOConstraint.Load(@"ypr-GRABIA.xml");

        foreach (TMONode node in tmo.nodes)
        {
            TMOMat mat = node.frame_matrices[0];

            float yaw, pitch, roll;
            string sname = node.ShortName;
            Vector3 scaling;
            Vector3 translation = TMOMat.DecomposeMatrix(ref mat.m, out scaling);
            TMOMat.RotationToYawPitchRoll(ref mat.m, out yaw, out pitch, out roll);

            TMOConstraintItem item = constraint.GetItem(sname);

            if (yaw < item.Min.Y)
                yaw = item.Min.Y;
            if (yaw > item.Max.Y)
                yaw = item.Max.Y;

            if (pitch < item.Min.X)
                pitch = item.Min.X;
            if (pitch > item.Max.X)
                pitch = item.Max.X;

            if (roll < item.Min.Z)
                roll = item.Min.Z;
            if (roll > item.Max.Z)
                roll = item.Max.Z;

            mat.m = Matrix.Scaling(scaling) * Matrix.RotationYawPitchRoll(yaw, pitch, roll) * Matrix.Translation(translation);
        }
        tmo.Save(@"out.tmo");
    }
}
