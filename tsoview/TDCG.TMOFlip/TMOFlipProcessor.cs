using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TDCG.TMOFlip
{
public class TMOFlipProcessor
{
    public static string GetFlipNodesPath()
    {
        return Path.Combine(Application.StartupPath, @"flipnodes.txt");
    }

    public void Process(TMOFile tmo)
    {
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
                        cmat.Flip();
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
                        lmat.Flip();
                        rmat.Flip();
                        frame.matrices[lnode_id] = rmat;
                        frame.matrices[rnode_id] = lmat;
                    }
                }
            }
        }
    }
}
}
