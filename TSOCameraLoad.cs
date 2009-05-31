using System;

namespace TAHdecrypt
{
    static class TSOCameraLoad
    {
        static void Main(string[] args)
        {
            TSOCamera camera = TSOCamera.Load(@"camera.xml");
            camera.Dump();
        }
    }
}
