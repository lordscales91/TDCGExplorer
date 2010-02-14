using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TSOWeight
{
    public partial class Form1 : Form
    {
        WeightViewer viewer = null;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;

            this.viewer = new WeightViewer();

            if (viewer.InitializeApplication(this))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Figure fig;
                    if (viewer.TryGetFigure(out fig))
                    {
                        AssignTSOFiles(fig);
                    }
                };
                viewer.VertexEvent += delegate(object sender, EventArgs e)
                {
                    AssignSkinWeights();
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                this.timer1.Enabled = true;
            }
        }

        void AssignTSOFiles(Figure fig)
        {
            lvTSOFiles.Items.Clear();
            for (int i = 0; i < fig.TSOList.Count; i++)
            {
                TSOFile tso = fig.TSOList[i];
                ListViewItem li = new ListViewItem("TSO #" + i.ToString());
                li.Tag = tso;
                lvTSOFiles.Items.Add(li);
            }
            lvTSOFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void AssignFrames(TSOFile tso)
        {
            lvFrames.Items.Clear();
            foreach (TSOFrame frame in tso.frames)
            {
                ListViewItem li = new ListViewItem(frame.Name);
                li.Tag = frame;
                lvFrames.Items.Add(li);
            }
            lvFrames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void AssignMeshes(TSOFrame frame)
        {
            lvMeshes.Items.Clear();
            int nmesh = 0;
            foreach (TSOSubMesh mesh in frame.meshes)
            {
                ListViewItem li = new ListViewItem(string.Format("mesh #{0}", nmesh));
                li.Tag = mesh;
                lvMeshes.Items.Add(li);
                nmesh++;
            }
            lvMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void AssignBoneIndices(TSOSubMesh mesh)
        {
            lvBoneIndices.Items.Clear();
            foreach (TSONode bone in mesh.bones)
            {
                ListViewItem li = new ListViewItem(bone.Name);
                li.Tag = bone;
                lvBoneIndices.Items.Add(li);
            }
            lvBoneIndices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void AssignSkinWeights()
        {
            if (viewer.selected_vertex_id == -1)
                return;

            lvSkinWeights.Items.Clear();
            foreach (SkinWeight skin_weight in viewer.selected_mesh.vertices[viewer.selected_vertex_id].skin_weights)
            {
                TSONode bone = viewer.selected_mesh.GetBone(skin_weight.bone_index);
                float weight = skin_weight.weight;
                if (weight == 0.0f)
                    continue;
                ListViewItem li = new ListViewItem(bone.Name);
                li.SubItems.Add(weight.ToString("F3"));
                li.Tag = skin_weight;
                lvSkinWeights.Items.Add(li);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                viewer.selected_mesh = null;
                viewer.selected_vertex_id = -1;
                viewer.ClearCommands();
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    viewer.LoadAnyFile(src, (e.KeyState & 8) == 8);
            }
        }

        private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTSOFiles.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvTSOFiles.SelectedItems[0];
            TSOFile tso = li.Tag as TSOFile;
            AssignFrames(tso);
        }

        private void lvFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFrames.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvFrames.SelectedItems[0];
            TSOFrame frame = li.Tag as TSOFrame;
            AssignMeshes(frame);
        }

        private void lvMeshes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMeshes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvMeshes.SelectedItems[0];
            TSOSubMesh mesh = li.Tag as TSOSubMesh;
            AssignBoneIndices(mesh);
            viewer.selected_mesh = mesh;
            viewer.selected_vertex_id = -1;
        }

        private void lvBoneIndices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBoneIndices.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvBoneIndices.SelectedItems[0];
            TSONode bone = li.Tag as TSONode;
            viewer.selected_node = bone;
        }

        private void lvSkinWeights_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSkinWeights.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvSkinWeights.SelectedItems[0];
            SkinWeight skin_weight = li.Tag as SkinWeight;
            lvBoneIndices.SelectedItems.Clear();
            lvBoneIndices.Items[skin_weight.bone_index].Selected = true;
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            if (viewer.selected_vertex_id == -1)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                viewer.Camera.Center = WeightViewer.CalcSkindeformPosition(viewer.selected_mesh.vertices[viewer.selected_vertex_id], WeightViewer.ClipBoneMatrices(fig, viewer.selected_mesh));
                viewer.Camera.ResetTranslation();
            }
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            viewer.GainSkinWeight(viewer.selected_node);
            AssignSkinWeights();
        }

        private void tbWeight_ValueChanged(object sender, EventArgs e)
        {
            WeightViewer.weight = (float)(tbWeight.Value) * 0.1f;
        }

        private void tbRadius_ValueChanged(object sender, EventArgs e)
        {
            WeightViewer.radius = (float)(tbRadius.Value) * 0.1f;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lvTSOFiles.SelectedIndices.Count == 0)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "tso files|*.tso";
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string dest_file = dialog.FileName;
                    string extension = Path.GetExtension(dest_file);
                    if (extension == ".tso")
                    {
                        int index = lvTSOFiles.SelectedIndices[0];
                        TSOFile tso = fig.TSOList[index];
                        tso.Save(dest_file);
                    }
                }
            }
        }

        private void Œ³‚É–ß‚·UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Undo();
            AssignSkinWeights();
        }

        private void ‚â‚è’¼‚µRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Redo();
            AssignSkinWeights();
        }

        private void btnToon_Click(object sender, EventArgs e)
        {
            viewer.view_mode = Viewer.ViewMode.Toon;
        }

        private void btnHeat_Click(object sender, EventArgs e)
        {
            viewer.view_mode = Viewer.ViewMode.Weight;
        }
    }
}
