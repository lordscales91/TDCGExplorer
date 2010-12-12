using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TSOEditor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public void AssignTSOFiles(Figure fig)
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

        void AssignNodes(TSOFile tso)
        {
            lvNodes.BeginUpdate();
            lvNodes.Items.Clear();
            foreach (TSONode node in tso.nodes)
            {
                ListViewItem li = new ListViewItem(node.Name);
                li.Tag = node;
                lvNodes.Items.Add(li);
            }
            lvNodes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvNodes.EndUpdate();
        }

        void AssignTextures(TSOFile tso)
        {
            lvTextures.BeginUpdate();
            lvTextures.Items.Clear();
            foreach (TSOTex texture in tso.textures)
            {
                ListViewItem li = new ListViewItem(texture.Name);
                li.SubItems.Add(texture.FileName);
                li.Tag = texture;
                lvTextures.Items.Add(li);
            }
            lvTextures.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvTextures.EndUpdate();
        }

        void AssignSubScripts(TSOFile tso)
        {
            lvSubScripts.BeginUpdate();
            lvSubScripts.Items.Clear();
            foreach (TSOSubScript sub_script in tso.sub_scripts)
            {
                ListViewItem li = new ListViewItem(sub_script.Name);
                li.SubItems.Add(sub_script.FileName);
                li.Tag = sub_script;
                lvSubScripts.Items.Add(li);
            }
            lvSubScripts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvSubScripts.EndUpdate();
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

        public TSOFile GetSelectedTSOFile()
        {
            if (lvTSOFiles.SelectedItems.Count == 0)
                return null;

            ListViewItem li = lvTSOFiles.SelectedItems[0];
            TSOFile tso = li.Tag as TSOFile;
            return tso;
        }

        public TSONode GetSelectedNode()
        {
            if (lvNodes.SelectedItems.Count == 0)
                return null;

            ListViewItem li = lvNodes.SelectedItems[0];
            TSONode node = li.Tag as TSONode;
            return node;
        }

        public TSOTex GetSelectedTexture()
        {
            if (lvTextures.SelectedItems.Count == 0)
                return null;

            ListViewItem li = lvTextures.SelectedItems[0];
            TSOTex tex = li.Tag as TSOTex;
            return tex;
        }

        public TSOSubScript GetSelectedSubScript()
        {
            if (lvSubScripts.SelectedItems.Count == 0)
                return null;

            ListViewItem li = lvSubScripts.SelectedItems[0];
            TSOSubScript sub_script = li.Tag as TSOSubScript;
            return sub_script;
        }

        public TSOMesh GetSelectedMesh()
        {
            if (lvMeshes.SelectedItems.Count == 0)
                return null;

            ListViewItem li = lvMeshes.SelectedItems[0];
            TSOMesh mesh = li.Tag as TSOMesh;
            return mesh;
        }

        private void lvTSOFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            TSOFile tso = GetSelectedTSOFile();
            if (tso == null)
                return;

            AssignNodes(tso);
            AssignTextures(tso);
            AssignSubScripts(tso);
            AssignMeshes(tso);
        }

        private void lvNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            TSONode node = GetSelectedNode();
            if (node == null)
                return;

            Debug.WriteLine("selected " + node.Name);
        }

        private void lvTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            TSOTex tex = GetSelectedTexture();
            if (tex == null)
                return;

            Debug.WriteLine("selected " + tex.Name);
            pbTexThumbnail.Image = GetImage(tex);
        }

        static Image GetImage(TSOTex tex)
        {
            if (tex.data.Length == 0)
                return null;
            Image image;
            MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                tex.SaveBMP(bw);
                bw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                image = Bitmap.FromStream(ms);
            }
            return image;
        }

        private void lvSubScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            TSOSubScript sub_script = GetSelectedSubScript();
            if (sub_script == null)
                return;

            Debug.WriteLine("selected " + sub_script.Name);
        }

        private void lvMeshes_SelectedIndexChanged(object sender, EventArgs e)
        {
            TSOMesh mesh = GetSelectedMesh();
            if (mesh == null)
                return;

            Debug.WriteLine("selected " + mesh.Name);
        }

        private void btnTexLoad_Click(object sender, EventArgs e)
        {
            TSOTex tex = GetSelectedTexture();
            if (tex == null)
                return;

            openFileDialog1.FileName = tex.FileName.Trim('"');
            openFileDialog1.Filter = "Texture files (*.tga;*.bmp)|*.tga;*.bmp|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(openFileDialog1.FileName).ToLower();
                using (BinaryReader br = new BinaryReader(openFileDialog1.OpenFile()))
                {
                    if (ext == ".tga")
                        tex.LoadTGA(br);
                    else
                    if (ext == ".bmp")
                        tex.LoadBMP(br);
                }
            }
        }

        private void btnTexSave_Click(object sender, EventArgs e)
        {
            TSOTex tex = GetSelectedTexture();
            if (tex == null)
                return;

            saveFileDialog1.FileName = tex.FileName.Trim('"');
            saveFileDialog1.Filter = "Texture files (*.tga;*.bmp)|*.tga;*.bmp|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(saveFileDialog1.FileName).ToLower();
                using (BinaryWriter bw = new BinaryWriter(saveFileDialog1.OpenFile()))
                {
                    if (ext == ".tga")
                        tex.SaveTGA(bw);
                    else
                    if (ext == ".bmp")
                        tex.SaveBMP(bw);
                }
            }
        }
    }
}
