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
    }
}
}
