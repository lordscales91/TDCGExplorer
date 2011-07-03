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
        public List<string> NameList
        {
            get { return Selected.Keys.ToList(); }
        }
        public bool UseMan { get; set; }
        public Dictionary<string, bool> Selected { get; set; }

        public CorrespondTableList()
        {
            UseMan = false;
            Selected = new Dictionary<string, bool>();
        }

        /// 使うボーン対応表を結合して得ます。
        public CorrespondTable GetCorrespondTable()
        {
            CorrespondTable cortable = new CorrespondTable();

            List<string> names = new List<string>();

            if (!UseMan)
            {
                names.Add("Girl2Miku_Default");

                foreach (string name in Selected.Keys)
                {
                    if (Selected[name])
                        names.Add(name);
                }
            }
            else
            {
                names.Add("Man2Miku_Default");
            }

            string source_path = GetSourcePath();

            foreach (string name in names)
            {
                cortable.Load(Path.Combine(source_path, name));
            }

            return cortable;
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
                string name = Path.GetFileName(path);
                
                if (!DefaultNameList.Contains(name))
                    Selected.Add(name, false);
            }
        }
    }
}
