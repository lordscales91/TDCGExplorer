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
    public class Kami_Default : IPhysObTemplate
    {
        string name = "デフォルト"; // 枠に表示される名前
        int group = 0;              // 表示される枠の種類（0:髪 1:乳 2:スカート 3:その他）

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("中髪後１");
            phys_list.MakeChain("左髪横１");
            phys_list.MakeChain("右髪横１");
            phys_list.MakeChain("左髪後１");
            phys_list.MakeChain("右髪後１");
            phys_list.MakeChain("左髪前１");
            phys_list.MakeChain("右髪前１");

            SetParameter(phys_list.GetBodyListByName(".髪.*"));
            SetParameterEnd(phys_list.GetBodyListByName(".髪.*先"));
            SetParameter(phys_list.GetJointListByName(".髪.*"));

            phys_list.GetBodyByName("中髪後１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("中髪後２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetJointByName("中髪後２-中髪後３").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("中髪後２-中髪後３").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("中髪後３-中髪後４").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("中髪後３-中髪後４").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("左髪横１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪横２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪横３").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("右髪横１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪横２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪横３").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("左髪後１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪後２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetJointByName("左髪後２-左髪後３").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後２-左髪後３").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後３-左髪後４").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後３-左髪後４").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("右髪後１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪後２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetJointByName("右髪後２-右髪後３").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後２-右髪後３").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後３-右髪後４").rotation_min.X = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後３-右髪後４").rotation_max.X = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("左髪前１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前３").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前先").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("右髪前１").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前２").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前３").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前先").type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
        }

        private void SetParameter(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 2; // 諸データ：グループ // 00
                body.group_non_collision = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_id = 2; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.size.X = 0.4f; // 形状：半径(幅) // CD CC CC 3F // 1.6
                body.size.Y *= 0.6f; // 形状：高さ // CD CC CC 3D // 0.1

                body.weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.position_dim = 0.8f; // 諸データ：移動減 // 00 00 00 00
                body.rotation_dim = 0.8f; // 諸データ：回転減 // 00 00 00 00
                body.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.friction = 0.8f; // 諸データ：摩擦力 // 00 00 00 00
                body.type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameterEnd(List<PMD_RBody> body_list)
        {
            foreach (PMD_RBody body in body_list)
            {
                body.group_id = 2; // 諸データ：グループ // 00
                body.group_non_collision = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_id = 0; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.size.X = 0.3f; // 形状：半径(幅) // CD CC CC 3F // 1.6

                body.weight = 1.0f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.position_dim = 0.8f; // 諸データ：移動減 // 00 00 00 00
                body.rotation_dim = 0.8f; // 諸データ：回転減 // 00 00 00 00
                body.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.friction = 0.8f; // 諸データ：摩擦力 // 00 00 00 00
                body.type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.rotation_min.X = Geometry.DegreeToRadian(-20.0f);
                joint.rotation_max.X = Geometry.DegreeToRadian(+20.0f);
                joint.rotation_min.Z = Geometry.DegreeToRadian(-20.0f);
                joint.rotation_max.Z = Geometry.DegreeToRadian(+20.0f);
                joint.spring_rotation = new Vector3(10.0f, 10.0f, 10.0f);
            }
        }
    }
}
