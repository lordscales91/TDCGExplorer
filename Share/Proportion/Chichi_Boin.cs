using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class Chichi_Boin : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; } }

        static float DegreeToRadian(float angle)
        {
           return (float)(Math.PI * angle / 180.0);
        }

        public void Execute()
        {
            TPONode node;

            node = nodes["Chichi_Right2"];
            node.Scale(1.8F, 1.8F, 2.2F);

            node = nodes["Chichi_Left2"];
            node.Scale(1.8F, 1.8F, 2.2F);
        }
    }
}
