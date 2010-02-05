using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TMOMorphing
{
    public partial class Form1 : Form
    {
        internal Viewer viewer = null;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;
            viewer = new Viewer();

            if (viewer.InitializeApplication(this))
            {
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                timer1.Enabled = true;
            }

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }
    }
}
