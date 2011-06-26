using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using CSScriptLibrary;
using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
    /// <summary>
    /// 体型＆物理スクリプトのリストを扱います。
    /// </summary>
    public class TemplateList
    {
        /// <summary>
        /// 物理オブジェクトスクリプトのリスト
        /// </summary>
        public List<IPhysObTemplate> phys_items = new List<IPhysObTemplate>();

        public Dictionary<IPhysObTemplate, bool> phys_flag
            = new Dictionary<IPhysObTemplate, bool>();

        /// <summary>
        /// 体型＆物理スクリプトを読み込みます。
        /// </summary>
        public bool Load()
        {
            ProportionList.Instance.Load();

            // 物理オブジェクトスクリプトを読み込みます。
            string phys_path = Path.Combine(Application.StartupPath, @"PhysObTemplate");
            if (!Directory.Exists(phys_path))
            {
                MessageBox.Show("Tso2Pmdを正常に起動できませんでした。\n"
                    + "PhysObTemplateフォルダが見つかりません。");
                return false;
            }

            string[] phys_script_files = Directory.GetFiles(phys_path, "*.cs");
            foreach (string script_file in phys_script_files)
            {
                try
                {
                    StreamReader sr = new StreamReader(script_file, Encoding.GetEncoding("Shift_JIS"));
                    string text = sr.ReadToEnd();
                    sr.Close();

                    string class_name = "TDCG.PhysObTemplate." + Path.GetFileNameWithoutExtension(script_file);
                    //var script = CSScript.Load(script_file, Path.GetTempFileName(), true).CreateInstance(class_name).AlignToInterface<IPhysObTemplate>();
                    var script = CSScript.LoadCode(text).CreateInstance(class_name).AlignToInterface<IPhysObTemplate>();
                    phys_items.Add(script);
                    phys_flag.Add(script, false);
                }
                catch
                {
                    MessageBox.Show("Tso2Pmdを正常に起動できませんでした。\n"
                        + "スクリプトファイル（PhysObTemplate/" + Path.GetFileName(script_file) + ")を\n"
                        + "読込中にエラーが発生しました。");
                    return false;
                }
            }

            return true;
        }


        public void PhysObExecute(ref T2PPhysObjectList physOb_list)
        {
            foreach (IPhysObTemplate i in phys_items)
            {
                if (phys_flag[i] == true)
                {
                    try
                    {
                        i.Execute(ref physOb_list);
                    }
                    catch (KeyNotFoundException)
                    {
                        //メッセージボックスを表示する
                        MessageBox.Show(
                            "物理テンプレート（" + i.Name() + "）実行時に、エラーが発生しました。\n"
                            + "物理テンプレート（" + i.Name() + "）は無視されます。",
                            "エラー",
                            MessageBoxButtons.OK);
                    }
                }
            }
        }
    }
}
