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
            if (! Directory.Exists(FacePath))
                return;

            string[] files = Directory.GetFiles(FacePath, "*.png");
            lvFaces.Items.Clear();
            ilFaces.Images.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilFaces.Images.Add(thumbnail);
                }
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
