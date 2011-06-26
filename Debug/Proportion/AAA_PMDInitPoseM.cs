using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class AAA_PMDInitPoseM : IProportion
    {
        Dictionary<string, TPONode> nodes;
        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}

        static float DegreeToRadian(float angle)
        {
           return (float)(Math.PI * angle / 180.0);
        }

        public void Execute()
        {
                TPONode node;

                node = nodes["M_Hips"];
                node.Move(0F, 0F, 0F);
                node.RotateX(DegreeToRadian(0F));

                node = nodes["Neck"];
                node.RotateX(DegreeToRadian(0F));

                node = nodes["RightArm"];
                node.RotateZ(DegreeToRadian(40F));
                node = nodes["LeftArm"];
                node.RotateZ(DegreeToRadian(-40F));
        }
    }
}
