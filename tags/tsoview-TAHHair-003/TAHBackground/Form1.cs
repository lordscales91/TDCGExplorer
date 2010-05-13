using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;
using TDCG.TAHTool;

namespace TAHBackground
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
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

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name;

                if (entry.flag % 2 == 1)
                {
                    byte[] data_output;
                    decrypter.ExtractResource(entry, out data_output);
                    file_name += GetExtensionFromMagic(data_output);
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tbn")
                {
                    if (file_name.StartsWith("script/backgrounds/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
                else
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
                else
                if (ext == ".tso")
                {
                    if (file_name.StartsWith("data/bgmodel/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
            }
        }

        List<string> GetPSDPathList()
        {
            List<string> ret = new List<string>();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name.ToLower();
                string ext = Path.GetExtension(file_name);
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                        ret.Add(file_name);
                }
            }
            return ret;
        }

        private void DumpFiles()
        {
            bwCompress.RunWorkerAsync(source_file);
        }

        static string GetExtensionFromMagic(byte[] magic)
        {
            string ext;
            if (magic[0] == '8' && magic[1] == 'B' && magic[2] == 'P' && magic[3] == 'S')
                ext = ".psd";
            else
            if (magic[0] == 'T' && magic[1] == 'M' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tmo";
            else
            if (magic[0] == 'T' && magic[1] == 'S' && magic[2] == 'O' && magic[3] == '1')
                ext = ".tso";
            else
            if (magic[0] == 'O' && magic[1] == 'g' && magic[2] == 'g' && magic[3] == 'S')
                ext = ".ogg";
            else
            if (magic[0] == 'B' && magic[1] == 'B' && magic[2] == 'B' && magic[3] == 'B')
                ext = ".tbn";
            else
            if (magic[0] == 0x89 && magic[1] == 'P' && magic[2] == 'N' && magic[3] == 'G')
                ext = ".png";
            else
                ext = ".cgfx";
            return ext;
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name.ToLower();

                if (entry.flag % 2 == 1)
                {
                    byte[] data_output;
                    decrypter.ExtractResource(entry, out data_output);
                    file_name += GetExtensionFromMagic(data_output);
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tbn")
                {
                    if (file_name.StartsWith("script/backgrounds/"))
                        entries[file_name] = entry;
                }
                else
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                        entries[file_name] = entry;
                }
                else
                if (ext == ".tso")
                {
                    if (file_name.StartsWith("data/bgmodel/"))
                        entries[file_name] = entry;
                }
            }

            List<string> PSDPathList = GetPSDPathList();
            int entries_count = PSDPathList.Count;
            int current_index = 0;
            foreach (string psd_path in PSDPathList)
            {
                Console.WriteLine("psd {0}", psd_path);
                byte[] data_output;

                TAHEntry psd_entry = entries[psd_path];
                decrypter.ExtractResource(psd_entry, out data_output);
                MemoryStream psd_stream = new MemoryStream(data_output);

                string tbn_path = PngBack.GetTBNPathFromPSDPath(psd_path);
                Console.WriteLine("tbn {0}", tbn_path);
                TAHEntry tbn_entry = entries[tbn_path];
                decrypter.ExtractResource(tbn_entry, out data_output);
                MemoryStream tbn_stream = new MemoryStream(data_output);

                PngBack back = new PngBack();
                back.Load(tbn_stream, psd_stream);
                foreach (string str in PngBack.GetTSOPathListFromTBNFile(back.Tbn))
                {
                    string tso_path = str.ToLower();
                    Console.WriteLine("tso {0}", tso_path);
                    TAHEntry tso_entry;
                    if (entries.TryGetValue(tso_path, out tso_entry))
                    {
                        decrypter.ExtractResource(tso_entry, out data_output);
                        back.AddTSOFile(data_output);
                    }
                }
                string png_path = Path.GetFileNameWithoutExtension(psd_path) + @".png";
                back.Save(png_path);
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
            }
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
