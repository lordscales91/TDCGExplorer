using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TDCG;
using jp.nyatla.nymmd.cs.struct_type.pmd;

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
        public List<PMD_Bone> boneStructure = new List<PMD_Bone>();
        public Dictionary<string, PMD_Bone> boneStructure_dic = new Dictionary<string, PMD_Bone>();
        public List<DispBoneGroup> dispBoneGroup = new List<DispBoneGroup>();

        //public Dictionary<string, string> boneCorrespond_v2t = new Dictionary<string, string>();

        public CorrespondTable(string name)
        {
            System.IO.StreamReader sr;

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Application.StartupPath + @"/CorrespondTable/" + name + "_skinning.txt",
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                skinning.Add(data[1].Trim(), data[2].Trim());
            }
            sr.Close();

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Application.StartupPath + @"/CorrespondTable/" + name + "_bonePosition.txt",
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');
                bonePosition.Add(data[2].Trim(), data[1].Trim());
            }
            sr.Close();

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Application.StartupPath + @"/CorrespondTable/" + name + "_boneStructure.txt",
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                // PMD_Boneデータを生成
                PMD_Bone pmd_b = new PMD_Bone();

                pmd_b.szName = data[0].Trim();
                pmd_b.cbKind = int.Parse(data[1].Trim()); // ボーンの種類 0:回転 1:回転と移動 2:IK 3:不明 4:IK影響下 5:回転影響下 6:IK接続先 7:非表示 8:捻り 9:回転運動
                if (data[2] == "") pmd_b.ParentName = null;
                else pmd_b.ParentName = data[2].Trim();
                if (data[3] == "") pmd_b.ChildName = null;
                else pmd_b.ChildName = data[3].Trim();
                if (data[4] == "") pmd_b.IKTargetName = null;
                else pmd_b.IKTargetName = data[4].Trim();

                boneStructure.Add(pmd_b);
                boneStructure_dic.Add(pmd_b.szName, pmd_b);

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

            /*//内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Application.StartupPath + @"/CorrespondTable/" + name + "_vmd2tmo.txt",
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                boneCorrespond_v2t.Add(data[0].Trim(), data[1].Trim());
            }
            sr.Close();*/

        }
    }
}
