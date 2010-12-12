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
        TSOFile tso;

        public Form2(string[] args)
        {
            InitializeComponent();
            tso = new TSOFile();
            foreach (string arg in args)
                LoadTSOFile(arg);
        }

        private void Form2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    LoadTSOFile(src);
            }
        }

        private void Form2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        void LoadTSOFile(string src)
        {
            Debug.WriteLine("loading " + src);
            tso.Load(src);
            AssignNodes(tso);
            AssignTextures(tso);
            AssignSubScripts(tso);
            AssignMeshes(tso);
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

        private void lvNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvNodes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvNodes.SelectedItems[0];
            TSONode node = li.Tag as TSONode;

            Debug.WriteLine("selected " + node.Name);
        }

        private void lvTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvTextures.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvTextures.SelectedItems[0];
            TSOTex tex = li.Tag as TSOTex;

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
                tex.Save(bw);
                bw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                image = Bitmap.FromStream(ms);
            }
            return image;
        }

        private void lvSubScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSubScripts.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvSubScripts.SelectedItems[0];
            TSOSubScript sub_script = li.Tag as TSOSubScript;

            Debug.WriteLine("selected " + sub_script.Name);
        }

        private void lvMeshes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMeshes.SelectedItems.Count == 0)
                return;

            ListViewItem li = lvMeshes.SelectedItems[0];
            TSOMesh mesh = li.Tag as TSOMesh;

            Debug.WriteLine("selected " + mesh.Name);
        }
    }
}
