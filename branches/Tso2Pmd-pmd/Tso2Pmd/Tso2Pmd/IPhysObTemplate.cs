using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tso2Pmd
{
    /// <summary>
    /// 物理オブジェクトを扱います。
    /// </summary>
    public interface IPhysObTemplate
    {
        /// <summary>
        /// 枠表示名
        /// </summary>
        string Name();

        /// <summary>
        /// 枠表示グループ
        /// </summary>
        int Group();

        /// <summary>
        /// 物理オブジェクトを変更する。
        /// </summary>
        void Execute(ref T2PPhysObjectList physOb_list);
    }
}
