using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGUtils
{
    class CorrespondBoneForTmo2Vpd
    {
        Dictionary<string, List<string>> correspond_bone = new Dictionary<string, List<string>>();
        
        public Dictionary<string, List<string>> CorrespondBone { get { return correspond_bone; } }

        public CorrespondBoneForTmo2Vpd(string name)
        {
            System.IO.StreamReader sr;

            //内容を一行ずつ読み込む
            sr = new System.IO.StreamReader(
                Application.StartupPath + @"/CorrespondTable/" + name + "_tmo2vpd.txt",
                System.Text.Encoding.GetEncoding("shift_jis"));
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] data = line.Split(',');

                List<string> data_list = new List<string>();

                for (int i = 1; i < data.Length; i++)
                {
                    if (data[i].Trim() == "") break;
                    data_list.Add(data[i].Trim());
                }

                correspond_bone.Add(data[0].Trim(), data_list);
            }
            sr.Close();
        }
    }
}
