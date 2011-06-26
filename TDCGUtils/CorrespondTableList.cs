using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TDCGUtils
{
    public class CorrespondTableList
    {
        List<string> name_list = new List<string>();
        bool man_flag = false;
        Dictionary<string, bool> flag
            = new Dictionary<string, bool>();
        Dictionary<string, CorrespondTable> ct_dic
            = new Dictionary<string, CorrespondTable>();

        public List<string> NameList { get { return name_list; } }
        public bool SetManFlag { set { man_flag = value; } }
        public Dictionary<string, bool> Flag { get { return flag; } set { flag = value; } }

        public CorrespondTable GetCorrespondTable()
        {
            CorrespondTable ct = new CorrespondTable();

            if (man_flag == false)
            {
                ct.Add(ct_dic["Girl2Miku_Default"]);

                foreach (KeyValuePair<string, bool> kvp in flag)
                    if (kvp.Value == true) ct.Add(ct_dic[kvp.Key]);
            }
            else
            {
                ct.Add(ct_dic["Man2Miku_Default"]);
            }

            return ct;
        }

        public void Load()
        {
            string source_path = Path.Combine(Application.StartupPath, @"CorrespondTable");
            foreach (string path in Directory.GetDirectories(source_path))
            {
                string name = Path.GetFileName(path);
                CorrespondTable ct = new CorrespondTable(path);

                if (!(name == "Girl2Miku_Default" || name == "Man2Miku_Default"))
                {
                    name_list.Add(name);
                    flag.Add(name, false);
                }
                
                ct_dic.Add(name, ct);
            }
        }
    }
}
