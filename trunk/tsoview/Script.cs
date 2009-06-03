using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{

public class Script
{
    public void Hello(TSOSample sample)
    {
        Console.WriteLine("TSOFigureList.Count {0}", sample.TSOFigureList.Count);
        /*
        sample.LoadMotion(@"GRABIA_XP_N000.tmo");
        sample.LoadMotion(@"GRABIA_XP_N001.tmo");
        sample.LoadMotion(@"GRABIA_XP_N002.tmo");

        sample.Motion(  0, @"GRABIA_XP_N000");
        sample.Motion( 60, @"GRABIA_XP_N001");
        sample.Motion(120, @"GRABIA_XP_N002");
        sample.Motion(180, @"GRABIA_XP_N000");

        sample.Camera(  0, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f);
        sample.Camera( 60, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -20.0f);
        sample.Camera(180, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f, 60);
        */
    }
}
}
