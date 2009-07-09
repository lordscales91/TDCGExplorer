using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOBone
{

//指定boneを先端とするbone treeを辿る。
//bone行列を出力する。
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: TSOBone <source file> <tail node name>");
            return;
        }
        string source_file = args[0];
        string tail_node_name = args[1];

        TSOFile tso = new TSOFile();
        tso.Load(source_file);

        Program program = new Program(tso);
        program.NodeTreeDump(tail_node_name);
    }

    internal TSOFile tso;
    internal Dictionary<string, TSONode> nodemap;

    public Program(TSOFile tso)
    {
        this.tso = tso;

        nodemap = new Dictionary<string, TSONode>();
        foreach (TSONode node in tso.nodes)
            nodemap[node.ShortName] = node;
    }

    public void NodeTreeDump(string tail_node_name)
    {
        TSONode node;

        if (nodemap.TryGetValue(tail_node_name, out node))
        {
            while (node != null)
            {
                DumpNode(node);
                node = node.parent;
            }
        }
    }

    public void DumpNode(TSONode node)
    {
        Console.WriteLine("node {0}", node.ShortName);
        DumpMatrix(ref node.transformation_matrix);
    }

    public void DumpMatrix(ref Matrix m)
    {
        Console.WriteLine("[ [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M11, m.M12, m.M13, m.M14);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M21, m.M22, m.M23, m.M24);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ], ", m.M31, m.M32, m.M33, m.M34);
        Console.WriteLine("  [ {0:F2}, {1:F2}, {2:F2}, {3:F2} ] ]", m.M41, m.M42, m.M43, m.M44);
    }
}
}
