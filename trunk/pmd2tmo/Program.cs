using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace pmd2tmo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: pmd2tmo <source pmd>");
                return;
            }

            string source_file = args[0];

            PmdFile pmd = new PmdFile();
            pmd.Load(source_file);

            foreach (PmdNode node in pmd.nodes)
            {
                Console.WriteLine("id:{0} name:{1} parent_node_id:{2} position:[ {3} {4} {5} ]", node.id, node.name, node.parent_node_id, node.position.X, node.position.Y, node.position.Z);
            }
        }
    }
}
