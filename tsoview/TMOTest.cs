using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        TMOFrame frame = tmo.frames[0];
        foreach (TMOMat mat in frame.matrices)
        {
            float yaw, pitch, roll;
            TMOMat.RotationToYawPitchRoll(ref mat.m, out yaw, out pitch, out roll);
            Console.WriteLine("yaw {0:F2} pitch {1:F2} roll {2:F2}", yaw, pitch, roll);
        }
    }
}
