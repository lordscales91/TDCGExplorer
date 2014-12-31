using System;
using System.Collections.Generic;

namespace TDCG.TAHTool
{
    public class TAHFileUtils
    {
        public static string GetExtensionFromMagic(byte[] magic)
        {
            string ext;
            if (magic[0] == '8' && magic[1] == 'B' && magic[2] == 'P' && magic[3] == 'S')
                ext = ".psd";
            else
            if (magic[0] == 'T' && magic[1] == 'M' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tmo";
            else
            if (magic[0] == 'T' && magic[1] == 'S' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tso";
            else
            if (magic[0] == 'O' && magic[1] == 'g' && magic[2] == 'g' && magic[3] == 'S')
                ext = ".ogg";
            else
            if (magic[0] == 'B' && magic[1] == 'B' && magic[2] == 'B' && magic[3] == 'B')
                ext = ".tbn";
            else
            if (magic[0] == 0x89 && magic[1] == 'P' && magic[2] == 'N' && magic[3] == 'G')
                ext = ".png";
            else
                ext = ".cgfx";
            return ext;
        }
    }
}
