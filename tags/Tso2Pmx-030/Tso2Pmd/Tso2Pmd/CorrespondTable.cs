using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Tso2Pmd
{
    public struct BoneDispGroup
    {
        public string name;
        public List<string> bone_names;
    }

    /// <summary>
    /// ボーン対応表を扱います。
    /// </summary>
    public class CorrespondTable
    {
        public Dictionary<string, string> skinning = new Dictionary<string, string>();
        public Dictionary<string, string> bonePosition = new Dictionary<string, string>();
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

        void ReadSkinning(string path)
        {
            StreamReader sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                skinning.Add(data[0].Trim(), data[1].Trim());
            }
            sr.Close();
        }

        void ReadBonePosition(string path)
        {
            StreamReader sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                bonePosition.Add(data[1].Trim(), data[0].Trim());
            }
            sr.Close();
        }

        void ReadBoneStructure(string path)
        {
            StreamReader sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                string bone_name = data[0].Trim();
                string disp_name = data[5].Trim();

                // PMD_Boneデータを生成
                PMD_Bone pmd_b = new PMD_Bone();

                pmd_b.name = bone_name;

                pmd_b.kind = int.Parse(data[1].Trim());

                if (data[2] == "")
                    pmd_b.ParentName = null;
                else
                    pmd_b.ParentName = data[2].Trim();

                if (data[3] == "")
                    pmd_b.TailName = null;
                else
                    pmd_b.TailName = data[3].Trim();

                if (data[4] == "")
                    pmd_b.TargetName = null;
                else
                    pmd_b.TargetName = data[4].Trim();

                boneStructure.Add(pmd_b.name, pmd_b);

                // 枠に表示するボーン名の設定
                if (disp_name != "")
                {
                    bool found = false;
                    foreach (BoneDispGroup group in boneDispGroups)
                    {
                        if (group.name == disp_name)
                        {
                            group.bone_names.Add(bone_name);
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        BoneDispGroup group = new BoneDispGroup();
                        group.name = disp_name;
                        group.bone_names = new List<string>();
                        group.bone_names.Add(bone_name);
                        boneDispGroups.Add(group);
                    }
                }
            }
            sr.Close();
        }

        void ReadIKBone(string path)
        {
            StreamReader sr = new StreamReader(path,
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                PMD_IK ik = new PMD_IK();

                ik.effector_node_name = data[0].Trim();
                ik.target_node_name = data[1].Trim();
                int chain_length = int.Parse(data[2].Trim());
                ik.niteration = int.Parse(data[3].Trim());
                ik.weight = float.Parse(data[4].Trim());

                for (int i = 5; i < data.Length; i++)
                {
                    ik.chain_node_names.Add(data[i].Trim());
                }

                iks.Add(ik);
            }
            sr.Close();
        }

        /// <summary>
        /// 指定対応表を結合します。
        /// </summary>
        /// <param name="ct">結合する対応表</param>
        public void Update(CorrespondTable ct)
        {
            foreach (KeyValuePair<string, string> kvp in ct.skinning)
            {
                if (skinning.ContainsKey(kvp.Key))
                    skinning[kvp.Key] = kvp.Value;
                else
                    skinning.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, string> kvp in ct.bonePosition)
            {
                if (bonePosition.ContainsKey(kvp.Key))
                    bonePosition[kvp.Key] = kvp.Value;
                else
                    bonePosition.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, PMD_Bone> kvp in ct.boneStructure)
            {
                if (boneStructure.ContainsKey(kvp.Key))
                    boneStructure[kvp.Key] = kvp.Value;
                else
                    boneStructure.Add(kvp.Key, kvp.Value);
            }

            foreach (BoneDispGroup group in ct.boneDispGroups)
            {
                boneDispGroups.Add(group);
            }

            foreach (PMD_IK ik in ct.iks)
            {
                iks.Add(ik);
            }
        }
    }
}
