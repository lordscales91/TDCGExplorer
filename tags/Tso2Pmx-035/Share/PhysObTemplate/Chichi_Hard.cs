//css_reference Microsoft.DirectX.Direct3DX;

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;
using Tso2Pmd;

namespace TDCG.PhysObTemplate
{
    public class Chichi_Hard : IPhysObTemplate
    {
        string name = "微ゆれ";   // 枠に表示される名前
        int group = 1;          // 表示される枠の種類（0:髪 1:乳 2:スカート 3:その他）

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("右乳１");
            phys_list.MakeChain("左乳１");
        
            SetParameter(phys_list.GetBodyListByName(".乳."));
            SetParameterEnd(phys_list.GetBodyListByName(".乳先"));
            SetParameter(phys_list.GetJointListByName(".乳."));
            phys_list.GetBodyByName("右乳１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左乳１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右乳２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左乳２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
        }

        private void SetParameter(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 3; // 諸データ：グループ // 00
                body.group_non_collision = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_id = 2; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.size.X = 0.2f; // 形状：半径(幅) // CD CC CC 3F // 1.6

                body.weight = 0.1f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
                body.rotation_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
                body.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
                body.type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameterEnd(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 3; // 諸データ：グループ // 00
                body.group_non_collision = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_id = 0; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.size.X = 0.2f; // 形状：半径(幅) // CD CC CC 3F // 1.6

                body.weight = 0.01f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.position_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
                body.rotation_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
                body.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
                body.type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.spring_rotation = new Vector3(200.0f, 200.0f, 200.0f);
            }
        }
    }
}
