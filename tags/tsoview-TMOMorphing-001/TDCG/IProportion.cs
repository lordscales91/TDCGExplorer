using System;
using System.Collections.Generic;

namespace TDCG
{
    /// <summary>
    /// �̌^�������܂��B
    /// </summary>
    public interface IProportion
    {
        /// <summary>
        /// �̌^node����
        /// </summary>
        Dictionary<string, TPONode> Nodes { set; }

        /// <summary>
        /// TPONode�ɕό`�W����ݒ肵�܂��B
        /// </summary>
        void Execute();
    }
}
