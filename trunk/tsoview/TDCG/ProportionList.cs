using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CSScriptLibrary;

namespace TDCG
{
public class ProportionList
{
    public List<IProportion> items = new List<IProportion>();

    public static string GetProportionPath()
    {
        return Path.Combine(Application.StartupPath, @"Proportion");
    }

    public void Load()
    {
        string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
        foreach (string script_file in script_files)
        {
            string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
            var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
            items.Add(script);
        }
    }
}
}
