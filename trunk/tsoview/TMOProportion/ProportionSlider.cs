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

        static string GetClassNameWithoutNameSpace(string name)
        {
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        string class_name = null;
        public string ClassName
        {
            get
            {
                return class_name;
            }
            set
            {
                class_name = value;
                lbClassName.Text = GetClassNameWithoutNameSpace(class_name);
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
                    lbRatio.Text = string.Format("{0:F2}", ratio);
                    tbRatio.Value = (int)(ratio * 10.0f);
                    if (ValueChanged != null)
                        ValueChanged(this, new EventArgs());
                }
            }

        }
        
        public ProportionSlider()
        {
            InitializeComponent();
        }

        private void tbRatio_ValueChanged(object sender, EventArgs e)
        {
            Ratio = tbRatio.Value * 0.1f;
        }
    }
}
