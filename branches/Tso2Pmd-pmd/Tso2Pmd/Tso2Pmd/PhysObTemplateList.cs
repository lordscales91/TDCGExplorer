using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CSScriptLibrary;

namespace Tso2Pmd
{
/// <summary>
/// 物理オブジェクトスクリプトのリストを扱います。
/// </summary>
public class PhysObTemplateList
{
    static readonly PhysObTemplateList instance = new PhysObTemplateList();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static PhysObTemplateList()
    {
    }

    PhysObTemplateList()
    {
    }

    /// <summary>
    /// 物理オブジェクトリスト
    /// </summary>
    public static PhysObTemplateList Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// 物理オブジェクトスクリプトのリスト
    /// </summary>
    public List<IPhysObTemplate> items = new List<IPhysObTemplate>();

    public Dictionary<IPhysObTemplate, bool> flags = new Dictionary<IPhysObTemplate, bool>();

    /// <summary>
    /// 物理オブジェクトスクリプトフォルダのパスを得ます。
    /// </summary>
    /// <returns>物理オブジェクトスクリプトフォルダのパス</returns>
    public static string GetPhysObTemplatePath()
    {
        return Path.Combine(Application.StartupPath, @"PhysObTemplate");
    }

    /// <summary>
    /// 物理オブジェクトスクリプトを読み込みます。
    /// </summary>
    public void Load()
    {
        // 物理オブジェクトスクリプトを読み込みます。
        string template_path = GetPhysObTemplatePath();

        string[] script_files = Directory.GetFiles(template_path, "*.cs");
        foreach (string script_file in script_files)
        {
            string assembly_file = Path.GetTempFileName();
            string class_name = "TDCG.PhysObTemplate." + Path.GetFileNameWithoutExtension(script_file);
            var script = CSScript.Load(script_file, assembly_file, true, null).CreateInstance(class_name).AlignToInterface<IPhysObTemplate>();

            items.Add(script);
            flags.Add(script, false);
        }
    }
}
}
