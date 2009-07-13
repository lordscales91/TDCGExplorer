using System;
using System.Collections.Generic;

namespace TMOTool
{
    public interface IProportion
    {
        Dictionary<string, TDCG.TPONode> Nodes { set; }
        void Execute();
    }
}
