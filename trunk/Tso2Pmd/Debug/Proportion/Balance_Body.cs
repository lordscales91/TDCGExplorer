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
    public class Balance_Body : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["W_Hips"];
            node.Scale1(1.00F, 0.90F, 1.00F);

            node = nodes["W_LeftHips_Dummy"];
            node.Scale(1.00F, 1.20F, 1.00F);
            node.Move(0.00F, -1.00F, 0.00F);

            node = nodes["W_LeftUpLeg"];
            node.Scale1(1.00F, 0.90F, 1.00F);

            node = nodes["W_RightHips_Dummy"];
            node.Scale(1.00F, 1.20F, 1.00F);
            node.Move(0.00F, -1.00F, 0.00F);

            node = nodes["W_RightUpLeg"];
            node.Scale1(1.00F, 0.90F, 1.00F);

            node = nodes["W_Spine_Dummy"];
            node.Scale1(1.10F, 0.90F, 1.00F);

            node = nodes["W_Spine1"];
            node.Scale1(1.10F, 0.40F, 1.20F);

            node = nodes["W_Spine2"];
            node.Scale1(1.00F, 0.70F, 1.00F);
        }
    }
}
