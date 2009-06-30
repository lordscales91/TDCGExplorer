using System;
using TAHdecrypt;

namespace TSOCameraDump
{
    static class Program
    {
        static void Main(string[] args)
        {
            TSOCamera camera = new TSOCamera();
            camera.Dump();
        }
    }
}
