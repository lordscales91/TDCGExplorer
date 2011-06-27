using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;
//css_reference Microsoft.DirectX.dll;
//css_reference Microsoft.DirectX.Direct3D.dll;
//css_reference Microsoft.DirectX.Direct3DX.dll;

namespace TDCG.Proportion
{
    public class Length_Leg : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["W_LeftUpLeg"];
            node.Move(0.00F, -0.20F, 0.00F);

            node = nodes["W_LeftUpLegRoll"];
            node.Move(0.00F, -0.20F, 0.00F);

            node = nodes["W_LeftLeg"];
            node.Move(0.00F, -0.30F, 0.00F);

            node = nodes["W_LeftLegRoll"];
            node.Move(0.00F, -0.30F, 0.00F);


            node = nodes["W_RightUpLeg"];
            node.Move(0.00F, -0.20F, 0.00F);

            node = nodes["W_RightUpLegRoll"];
            node.Move(0.00F, -0.20F, 0.00F);

            node = nodes["W_RightLeg"];
            node.Move(0.00F, -0.30F, 0.00F);

            node = nodes["W_RightLegRoll"];
            node.Move(0.00F, -0.30F, 0.00F);
        }
    }
}
