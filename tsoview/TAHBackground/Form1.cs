using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TAHTool;

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
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
                else if (ext == ".psd")
                {
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
            }
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

    }
}
