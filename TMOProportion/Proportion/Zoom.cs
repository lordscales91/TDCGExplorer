using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class Zoom : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            float scale = 1.25F;
            float inv_scale = 1.0F/scale;

            node = nodes["W_Hips"];
            node.Scale(scale, scale, scale);

            node = nodes["W_Neck"];
            node.Scale(inv_scale, inv_scale, inv_scale);
        }
    }
}
