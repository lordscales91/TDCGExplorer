using System;
using TDCG;

namespace CameraLoad
{
    static class Program
    {
        static void Main(string[] args)
        {
            Camera camera = Camera.Load(@"camera.xml");
            camera.Dump();
        }
    }
}
