using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
    public class T2PPhysObjectList
    {
        List<PMD_Bone> bone_list;
        public List<PMD_RBody> rbody_list = new List<PMD_RBody>();
        public List<PMD_Joint> joint_list = new List<PMD_Joint>();

        public T2PPhysObjectList(List<PMD_Bone> bone_list)
        {
            this.bone_list = bone_list;
        }

        // 名前からボーンIDを得る
        private short GetBoneIDByName(string name)
        {
            for (short i = 0; i < (short)bone_list.Count; i++)
            {
                if (bone_list[i].name == name) return i;
            }

            return -1;
        }
        // 名前から剛体IDを得る
        private sbyte GetBodyIDByName(string name)
        {
            for (sbyte i = 0; i < (sbyte)rbody_list.Count; i++)
            {
                if (rbody_list[i].name == name) return i;
            }

            return -1;
        }
        // 名前からジョイントIDを得る
        private sbyte GetJointIDByName(string name)
        {
            for (sbyte i = 0; i < (sbyte)joint_list.Count; i++)
            {
                if (joint_list[i].name == name) return i;
            }

            return -1;
        }
        // 名前からボーンを得る
        public PMD_Bone GetBoneByName(string name)
        {
            return bone_list[GetBoneIDByName(name)];
        }
        // 名前から剛体を得る
        public PMD_RBody GetBodyByName(string name)
        {
            return rbody_list[GetBodyIDByName(name)];
        }
        // 名前からジョイントを得る
        public PMD_Joint GetJointByName(string name)
        {
            return joint_list[GetJointIDByName(name)];
        }

        // 正規表現から剛体リストを得る
        public List<PMD_RBody> GetBodyListByName(string exp)
        {
            List<PMD_RBody> list = new List<PMD_RBody>();

            for (int i = 0; i < rbody_list.Count; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    rbody_list[i].name,
                    exp))
                {
                    list.Add(rbody_list[i]);
                }
            }

            return list;
        }
        // 正規表現からジョイントリストを得る
        public List<PMD_Joint> GetJointListByName(string exp)
        {
            List<PMD_Joint> list = new List<PMD_Joint>();

            for (int i = 0; i < joint_list.Count; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    joint_list[i].name,
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
            if (GetBoneByName(name).TailName != null)
            {
                MakeBodyFromBone(name);
                MakeJointFromBone(name);

                MakeChain(GetBoneByName(name).TailName);
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
        public void MakeBodyFromBoneEnd(short bone_num)
        {
            PMD_RBody rbody = new PMD_RBody();

            rbody.name = bone_list[bone_num].name; // 諸データ：名称 // 頭
            rbody.rel_bone_id = bone_num; // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭
            rbody.position = new Vector3(0.0f, 0.0f, 0.0f);

            rbody.group_id = 0; // 諸データ：グループ // 00
            rbody.group_non_collision = 0xFFFF; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            rbody.shape_id = 0; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            rbody.size = new Vector3(0.4f, 0.0f, 0.0f); // 形状：半径(幅) // CD CC CC 3F // 1.6

            rbody.weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
            rbody.position_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
            rbody.rotation_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
            rbody.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
            rbody.friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
            rbody.type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            rbody_list.Add(rbody);
        }

        // (指定したボーン→その子ボーン)にフィットするような剛体を生成
        public void MakeBodyFromBone(string bone_name)
        {
            MakeBodyFromTwoVector(
                bone_name, 
                GetBoneByName(bone_name).position,
                GetBoneByName(GetBoneByName(bone_name).TailName).position);
        }
 
        // (ベクトル１→ベクトル２)にフィットするような剛体を生成
        public void MakeBodyFromTwoVector(string bone_name, Vector3 v1, Vector3 v2)
        {
            PMD_RBody rbody = new PMD_RBody();

            rbody.name = bone_name; // 諸データ：名称 // 頭
            rbody.rel_bone_id = GetBoneIDByName(bone_name); // 諸データ：関連ボーン番号 // 03 00 == 3 // 頭

            float x1 = v1.X;
            float y1 = v1.Y;
            float z1 = v1.Z;
            float x2 = v2.X;
            float y2 = v2.Y;
            float z2 = v2.Z;
            double L = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));

            // 位置：位置(x, y, z)
            rbody.position.X = (x1 + x2) / 2.0f - x1;
            rbody.position.Y = (y1 + y2) / 2.0f - y1;
            rbody.position.Z = (z1 + z2) / 2.0f - z1;

            // 位置：回転(rad(x), rad(y), rad(z))
            if (y1 >= y2)
                rbody.rotation.X = (float)Math.Asin((z1 - z2) / L);
            else
                rbody.rotation.X = (float)Math.Asin((z2 - z1) / L);

            if (y1 >= y2)
                rbody.rotation.Z = (float)Math.Asin(-(x1 - x2) / L);
            else
                rbody.rotation.Z = (float)Math.Asin(-(x2 - x1) / L);

            rbody.group_id = 0; // 諸データ：グループ // 00
            rbody.group_non_collision = 0xFFFF; // 諸データ：グループ：対象 // 0xFFFFとの差 // 38 FE
            rbody.shape_id = 2; // 形状：タイプ(0:球、1:箱、2:カプセル) // 00 // 球
            // 形状：半径(幅) // CD CC CC 3F // 1.6
            // 形状：高さ // CD CC CC 3D // 0.1
            rbody.size = new Vector3(0.4f, (float)(L * 0.8), 0.0f);

            rbody.weight = 0.5f; // 諸データ：質量 // 00 00 80 3F // 1.0
            rbody.position_dim = 0.5f; // 諸データ：移動減 // 00 00 00 00
            rbody.rotation_dim = 0.5f; // 諸データ：回転減 // 00 00 00 00
            rbody.recoil = 0.0f; // 諸データ：反発力 // 00 00 00 00
            rbody.friction = 0.0f; // 諸データ：摩擦力 // 00 00 00 00
            rbody.type = 0; // 諸データ：タイプ(0:Bone追従、1:物理演算、2:物理演算(Bone位置合せ)) // 00 // Bone追従

            rbody_list.Add(rbody);
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
            PMD_Joint joint = new PMD_Joint();

            joint.name = GetBoneByName(bone_list[bone_num].ParentName).name
                + "-" + bone_list[bone_num].name; // 諸データ：名称 // 右髪1

            joint.rbody_a_id = GetBodyIDByName(GetBoneByName(bone_list[bone_num].ParentName).name); // 諸データ：剛体A
            joint.rbody_b_id = GetBodyIDByName(bone_list[bone_num].name); // 諸データ：剛体B

            joint.position = bone_list[bone_num].position;
            joint.rotation = Vector3.Empty;

            joint.position_min = Vector3.Empty;
            joint.position_max = Vector3.Empty;
            joint.rotation_min = Vector3.Empty;
            joint.rotation_max = Vector3.Empty;

            joint.spring_position = Vector3.Empty;
            joint.spring_rotation = Vector3.Empty;

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
            PMD_Joint joint = new PMD_Joint();

            // 諸データ：名称 // 右髪1
            joint.name = bone_list[bone_num1].name + "-" + bone_list[bone_num2].name;

            joint.rbody_a_id = GetBodyIDByName(bone_list[bone_num1].name); // 諸データ：剛体A
            joint.rbody_b_id = GetBodyIDByName(bone_list[bone_num2].name); // 諸データ：剛体B

            // 諸データ：位置(x, y, z) // 諸データ：位置合せでも設定可
            joint.position.X = 0.5f * (bone_list[bone_num1].position.X + bone_list[bone_num2].position.X);
            joint.position.Y = 0.5f * (bone_list[bone_num1].position.Y + bone_list[bone_num2].position.Y);
            joint.position.Z = 0.5f * (bone_list[bone_num1].position.Z + bone_list[bone_num2].position.Z);

            // 諸データ：回転(rad(x), rad(y), rad(z))
            joint.rotation = new Vector3(0.0f, 0.0f, 0.0f);

            // 制限：移動1(x, y, z)
            joint.position_min = new Vector3(-0.5f, -0.5f, -0.5f);

            // 制限：移動2(x, y, z)
            joint.position_max = new Vector3(+0.5f, +0.5f, +0.5f);

            // 制限：回転1(rad(x), rad(y), rad(z))
            joint.rotation_min.X = (float)((-60.0 / 180.0) * Math.PI);
            joint.rotation_min.Y = (float)((-60.0 / 180.0) * Math.PI);
            joint.rotation_min.Z = (float)((-60.0 / 180.0) * Math.PI);

            // 制限：回転2(rad(x), rad(y), rad(z))
            joint.rotation_max.X = (float)((60.0 / 180.0) * Math.PI);
            joint.rotation_max.Y = (float)((60.0 / 180.0) * Math.PI);
            joint.rotation_max.Z = (float)((60.0 / 180.0) * Math.PI);

            // ばね：移動(x, y, z)
            joint.spring_position = new Vector3(10.0f, 10.0f, 10.0f);
            // ばね：回転(rad(x), rad(y), rad(z))
            joint.spring_rotation = new Vector3(10.0f, 10.0f, 10.0f);

            joint_list.Add(joint);
        }
    }
}
