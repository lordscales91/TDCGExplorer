using System;
using System.Collections.Generic;

namespace TDCG
{
    public interface IProportion
    {
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONode�ɕό`�W����ݒ肷��B
        /// </summary>
        void Execute();
    }
}
