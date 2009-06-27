using System;
using System.Collections.Generic;
using System.IO;

namespace TAHdecrypt
{
    public interface ITMOCommand
    {
        Dictionary<string, TMONode> Nodes { set; }
        void Execute();
    }

    class TMODom
    {
        static void Main(string[] args) 
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: TMODom <tmo file>");
                return;
            }

            string source_file = args[0];
            string dest_file = source_file + ".tmp";

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
                ITMOCommand command = new TDCG.TMOTool.Command.Dom();
                command.Nodes = nodes;
                command.Execute();
            } catch (KeyNotFoundException) {
                Console.WriteLine("node not found.");
            }

            tmo.Save(dest_file);

            System.IO.File.Delete(source_file);
            System.IO.File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return;
        }
    }
}
