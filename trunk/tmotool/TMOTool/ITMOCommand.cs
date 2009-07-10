using System;
using System.Collections.Generic;

namespace TMOTool
{
    public interface ITMOCommand
    {
        Dictionary<string, TDCG.TMONode> Nodes { set; }
        void Execute();
    }
}
