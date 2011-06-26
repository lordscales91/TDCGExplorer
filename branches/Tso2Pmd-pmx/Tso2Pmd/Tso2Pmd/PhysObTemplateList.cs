using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CSScriptLibrary;
using TDCG;
using TDCGUtils;

namespace Tso2Pmd
{
/// <summary>
/// 物理オブジェクトスクリプトのリストを扱います。
/// </summary>
public class PhysObTemplateList
{
    /// <summary>
    /// 物理オブジェクトスクリプトのリスト
    /// </summary>
    public List<IPhysObTemplate> items = new List<IPhysObTemplate>();

    /// <summary>
    /// 物理オブジェクトスクリプトフォルダのパスを得ます。
    /// </summary>
    /// <returns>物理オブジェクトスクリプトフォルダのパス</returns>
    public static string GetProportionPath()
    {
        return Path.Combine(Application.StartupPath, @"PhysObject");
    }

    /// <summary>
    /// 物理オブジェクトスクリプトを読み込みます。
    /// </summary>
    public void Load()
    {
        string proportion_path = GetProportionPath();
        if (! Directory.Exists(proportion_path))
            return;

        string[] script_files = Directory.GetFiles(proportion_path, "*.cs");
        foreach (string script_file in script_files)
        {
            string class_name = "TDCG.PhysObTemplate." + Path.GetFileNameWithoutExtension(script_file);
            var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IPhysObTemplate>();
            items.Add(script);
        }
    }
}
}
