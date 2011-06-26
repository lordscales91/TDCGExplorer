using System;
using System.Collections.Generic;

namespace TDCG
{
    /// <summary>
    /// 体型を扱います。
    /// </summary>
    public interface IProportion
    {
        /// <summary>
        /// 体型node辞書
        /// </summary>
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONodeに変形係数を設定します。
        /// </summary>
        void Execute();
    }
}
