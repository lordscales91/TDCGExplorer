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
        
        public string FileName { get; set; }

        private void btnGetSaves_Click(object sender, EventArgs e)
        {
            string[] files = GetFiles();
            UpdateImageList(files);
            UpdateViewItems(files);
        }

        private static int CompareFiles(string x, string y)
        {
            return DateTime.Compare(File.GetCreationTime(y), File.GetCreationTime(x));
        }

        public string[] GetFiles()
        {
            if (Directory.Exists(SavePath))
            {
                string[] files = Directory.GetFiles(SavePath, "*.png");
                Array.Sort(files, CompareFiles);
                return files;
            }
            else
                return null;
        }

        public void UpdateImageList(string[] files)
        {
            ilSaves.Images.Clear();
            foreach (string file in files)
            {
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilSaves.Images.Add(thumbnail);
                }
            }
        }

        public void UpdateViewItems(string[] files)
        {
            lvSaves.Items.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                lvSaves.Items.Add(Path.GetFileName(file), i);
            }
        }

        private void lvSaves_DoubleClick(object sender, EventArgs e)
        {
            if (lvSaves.SelectedItems.Count != 0)
                FileName = lvSaves.SelectedItems[0].Text;
            else
                FileName = null;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
