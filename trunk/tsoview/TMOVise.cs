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
        TMOConstraint constraint = TMOConstraint.Load(@"angle-GRABIA.xml");

        foreach (TMONode node in tmo.nodes)
        {
            TMOMat mat = node.frame_matrices[0];

            string sname = node.ShortName;
            Vector3 scaling;
            Vector3 translation = TMOMat.DecomposeMatrix(ref mat.m, out scaling);
            Vector3 angle = TMOMat.ToAngle(mat.m);

            TMOConstraintItem item = constraint.GetItem(sname);

            if (angle.X < item.Min.X)
                angle.X = item.Min.X;
            if (angle.X > item.Max.X)
                angle.X = item.Max.X;

            if (angle.Y < item.Min.Y)
                angle.Y = item.Min.Y;
            if (angle.Y > item.Max.Y)
                angle.Y = item.Max.Y;

            if (angle.Z < item.Min.Z)
                angle.Z = item.Min.Z;
            if (angle.Z > item.Max.Z)
                angle.Z = item.Max.Z;

            Quaternion q = TMOMat.ToQuaternion(angle);
            mat.m = Matrix.Scaling(scaling) * Matrix.RotationQuaternion(q) * Matrix.Translation(translation);
        }
        tmo.Save(@"out.tmo");
    }
}
