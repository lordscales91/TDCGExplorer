using System;

namespace TAHdecrypt
{
    static class TSOCameraDump
    {
        static void Main(string[] args)
        {
            TSOCamera camera = new TSOCamera();
            camera.Dump();
        }
    }
}
