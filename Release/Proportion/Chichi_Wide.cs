using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class Chichi_Wide : IProportion
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

            node = nodes["Chichi_Right1"];
            node.RotateY(DegreeToRadian(-5.0F));

            node = nodes["Chichi_Right2"];
            node.RotateY(DegreeToRadian(-2.5F));

            node = nodes["Chichi_Left1"];
            node.RotateY(DegreeToRadian(5.0F));

            node = nodes["Chichi_Left2"];
            node.RotateY(DegreeToRadian(2.5F));
        }
    }
}
