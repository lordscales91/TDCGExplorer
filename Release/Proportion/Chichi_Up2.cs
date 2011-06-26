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
    public class Chichi_Up2 : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        public void Execute()
        {
            TPONode node;

            node = nodes["Chichi_Right1"];
            node.RotateX2(Geometry.DegreeToRadian(-10.00F));

            node = nodes["Chichi_Right2"];
            node.RotateX2(Geometry.DegreeToRadian(-5.00F));

            node = nodes["Chichi_Left1"];
            node.RotateX2(Geometry.DegreeToRadian(-10.00F));

            node = nodes["Chichi_Left2"];
            node.RotateX2(Geometry.DegreeToRadian(-5.00F));
        }
    }
}
