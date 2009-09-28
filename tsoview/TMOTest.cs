using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using TDCG;

class TMOConstraintItem
{
    public string ShortName { get; set; }
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
}

class TMOTest
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOTest.exe <tmo directory>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();

        Dictionary<string, Vector3> min_dic = new Dictionary<string, Vector3>();
        Dictionary<string, Vector3> max_dic = new Dictionary<string, Vector3>();

        string[] files = Directory.GetFiles(source_file, "*.tmo");
        foreach (string file in files)
        {
            tmo.Load(file);
            foreach (TMONode node in tmo.nodes)
            {
                TMOMat mat = node.frame_matrices[0];

                float yaw, pitch, roll;
                string sname = node.ShortName;
                TMOMat.RotationToYawPitchRoll(ref mat.m, out yaw, out pitch, out roll);

                if (! min_dic.ContainsKey(sname))
                    min_dic[sname] = Vector3.Empty;
                if (! max_dic.ContainsKey(sname))
                    max_dic[sname] = Vector3.Empty;

                Vector3 min = min_dic[sname];
                Vector3 max = max_dic[sname]; 

                if (yaw < min_dic[sname].Y)
                    min.Y = yaw;
                if (yaw > max_dic[sname].Y)
                    max.Y = yaw;

                if (pitch < min_dic[sname].X)
                    min.X = pitch;
                if (pitch > max_dic[sname].X)
                    max.X = pitch;

                if (roll < min_dic[sname].Z)
                    min.Z = roll;
                if (roll > max_dic[sname].Z)
                    max.Z = roll;

                min_dic[sname] = min;
                max_dic[sname] = max;
            }
        }
        foreach (string sname in min_dic.Keys)
        {
            Console.WriteLine("node {0} yaw {1:F2}..{2:F2} pitch {3:F2}..{4:F2} roll {5:F2}..{6:F2}", sname,
                    Geometry.RadianToDegree(min_dic[sname].Y),
                    Geometry.RadianToDegree(max_dic[sname].Y),
                    Geometry.RadianToDegree(min_dic[sname].X),
                    Geometry.RadianToDegree(max_dic[sname].X),
                    Geometry.RadianToDegree(min_dic[sname].Z),
                    Geometry.RadianToDegree(max_dic[sname].Z));
        }
    }
}
