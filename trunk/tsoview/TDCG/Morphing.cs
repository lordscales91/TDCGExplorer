using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
    public Morph()
    {
        name = null;
        tmo = null;
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
    public MorphGroup()
    {
        name = null;
        nodes_range = new NodesRange();
        items = new List<Morph>();
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
    /// <param name="source_file">フォルダ名</param>
    public void Load(string source_file)
    {
    }

    /// <summary>
    /// モーフ変形を実行します。
    /// </summary>
    /// <param name="tmo">対象tmo</param>
    public void Morph(TMOFile tmo)
    {
    }
}
}
