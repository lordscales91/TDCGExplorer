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
    public partial class Form3 : Form
    {
        string pose_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG\pose";

        public Form3()
        {
            InitializeComponent();
        }

        private void btnGetPoses_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(pose_path, "*.png");
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
    }
}
