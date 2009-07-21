using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;
using CSScriptLibrary;

namespace TAHTool
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

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

        public Form1()
        {
            InitializeComponent();

            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (diaOpen1.ShowDialog() == DialogResult.OK)
            {
                source_file = diaOpen1.FileName;
                decrypter.Load(source_file);
                btnCompress.Enabled = false;
                btnLoad.Enabled = false;
                lbStatus.Text = "Processing...";
                DumpEntries();
                lbStatus.Text = "ok. Loaded";
                btnLoad.Enabled = true;
                btnCompress.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            Dictionary<string, Proportion> portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in config.Proportions)
                portion_map[portion.ClassName] = portion;

            Encrypter encrypter = new Encrypter();
            encrypter.SourcePath = @".";

            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();

            foreach (TAHEntry entry in decrypter.Entries)
            {
                string ext = Path.GetExtension(entry.file_name).ToLower();
                if (ext == ".tmo")
                {
                    string true_file_name = encrypter.SourcePath + "/" + entry.file_name;
                    entries[true_file_name] = entry;
                    encrypter.Add(true_file_name);
                }
            }
            
            int entries_count = encrypter.Count;
            int current_index = 0;
            encrypter.GetFileEntryStream = delegate(string true_file_name)
            {
                Console.WriteLine("compressing {0}", true_file_name);
                TAHEntry entry = entries[true_file_name];
                byte[] data_output;
                decrypter.ExtractResource(entry, out data_output);
                
                TMOFile tmo = new TMOFile();
                MemoryStream tmo_stream = new MemoryStream(data_output);
                tmo.Load(tmo_stream);

                if (tmo.nodes[0].Name == "|W_Hips")
                {
                    tpo_list.Tmo = tmo;
                    tpo_list.SetProportionList(pro_list);

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

                    tmo_stream.Seek(0, SeekOrigin.Begin);
                    tmo.Save(tmo_stream);
                }
                tmo_stream.Seek(0, SeekOrigin.Begin);
                
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
                return tmo_stream;
            };

            encrypter.Save(@"tmo-" + Path.GetFileName(source_file));
        }

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                gvEntries.Rows.Add(row);
            }
        }

        private void DumpFiles()
        {
            bwCompress.RunWorkerAsync(source_file);
        }

        static string GetExtensionFromMagic(byte[] data_output)
        {
            string ext;
            string magic = System.Text.Encoding.ASCII.GetString(data_output, 0, 4);
            switch (magic)
            {
                case "8BPS":
                    ext = ".psd";
                    break;
                case "TMO1":
                    ext = ".tmo";
                    break;
                case "TSO1":
                    ext = ".tso";
                    break;
                case "OggS":
                    ext = ".ogg";
                    break;
                case "BBBB":
                    ext = ".tbn";
                    break;
                default:
                    ext = ".cgfx";
                    break;
            }
            return ext;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            decrypter.Close();
        }

        private void bwCompress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed");
            lbStatus.Text = "ok. Compressed";
            pbStatus.Value = 0;
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        private void bwCompress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
