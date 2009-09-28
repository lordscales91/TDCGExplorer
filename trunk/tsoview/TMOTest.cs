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
            Console.WriteLine("TMOTest.exe <tmo directory>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();

        Dictionary<string, float> min_yaw_dic = new Dictionary<string, float>();
        Dictionary<string, float> max_yaw_dic = new Dictionary<string, float>();

        Dictionary<string, float> min_pitch_dic = new Dictionary<string, float>();
        Dictionary<string, float> max_pitch_dic = new Dictionary<string, float>();

        Dictionary<string, float> min_roll_dic = new Dictionary<string, float>();
        Dictionary<string, float> max_roll_dic = new Dictionary<string, float>();

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
                //Console.WriteLine("node {0} yaw {1:F2} pitch {2:F2} roll {3:F2}", sname, Geometry.RadianToDegree(yaw), Geometry.RadianToDegree(pitch), Geometry.RadianToDegree(roll));

                if (! min_yaw_dic.ContainsKey(sname))
                    min_yaw_dic[sname] = 0.0f;
                if (! max_yaw_dic.ContainsKey(sname))
                    max_yaw_dic[sname] = 0.0f;

                if (yaw < min_yaw_dic[sname])
                    min_yaw_dic[sname] = yaw;
                if (yaw > max_yaw_dic[sname])
                    max_yaw_dic[sname] = yaw;

                if (! min_pitch_dic.ContainsKey(sname))
                    min_pitch_dic[sname] = 0.0f;
                if (! max_pitch_dic.ContainsKey(sname))
                    max_pitch_dic[sname] = 0.0f;

                if (pitch < min_pitch_dic[sname])
                    min_pitch_dic[sname] = pitch;
                if (pitch > max_pitch_dic[sname])
                    max_pitch_dic[sname] = pitch;

                if (! min_roll_dic.ContainsKey(sname))
                    min_roll_dic[sname] = 0.0f;
                if (! max_roll_dic.ContainsKey(sname))
                    max_roll_dic[sname] = 0.0f;

                if (roll < min_roll_dic[sname])
                    min_roll_dic[sname] = roll;
                if (roll > max_roll_dic[sname])
                    max_roll_dic[sname] = roll;
            }
        }
        foreach (string sname in min_yaw_dic.Keys)
        {
            Console.WriteLine("node {0} yaw {1:F2}..{2:F2} pitch {3:F2}..{4:F2} roll {5:F2}..{6:F2}", sname,
                    Geometry.RadianToDegree(min_yaw_dic[sname]),
                    Geometry.RadianToDegree(max_yaw_dic[sname]),
                    Geometry.RadianToDegree(min_pitch_dic[sname]),
                    Geometry.RadianToDegree(max_pitch_dic[sname]),
                    Geometry.RadianToDegree(min_roll_dic[sname]),
                    Geometry.RadianToDegree(max_roll_dic[sname]));
        }
    }
}
