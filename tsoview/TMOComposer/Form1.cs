using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TMOComposer
{
    public partial class Form1 : Form
    {
        Viewer viewer = null;
        TMOAnim tmoanim;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        string pose_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG\pose";

        public Form1()
        {
            InitializeComponent();
            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                string source_file = save_path + @"\system.tdcgsav.png";
                viewer.LoadAnyFile(source_file);
                viewer.SwitchMotionEnabled();
                timer1.Enabled = true;
            }
            CreateTMOAnim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(pose_path, "*.png");
            lvPoses.Items.Clear();
            imageList1.Images.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                using (Image thumbnail = Bitmap.FromFile(file))
                {
                    imageList1.Images.Add(thumbnail);
                }
                lvPoses.Items.Add(Path.GetFileName(file), i);
            }
        }

        private void CreateTMOAnim()
        {
            string file = @"TMOAnim.xml";
            if (File.Exists(file))
                tmoanim = TMOAnim.Load(file);
            else
                tmoanim = new TMOAnim();
            tmoAnimItemBindingSource.DataSource = tmoanim.items;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tmoanim.Dump(@"TMOAnim.xml");
            tmoanim.LoadSource();
            if (tmoanim.SourceTmo.frames != null)
            {
                tmoanim.Process();
                tmoanim.SaveSourceToFile(@"out.tmo");

                Figure fig;
                if (viewer.TryGetFigure(out fig))
                {
                    fig.Tmo = tmoanim.SourceTmo;
                    fig.UpdateNodeMapAndBoneMatrices();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (lvPoses.SelectedItems.Count != 0)
                    Clipboard.SetDataObject(lvPoses.SelectedItems[0].Text);
            }
        }

        private void lvPoses_DoubleClick(object sender, EventArgs e)
        {
            if (lvPoses.SelectedItems.Count == 0)
                return;

            TMOAnimItem item = new TMOAnimItem();
            item.PoseFile = lvPoses.SelectedItems[0].Text;
            item.Length = 30;
            tmoAnimItemBindingSource.Add(item);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gvTMOAnimItems.SelectedCells.Count == 0)
                return;

            int row = gvTMOAnimItems.SelectedCells[0].RowIndex;
            TMOAnimItem item = tmoanim.items[row];
            tmoAnimItemBindingSource.Remove(item);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (gvTMOAnimItems.SelectedCells.Count == 0)
                return;

            int row = gvTMOAnimItems.SelectedCells[0].RowIndex;
            if (row == 0)
                return;

            TMOAnimItem item = tmoanim.items[row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(row - 1, item);
            tmoAnimItemBindingSource.Position = row - 1;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (gvTMOAnimItems.SelectedCells.Count == 0)
                return;

            int row = gvTMOAnimItems.SelectedCells[0].RowIndex;
            if (row == tmoanim.items.Count-1)
                return;

            TMOAnimItem item = tmoanim.items[row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(row + 1, item);
            tmoAnimItemBindingSource.Position = row + 1;
        }
    }
}
