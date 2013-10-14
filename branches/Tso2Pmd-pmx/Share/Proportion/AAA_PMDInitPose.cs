using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class AAA_PMDInitPose : IProportion
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

            node = nodes["W_Hips"];
            node.Move(0F, 0F, 0F);
            node.RotateX2(DegreeToRadian(3F));

            node = nodes["W_Neck"];
            node.RotateX2(DegreeToRadian(-9F));

            node = nodes["W_RightShoulder"];
            node.RotateZ2(DegreeToRadian(19F));
            node = nodes["W_RightArm"];
            node.RotateZ2(DegreeToRadian(21F));
            node = nodes["W_RightHand"];
            node.RotateZ2(DegreeToRadian(5F));

            node = nodes["W_LeftShoulder"];
            node.RotateZ2(DegreeToRadian(-19F));
            node = nodes["W_LeftArm"];
            node.RotateZ2(DegreeToRadian(-21F));
            node = nodes["W_LeftHand"];
            node.RotateZ2(DegreeToRadian(-5F));

            node = nodes["Me_Left_HighLight_A"];
            node.RotateX(DegreeToRadian(1F));
            node.RotateY(DegreeToRadian(-28F));
            node = nodes["Me_Left_HighLight_B"];
            node.RotateY(DegreeToRadian(45F));
            
            node = nodes["W_LeftLegRoll"];
            node.RotateY(DegreeToRadian(10.00F));
            node = nodes["W_RightLegRoll"];
            node.RotateY(DegreeToRadian(-10.00F));
        }
    }
}
