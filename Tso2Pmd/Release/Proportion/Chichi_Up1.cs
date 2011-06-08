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
    public class Chichi_Up1 : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["Chichi_Right1"];
            node.Scale(1.04F, 1.04F, 0.91F);
            node.Move(-0.03F, 0.36F, -0.03F);

            node = nodes["Chichi_Right2"];
            node.Scale(1.04F, 1.04F, 0.98F);
            node.Move(0.00F, 0.16F, 0.00F);

            node = nodes["Chichi_Right3"];
            node.Move(0.06F, 0.06F, 0.00F);

            node = nodes["Chichi_Right4"];
            node.Move(0.00F, -0.10F, 0.00F);

            node = nodes["Chichi_Left1"];
            node.Move(0.03F, 0.36F, -0.03F);
            node.Scale1(1.04F, 1.04F, 0.91F);

            node = nodes["Chichi_Left2"];
            node.Move(0.00F, 0.16F, 0.00F);
            node.Scale1(1.04F, 1.04F, 0.98F);

            node = nodes["Chichi_Left3"];
            node.Move(-0.06F, 0.06F, 0.00F);

            node = nodes["Chichi_Left4"];
            node.Move(0.00F, -0.10F, 0.00F);
        }
    }
}
