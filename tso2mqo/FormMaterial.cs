using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tso2MqoGui
{
    public partial class FormMaterial : Form
    {
        public Dictionary<string, MaterialInfo>    materials;

        public FormMaterial()
        {
            InitializeComponent();
            DialogResult    = DialogResult.Cancel;
        }

        private void FormMaterial_Load(object sender, EventArgs e)
        {
            foreach(MaterialInfo i in materials.Values)
            {
                ListViewItem    item= lvMaterials.Items.Add(i.Name);
                item.Tag            = i;
                item.SubItems.Add(i.diffuse == null ? "" : i.diffuse);
                item.SubItems.Add(i.shadow  == null ? "" : i.shadow);
                item.SubItems.Add(i.shader  == null ? "" : i.shader);
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            // 正しく情報が設定されているかをチェックする
            foreach(ListViewItem i in lvMaterials.Items)
            {
                if(i.SubItems[1].Text == ""
                || i.SubItems[2].Text == ""
                || i.SubItems[3].Text == ""
                || !File.Exists(i.SubItems[1].Text)
                || !File.Exists(i.SubItems[2].Text)
                || !File.Exists(i.SubItems[3].Text))
                {
                    MessageBox.Show("マテリアルの情報が正しく設定されていないか、ファイルが存在しません");
                    i.Selected  = true;
                    return;
                }
            }


            DialogResult    = DialogResult.OK;
            Hide();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult    = DialogResult.Cancel;
            Hide();
        }

        private void lvMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lvMaterials.SelectedItems.Count > 0)
            {
                MaterialInfo    mi  = lvMaterials.SelectedItems[0].Tag as MaterialInfo;
                pgMaterial.SelectedObject   = mi;
            } else
            {
                pgMaterial.SelectedObject   = null;
            }
        }

        private void pgMaterial_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if(lvMaterials.SelectedItems.Count > 0)
            {
                ListViewItem    item= lvMaterials.SelectedItems[0];

                switch(e.ChangedItem.PropertyDescriptor.Name)
                {
                case "DiffuseTexture":  item.SubItems[1].Text= e.ChangedItem.Value.ToString(); break;
                case "ShadowTexture":   item.SubItems[2].Text= e.ChangedItem.Value.ToString(); break;
                case "ShaderFile":      item.SubItems[3].Text= e.ChangedItem.Value.ToString(); break;
                }
            }
        }
    }
}
