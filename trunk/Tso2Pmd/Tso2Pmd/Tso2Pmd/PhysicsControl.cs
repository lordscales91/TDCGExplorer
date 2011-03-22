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
        TemplateList template_list;

        public PhysicsControl()
        {
            InitializeComponent();
        }

        public void Initialize(TemplateList template_list)
        {
            this.template_list = template_list;

            foreach (IPhysObTemplate i in template_list.phys_items)
            {
                switch (i.Group())
                {
                    case 0: comboBox1.Items.Add(i.Name()); break;
                    case 1: comboBox2.Items.Add(i.Name()); break;
                    case 2: comboBox3.Items.Add(i.Name()); break;
                    case 3: checkedListBox1.Items.Add(i.Name()); 
                            checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, true);
                            break;
                    case 4: checkedListBox1.Items.Add(i.Name()); break;
                }
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void radioButton_Kami0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
        }

        private void radioButton_Kami1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
        }

        private void radioButton_Chichi0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = false;
        }

        private void radioButton_Chichi1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
        }

        private void radioButton_Skirt0_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = false;
        }

        private void radioButton_Skirt1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = true;
        }

        public void SetPhysFlag()
        {
            foreach (IPhysObTemplate i in template_list.phys_items)
            {
                switch( i.Group() )
                {
                    case 0:
                        if (radioButton_Kami1.Checked == true &&
                            i.Name() == comboBox1.SelectedItem.ToString())
                        {
                            template_list.phys_flag[i] = true;
                        }
                        else
                        {
                            template_list.phys_flag[i] = false;
                        }
                        break;

                    case 1:
                        if (radioButton_Chichi1.Checked == true &&
                            i.Name() == comboBox2.SelectedItem.ToString())
                        {
                            template_list.phys_flag[i] = true;
                        }
                        else
                        {
                            template_list.phys_flag[i] = false;
                        }
                        break;

                    case 2:
                        if (radioButton_Skirt1.Checked == true &&
                            i.Name() == comboBox3.SelectedItem.ToString())
                        {
                            template_list.phys_flag[i] = true;
                        }
                        else
                        {
                            template_list.phys_flag[i] = false;
                        }
                        break;

                    case 3:
                        template_list.phys_flag[i] = false;
                        for (int j = 0; j < checkedListBox1.CheckedItems.Count; j++)
                        {
                            if (i.Name() == checkedListBox1.CheckedItems[j].ToString())
                                template_list.phys_flag[i] = true;
                        }
                        break;
                }
            }
        }
    }
}
