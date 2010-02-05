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

        public MorphSlider()
        {
            InitializeComponent();
        }

        private void tbRatio_ValueChanged(object sender, EventArgs e)
        {
            Ratio = tbRatio.Value * 0.1f;
        }
    }
}
