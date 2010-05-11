using System;
using System.Collections.Generic;

namespace TDCG
{
    /// <summary>
    /// ‘ÌŒ^‚ğˆµ‚¢‚Ü‚·B
    /// </summary>
    public interface IProportion
    {
        /// <summary>
        /// ‘ÌŒ^node«‘
        /// </summary>
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONode‚É•ÏŒ`ŒW”‚ğİ’è‚µ‚Ü‚·B
        /// </summary>
        void Execute();
    }
}
