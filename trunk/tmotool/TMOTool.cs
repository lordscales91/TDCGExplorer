using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
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
        static Regex re;
        static List<string> script_names = new List<string>();

        static string GetCommandPath()
        {
            return Application.StartupPath + @"\command";
        }
        static string GetDestinationPath()
        {
            return Path.GetFullPath(@"updated");
        }

        static void Main(string[] args) 
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TMOTool <tmo file> [script name ...]");
                return;
            }

            string source_file = Path.GetFullPath(args[0]);
            for (int i = 1; i < args.Length; i++)
                script_names.Add(args[i]);
            try
            {
                string ext = Path.GetExtension(source_file).ToUpper();
                if (ext == ".TMO")
                {
                    re = new Regex(@"\A" + Regex.Escape(Path.GetDirectoryName(source_file)) + @"\\?");
                    DumpTMOEntries(source_file);
                }
                else if (Directory.Exists(source_file))
                {
                    re = new Regex(@"\A" + Regex.Escape(source_file) + @"\\?");
                    DumpDirEntries(source_file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            return;
        }

        public static void DumpDirEntries(string dir)
        {
            if (dir == GetDestinationPath())
                return;

            string[] tmo_files = Directory.GetFiles(dir, "*.TMO");
            foreach (string file in tmo_files)
            {
                DumpTMOEntries(file);
            }
            string[] entries = Directory.GetDirectories(dir);
            foreach (string entry in entries)
            {
                DumpDirEntries(entry);
            }
        }

        public static void DumpTMOEntries(string source_file)
        {
            string dest_file = Path.Combine(GetDestinationPath(), re.Replace(source_file, @""));
            Directory.CreateDirectory(Path.GetDirectoryName(dest_file));

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
            Console.WriteLine("processing " + source_file);

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
                foreach (string script_name in script_names)
                {
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
        }
    }
}
