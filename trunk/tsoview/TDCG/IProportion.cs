using System;
using System.Collections.Generic;

namespace TDCG
{
    public interface IProportion
    {
        Dictionary<string, TPONode> Nodes { set; }
        void Execute();
    }
}
