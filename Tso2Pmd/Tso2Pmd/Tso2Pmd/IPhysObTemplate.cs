using System;
using System.Collections.Generic;
using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
    /// <summary>
    /// 物理オブジェクトを扱います。
    /// </summary>
    public interface IPhysObTemplate
    {
        /// <summary>
        /// 物理オブジェクトnode辞書
        /// </summary>
        //T2PPhysObjectList PhysOb { set; }

        /// <summary>
        /// TPONodeに変形係数を設定します。
        /// </summary>
        void Execute(ref T2PPhysObjectList physOb_list);
    }
}
