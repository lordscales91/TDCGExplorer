using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

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
    public Morph(string name, TMOFile tmo)
    {
        this.name = name;
        this.tmo = tmo;
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
    public MorphGroup(string name, NodesRange nodes_range)
    {
        this.name = name;
        this.nodes_range = nodes_range;
        items = new List<Morph>();
    }

    /// �ό`�������������܂��B
    public void ClearRatios()
    {
        foreach (Morph morph in items)
            morph.Ratio = 0.0f;
    }

    /// �w�肵�����O�������[�t���������܂��B
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

    static void AddTreeNodes(List<TMONode> select_nodes, TMONode root_node)
    {
        select_nodes.Add(root_node);
        foreach (TMONode node in root_node.children)
            AddTreeNodes(select_nodes, node);
    }

    /// <summary>
    /// ���[�t�ό`�̑ΏۂƂȂ�m�[�h��I�����܂��B
    /// </summary>
    /// <param name="tmo">�Ώ�tmo</param>
    /// <returns>�m�[�h���X�g</returns>
    public List<TMONode> SelectNodes(TMOFile tmo)
    {
        List<TMONode> select_nodes = new List<TMONode>();
        foreach (string root_name in nodes_range.root_names)
        {
            TMONode root_node = tmo.FindNodeByName(root_name);
            if (root_node != null)
                AddTreeNodes(select_nodes, root_node);
        }
        return select_nodes;
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
    /// <param name="source_path">�t�H���_��</param>
    public void Load(string source_path)
    {
        foreach (string group_path in Directory.GetDirectories(source_path))
        {
            //Debug.WriteLine("group_path: " + group_path);
            string group_name = Path.GetFileName(group_path);
            Debug.WriteLine("group_name: " + group_name);
            
            NodesRange nodes_range = NodesRange.Load(Path.Combine(group_path, @"NodesRange.xml"));
            MorphGroup group = new MorphGroup(group_name, nodes_range);
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
    /// ���[�t�ό`�����s���܂��B
    /// </summary>
    /// <param name="tmo">�Ώ�tmo</param>
    public void Morph(TMOFile tmo)
    {
        tmo.LoadTransformationMatrixFromFrame(0);

        foreach (MorphGroup group in groups)
        {
            List<TMONode> select_nodes = group.SelectNodes(tmo);

            foreach (Morph morph in group.Items)
            {
                if (morph.Ratio == 0.0f)
                    continue;

                foreach (TMONode node in select_nodes)
                {
                    Matrix min = node.TransformationMatrix;
                    Matrix max = morph.Tmo.FindNodeByName(node.Name).TransformationMatrix;
                    node.TransformationMatrix = SlideMatrices.GetMatrixRatio(min, max, morph.Ratio);
                }
            }
        }
    }
}
}
