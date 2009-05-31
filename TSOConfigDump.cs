using System;

namespace TAHdecrypt
{
    static class TSOConfigDump
    {
        static void Main(string[] args)
        {
            TSOConfig config = new TSOConfig();
            config.Dump();
        }
    }
}
