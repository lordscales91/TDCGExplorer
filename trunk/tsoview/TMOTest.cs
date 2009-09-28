using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

class TMOTest
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOTest.exe <tmo file>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();
        tmo.Load(source_file);
        foreach (TMONode node in tmo.nodes)
        {
            TMOMat mat = node.frame_matrices[0];

            float yaw, pitch, roll;
            TMOMat.RotationToYawPitchRoll(ref mat.m, out yaw, out pitch, out roll);
            Console.WriteLine("node {0} yaw {1:F2} pitch {2:F2} roll {3:F2}", node.ShortName, Geometry.RadianToDegree(yaw), Geometry.RadianToDegree(pitch), Geometry.RadianToDegree(roll));
        }
    }
}
