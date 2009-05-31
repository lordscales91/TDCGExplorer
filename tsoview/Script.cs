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
        sample.LoadTMOFile(@"teni-1.tmo");
        sample.LoadTMOFile(30, @"teni-2.tmo");
        sample.LoadTMOFile(60, @"teni-3.tmo");
        sample.LoadTMOFile(90, @"teni-4.tmo");
        */
        sample.LookAt(  0, new Vector3(0.0f, +10.0f, 0.0f), new Vector3(0.0f, +20.0f, -40.0f));
        sample.LookAt( 90, new Vector3(0.0f, +10.0f, 0.0f), new Vector3(0.0f, +20.0f, -20.0f));
        sample.LookAt(180, new Vector3(0.0f, +10.0f, 0.0f), new Vector3(0.0f, +20.0f, -40.0f));
    }
}
}
