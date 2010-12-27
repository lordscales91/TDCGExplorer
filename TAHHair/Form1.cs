using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TDCG;
using TDCG.TAHTool;

namespace TAHHair
{
    public partial class Form1 : Form
    {
        TAHHairProcessor processor;

        public Form1()
        {
            InitializeComponent();

            processor = TAHHairProcessor.Load(Path.Combine(Application.StartupPath, @"TAHHairProcessor.xml"));
            //processor.Dump(@"TAHHairProcessor.xml");
            udTahVersion.Value = processor.TahVersion;
            SetColsItems();
        }

        private void SetColsItems()
        {
            cbColorSet.Items.Clear();
            foreach (string name in processor.GetColsNames())
            {
                cbColorSet.Items.Add(name);
            }
            foreach (string name in cbColorSet.Items)
            {
                if (name == processor.ColorSet)
                {
                    cbColorSet.SelectedItem = name;
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (diaOpen1.ShowDialog() == DialogResult.OK)
            {
                LoadTAHFile(diaOpen1.FileName);
            }
        }

        private void LoadTAHFile(string source_file)
        {
            processor.Open(source_file);

            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            lbStatus.Text = "Processing...";
            DumpEntries();
            lbStatus.Text = "ok. Loaded";
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in processor.GetTSOHairEntries())
            {
                string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                gvEntries.Rows.Add(row);
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void DumpFiles()
        {
            processor.ColorSet = (string)cbColorSet.SelectedItem;
            bwCompress.RunWorkerAsync();
        }

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            processor.ProgressChanged = delegate(int percent)
            {
                worker.ReportProgress(percent);
            };
            processor.Process();
        }

        private void bwCompress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void bwCompress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed");
            lbStatus.Text = "ok. Compressed";
            pbStatus.Value = 0;
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            processor.Close();
        }

        private void udTahVersion_ValueChanged(object sender, EventArgs e)
        {
            processor.TahVersion = (int)udTahVersion.Value;
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    LoadAnyFile(src, (e.KeyState & 8) == 8);
            }
        }

        public void LoadAnyFile(string source_file, bool append)
        {
            switch (Path.GetExtension(source_file).ToLower())
            {
                case ".tah":
                    LoadTAHFile(source_file);
                    break;
            }
        }
    }
}
