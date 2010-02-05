using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TDCG
{
/// ���[�t
public class Morph
{
    string name;
    /// ���O
    public string Name { get { return name; }}

    TMOFile tmo;
    /// tmo
    public TMOFile Tmo { get { return tmo; }}

    float ratio;
    /// �ό`����
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
    /// ���[�t�𐶐����܂��B
    /// </summary>
    public Morph()
    {
        name = null;
        tmo = null;
        ratio = 0.0f;
    }
}

/// ���[�t�O���[�v
public class MorphGroup
{
    string name;
    /// ���O
    public string Name { get { return name; }}

    NodesRange nodes_range;

    List<Morph> items;
    /// ���[�t���X�g
    public List<Morph> Items { get { return items; }}

    /// <summary>
    /// ���[�t�O���[�v�𐶐����܂��B
    /// </summary>
    public MorphGroup()
    {
        name = null;
        nodes_range = new NodesRange();
        items = new List<Morph>();
    }

    /// <summary>
    /// ���[�t�ό`�̑ΏۂƂȂ�m�[�h��I�����܂��B
    /// </summary>
    /// <param name="tmo">�Ώ�tmo</param>
    /// <returns>�m�[�h���X�g</returns>
    public List<TMONode> SelectNodes(TMOFile tmo)
    {
        return new List<TMONode>();
    }
}

/// ���[�t�B���O
public class Morphing
{
    List<MorphGroup> groups;
    /// ���[�t�O���[�v���X�g
    public List<MorphGroup> Groups { get { return groups; }}

    /// <summary>
    /// ���[�t�B���O�𐶐����܂��B
    /// </summary>
    public Morphing()
    {
        groups = new List<MorphGroup>();
    }

    /// <summary>
    /// ���[�t���C�u������ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�t�H���_��</param>
    public void Load(string source_file)
    {
    }

    /// <summary>
    /// ���[�t�ό`�����s���܂��B
    /// </summary>
    /// <param name="tmo">�Ώ�tmo</param>
    public void Morph(TMOFile tmo)
    {
    }
}
}
