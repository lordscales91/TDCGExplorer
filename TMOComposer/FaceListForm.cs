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
    public partial class FaceListForm : Form
    {
        public string FacePath { get; set; }

        public FaceListForm()
        {
            InitializeComponent();
        }

        public string FileName { get; set; }

        private void btnGetFaces_Click(object sender, EventArgs e)
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
            if (Directory.Exists(FacePath))
            {
                string[] files = Directory.GetFiles(FacePath, "*.png");
                Array.Sort(files, CompareFiles);
                return files;
            }
            else
                return null;
        }

        public void UpdateImageList(string[] files)
        {
            ilFaces.Images.Clear();
            foreach (string file in files)
            {
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilFaces.Images.Add(thumbnail);
                }
            }
        }

        public void UpdateViewItems(string[] files)
        {
            lvFaces.Items.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                lvFaces.Items.Add(Path.GetFileName(file), i);
            }
        }

        private void lvFaces_DoubleClick(object sender, EventArgs e)
        {
            if (lvFaces.SelectedItems.Count != 0)
                FileName = lvFaces.SelectedItems[0].Text;
            else
                FileName = null;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
