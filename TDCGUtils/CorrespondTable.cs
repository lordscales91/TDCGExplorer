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
    public struct DispBoneGroup
    {
        public string group_name;
        public List<string> bone_name_list;
    }

    public class CorrespondTable
    {
        public Dictionary<string, string> skinning = new Dictionary<string, string>();
        public Dictionary<string, string> bonePosition = new Dictionary<string, string>();
        public Dictionary<string, PMD_Bone> boneStructure = new Dictionary<string, PMD_Bone>();
        public List<DispBoneGroup> dispBoneGroup = new List<DispBoneGroup>();
        public List<PMD_IK> IKBone = new List<PMD_IK>();

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
                PMD_Bone pmd_b = new PMD_Bone((short)boneStructure.Count);

                pmd_b.name_ja = data[0].Trim();
                int kind = int.Parse(data[1].Trim());
                // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
                
                if (data[2] == "")
                    pmd_b.parent_node_id = -1;
                /*
                else
                    pmd_b.parent_node_id = data[2].Trim();
                */
                if (data[3] == "")
                    pmd_b.tail_node_id = -1;
                /*
                else
                    pmd_b.tail_node_id = data[3].Trim();
                */
                
                /*
                if (data[4] == "")
                    pmd_b.IKTargetName = null;
                else
                    pmd_b.IKTargetName = data[4].Trim();
                */

                boneStructure.Add(pmd_b.name_ja, pmd_b);

                // 枠に表示するボーン名の設定
                if (data[5].Trim() != "")
                {
                    bool flag = false;
                    foreach (DispBoneGroup dbg in dispBoneGroup)
                    {
                        if (dbg.group_name == data[5].Trim())
                        {
                            dbg.bone_name_list.Add(data[0].Trim());
                            flag = true;
                        }
                    }

                    if (flag == false)
                    {
                        DispBoneGroup dbg = new DispBoneGroup();
                        dbg.group_name = data[5].Trim();
                        dbg.bone_name_list = new List<string>();
                        dbg.bone_name_list.Add(data[0].Trim());
                        dispBoneGroup.Add(dbg);
                    }
                }
            }
            sr.Close();

            /*
            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Path.Combine(path, @"IKBone.txt"),
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                //TODO: ボーン番号を直せ！
                PMD_IK pmd_ik = new PMD_IK((short)IKBone.Count);
                pmd_ik.id = short.Parse(data[0].Trim());	// IKボーン番号
                pmd_ik.target_node_id = short.Parse(data[1].Trim());		// IKターゲットボーン番号 // IKボーンが最初に接続するボーン
                int numLink = int.Parse(data[2].Trim());	// IKチェーンの長さ(子の数)
                pmd_ik.links = new PMD_IL[numLink];
                pmd_ik.loop_count = int.Parse(data[3].Trim());      // 再帰演算回数 // IK値1
                pmd_ik.cons_angle = float.Parse(data[4].Trim()) * 4;       // IKの影響度 // IK値2

                for (int i = 5; i < data.Length; i++)
                {
                    pmd_ik.links[i] = new PMD_IL();
                    pmd_ik.links[i].node_id = short.Parse(data[i].Trim());
                }

                IKBone.Add(pmd_ik);
            }
            sr.Close();
            */
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

            // Konoa added.
            foreach (DispBoneGroup dbg in ct.dispBoneGroup)
            {
                dispBoneGroup.Add(dbg);
            }

            foreach (PMD_IK ik in ct.IKBone)
            {
                IKBone.Add(ik);
            }
        }
    }
}
