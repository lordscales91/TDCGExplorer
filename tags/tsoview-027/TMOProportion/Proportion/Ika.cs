using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TDCG.Proportion
{
    public class Ika : IProportion
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
                node.Scale1( 1.2F, 0.75F, 1.2F );

                node = nodes["W_LeftUpLeg"];
                node.Scale1( 1.10F, 0.66F, 1.10F );
                node.Move( -0.150F, 0.000F, 0.000F );
                node = nodes["W_LeftUpLegRoll"];
                node.Scale1( 1.05F, 0.68F, 1.05F );
                node = nodes["W_LeftLeg"];
                node.Scale1( 1.00F, 0.63F, 1.00F );
                node = nodes["W_LeftLegRoll"];
                node.Scale1( 0.90F, 0.84F, 0.90F );
                node = nodes["W_LeftFoot"];
                node.Scale1( 0.80F, 0.81F, 0.80F );
                node = nodes["W_LeftToeBase"];
                node.Scale1( 0.70F, 0.80F, 0.70F );
                node = nodes["W_LeftToes_End"];

                node = nodes["W_RightUpLeg"];
                node.Scale1( 1.10F, 0.66F, 1.10F );
                node.Move( 0.150F, 0.000F, 0.000F );
                node = nodes["W_RightUpLegRoll"];
                node.Scale1( 1.05F, 0.68F, 1.05F );
                node = nodes["W_RightLeg"];
                node.Scale1( 1.00F, 0.63F, 1.00F );
                node = nodes["W_RightLegRoll"];
                node.Scale1( 0.90F, 0.84F, 0.90F );
                node = nodes["W_RightFoot"];
                node.Scale1( 0.80F, 0.81F, 0.80F );
                node = nodes["W_RightToeBase"];
                node.Scale1( 0.70F, 0.80F, 0.70F );
                node = nodes["W_RightToes_End"];

                node = nodes["W_Spine_Dummy"];
                node.Scale1( 1.25F, 0.70F, 1.25F );
                node = nodes["W_Spine1"];
                node.Scale1( 1.20F, 0.70F, 1.20F );
                node.Move( 0.000F,-0.090F, 0.000F );
                node = nodes["W_Spine2"];
                node.Scale1( 1.10F, 0.60F, 1.10F );
                node.Move( 0.000F,-0.100F, 0.000F );
                node = nodes["W_Spine3"];
                node.Scale1( 1.00F, 1.00F, 1.00F );
                node.Move( 0.000F,-0.065F, 0.000F );

                node = nodes["W_RightShoulder_Dummy"];
                node.Scale1( 1.00F, 0.96F, 1.00F );
                node = nodes["W_RightShoulder"];
                node.Scale1( 1.00F, 0.75F, 1.00F );
                node = nodes["W_RightArm_Dummy"];
                node.Scale1( 0.71F, 1.00F, 1.00F );
                node = nodes["W_RightArm"];
                node.Scale1( 0.76F, 1.00F, 1.00F );
                node = nodes["W_RightArmRoll"];
                node.Scale1( 0.76F, 1.00F, 1.00F );
                node = nodes["W_RightForeArm"];
                node.Scale1( 0.70F, 0.90F, 0.90F );
                node = nodes["W_RightForeArmRoll"];
                node.Scale1( 0.72F, 1.00F, 1.00F );
                node = nodes["W_RightHand"];
                node.Scale1( 0.70F, 0.87F, 0.87F );
                node = nodes["W_RightHandPinky1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandPinky2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandPinky3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandPinky4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandRing1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandRing2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandRing3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandRing4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandMiddle1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandMiddle2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandMiddle3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandMiddle4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandIndex1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandIndex2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandIndex3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandIndex4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandThumb1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandThumb2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandThumb3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_RightHandThumb4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );

                node = nodes["W_LeftShoulder_Dummy"];
                node.Scale1( 1.00F, 0.96F, 1.00F );
                node = nodes["W_LeftShoulder"];
                node.Scale1( 1.00F, 0.75F, 1.00F );
                node = nodes["W_LeftArm_Dummy"];
                node.Scale1( 0.71F, 1.00F, 1.00F );
                node = nodes["W_LeftArm"];
                node.Scale1( 0.76F, 1.00F, 1.00F );
                node = nodes["W_LeftArmRoll"];
                node.Scale1( 0.76F, 1.00F, 1.00F );
                node = nodes["W_LeftForeArm"];
                node.Scale1( 0.70F, 0.90F, 0.90F );
                node = nodes["W_LeftForeArmRoll"];
                node.Scale1( 0.72F, 1.00F, 1.00F );
                node = nodes["W_LeftHand"];
                node.Scale1( 0.70F, 0.87F, 0.87F );
                node = nodes["W_LeftHandPinky1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandPinky2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandPinky3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandPinky4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandRing1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandRing2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandRing3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandRing4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandMiddle1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandMiddle2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandMiddle3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandMiddle4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandIndex1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandIndex2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandIndex3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandIndex4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandThumb1"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandThumb2"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandThumb3"];
                node.Scale1( 0.70F, 1.00F, 1.00F );
                node = nodes["W_LeftHandThumb4"];
                node.Scale1( 0.70F, 1.00F, 1.00F );

                node = nodes["W_Neck"];
                node.Scale1( 1.00F, 0.72F, 1.00F );
                node = nodes["face_oya"];
                node.Scale1( 1.20F, 0.94F, 1.20F );
                node.Move( 0.000F,-0.250F, 0.000F );
                node = nodes["sitakuti_oya"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["Ha_Down"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["sitakuti_l_1"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["sitakuti_r_1"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["sita_01"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["sita_02"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["sita_03"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["kutiyoko_r"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["kutiyoko_l"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["uekuti_oya"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["Ha_UP"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["uekuti_l_1"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["uekuti_r_1"];
                node.Scale1( 1.20F, 1.05F, 1.00F );
                node = nodes["Kami_Oya"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid1_L"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid2_L"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid3_L"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid1_R"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid2_R"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["kami_Front_Mid3_R"];
                node.Scale1( 1.20F, 1.05F, 1.20F );
                node = nodes["Chichi_Right1"];
                node.Scale1( 0.90F, 1.00F, 0.91F );
                node = nodes["Chichi_Left1"];
                node.Scale1( 0.90F, 1.00F, 0.91F );

                node = nodes["eyeline_sita_R"];
                node.Scale1(1.20F, 1.05F, 1.00F);
                node = nodes["eyeline_sita_R_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["eyeline_sita_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["eyeline_sita_L_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_oya_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_2_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_3_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_3_L_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_2_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_3_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["L_eyeline_3_R_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_oya_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_2_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_3_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_3_L_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_2_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_3_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["R_eyeline_3_R_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["hana"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["hana_END"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_1_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_2_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_3_R"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_3_R_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_1_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_2_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_3_L"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["mayu_3_L_end"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_Futi"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_End"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_HighLight_A"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_HighLight_A_END"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_HighLight_B"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_HighLight_B_END"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Right_Futi"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Right"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Right_End"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_RighLight_A"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_RighLight_A_END"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_RighLight_B"];
                node.Scale1( 1.20F,1.05F,1.00F );
                node = nodes["Me_Left_RighLight_B_END"];
                node.Scale1( 1.20F,1.05F,1.00F );
        }
    }
}
