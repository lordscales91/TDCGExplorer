using System;
using System.Collections.Generic;

namespace TDCG
{
    public interface IProportion
    {
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONode‚É•ÏŒ`ŒW”‚ğİ’è‚·‚éB
        /// </summary>
        void Execute();
    }
}
