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
    public class Length_Arm : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["W_LeftArmRoll"];
            node.Move(0.3F, 0.00F, 0.00F);

            node = nodes["W_LeftForeArmRoll"];
            node.Move(0.3F, 0.00F, 0.00F);

            node = nodes["W_RightArmRoll"];
            node.Move(-0.3F, 0.00F, 0.00F);

            node = nodes["W_RightForeArmRoll"];
            node.Move(-0.3F, 0.00F, 0.00F);
        }
    }
}
