using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class Type_Kemo : IProportion
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
                node.Scale1(1.08F, 0.977F, 0.977F);
                node = nodes["skirt_LeftB02"];
                node.RotateX(DegreeToRadian(-9F));
                node = nodes["skirt_LeftB03"];
                node.RotateX(DegreeToRadian(-9F));
                node = nodes["skirt_RightB02"];
                node.RotateX(DegreeToRadian(-9F));
                node = nodes["skirt_RightB03"];
                node.RotateX(DegreeToRadian(-9F));
                node = nodes["W_LeftHips_Dummy"];
                node.Scale1(1F, 1F, 1F);
                node = nodes["W_LeftUpLeg"];
                node.Move(0F, -0.01F, 0.059F);
                node.Scale1(0.984F, 1F, 0.959F);
                node = nodes["W_LeftUpLegRoll"];
                node.Move(0F, 0.164F, 0F);
                node.Scale1(0.974F, 1F, 0.812F);
                node = nodes["W_LeftLeg"];
                node.Move(0F, 0.223F, 0F);
                node.Scale1(1.082F, 1F, 0.970F);
                node = nodes["W_LeftLegRoll"];
                node.Move(0F, -0.6F, 0F);
                node.Scale1(0.897F, 1F, 0.993F);
                node = nodes["W_LeftFoot"];
                node.Scale1(1F, 1F, 1F);
                node = nodes["W_RightHips_Dummy"];
                node.Scale1(1F, 1F, 1F);
                node = nodes["W_RightUpLeg"];
                node.Move(0F, -0.01F, 0.059F);
                node.Scale1(0.984F, 1F, 0.959F);
                node = nodes["W_RightUpLegRoll"];
                node.Move(0F, 0.164F, 0F);
                node.Scale1(0.974F, 1F, 0.812F);
                node = nodes["W_RightLeg"];
                node.Move(0F, 0.223F, 0F);
                node.Scale1(1.082F, 1F, 0.970F);
                node = nodes["W_RightLegRoll"];
                node.Move(0F, -0.6F, 0F);
                node.Scale1(0.897F, 1F, 0.993F);
                node = nodes["W_RightFoot"];
                node.Scale1(1F, 1F, 1F);
                node = nodes["W_Spine_Dummy"];
                node.Move(0F, 0F, 0F);
                node.Scale1(1.16F, 0.978F, 0.978F);
                node = nodes["W_Spine1"];
                node.Move(0F, 0.201F, 0.084F);
                node.Scale1(0.878F, 1.052F, 1.052F);
                node = nodes["W_Spine2"];
                node.Move(0F, 0.105F, -0.196F);
                node.Scale1(1.025F, 0.837F, 0.837F);
                node = nodes["W_Spine3"];
                node.Move(0F, 0F, 0F);
                node.Scale1(0.967F, 0.864F, 0.864F);
                node = nodes["W_RightShoulder_Dummy"];
                node.Move(0F, 0.026F, 0F);
                node.Scale1(1F, 0.938F, 0.890F);
                node.RotateZ(DegreeToRadian(1.719F));
                node = nodes["W_RightShoulder"];
                node.Move(0F, -0.05F, 0F);
                node.Scale1(1F, 1F, 0.870F);
                node.RotateZ(DegreeToRadian(-2.292F));
                node = nodes["W_RightArm_Dummy"];
                node.Move(0F, 0F, 0F);
                node = nodes["W_RightArm"];
                node.Scale1(1F, 0.960F, 1F);
                node.RotateZ(DegreeToRadian(-0.573F));
                node = nodes["W_RightArmRoll"];
                node.Scale1(1F, 1.010F, 1F);
                node = nodes["W_RightForeArm"];
                node.Scale1(1F, 0.910F, 1.004F);
                node = nodes["W_RightForeArmRoll"];
                node.Move(0F, 0F, 0F);
                node = nodes["W_RightHand"];
                node.Scale1(1F, 0.915F, 0.920F);
                node = nodes["W_RightHandPinky1"];
                node.Scale1(1F, 1F, 1F);
                node.RotateZ(DegreeToRadian(3.438F));
                node = nodes["W_RightHandPinky2"];
                node.Scale1(1F, 0.910F, 1F);
                node.RotateZ(DegreeToRadian(-3.438F));
                node = nodes["W_RightHandPinky3"];
                node.Scale1(1F, 0.900F, 1F);
                node = nodes["W_RightHandRing1"];
                node.Scale1(1F, 0.880F, 1F);
                node = nodes["W_RightHandRing2"];
                node.Scale1(1F, 0.940F, 1F);
                node = nodes["W_RightHandRing3"];
                node.Move(0F, 0F, -0.03F);
                node.Scale1(1F, 0.970F, 1F);
                node.RotateX(DegreeToRadian(9.921F));
                node.RotateY(DegreeToRadian(5.880F));
                node.RotateZ(DegreeToRadian(-3.607F));
                node = nodes["W_RightHandMiddle1"];
                node.Scale1(1F, 0.860F, 1F);
                node = nodes["W_RightHandMiddle2"];
                node.Scale1(1F, 0.772F, 1F);
                node = nodes["W_RightHandMiddle3"];
                node.Scale1(1F, 0.695F, 1F);
                node.RotateX(DegreeToRadian(-5.157F));
                node = nodes["W_RightHandIndex1"];
                node.Move(0F, -0.1F, 0F);
                node.Scale1(1F, 0.930F, 1.090F);
                node.RotateZ(DegreeToRadian(-4.584F));
                node = nodes["W_RightHandIndex2"];
                node.Move(0F, 0.1F, 0F);
                node.Scale1(1F, 0.960F, 1F);
                node.RotateZ(DegreeToRadian(9.740F));
                node = nodes["W_RightHandIndex3"];
                node.Scale1(1F, 0.820F, 1F);
                node.RotateX(DegreeToRadian(9.167F));
                node = nodes["W_RightHandThumb2"];
                node.Scale1(1F, 0.940F, 0.912F);
                node.RotateX(DegreeToRadian(0F));
                node.RotateY(DegreeToRadian(0F));
                node.RotateZ(DegreeToRadian(0F));
                node = nodes["W_RightHandThumb3"];
                node.Scale1(1F, 1F, 0.900F);
                node = nodes["W_LeftShoulder_Dummy"];
                node.Move(0F, 0.026F, 0F);
                node.Scale1(1F, 0.938F, 0.890F);
                node.RotateZ(DegreeToRadian(-1.719F));
                node = nodes["W_LeftShoulder"];
                node.Move(0F, -0.05F, 0F);
                node.Scale1(1F, 1F, 0.870F);
                node.RotateZ(DegreeToRadian(2.292F));
                node = nodes["W_LeftArm_Dummy"];
                node.Move(0F, 0F, 0F);
                node = nodes["W_LeftArm"];
                node.Scale1(1F, 0.960F, 1F);
                node.RotateZ(DegreeToRadian(0.573F));
                node = nodes["W_LeftArmRoll"];
                node.Scale1(1F, 1.010F, 1F);
                node = nodes["W_LeftForeArm"];
                node.Scale1(1F, 0.910F, 1.004F);
                node = nodes["W_LeftForeArmRoll"];
                node.Move(0F, 0F, 0F);
                node = nodes["W_LeftHand"];
                node.Scale1(1F, 0.915F, 0.920F);
                node = nodes["W_LeftHandPinky1"];
                node.Scale1(1F, 1F, 1F);
                node.RotateZ(DegreeToRadian(-3.438F));
                node = nodes["W_LeftHandPinky2"];
                node.Scale1(1F, 0.910F, 1F);
                node.RotateZ(DegreeToRadian(3.438F));
                node = nodes["W_LeftHandPinky3"];
                node.Scale1(1F, 0.900F, 1F);
                node = nodes["W_LeftHandRing1"];
                node.Scale1(1F, 0.880F, 1F);
                node = nodes["W_LeftHandRing2"];
                node.Scale1(1F, 0.940F, 1F);
                node = nodes["W_LeftHandRing3"];
                node.Move(0F, 0F, -0.03F);
                node.Scale1(1F, 0.970F, 1F);
                node.RotateX(DegreeToRadian(9.921F));
                node.RotateY(DegreeToRadian(-5.880F));
                node.RotateZ(DegreeToRadian(3.607F));
                node = nodes["W_LeftHandMiddle1"];
                node.Scale1(1F, 0.860F, 1F);
                node = nodes["W_LeftHandMiddle2"];
                node.Scale1(1F, 0.772F, 1F);
                node = nodes["W_LeftHandMiddle3"];
                node.Scale1(1F, 0.695F, 1F);
                node.RotateX(DegreeToRadian(-5.157F));
                node = nodes["W_LeftHandIndex1"];
                node.Move(0F, -0.1F, 0F);
                node.Scale1(1F, 0.930F, 1.090F);
                node.RotateZ(DegreeToRadian(4.584F));
                node = nodes["W_LeftHandIndex2"];
                node.Move(0F, 0.1F, 0F);
                node.Scale1(1F, 0.960F, 1F);
                node.RotateZ(DegreeToRadian(-9.740F));
                node = nodes["W_LeftHandIndex3"];
                node.Scale1(1F, 0.820F, 1F);
                node.RotateX(DegreeToRadian(9.167F));
                node = nodes["W_LeftHandThumb2"];
                node.Scale1(1F, 0.940F, 0.912F);
                node = nodes["W_LeftHandThumb3"];
                node.Scale1(1F, 1F, 0.900F);
                node = nodes["W_Neck"];
                node.Move(0F, 0.1F, 0F);
                node.Scale1(1F, 1F, 1F);
                node = nodes["Head"];
                node.Move(0F, 0F, 0F);
                node.Scale1(1F, 1F, 1F);
                node = nodes["face_oya"];
                node.Move(0F, 0F, 0F);
                node = nodes["Chichi_Right1"];
                node.Move(0.025F, 0F, 0F);
                node.Scale1(0.994F, 0.949F, 0.737F);
                node.RotateX(DegreeToRadian(-1.234F));
                node.RotateY(DegreeToRadian(15.817F));
                node.RotateZ(DegreeToRadian(-1.279F));
                node = nodes["Chichi_Right2"];
                node.Move(0.06F, 0F, 0F);
                node.Scale1(1.111F, 1.037F, 1.082F);
                node.RotateX(DegreeToRadian(-1.384F));
                node.RotateY(DegreeToRadian(-17.798F));
                node.RotateZ(DegreeToRadian(8.949F));
                node = nodes["Chichi_Right3"];
                node.Move(-0.1F, 0F, -0.1F);
                node.Scale1(1F, 1F, 1.297F);
                node.RotateX(DegreeToRadian(-9.577F));
                node.RotateY(DegreeToRadian(4.577F));
                node.RotateZ(DegreeToRadian(3.594F));
                node = nodes["Chichi_Right4"];
                node.Scale1(1.000F, 1.000F, 1.321F);
                node.RotateX(DegreeToRadian(-11.459F));
                node = nodes["Chichi_Left1"];
                node.Move(-0.025F, 0F, 0F);
                node.Scale1(0.994F, 0.949F, 0.737F);
                node.RotateX(DegreeToRadian(-1.234F));
                node.RotateY(DegreeToRadian(-15.817F));
                node.RotateZ(DegreeToRadian(1.279F));
                node = nodes["Chichi_Left2"];
                node.Move(-0.06F, 0F, 0F);
                node.Scale1(1.111F, 1.037F, 1.082F);
                node.RotateX(DegreeToRadian(-1.384F));
                node.RotateY(DegreeToRadian(17.798F));
                node.RotateZ(DegreeToRadian(-8.949F));
                node = nodes["Chichi_Left3"];
                node.Move(0.1F, 0F, -0.1F);
                node.Scale1(1F, 1F, 1.297F);
                node.RotateX(DegreeToRadian(-9.577F));
                node.RotateY(DegreeToRadian(-4.577F));
                node.RotateZ(DegreeToRadian(-3.594F));
                node = nodes["Chichi_Left4"];
                node.Scale1(1.000F, 1.000F, 1.321F);
                node.RotateX(DegreeToRadian(-11.459F));
        }
    }
}
