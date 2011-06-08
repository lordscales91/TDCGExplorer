using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;
using Tso2Pmd;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace TDCG.PhysObTemplate
{
    public class Kami_Twin : IPhysObTemplate
    {
        string name = "ツインテール"; // 枠に表示される名前
        int group = 0;              // 表示される枠の種類（0:髪 1:乳 2:スカート 3:その他）

        public string Name() { return name; }
        public int Group() { return group; }

        public void Execute(ref T2PPhysObjectList phys_list)
        {
            phys_list.MakeChain("左髪横１");
            phys_list.MakeChain("右髪横１");
            phys_list.MakeChain("左髪後１");
            phys_list.MakeChain("右髪後１");
            phys_list.MakeChain("左髪前１");
            phys_list.MakeChain("右髪前１");

            SetParameter(phys_list.GetBodyListByName(".髪.*"));
            SetParameterEnd(phys_list.GetBodyListByName(".髪.*先"));
            SetParameter(phys_list.GetJointListByName(".髪.*"));

            phys_list.GetBodyByName("左髪横１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪横２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪横３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("右髪横１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪横２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪横３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("左髪後１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪後２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetJointByName("左髪後２-左髪後３").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後２-左髪後３").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後３-左髪後４").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("左髪後３-左髪後４").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("右髪後１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪後２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetJointByName("右髪後２-右髪後３").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後２-右髪後３").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後３-右髪後４").constrain_rot_1.x = (float)((-5.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            phys_list.GetJointByName("右髪後３-右髪後４").constrain_rot_2.x = (float)((30.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))

            phys_list.GetBodyByName("左髪前１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("左髪前先").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            phys_list.GetBodyByName("右髪前１").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前２").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前３").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            phys_list.GetBodyByName("右髪前先").rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
        }

        private void SetParameter(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 2; // 諸データ：グループ // 00
                body.rigidbody_group_target = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_type = 2; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.shape_w = 0.4f; // 形状：半径(幅) // CD CC CC 3F // 1.6
                body.shape_h *= 0.6f; // 形状：高さ // CD CC CC 3D // 0.1

                body.rigidbody_weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.8f; // 諸データ：移動減 // 00 00 00 00
                body.rigidbody_rot_dim = 0.8f; // 諸データ：回転減 // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.rigidbody_friction = 0.8f; // 諸データ：摩擦力 // 00 00 00 00
                body.rigidbody_type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameterEnd(List<PMD_RigidBody> body_list)
        {
            foreach (PMD_RigidBody body in body_list)
            {
                body.rigidbody_group_index = 2; // 諸データ：グループ // 00
                body.rigidbody_group_target = 1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
                body.shape_type = 0; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
                body.shape_w = 0.3f; // 形状：半径(幅) // CD CC CC 3F // 1.6

                body.rigidbody_weight = 1.0f; // 諸データ：質量 // 00 00 80 3F // 1.0
                body.rigidbody_pos_dim = 0.8f; // 諸データ：移動減 // 00 00 00 00
                body.rigidbody_rot_dim = 0.8f; // 諸データ：回転減 // 00 00 00 00
                body.rigidbody_recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
                body.rigidbody_friction = 0.8f; // 諸データ：摩擦力 // 00 00 00 00
                body.rigidbody_type = 1; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従
            }
        }

        private void SetParameter(List<PMD_Joint> joint_list)
        {
            foreach (PMD_Joint joint in joint_list)
            {
                joint.constrain_rot_1.x = (float)((-20.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.x = (float)((20.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                joint.constrain_rot_1.z = (float)((-20.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
                joint.constrain_rot_2.z = (float)((20.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
                joint.spring_rot.x = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))
                joint.spring_rot.y = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))
                joint.spring_rot.z = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))
            }
        }
    }
}
