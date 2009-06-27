using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using CSScriptLibrary;

namespace TAHdecrypt
{
    public interface ITMOCommand
    {
        Dictionary<string, TMONode> Nodes { set; }
        void Execute();
    }

    public class TMOTool
    {
        static string GetCommandPath()
        {
            return Application.StartupPath + @"\command";
        }
        static string GetDestinationPath()
        {
            return @"updated";
        }

        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TMOTool <source file> [script name ...]");
                return;
            }

            string source_file = args[0];
            string dest_file = Path.Combine(GetDestinationPath(), Path.GetFileName(source_file));
            Directory.CreateDirectory(GetDestinationPath());

            TMOFile tmo = new TMOFile();
            try
            {
                tmo.Load(source_file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (tmo.nodes[0].ShortName != "W_Hips") {
                Console.WriteLine("Passed: root node is not W_Hips");
                return;
            }

            Dictionary<string, TMONode> nodes = new Dictionary<string, TMONode>();

            foreach(TMONode node in tmo.nodes)
            try {
                nodes.Add(node.ShortName, node);
            } catch (ArgumentException) {
                Console.WriteLine("node {0} already exists.", node.ShortName);
            }

            try {
                for (int i = 1; i < args.Length; i++)
                {
                    string script_name = args[i];
                    string script_file = Path.Combine(GetCommandPath(), script_name + ".cs");
                    var script = CSScript.Load(script_file).CreateInstance("TDCG.TMOTool.Command." + script_name).AlignToInterface<ITMOCommand>();
                    script.Nodes = nodes;
                    script.Execute();
                }
            } catch (KeyNotFoundException) {
                Console.WriteLine("node not found.");
            }

            tmo.Save(dest_file);
            Console.WriteLine("saved " + dest_file);

            return;
        }
    }
}
