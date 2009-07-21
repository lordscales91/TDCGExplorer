using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TAHTool
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

        public Form1()
        {
            InitializeComponent();
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
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
                return new MemoryStream(data_output);
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
