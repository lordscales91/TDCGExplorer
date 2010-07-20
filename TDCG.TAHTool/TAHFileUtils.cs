using System;
using System.Collections.Generic;

namespace TDCG.TAHTool
{
    public class TAHFileUtils
    {
        public static string GetExtensionFromMagic(byte[] data_output)
        {
            string ext;
            string magic = System.Text.Encoding.ASCII.GetString(data_output, 0, 4);
            switch (magic)
            {
                case "8BPS":
                    ext = ".psd";
                    break;
                case "TMO1":
                    ext = ".tmo";
                    break;
                case "TSO1":
                    ext = ".tso";
                    break;
                case "OggS":
                    ext = ".ogg";
                    break;
                case "BBBB":
                    ext = ".tbn";
                    break;
                default:
                    ext = ".cgfx";
                    break;
            }
            return ext;
        }
    }
}
