using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TDCG;
using CSScriptLibrary;

namespace TMOProp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: TMOProp <source tmo>");
                return;
            }

            string source_file = args[0];

            Program program = new Program();
            program.SetProportionList();
            program.Process(source_file);
        }

        List<IProportion> pro_list = new List<IProportion>();
        TPOFileList tpo_list = new TPOFileList();

        public string GetProportionPath()
        {
            return Path.Combine(Application.StartupPath, @"Proportion");
        }

        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public void SetProportionList()
        {
            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }
            tpo_list.SetProportionList(pro_list);
        }

        public bool Process(string source_file)
        {
            TMOFile tmo;
            tmo = new TMOFile();
            tmo.Load(source_file);
            
            TPOConfig tpo_config = TPOConfig.Load(GetTPOConfigPath());
            Dictionary<string, Proportion> portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in tpo_config.Proportions)
                portion_map[portion.ClassName] = portion;
            
            if (tmo.nodes[0].Name == "|W_Hips")
            {
                tpo_list.Tmo = tmo;

                for (int i = 0; i < tpo_list.Count; i++)
                {
                    TPOFile tpo = tpo_list[i];
                    {
                        Debug.Assert(tpo.Proportion != null, "tpo.Proportion should not be null");
                        Proportion portion;
                        if (portion_map.TryGetValue(tpo.Proportion.ToString(), out portion))
                            tpo.Ratio = portion.Ratio;
                    }
                }

                tpo_list.Transform();
            }

            string dest_file = source_file + ".tmp";
            Console.WriteLine("Save File: " + dest_file);
            tmo.Save(dest_file);

            File.Delete(source_file);
            File.Move(dest_file, source_file);
            Console.WriteLine("updated " + source_file);

            return true;
        }
    }
}
