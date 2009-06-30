using System;
using TDCG.Camera;

namespace TSOCameraLoad
{
    static class Program
    {
        static void Main(string[] args)
        {
            TSOCamera camera = TSOCamera.Load(@"camera.xml");
            camera.Dump();
        }
    }
}
