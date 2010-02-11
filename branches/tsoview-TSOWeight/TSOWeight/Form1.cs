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
                viewer.Camera.SetTranslation(0.0f, +18.0f, +10.0f);

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
            foreach (TSOMesh mesh in frame.meshes)
            {
                ListViewItem li = new ListViewItem(string.Format("mesh #{0}", nmesh));
                li.Tag = mesh;
                lvMeshes.Items.Add(li);
                nmesh++;
            }
            lvMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void AssignBoneIndices(TSOMesh mesh)
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
            Vertex selected_vertex = viewer.selected_mesh.vertices[viewer.selected_vertex_id];
            lvSkinWeights.Items.Clear();
            foreach (SkinWeight skin_weight in selected_vertex.skin_weights)
            {
                TSONode bone = viewer.selected_mesh.GetBone(skin_weight.bone_index);
                float weight = skin_weight.weight;
                if (weight == 0.0f)
                    continue;
                ListViewItem li = new ListViewItem(bone.Name);
                li.SubItems.Add(weight.ToString("F3"));
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
            TSOMesh mesh = li.Tag as TSOMesh;
            AssignBoneIndices(mesh);
            viewer.selected_mesh = mesh;
        }

        private void lvBoneIndices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBoneIndices.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvBoneIndices.SelectedItems[0];
            TSONode bone = li.Tag as TSONode;
            viewer.selected_node = bone;
        }

        private void cbBoneHeatingView_CheckedChanged(object sender, EventArgs e)
        {
            viewer.BoneHeatingViewSwitch = cbBoneHeatingView.Checked;
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                Vertex selected_vertex = viewer.selected_mesh.vertices[viewer.selected_vertex_id];
                viewer.Camera.Reset();
                viewer.Camera.Center = WeightViewer.CalcSkindeformPosition(ref selected_vertex, WeightViewer.ClipBoneMatrices(fig, viewer.selected_mesh));
            }
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            viewer.GainSkinWeight(viewer.selected_node);
        }
    }
}
