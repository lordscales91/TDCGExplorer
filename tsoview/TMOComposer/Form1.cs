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
        PngSave pngsave;
        Form2 form2 = null;

        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        string pose_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG\pose";
        string pngsave_file = @"PngSave.xml";
        string out_tmo_file = @"out.tmo";

        public Form1()
        {
            InitializeComponent();
            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                CreatePngSave();
                viewer.SwitchMotionEnabled();
                timer1.Enabled = true;
            }
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

        private void CreatePngSave()
        {
            if (File.Exists(pngsave_file))
                pngsave = PngSave.Load(pngsave_file);
            else
                pngsave = new PngSave();
            pngSaveItemBindingSource.DataSource = pngsave.items;

            foreach (PngSaveItem item in pngsave.items)
            {
                item.FigureIndex = viewer.FigureList.Count;
                viewer.LoadAnyFile(save_path + @"\" + item.File, true);
                Animate(item);
            }
            
            if (pngSaveItemBindingSource.Count == 0)
                CreatePngSaveItem("system.tdcgsav.png");
        }

        void CreatePngSaveItem(string file)
        {
            PngSaveItem item = new PngSaveItem();
            item.File = file;
            pngSaveItemBindingSource.Add(item);

            item.FigureIndex = viewer.FigureList.Count;
            viewer.LoadAnyFile(save_path + @"\" + item.File, true);
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;
            PngSaveItem item = pngsave.items[pngsave_row];

            pngsave.Dump(pngsave_file);
            Animate(item);
        }

        private void Animate(PngSaveItem item)
        {
            TMOAnim tmoanim = item.tmoanim;
            tmoanim.LoadSource();
            if (tmoanim.SourceTmo.frames != null)
            {
                tmoanim.Process();
                tmoanim.SaveSourceToFile(out_tmo_file);

                Figure fig = viewer.FigureList[item.FigureIndex];
                fig.Tmo = tmoanim.SourceTmo;
                fig.UpdateNodeMapAndBoneMatrices();
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
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;
            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;
            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == 0)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(tmoanim_row - 1, item);
            tmoAnimItemBindingSource.Position = tmoanim_row - 1;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;
            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == tmoAnimItemBindingSource.Count - 1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(tmoanim_row + 1, item);
            tmoAnimItemBindingSource.Position = tmoanim_row + 1;
        }

        private void btnRec_Click(object sender, EventArgs e)
        {
            SaveToPngEachFrame();
        }

        private void gvFigures_SelectionChanged(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;
            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            tmoAnimItemBindingSource.DataSource = tmoanim.items;
            viewer.SetFigureIndex(pngsave_row);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (form2.ShowDialog(this) == DialogResult.OK)
            {
                if (form2.File == null)
                    return;

                CreatePngSaveItem(form2.File);
            }
        }
    }
}
