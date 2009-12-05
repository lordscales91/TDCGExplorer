using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CSScriptLibrary;

namespace TDCG
{
    /// <summary>
    /// �̌^�X�N���v�g�̃��X�g�������܂��B
    /// </summary>
public class ProportionList
{
    /// <summary>
    /// �̌^�X�N���v�g�̃��X�g
    /// </summary>
    public List<IProportion> items = new List<IProportion>();

    /// <summary>
    /// �̌^�X�N���v�g�t�H���_�̃p�X�𓾂܂��B
    /// </summary>
    /// <returns>�̌^�X�N���v�g�t�H���_�̃p�X</returns>
    public static string GetProportionPath()
    {
        return Path.Combine(Application.StartupPath, @"Proportion");
    }

    /// <summary>
    /// �̌^�X�N���v�g��ǂݍ��݂܂��B
    /// </summary>
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
