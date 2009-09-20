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
        public List<PngSaveItem> items = new List<PngSaveItem>();
        Form2 form2 = null;

        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        string pose_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG\pose";
        string tmoanim_file = @"TMOAnim.xml";
        string out_tmo_file = @"out.tmo";

        public Form1()
        {
            InitializeComponent();
            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                string save_file = "system.tdcgsav.png";
                
                PngSaveItem item = new PngSaveItem();
                item.File = save_file;
                pngSaveItemBindingSource.Add(item);

                viewer.LoadAnyFile(save_path + @"\" + item.File);
                
                viewer.SwitchMotionEnabled();
                timer1.Enabled = true;
            }
            CreateTMOAnim();
            form2 = new Form2();
        }

        private void SaveToPngEachFrame()
        {
            timer1.Enabled = false;

            string dest_path = @"snapshots";
            Directory.CreateDirectory(dest_path);

            int orig_frame_idx = viewer.FrameIndex; // backup
            int frame_len = viewer.GetMaxFrameLength();
            for (int frame_idx = 0; frame_idx < frame_len; frame_idx += 5)
            {
                viewer.FrameMove(frame_idx);
                viewer.Render();
                viewer.SaveToPng(dest_path + @"\" + String.Format("{0:D3}.png", frame_idx));
            }
            viewer.FrameIndex = orig_frame_idx; // restore
            timer1.Enabled = true;
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

        private void CreateTMOAnim()
        {
            if (File.Exists(tmoanim_file))
                tmoanim = TMOAnim.Load(tmoanim_file);
            else
                tmoanim = new TMOAnim();
            tmoAnimItemBindingSource.DataSource = tmoanim.items;
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            tmoanim.Dump(tmoanim_file);
            tmoanim.LoadSource();
            if (tmoanim.SourceTmo.frames != null)
            {
                tmoanim.Process();
                tmoanim.SaveSourceToFile(out_tmo_file);

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

        private void btnRec_Click(object sender, EventArgs e)
        {
            SaveToPngEachFrame();
        }

        private void gvFigures_SelectionChanged(object sender, EventArgs e)
        {
            if (gvFigures.SelectedCells.Count == 0)
                return;

            int row = gvFigures.SelectedCells[0].RowIndex;
            viewer.SetFigureIndex(row);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (form2.ShowDialog(this) == DialogResult.OK)
            {
                if (form2.File == null)
                    return;

                PngSaveItem item = new PngSaveItem();
                item.File = form2.File;
                pngSaveItemBindingSource.Add(item);

                viewer.LoadAnyFile(save_path + @"\" + item.File, true);
            }
        }
    }
}
