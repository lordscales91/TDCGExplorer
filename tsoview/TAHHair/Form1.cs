using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TDCG.TAHTool;

namespace TAHHair
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
            Regex re_hair_tsofile = new Regex(@"(U|B|C|D)00\.tso$");
            foreach (TAHEntry entry in decrypter.Entries)
            {
                if (entry.flag % 2 == 1)
                    continue;

                string file_name = entry.file_name;

                if (re_hair_tsofile.IsMatch(file_name))
                {
                    string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                    gvEntries.Rows.Add(row);
                }
            }
        }

    }
}
