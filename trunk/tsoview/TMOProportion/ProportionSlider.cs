using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOProportion
{
    public partial class ProportionSlider : UserControl
    {
        public event EventHandler ValueChanged;

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
                    int ratioTrackBarValue = (int)(ratio * 10.0f);
                    trackBar.Value = ratioTrackBarValue;
                    lbRatio.Text = string.Format("{0:F2}", ratio);
                    if (ValueChanged != null)
                        ValueChanged(this, new EventArgs());
                }
            }

        }
        public ProportionSlider()
        {
            InitializeComponent();
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            Ratio = trackBar.Value * 0.1f;
        }
    }
}
