using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tso2Pmd
{
    public partial class PhysicsControl : UserControl
    {
        // 物理テンプレート設定リスト
        PhysObTemplateList pTemplate_list = new PhysObTemplateList();
        public PhysObTemplateList PhysTemplateList { get { return pTemplate_list; } }

        public PhysicsControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            // 物理テンプレート設定リストを読み込む
            pTemplate_list.Load();

            foreach (IPhysObTemplate i in pTemplate_list.items)
            {
                switch (i.Group())
                {
                    case 0: comboBox1.Items.Add(i.Name()); break;
                    case 1: comboBox2.Items.Add(i.Name()); break;
                    case 2: comboBox3.Items.Add(i.Name()); break;
                    case 3: checkedListBox1.Items.Add(i.Name()); break;
                }
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void radioButton_Kami0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;

            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 0) pTemplate_list.flag[i] = false;
        }

        private void radioButton_Kami1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
         
            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 0 && i.Name() == comboBox1.SelectedItem.ToString())
            //        pTemplate_list.flag[i] = true;
        }

        private void radioButton_Chichi0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = false;

            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 1) pTemplate_list.flag[i] = false;
        }

        private void radioButton_Chichi1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;

            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 1 && i.Name() == comboBox2.SelectedItem.ToString())
            //        pTemplate_list.flag[i] = true;
        }

        private void radioButton_Skirt0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = false;

            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 2) pTemplate_list.flag[i] = false;
        }

        private void radioButton_Skirt1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = true;

            //foreach (IPhysObTemplate i in pTemplate_list.items)
            //    if (i.Group() == 2 && i.Name() == comboBox3.SelectedItem.ToString())
            //        pTemplate_list.flag[i] = true;
        }
    }
}
