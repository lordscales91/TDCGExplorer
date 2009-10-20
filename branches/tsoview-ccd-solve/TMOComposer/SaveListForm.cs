using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOComposer
{
    public partial class SaveListForm : Form
    {
        public string SavePath { get; set; }

        public SaveListForm()
        {
            InitializeComponent();
        }
        
        public string File { get; set; }

        private void btnGetSaves_Click(object sender, EventArgs e)
        {
            if (! Directory.Exists(SavePath))
                return;

            string[] files = Directory.GetFiles(SavePath, "*.png");
            lvSaves.Items.Clear();
            ilSaves.Images.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilSaves.Images.Add(thumbnail);
                }
                lvSaves.Items.Add(Path.GetFileName(file), i);
            }
        }

        private void lvSaves_DoubleClick(object sender, EventArgs e)
        {
            if (lvSaves.SelectedItems.Count != 0)
                File = lvSaves.SelectedItems[0].Text;
            else
                File = null;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
