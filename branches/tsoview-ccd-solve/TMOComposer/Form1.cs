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
        Form3 form3 = null;
        TmoAnimItemForm tmoAnimItemForm = null;
        TSOConfig tso_config;

        string save_path = null;
        string pose_path = null;
        string pngsave_file = @"PngSave.xml";

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            
            this.ClientSize = tso_config.ClientSize;
            save_path = tso_config.SavePath;
            pose_path = tso_config.PosePath;
            TMOAnim.PoseRoot = tso_config.PosePath;
            TMOAnim.FaceRoot = tso_config.FacePath;

            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                CreatePngSave();
                viewer.Camera.SetTranslation(0.0f, +10.0f, -44.0f);
                viewer.SwitchMotionEnabled();
                timer1.Enabled = true;
            }
            form2 = new Form2();
            form2.SavePath = tso_config.SavePath;
            form3 = new Form3();
            form3.FacePath = tso_config.FacePath;
            tmoAnimItemForm = new TmoAnimItemForm();
            tmoAnimItemForm.SetForm3(form3);
            this.tso_config = tso_config;
        }

        private void SaveToPngEachFrame(int step)
        {
            timer1.Enabled = false;

            string dest_path = @"snapshots";
            Directory.CreateDirectory(dest_path);

            int orig_frame_idx = viewer.FrameIndex; // backup
            int frame_len = viewer.GetMaxFrameLength();
            for (int frame_idx = 0; frame_idx < frame_len; frame_idx += step)
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
            if (! Directory.Exists(pose_path))
                return;

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
            {
                pngsave = PngSave.Load(pngsave_file);
                pngsave.UpdateID();
            }
            else
                pngsave = new PngSave();
            pngSaveItemBindingSource.DataSource = pngsave.items;
            
            int position = 0;
            foreach (PngSaveItem item in pngsave.items)
            {
                pngSaveItemBindingSource.Position = position;
                viewer.LoadAnyFile(save_path + @"\" + item.File, true);
                Animate(item);
                position++;
            }
            
            if (pngSaveItemBindingSource.Count == 0)
                CreatePngSaveItem("system.tdcgsav.png");

            pngSaveItemBindingSource.Position = 0;
        }

        void CreatePngSaveItem(string file)
        {
            PngSaveItem item = new PngSaveItem();
            item.File = file;
            pngSaveItemBindingSource.Add(item);

            pngSaveItemBindingSource.Position = pngSaveItemBindingSource.Count - 1;
            viewer.LoadAnyFile(save_path + @"\" + item.File, true);
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            PngSaveItem item = pngsave.items[pngsave_row];

            pngsave.Dump(pngsave_file);

            gvTMOAnimItems.ClearSelection();

            if (!viewer.IsMotionEnabled())
                viewer.SwitchMotionEnabled();

            Animate(item);
        }

        private void Animate(PngSaveItem item)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            if (pngsave_row >= viewer.FigureList.Count)
                return;

            TMOAnim tmoanim = item.tmoanim;
            tmoanim.SavePoseToFile();
            tmoanim.LoadSource();
            if (tmoanim.SourceTmo.frames != null)
            {
                tmoanim.Process();
                tmoanim.SaveSourceToFile();

                Figure fig = viewer.FigureList[pngsave_row];
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
            tmoAnimItemBindingSource.Add(item);
            pngsave.UpdateID();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;
            
            if (tmoanim_row == -1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
            pngsave.UpdateID();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == -1)
                return;

            if (tmoanim_row == 0)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(tmoanim_row - 1, item);
            tmoAnimItemBindingSource.Position = tmoanim_row - 1;
            pngsave.UpdateID();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == -1)
                return;

            if (tmoanim_row >= tmoAnimItemBindingSource.Count - 1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemBindingSource.Remove(item);
            tmoAnimItemBindingSource.Insert(tmoanim_row + 1, item);
            tmoAnimItemBindingSource.Position = tmoanim_row + 1;
            pngsave.UpdateID();
        }

        private void btnRec_Click(object sender, EventArgs e)
        {
            SaveToPngEachFrame(tso_config.RecordStep);
        }

        private void gvFigures_SelectionChanged(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

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

        private void btnDel_Click(object sender, EventArgs e)
        {
            viewer.RemoveSelectedFigure();

            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            PngSaveItem item = pngsave.items[pngsave_row];
            tmoAnimItemBindingSource.DataSource = null;
            pngSaveItemBindingSource.Remove(item);
            pngsave.UpdateID();
        }

        private void gvTMOAnimItems_DoubleClick(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == -1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];
            tmoAnimItemForm.SetTmoAnimItem(item);

            TMOFile tmo = tmoanim.GetTmo(item);
            tmo.SaveTransformationMatrix(0);
            if (tmoAnimItemForm.ShowDialog(this) == DialogResult.OK)
            {
                tmoAnimItemBindingSource.ResetBindings(false);
                item.CopyFace();
                tmo.LoadTransformationMatrix(0);
            }
        }

        private void gvTMOAnimItems_SelectionChanged(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            if (pngsave_row >= viewer.FigureList.Count)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == -1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row];

            if (viewer.IsMotionEnabled())
                viewer.SwitchMotionEnabled();

            Figure fig = viewer.FigureList[pngsave_row];
            TMOFile tmo = tmoanim.GetTmo(item);
            if (tmo.frames != null)
            {
                viewer.Solved = true;
                fig.Tmo = tmo;
                fig.UpdateNodeMapAndBoneMatrices();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            int pngsave_row = pngSaveItemBindingSource.Position;
            int tmoanim_row = tmoAnimItemBindingSource.Position;

            if (pngsave_row == -1)
                return;

            TMOAnim tmoanim = pngsave.items[pngsave_row].tmoanim;

            if (tmoanim_row == -1)
                return;

            TMOAnimItem item = tmoanim.items[tmoanim_row].Dup();
            tmoAnimItemBindingSource.Insert(tmoanim_row + 1, item);
            tmoAnimItemBindingSource.Position = tmoanim_row + 1;
            pngsave.UpdateID();
        }

        private void cbLimitRotation_CheckedChanged(object sender, EventArgs e)
        {
            viewer.LimitRotationEnabled = cbLimitRotation.Checked;
        }

        private void cbFloor_CheckedChanged(object sender, EventArgs e)
        {
            viewer.FloorEnabled = cbFloor.Checked;
        }
    }
}
