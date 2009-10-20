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
        }

        public string File { get; set; }

        private void btnGetPoses_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(PosePath))
                return;

            string[] files = Directory.GetFiles(PosePath, "*.png");
            lvPoses.Items.Clear();
            ilPoses.Images.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    ilPoses.Images.Add(thumbnail);
                }
                lvPoses.Items.Add(Path.GetFileName(file), i);
            }
        }

        private void lvPoses_DoubleClick(object sender, EventArgs e)
        {
            if (lvPoses.SelectedItems.Count != 0)
                File = lvPoses.SelectedItems[0].Text;
            else
                File = null;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
