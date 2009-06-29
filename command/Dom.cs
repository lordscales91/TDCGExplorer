using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TAHdecrypt;

namespace TDCG.TMOTool.Command
{
    public class Dom : ITMOCommand
    {
        Dictionary<string, TMONode> nodes;
        public Dictionary<string, TMONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TMONode node;

            node = nodes["W_Hips"];
            node.Scale1(1.3F, 1.0F, 1.4F);

            node = nodes["W_LeftUpLeg"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_LeftUpLegRoll"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_LeftLeg"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_RightUpLeg"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_RightUpLegRoll"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_RightLeg"];
            node.Scale1(1.3F, 1.0F, 1.2F);

            node = nodes["W_Spine_Dummy"];
            node.Scale1(1.3F, 1.0F, 1.3F);
        }
    }
}
