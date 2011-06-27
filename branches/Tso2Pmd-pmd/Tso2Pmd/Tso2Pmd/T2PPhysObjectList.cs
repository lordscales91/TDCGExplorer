using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

using jp.nyatla.nymmd.cs.types;
using jp.nyatla.nymmd.cs.core;
using jp.nyatla.nymmd.cs.struct_type;
using jp.nyatla.nymmd.cs.struct_type.pmd;

namespace Tso2Pmd
{
    public class T2PPhysObjectList
    {
        List<TDCGUtils.PMD_Bone> bone_list;
        public List<TDCGUtils.PMD_RigidBody> body_list = new List<TDCGUtils.PMD_RigidBody>();
        public List<TDCGUtils.PMD_Joint> joint_list = new List<TDCGUtils.PMD_Joint>();

        public T2PPhysObjectList(List<TDCGUtils.PMD_Bone> bone_list)
        {
            this.bone_list = bone_list;
        }

        // 名前からボーンIDを得る
        private int GetBoneIDByName(string name)
        {
            for (int i = 0; i < bone_list.Count; i++)
            {
                if (bone_list[i].szName == name) return i;
            }

            return -1;
        }
        // 名前から剛体IDを得る
        private int GetBodyIDByName(string name)
        {
            for (int i = 0; i < body_list.Count; i++)
            {
                if (body_list[i].rigidbody_name == name) return i;
            }

            return -1;
        }
        // 名前からジョイントIDを得る
        private int GetJointIDByName(string name)
        {
            for (int i = 0; i < joint_list.Count; i++)
            {
                if (joint_list[i].joint_name == name) return i;
            }

            return -1;
        }
        // 名前からボーンを得る
        public TDCGUtils.PMD_Bone GetBoneByName(string name)
        {
            return bone_list[GetBoneIDByName(name)];
        }
        // 名前から剛体を得る
        public TDCGUtils.PMD_RigidBody GetBodyByName(string name)
        {
            return body_list[GetBodyIDByName(name)];
        }
        // 名前からジョイントを得る
        public TDCGUtils.PMD_Joint GetJointByName(string name)
        {
            return joint_list[GetJointIDByName(name)];
        }

        // 正規表現から剛体リストを得る
        public List<TDCGUtils.PMD_RigidBody> GetBodyListByName(string exp)
        {
            List<TDCGUtils.PMD_RigidBody> list = new List<TDCGUtils.PMD_RigidBody>();

            for (int i = 0; i < body_list.Count; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    body_list[i].rigidbody_name,
                    exp))
                {
                    list.Add(body_list[i]);
                }
            }

