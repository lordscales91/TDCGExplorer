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
        public WeightViewer viewer = null;
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
                    else
                    {
                        viewer.SelectedMesh = null;
                        viewer.ClearCommands();
                    }
                };
                viewer.SelectedNodeChanged += delegate(object sender, EventArgs e)
                {
                    Console.WriteLine("select node {0}", viewer.SelectedNode.Name);
                };
                viewer.SelectedVertexChanged += delegate(object sender, EventArgs e)
                {
                    AssignSkinWeights(viewer.SelectedVertex);
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                //this.timer1.Enabled = true;
            }
        }

        void AssignTSOFiles(Figure fig)
        {
            lvTSOFiles.BeginUpdate();
            lvTSOFiles.Items.Clear();
            for (int i = 0; i < fig.TSOList.Count; i++)
            {
                TSOFile tso = fig.TSOList[i];
                ListViewItem li = new ListViewItem("TSO #" + i.ToString());
                li.Tag = tso;
                lvTSOFiles.Items.Add(li);
            }
            lvTSOFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvTSOFiles.EndUpdate();
        }

        void AssignMeshes(TSOFile tso)
        {
            lvMeshes.BeginUpdate();
            lvMeshes.Items.Clear();
            foreach (TSOMesh mesh in tso.meshes)
            {
                ListViewItem li = new ListViewItem(mesh.Name);
                li.Tag = mesh;
                lvMeshes.Items.Add(li);
            }
            lvMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvMeshes.EndUpdate();
        }

        void AssignSkinWeights(Vertex vertex)
        {
            if (vertex == null)
                return;

            lvSkinWeights.BeginUpdate();
            lvSkinWeights.Items.Clear();
            foreach (SkinWeight skin_weight in vertex.skin_weights)
            {
                TSONode bone = viewer.SelectedSubMesh.GetBone(skin_weight.bone_index);
                float weight = skin_weight.weight;
                if (weight == 0.0f)
                    continue;
                ListViewItem li = new ListViewItem(bone.Name);
                li.SubItems.Add(weight.ToString("F3"));
                li.Tag = skin_weight;
                lvSkinWeights.Items.Add(li);
            }
            lvSkinWeights.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvSkinWeights.EndUpdate();
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
                Invalidate(false);
            }
        }

        private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTSOFiles.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvTSOFiles.SelectedItems[0];
            TSOFile tso = li.Tag as TSOFile;
            AssignMeshes(tso);
            viewer.SelectedTSOFile = tso;
            Invalidate(false);
        }

        private void lvFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMeshes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvMeshes.SelectedItems[0];
            TSOMesh mesh = li.Tag as TSOMesh;

            viewer.SelectedMesh = mesh;
            Invalidate(false);
        }

        private void lvSkinWeights_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSkinWeights.SelectedItems.Count == 0)
                return;

            if (viewer.SelectedSubMesh == null)
                return;

            ListViewItem li = lvSkinWeights.SelectedItems[0];
            SkinWeight skin_weight = li.Tag as SkinWeight;

            viewer.SelectedNode = viewer.SelectedSubMesh.GetBone(skin_weight.bone_index);
            Invalidate(false);
        }

        private void tbWeight_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)(tbWeight.Value - 20) * 0.050f;
            lbWeight.Text = string.Format("{0:F3}", value);
            viewer.weight = value;
            //Invalidate(false);
        }

        private void tbRadius_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)(tbRadius.Value) * 0.025f;
            lbRadius.Text = string.Format("{0:F3}", value);
            viewer.radius = value;
            Invalidate(false);
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            viewer.GainSkinWeight(viewer.SelectedNode);
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void btnErase_Click(object sender, EventArgs e)
        {
            viewer.ReduceSkinWeight(viewer.SelectedNode);
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void SaveFigure()
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

        private void editUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Undo();
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void editRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Redo();
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFigure();
        }

        private void fileExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fileNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.ClearFigureList();
            Invalidate(false);
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFigure();
            Invalidate(false);
        }

        private void LoadFigure()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "png files|*.png|tso files|*.tso";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string source_file = dialog.FileName;
                viewer.LoadAnyFile(source_file);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            viewer.FrameMove();
            viewer.Render();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void cameraResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Camera.Reset();
            viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
            Invalidate(false);
        }

        private void cameraSelectedVertexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer.SelectedVertex == null)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                viewer.Camera.Center = viewer.SelectedVertex.CalcSkindeformPosition(fig.ClipBoneMatrices(viewer.SelectedSubMesh));
                viewer.Camera.ResetTranslation();
            }
            Invalidate(false);
        }

        private void cameraSelectedBoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer.SelectedNode == null)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                TMONode bone;
                if (fig.nodemap.TryGetValue(viewer.SelectedNode, out bone))
                {
                    viewer.Camera.Center = WeightViewer.GetMatrixTranslation(ref bone.combined_matrix);
                    viewer.Camera.ResetTranslation();
                }
            }
            Invalidate(false);
        }

        private void toonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Toon;
            Invalidate(false);
        }

        private void heatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Heat;
            Invalidate(false);
        }

        private void wireToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_view_mode = WeightViewer.MeshViewMode.Wire;
            Invalidate(false);
        }

        private void meshAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_selection_mode = WeightViewer.MeshSelectionMode.AllMeshes;
            Invalidate(false);
        }

        private void meshSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.mesh_selection_mode = WeightViewer.MeshSelectionMode.SelectedMesh;
            Invalidate(false);
        }

        private void vertexAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.AllVertices;
            Invalidate(false);
        }

        private void vertexCcwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.CcwVertices;
            Invalidate(false);
        }

        private void vertexNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.vertex_selection_mode = WeightViewer.VertexSelectionMode.None;
            Invalidate(false);
        }
    }
}
