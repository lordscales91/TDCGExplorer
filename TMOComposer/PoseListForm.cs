using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TMOComposer
{
    public partial class PoseListForm : Form
    {
        public string PosePath { get; set; }

        public PoseListForm()
        {
            InitializeComponent();
            AddListView(lvPoses);
        }

        public string FileName { get; set; }

        List<ListView> listviews = new List<ListView>();

        public void AddListView(ListView lv)
        {
            lv.LargeImageList = ilPoses;
            listviews.Add(lv);
        }

        private void btnGetPoses_Click(object sender, EventArgs e)
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
            if (Directory.Exists(PosePath))
            {
                string[] files = Directory.GetFiles(PosePath, "*.png");
                Array.Sort(files, CompareFiles);
                return files;
            }
            else
                return null;
        }

        public void UpdateImageList(string[] files)
        {
            ilPoses.Images.Clear();
            foreach (string file in files)
            {
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilPoses.Images.Add(thumbnail);
                }
            }
        }

        public void UpdateViewItems(string[] files)
        {
            foreach (ListView lv in listviews)
            {
                lv.Items.Clear();
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    lv.Items.Add(Path.GetFileName(file), i);
                }
            }
        }

        private void lvPoses_DoubleClick(object sender, EventArgs e)
        {
            if (lvPoses.SelectedItems.Count != 0)
                FileName = lvPoses.SelectedItems[0].Text;
            else
                FileName = null;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
