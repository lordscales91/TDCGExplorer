using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOMorphing
{
    public partial class MorphSlider : UserControl
    {
        public event EventHandler ValueChanged;

        string Group_name = null;
        public string GroupName
        {
            get
            {
                return Group_name;
            }
            set
            {
                Group_name = value;
                lbGroupName.Text = Group_name;
            }
        }

        float ratio = 0.0f;
        public float Ratio
        {
            get
            {
                return ratio;
            }
            set
            {
                if (ratio != value)
                {
                    ratio = value;
                    tbRatio.Value = (int)(ratio * 10.0f);
                    if (ValueChanged != null)
                        ValueChanged(this, new EventArgs());
                }
            }

        }

        string morph_name = null;
        public string MorphName
        {
            get
            {
                return morph_name;
            }
            set
            {
                morph_name = value;
                cbMorphNames.SelectedIndex = cbMorphNames.FindString(morph_name);
            }
        }

        public MorphSlider()
        {
            InitializeComponent();
        }

        public void SetMorphNames(List<string> names)
        {
            cbMorphNames.Items.Clear();
            foreach (string name in names)
            {
                cbMorphNames.Items.Add(name);
            }
        }

        private void tbRatio_ValueChanged(object sender, EventArgs e)
        {
            Ratio = tbRatio.Value * 0.1f;
        }

        private void cbMorphNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            MorphName = (string)cbMorphNames.SelectedItem;
        }
    }
}
