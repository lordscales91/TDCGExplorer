using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TSOEditor
{
    public partial class Form1 : Form
    {
        TSOFile tso;

        public Form1()
        {
            InitializeComponent();
            tso = new TSOFile();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    Debug.WriteLine("loading " + src);
                    tso.Load(src);
                    AssignNodes(tso);
                    AssignTextures(tso);
                    AssignSubScripts(tso);
                    AssignMeshes(tso);
                }
            }
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
    }
}
