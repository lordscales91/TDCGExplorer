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
    public class Balance_Leg : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["W_LeftHips_Dummy"];
            node.Scale(1.00F, 1.40F, 1.00F);

            node = nodes["W_LeftUpLeg"];
            node.Scale1(1.00F, 0.40F, 1.00F);

            node = nodes["W_LeftUpLegRoll"];
            node.Scale1(1.00F, 0.70F, 1.00F);

            node = nodes["W_LeftLegRoll"];
            node.Scale1(1.00F, 0.80F, 1.00F);

            node = nodes["W_LeftFoot"];
            node.Scale(1.00F, 0.70F, 1.00F);

            node = nodes["W_RightHips_Dummy"];
            node.Scale(1.00F, 1.40F, 1.00F);

            node = nodes["W_RightUpLeg"];
            node.Scale1(1.00F, 0.40F, 1.00F);

            node = nodes["W_RightUpLegRoll"];
            node.Scale1(1.00F, 0.70F, 1.00F);

            node = nodes["W_RightLegRoll"];
            node.Scale1(1.00F, 0.80F, 1.00F);

            node = nodes["W_RightFoot"];
            node.Scale(1.00F, 0.70F, 1.00F);
        }
    }
}
