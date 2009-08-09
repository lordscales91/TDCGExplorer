using System;
using System.Collections.Generic;

namespace TDCG
{
    public interface IProportion
    {
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONodeに変形係数を設定する。
        /// </summary>
        void Execute();
    }
}
