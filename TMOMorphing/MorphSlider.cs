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

        Dictionary<string, float> ratiomap = new Dictionary<string, float>();
        public float Ratio
        {
            get
            {
                return ratiomap[morph_name];
            }
            set
            {
                if (ratiomap[morph_name] == value)
                    return;

                ratiomap[morph_name] = value;
                tbRatio.Value = (int)(ratiomap[morph_name] * 10.0f);
                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
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
                tbRatio.Value = (int)(ratiomap[morph_name] * 10.0f);
                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
            }
        }

        public MorphSlider()
        {
            InitializeComponent();
        }

        void SelectFirstItem()
        {
            if (cbMorphNames.Items.Count > 0)
                cbMorphNames.SelectedIndex = 0;
        }

        public void SetMorphNames(List<string> names)
        {
            ratiomap.Clear();
            foreach (string name in names)
            {
                ratiomap[name] = 0.0f;
            }
            cbMorphNames.Items.Clear();
            foreach (string name in names)
            {
                cbMorphNames.Items.Add(name);
            }
            SelectFirstItem();
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
