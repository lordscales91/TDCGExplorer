using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{

public class Script
{
    public void Hello(Viewer viewer)
    {
        Console.WriteLine("FigureList.Count {0}", viewer.FigureList.Count);
        /*
        viewer.LoadMotion(@"viewer.tmo");

        Figure fig;
        if (viewer.TryGetFigure(out fig))
        {
            fig.SetMotion(  0, viewer.GetMotion("viewer"));
        }

        TSOCamera cam = viewer.Camera;
        cam.SetMotion(  0, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f);
        cam.SetMotion( 60, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -20.0f);
        cam.SetMotion(180, 0.0f, +10.0f, 0.0f, 0.0f, +20.0f, -40.0f, 60);
        */
    }
}
}
