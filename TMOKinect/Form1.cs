using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TMOKinect
{
    public partial class Form1 : Form
    {
        CCDViewer viewer = null;
        TSOConfig tso_config;

        string save_path = null;
        string pose_path = null;

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            save_path = tso_config.SavePath;
            pose_path = tso_config.PosePath;

            viewer = new CCDViewer();
            if (viewer.InitializeApplication(this))
            {
                viewer.LoadAnyFile(Path.Combine(save_path, @"system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
            }
            this.tso_config = tso_config;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            viewer.FrameMove();
            viewer.FrameMoveDerived();
            viewer.Render();
            this.Invalidate();
        }
    }
}
