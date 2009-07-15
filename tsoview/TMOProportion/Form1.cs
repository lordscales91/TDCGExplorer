using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TMOProportion
{
    public partial class Form1 : Form
    {
        internal Viewer viewer = null;

        public Form1()
        {
            InitializeComponent();
            viewer = new Viewer();

            if (viewer.InitializeApplication(this, true))
            {
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }
    }
}