            return list;
        }
        // 正規表現からジョイントリストを得る
        public List<TDCGUtils.PMD_Joint> GetJointListByName(string exp)
        {
            List<TDCGUtils.PMD_Joint> list = new List<TDCGUtils.PMD_Joint>();

            for (int i = 0; i < joint_list.Count; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    joint_list[i].joint_name,
                    exp))
                {
                    list.Add(joint_list[i]);
                }
            }

            return list;
        }

        // ①(指定したボーン→その子ボーン)にフィットするような剛体を生成
        // ②(生成した剛体)と(指定したボーンの親ボーンの剛体)を、指定したボーンの位置でジョイントする
        // ③次に、指定したボーンの子ボーンに対しても同じ処理を行う
        // ④(①～③)を子ボーンがなくなるまで繰り返す
        public void MakeChain(string name)
        {
            if (GetBoneByName(name).ChildName != null)
            {
                MakeBodyFromBone(name);
                MakeJointFromBone(name);

                MakeChain(GetBoneByName(name).ChildName);
            }
            else
            {
                MakeBodyFromBoneEnd(name);
                MakeJointFromBone(name);
            }
        }

        // 指定したボーン位置に合わせた剛体を生成
        public void MakeBodyFromBoneEnd(string bone_name)
        {
            MakeBodyFromBoneEnd(GetBoneIDByName(bone_name));
        }

        // 指定したボーン位置に合わせた剛体を生成
        public void MakeBodyFromBoneEnd(int bone_num)
        {
            TDCGUtils.PMD_RigidBody rigidbody = new TDCGUtils.PMD_RigidBody();

            rigidbody.rigidbody_name = bone_list[bone_num].szName; // 諸データ：名称 // 頭
            rigidbody.rigidbody_rel_bone_index = bone_num; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
            rigidbody.pos_pos.X = 0.0f;
            rigidbody.pos_pos.Y = 0.0f;
            rigidbody.pos_pos.Z = 0.0f;

            rigidbody.rigidbody_group_index = 0; // 諸データ：グループ // 00
            rigidbody.rigidbody_group_target = -1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            rigidbody.shape_type = 0; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            rigidbody.shape_w = 0.4f; // 形状：半径(幅) // CD CC CC 3F // 1.6

            rigidbody.rigidbody_weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
            rigidbody.rigidbody_pos_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
            rigidbody.rigidbody_rot_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
            rigidbody.rigidbody_recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
            rigidbody.rigidbody_friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
            rigidbody.rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            body_list.Add(rigidbody);
        }

        // (指定したボーン→その子ボーン)にフィットするような剛体を生成
        public void MakeBodyFromBone(string bone_name)
        {
            MakeBodyFromTwoVector(
                bone_name, 
                GetBoneByName(bone_name).vec3Position,
                GetBoneByName(GetBoneByName(bone_name).ChildName).vec3Position);
        }
 
        // (ベクトル１→ベクトル２)にフィットするような剛体を生成
        public void MakeBodyFromTwoVector(string bone_name, Vector3 v1, Vector3 v2)
        {
            TDCGUtils.PMD_RigidBody rigidbody = new TDCGUtils.PMD_RigidBody();

            rigidbody.rigidbody_name = bone_name; // 諸データ：名称 // 頭
            rigidbody.rigidbody_rel_bone_index = GetBoneIDByName(bone_name); // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
 
            float x1 = v1.X;
            float y1 = v1.Y;
            float z1 = v1.Z;
            float x2 = v2.X;
            float y2 = v2.Y;
            float z2 = v2.Z;
            double L = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
            rigidbody.pos_pos.X = (x1 + x2) / 2.0f - x1; // 位置：位置(x, y, z)
            rigidbody.pos_pos.Y = (y1 + y2) / 2.0f - y1; // 位置：位置(x, y, z)
            rigidbody.pos_pos.Z = (z1 + z2) / 2.0f - z1; // 位置：位置(x, y, z)
            if (y1 >= y2) rigidbody.pos_rot.X = (float)Math.Asin((z1 - z2) / L); // 位置：回転(rad(x), rad(y), rad(z))
            else rigidbody.pos_rot.X = (float)Math.Asin((z2 - z1) / L);
            if (y1 >= y2) rigidbody.pos_rot.Z = (float)Math.Asin(-(x1 - x2) / L); // 位置：回転(rad(x), rad(y), rad(z))
            else rigidbody.pos_rot.Z = (float)Math.Asin(-(x2 - x1) / L);

            rigidbody.rigidbody_group_index = 0; // 諸データ：グループ // 00
            rigidbody.rigidbody_group_target = -1; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            rigidbody.shape_type = 2; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            rigidbody.shape_w = 0.4f; // 形状：半径(幅) // CD CC CC 3F // 1.6
            rigidbody.shape_h = (float)(L * 0.8); // 形状：高さ // CD CC CC 3D // 0.1

            rigidbody.rigidbody_weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
            rigidbody.rigidbody_pos_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
            rigidbody.rigidbody_rot_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
            rigidbody.rigidbody_recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
            rigidbody.rigidbody_friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
            rigidbody.rigidbody_type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            body_list.Add(rigidbody);
        }

        // (指定したボーンにフィットさせた剛体)と、(その親ボーンにフィットさせた剛体)を、
        // 指定したボーンの位置でジョイントする
        public void MakeJointFromBone(string bone_name)
        {
            MakeJointFromBone(GetBoneIDByName(bone_name));
        }

        // (指定したボーンにフィットさせた剛体)と、(その親ボーンにフィットさせた剛体)を、
        // 指定したボーンの位置でジョイントする
        public void MakeJointFromBone(int bone_num)
        {
            TDCGUtils.PMD_Joint joint = new TDCGUtils.PMD_Joint();

            joint.joint_name = GetBoneByName(bone_list[bone_num].ParentName).szName
                + "-" + bone_list[bone_num].szName; // 諸データ：名称 // 右髪1
            joint.joint_rigidbody_a = GetBodyIDByName(GetBoneByName(bone_list[bone_num].ParentName).szName); // 諸データ：剛体A
            joint.joint_rigidbody_b = GetBodyIDByName(bone_list[bone_num].szName); // 諸データ：剛体B
            joint.joint_pos.X = bone_list[bone_num].vec3Position.X; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_pos.Y = bone_list[bone_num].vec3Position.Y; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_pos.Z = bone_list[bone_num].vec3Position.Z; // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_rot.X = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.joint_rot.Y = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.joint_rot.Z = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.constrain_pos_1.X = 0.0f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.X = 0.0f; // 制限：移動2(x, y, z)
            joint.constrain_pos_1.Y = 0.0f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.Y = 0.0f; // 制限：移動2(x, y, z)
            joint.constrain_pos_1.Z = 0.0f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.Z = 0.0f; // 制限：移動2(x, y, z)
            joint.constrain_rot_1.X = 0.0f; // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.X = 0.0f; // 制限：回転2(rad(x), rad(y), rad(z))
            joint.constrain_rot_1.Y = 0.0f; // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.Y = 0.0f; // 制限：回転2(rad(x), rad(y), rad(z))
            joint.constrain_rot_1.Z = 0.0f; // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.Z = 0.0f; // 制限：回転2(rad(x), rad(y), rad(z))
            joint.spring_pos.X = 0.0f; // ばね：移動(x, y, z)
            joint.spring_pos.Y = 0.0f; // ばね：移動(x, y, z)
            joint.spring_pos.Z = 0.0f; // ばね：移動(x, y, z)
            joint.spring_rot.X = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            joint.spring_rot.Y = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))
            joint.spring_rot.Z = 0.0f; // ばね：回転(rad(x), rad(y), rad(z))

            joint_list.Add(joint);
        }

        // 指定した２つのボーンにフィットさせている剛体をジョイントする
        public void MakeJointFromTwoBones(string bone_name1, string bone_name2)
        {
            MakeJointFromTwoBones(GetBoneIDByName(bone_name1), GetBoneIDByName(bone_name2));
        }

        // 指定した２つのボーンにフィットさせている剛体をジョイントする
        public void MakeJointFromTwoBones(int bone_num1, int bone_num2)
        {
            TDCGUtils.PMD_Joint joint = new TDCGUtils.PMD_Joint();

            joint.joint_name = bone_list[bone_num1].szName + "-" + bone_list[bone_num2].szName; // 諸データ：名称 // 右髪1
            joint.joint_rigidbody_a = GetBodyIDByName(bone_list[bone_num1].szName); // 諸データ：剛体A
            joint.joint_rigidbody_b = GetBodyIDByName(bone_list[bone_num2].szName); // 諸データ：剛体B
            joint.joint_pos.X = 0.5f * (bone_list[bone_num1].vec3Position.X + bone_list[bone_num2].vec3Position.X); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_pos.Y = 0.5f * (bone_list[bone_num1].vec3Position.Y + bone_list[bone_num2].vec3Position.Y); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_pos.Z = 0.5f * (bone_list[bone_num1].vec3Position.Z + bone_list[bone_num2].vec3Position.Z); // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.joint_rot.X = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.joint_rot.Y = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.joint_rot.Z = 0.0f; // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.constrain_pos_1.X = -0.5f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.X = 0.5f; // 制限：移動2(x, y, z)
            joint.constrain_pos_1.Y = -0.5f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.Y = 0.5f; // 制限：移動2(x, y, z)
            joint.constrain_pos_1.Z = -0.5f; // 制限：移動1(x, y, z)
            joint.constrain_pos_2.Z = 0.5f; // 制限：移動2(x, y, z)
            joint.constrain_rot_1.X = (float)((-60.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.X = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            joint.constrain_rot_1.Y = (float)((-60.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.Y = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            joint.constrain_rot_1.Z = (float)((-60.0 / 180.0) * Math.PI); // 制限：回転1(rad(x), rad(y), rad(z))
            joint.constrain_rot_2.Z = (float)((60.0 / 180.0) * Math.PI); // 制限：回転2(rad(x), rad(y), rad(z))
            joint.spring_pos.X = 10.0f; // ばね：移動(x, y, z)
            joint.spring_pos.Y = 10.0f; // ばね：移動(x, y, z)
            joint.spring_pos.Z = 10.0f; // ばね：移動(x, y, z)
            joint.spring_rot.X = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))
            joint.spring_rot.Y = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))
            joint.spring_rot.Z = 10.0f; // ばね：回転(rad(x), rad(y), rad(z))

            joint_list.Add(joint);
        }
    }
}
