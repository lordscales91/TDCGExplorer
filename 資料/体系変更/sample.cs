using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class sample : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;
			;
        }
    }
}
