using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Tso2Pmd
{
    /// <summary>
    /// ボーン対応表リストを扱います。
    /// </summary>
    public class CorrespondTableList
    {
        Dictionary<string, CorrespondTable> ct_dic = new Dictionary<string, CorrespondTable>();

        public List<string> NameList
        {
            get { return Used.Keys.ToList(); }
        }
        public bool ManUsed { get; set; }
        public Dictionary<string, bool> Used { get; set; }

        public CorrespondTableList()
        {
            ManUsed = false;
            Used = new Dictionary<string, bool>();
        }

        /// 使うボーン対応表を結合して得ます。
        public CorrespondTable GetCorrespondTable()
        {
            CorrespondTable ct = new CorrespondTable();

            if (!ManUsed)
            {
                ct.Update(ct_dic["Girl2Miku_Default"]);

                foreach (string name in Used.Keys)
                {
                    if (Used[name])
                        ct.Update(ct_dic[name]);
                }
            }
            else
            {
                ct.Update(ct_dic["Man2Miku_Default"]);
            }

            return ct;
        }

        public string GetSourcePath()
        {
            return Path.Combine(Application.StartupPath, @"CorrespondTable");
        }

        static string[] DefaultNameList = new string[] { "Girl2Miku_Default", "Man2Miku_Default" };

        /// ボーン対応表を読み込みます。
        public void Load()
        {
            foreach (string path in Directory.GetDirectories(GetSourcePath()))
            {
                CorrespondTable ct = new CorrespondTable();
                
                ct.Load(path);

                string name = Path.GetFileName(path);
                
                if (!DefaultNameList.Contains(name))
                {
                    Used.Add(name, false);
                }
                ct_dic.Add(name, ct);
            }
        }
    }
}
