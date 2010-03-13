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

            if (viewer.InitializeApplication(this, true))
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
                viewer.SubMeshEvent += delegate(object sender, EventArgs e)
                {
                    TSOSubMesh selected_sub_mesh = viewer.SelectedSubMesh;
                    lvSubMeshes.SelectedItems.Clear();
                    foreach (ListViewItem li in lvSubMeshes.Items)
                    {
                        TSOSubMesh sub_mesh = li.Tag as TSOSubMesh;
                        if (sub_mesh == selected_sub_mesh)
                        {
                            li.Selected = true;
                            break;
                        }
                    }
                };
                viewer.VertexEvent += delegate(object sender, EventArgs e)
                {
                    AssignSkinWeights(viewer.SelectedVertex);
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
                viewer.SwitchShadowShown();
                //viewer.SwitchSpriteShown();
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
            lvTSOFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
            lvMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvMeshes.EndUpdate();
        }

        void AssignSubMeshes(TSOMesh mesh)
        {
            lvSubMeshes.BeginUpdate();
            lvSubMeshes.Items.Clear();
            int nsub_mesh = 0;
            foreach (TSOSubMesh sub_mesh in mesh.sub_meshes)
            {
                ListViewItem li = new ListViewItem(string.Format("sub_mesh #{0}", nsub_mesh));
                li.Tag = sub_mesh;
                lvSubMeshes.Items.Add(li);
                nsub_mesh++;
            }
            lvSubMeshes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvSubMeshes.EndUpdate();
        }

        void AssignBoneIndices(TSOSubMesh sub_mesh)
        {
            lvBoneIndices.BeginUpdate();
            lvBoneIndices.Items.Clear();
            foreach (TSONode bone in sub_mesh.bones)
            {
                ListViewItem li = new ListViewItem(bone.Name);
                li.Tag = bone;
                lvBoneIndices.Items.Add(li);
            }
            lvBoneIndices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvBoneIndices.EndUpdate();
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
        }

        private void lvFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMeshes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvMeshes.SelectedItems[0];
            TSOMesh mesh = li.Tag as TSOMesh;
            AssignSubMeshes(mesh);
            viewer.SelectedMesh = mesh;
            Invalidate(false);
        }

        private void lvMeshes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSubMeshes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvSubMeshes.SelectedItems[0];
            TSOSubMesh sub_mesh = li.Tag as TSOSubMesh;
            AssignBoneIndices(sub_mesh);
            viewer.SelectedSubMesh = sub_mesh;
        }

        private void lvBoneIndices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBoneIndices.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvBoneIndices.SelectedItems[0];
            TSONode bone = li.Tag as TSONode;
            viewer.SelectedNode = bone;
            Invalidate(false);
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
            if (viewer.SelectedVertex == null)
                return;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                viewer.Camera.Center = WeightViewer.CalcSkindeformPosition(viewer.SelectedVertex, WeightViewer.ClipBoneMatrices(fig, viewer.SelectedSubMesh));
                viewer.Camera.ResetTranslation();
            }
            Invalidate(false);
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            viewer.GainSkinWeight(viewer.SelectedNode);
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void tbWeight_ValueChanged(object sender, EventArgs e)
        {
            WeightViewer.weight = (float)(tbWeight.Value) * 0.05f;
            Invalidate(false);
        }

        private void tbRadius_ValueChanged(object sender, EventArgs e)
        {
            WeightViewer.radius = (float)(tbRadius.Value) * 0.05f;
            Invalidate(false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFigure();
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

        private void 元に戻すUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Undo();
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void やり直しRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Redo();
            AssignSkinWeights(viewer.SelectedVertex);
            Invalidate(false);
        }

        private void btnToon_Click(object sender, EventArgs e)
        {
            viewer.view_mode = WeightViewer.ViewMode.Toon;
            Invalidate(false);
        }

        private void btnHeat_Click(object sender, EventArgs e)
        {
            viewer.view_mode = WeightViewer.ViewMode.Weight;
            Invalidate(false);
        }

        private void 名前を付けて保存AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFigure();
        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 新規作成NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.ClearFigureList();
            Invalidate(false);
        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
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
    }
}
