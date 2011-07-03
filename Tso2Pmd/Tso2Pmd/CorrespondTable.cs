using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Tso2Pmd
{
    public class BoneDispGroup
    {
        public string name;
        public List<string> bone_names;
    }

    /// <summary>
    /// ボーン対応表を扱います。
    /// </summary>
    public class CorrespondTable
    {
        public List<string> PathList = new List<string>();

        public Dictionary<string, string> skinning = new Dictionary<string, string>();
        public Dictionary<string, string> bonePositions = new Dictionary<string, string>();
        public Dictionary<string, PMD_Bone> boneStructure = new Dictionary<string, PMD_Bone>();
        public List<BoneDispGroup> boneDispGroups = new List<BoneDispGroup>();
        public List<PMD_IK> iks = new List<PMD_IK>();

        public void Load(string path)
        {
            ReadSkinning(Path.Combine(path, @"skinning.txt"));
            ReadBonePosition(Path.Combine(path, @"bonePosition.txt"));
            ReadBoneStructure(Path.Combine(path, @"boneStructure.txt"));
            ReadIKBone(Path.Combine(path, @"IKBone.txt"));
        }

        static Encoding encoding = Encoding.GetEncoding("Shift_JIS");

        public void ReadSkinning(string path)
        {
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] row = line.Split(',');

                    string tso_bone_name = row[0].Trim();
                    string pmd_bone_name = row[1].Trim();

                    // すでにtso_bone_nameが存在するなら上書き
                    skinning[tso_bone_name] = pmd_bone_name;
                }
            }
        }

        void ReadBonePosition(string path)
        {
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] row = line.Split(',');

                    string tso_bone_name = row[0].Trim();
                    string pmd_bone_name = row[1].Trim();

                    // すでにpmd_bone_nameが存在するなら上書き
                    bonePositions[pmd_bone_name] = tso_bone_name;
                }
            }
        }

        void ReadBoneStructure(string path)
        {
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] row = line.Split(',');

                    string bone_name = row[0].Trim();
                    string bone_kind = row[1].Trim();
                    string parent_name = row[2].Trim();
                    string tail_name = row[3].Trim();
                    string target_name = row[4].Trim();
                    string disp_name = row[5].Trim();

                    PMD_Bone pmd_b = new PMD_Bone();

                    pmd_b.name = bone_name;
                    pmd_b.kind = int.Parse(bone_kind);
                    pmd_b.ParentName = (parent_name != "") ? parent_name : null;
                    pmd_b.TailName = (tail_name != "") ? tail_name : null;
                    pmd_b.TargetName = (target_name != "") ? target_name : null;

                    // すでにpmd_b.nameが存在するなら上書き
                    boneStructure[pmd_b.name] = pmd_b;

                    if (disp_name != "")
                        AddBoneNameInDisp(bone_name, disp_name);
                }
            }
        }

        // 指定名称を持つ表示枠に指定ボーン名を入れる
        void AddBoneNameInDisp(string bone_name, string disp_name)
        {
            BoneDispGroup found_group = null;

            // 指定名称を持つ表示枠を検索する
            foreach (BoneDispGroup group in boneDispGroups)
            {
                if (group.name == disp_name)
                {
                    found_group = group;
                    break;
                }
            }

            // なければ追加する
            if (found_group == null)
            {
                found_group = new BoneDispGroup();
                found_group.name = disp_name;
                found_group.bone_names = new List<string>();
                boneDispGroups.Add(found_group);
            }

            // 表示枠にボーン名を追加する
            // すでに同じ名前が入っているなら追加しない
            if (!found_group.bone_names.Contains(bone_name))
                found_group.bone_names.Add(bone_name);
        }

        void ReadIKBone(string path)
        {
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] row = line.Split(',');

                    PMD_IK ik = new PMD_IK();

                    ik.effector_node_name = row[0].Trim();
                    ik.target_node_name = row[1].Trim();
                    int chain_length = int.Parse(row[2].Trim());
                    ik.niteration = short.Parse(row[3].Trim());
                    ik.weight = float.Parse(row[4].Trim());

                    for (int i = 5; i < row.Length; i++)
                    {
                        ik.chain_node_names.Add(row[i].Trim());
                    }

                    iks.Add(ik);
                }
            }
        }
    }
}
