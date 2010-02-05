using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
/// モーフ
public class Morph
{
    string name;
    /// 名前
    public string Name { get { return name; }}

    TMOFile tmo;
    /// tmo
    public TMOFile Tmo { get { return tmo; }}

    float ratio;
    /// 変形割合
    public float Ratio
    {
        get
        {
            return ratio;
        }
        set
        {
            ratio = value;
        }
    }

    /// <summary>
    /// モーフを生成します。
    /// </summary>
    public Morph(string name, TMOFile tmo)
    {
        this.name = name;
        this.tmo = tmo;
        ratio = 0.0f;
    }
}

/// モーフグループ
public class MorphGroup
{
    string name;
    /// 名前
    public string Name { get { return name; }}

    NodesRange nodes_range;

    List<Morph> items;
    /// モーフリスト
    public List<Morph> Items { get { return items; }}

    /// <summary>
    /// モーフグループを生成します。
    /// </summary>
    public MorphGroup(string name)
    {
        this.name = name;
        nodes_range = new NodesRange();
        items = new List<Morph>();
    }

    public void ClearRatios()
    {
        foreach (Morph morph in items)
            morph.Ratio = 0.0f;
    }

    public Morph FindItemByName(string name)
    {
        Morph found = null;
        foreach (Morph morph in items)
        {
            if (morph.Name == name)
            {
                found = morph;
                break;
            }
        }
        return found;
    }

    /// <summary>
    /// モーフ変形の対象となるノードを選択します。
    /// </summary>
    /// <param name="tmo">対象tmo</param>
    /// <returns>ノードリスト</returns>
    public List<TMONode> SelectNodes(TMOFile tmo)
    {
        return new List<TMONode>();
    }
}

/// モーフィング
public class Morphing
{
    List<MorphGroup> groups;
    /// モーフグループリスト
    public List<MorphGroup> Groups { get { return groups; }}

    /// <summary>
    /// モーフィングを生成します。
    /// </summary>
    public Morphing()
    {
        groups = new List<MorphGroup>();
    }

    /// <summary>
    /// モーフライブラリを読み込みます。
    /// </summary>
    /// <param name="source_path">フォルダ名</param>
    public void Load(string source_path)
    {
        foreach (string group_path in Directory.GetDirectories(source_path))
        {
            //Debug.WriteLine("group_path: " + group_path);
            string group_name = Path.GetFileName(group_path);
            Debug.WriteLine("group_name: " + group_name);
            
            MorphGroup group = new MorphGroup(group_name);
            groups.Add(group);

            foreach (string tmo_file in Directory.GetFiles(Path.Combine(source_path, group_path), @"*.tmo"))
            {
                //Debug.WriteLine("tmo_file: " + tmo_file);
                string morph_name = Path.GetFileNameWithoutExtension(tmo_file);
                Debug.WriteLine("morph_name: " + morph_name);

                TMOFile tmo = new TMOFile();
                tmo.Load(tmo_file);
                tmo.LoadTransformationMatrixFromFrame(0);

                Morph morph = new Morph(morph_name, tmo);
                group.Items.Add(morph);
            }
        }
    }

    /// <summary>
    /// モーフ変形を実行します。
    /// </summary>
    /// <param name="tmo">対象tmo</param>
    public void Morph(TMOFile tmo)
    {
        tmo.LoadTransformationMatrixFromFrame(0);

        foreach (TMONode node in tmo.nodes)
        {
            if (node.Name == "W_Hips")
            {
                node.TransformationMatrix = Matrix.Identity;
                continue;
            }

            foreach (MorphGroup group in groups)
            {
                foreach (Morph morph in group.Items)
                {
                    if (morph.Ratio == 0.0f)
                        continue;

                    Matrix m = morph.Tmo.FindNodeByName(node.Name).TransformationMatrix;
                    node.TransformationMatrix = m;
                }
            }
        }
    }
}
}
