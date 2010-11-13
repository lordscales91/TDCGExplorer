using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TDCG;

namespace TMOFlip
{
class Program
{
    public static string GetFlipNodesPath()
    {
        return Path.Combine(Application.StartupPath, @"flipnodes.txt");
    }

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("TMOFlip.exe <tmo file>");
            return;
        }
        string source_file = args[0];

        TMOFile tmo = new TMOFile();
        tmo.Load(source_file);

        Dictionary<string, TMONode> nodemap;
        nodemap = new Dictionary<string, TMONode>();

        foreach (TMONode node in tmo.nodes)
        {
            nodemap.Add(node.Name, node);
        }

        char[] delim = { ' ' };
        using (StreamReader source = new StreamReader(File.OpenRead(GetFlipNodesPath())))
        {
            string line;
            while ((line = source.ReadLine()) != null)
            {
                string[] tokens = line.Split(delim);
                string op = tokens[0];
                if (op == "flip")
                {
                    Debug.Assert(tokens.Length == 2, "tokens length should be 2");
                    string cnode_name = tokens[1];
                    int cnode_id = nodemap[cnode_name].ID;

                    foreach (TMOFrame frame in tmo.frames)
                    {
                        TMOMat cmat = frame.matrices[cnode_id];
                        FlipMatrix(ref cmat.m);
                    }
                }
                else
                if (op == "swap")
                {
                    Debug.Assert(tokens.Length == 3, "tokens length should be 3");
                    string lnode_name = tokens[1];
                    string rnode_name = tokens[2];
                    int lnode_id = nodemap[lnode_name].ID;
                    int rnode_id = nodemap[rnode_name].ID;

                    foreach (TMOFrame frame in tmo.frames)
                    {
                        TMOMat lmat = frame.matrices[lnode_id];
                        TMOMat rmat = frame.matrices[rnode_id];
                        FlipMatrix(ref lmat.m);
                        FlipMatrix(ref rmat.m);
                        frame.matrices[lnode_id] = rmat;
                        frame.matrices[rnode_id] = lmat;
                    }
                }
            }
        }

        string dest_path = Path.GetDirectoryName(source_file);
        string dest_file = Path.GetFileNameWithoutExtension(source_file) + @".new.tmo";
        dest_path = Path.Combine(dest_path, dest_file);
        Console.WriteLine("Save File: " + dest_path);
        tmo.Save(dest_path);
    }

    static void FlipMatrix(ref Matrix m)
    {
        //y‰ñ“]
        m.M31 = -m.M31;
        m.M13 = -m.M13;

        //z‰ñ“]
        m.M21 = -m.M21;
        m.M12 = -m.M12;

        //xˆÚ“®
        m.M41 = -m.M41;
    }
}
}
