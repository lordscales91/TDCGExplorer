using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TMOTool.Command
{
    public class Boin : IProportion
    {
        Dictionary<string, TDCG.TPONode> nodes;
        public Dictionary<string, TDCG.TPONode> Nodes { set { nodes = value; }}

        static float DegreeToRadian(float angle)
        {
           return (float)(Math.PI * angle / 180.0);
        }

        public void Execute()
        {
            TDCG.TPONode node;

            node = nodes["Chichi_Right1"];
            node.RotateY(DegreeToRadian(-5));
            node.RotateX(DegreeToRadian(5));

            node = nodes["Chichi_Left1"];
            node.RotateY(DegreeToRadian(10));
            node.RotateX(DegreeToRadian(10));

            node = nodes["Chichi_Right2"];
            node.Scale(1.8F, 1.8F, 2.2F);

            node = nodes["Chichi_Left2"];
            node.Scale(1.8F, 1.8F, 2.2F);

            node = nodes["W_Spine_Dummy"];
            node.Scale1(1.1F, 1.0F, 1.1F);

            node = nodes["W_Spine1"];
            node.Scale1(1.2F, 1.0F, 1.2F);

            node = nodes["W_Spine2"];
            node.Scale1(1.3F, 1.0F, 1.3F);

            node = nodes["W_Spine3"];
            node.Scale1(1.1F, 1.0F, 1.1F);
        }
    }
}
