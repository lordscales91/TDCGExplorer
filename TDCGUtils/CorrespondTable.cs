using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;

using TDCG;

namespace TDCGUtils
{
    public struct BoneDispGroup
    {
        public string name;
        public List<string> bone_names;
    }

    public class CorrespondTable
    {
        public Dictionary<string, string> skinning = new Dictionary<string, string>();
        public Dictionary<string, string> bonePosition = new Dictionary<string, string>();
        public Dictionary<string, PMD_Bone> boneStructure = new Dictionary<string, PMD_Bone>();
        public List<BoneDispGroup> boneDispGroups = new List<BoneDispGroup>();
        public List<PMD_IK> iks = new List<PMD_IK>();

        public CorrespondTable()
        {
        }

        public CorrespondTable(string path)
        {
            System.IO.StreamReader sr;

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Path.Combine(path, @"skinning.txt"),
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                skinning.Add(data[0].Trim(), data[1].Trim());
            }
            sr.Close();

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Path.Combine(path, @"bonePosition.txt"),
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                bonePosition.Add(data[1].Trim(), data[0].Trim());
            }
            sr.Close();

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Path.Combine(path, @"boneStructure.txt"),
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                // PMD_Boneデータを生成
                PMD_Bone pmd_b = new PMD_Bone();

                pmd_b.name = data[0].Trim();

                // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
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
                if (data[5].Trim() != "")
                {
                    bool flag = false;
                    foreach (BoneDispGroup dbg in boneDispGroups)
                    {
                        if (dbg.name == data[5].Trim())
                        {
                            dbg.bone_names.Add(data[0].Trim());
                            flag = true;
                        }
                    }

                    if (flag == false)
                    {
                        BoneDispGroup dbg = new BoneDispGroup();
                        dbg.name = data[5].Trim();
                        dbg.bone_names = new List<string>();
                        dbg.bone_names.Add(data[0].Trim());
                        boneDispGroups.Add(dbg);
                    }
                }
            }
            sr.Close();

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Path.Combine(path, @"IKBone.txt"),
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                PMD_IK pmd_ik = new PMD_IK();

                pmd_ik.target_node_name = data[0].Trim();
                pmd_ik.effector_node_name = data[1].Trim();
                int chain_length = int.Parse(data[2].Trim());
                pmd_ik.niteration = int.Parse(data[3].Trim());
                pmd_ik.weight = float.Parse(data[4].Trim());

                for (int i = 5; i < data.Length; i++)
                {
                    pmd_ik.chain_node_names.Add(data[i].Trim());
                }

                iks.Add(pmd_ik);
            }
            sr.Close();
        }

        public void Add(CorrespondTable ct)
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

            foreach (BoneDispGroup dbg in ct.boneDispGroups)
            {
                boneDispGroups.Add(dbg);
            }

            foreach (PMD_IK ik in ct.iks)
            {
                iks.Add(ik);
            }
        }
    }
}
