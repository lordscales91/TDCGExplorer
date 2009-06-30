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
        sample.LoadMotion(@"sample.tmo");

        TSOFigure fig;
        if (sample.TryGetFigure(out fig))
        {
            fig.SetMotion(  0, sample.GetMotion("sample"));
        }

        TSOCamera cam = sample.Camera;
        cam.SetMotion(  0, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f);
        cam.SetMotion( 60, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -20.0f);
        cam.SetMotion(180, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f, 60);
        */
    }
}
}
