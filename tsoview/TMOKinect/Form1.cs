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
            save_path = tso_config.SavePath;
            pose_path = tso_config.PosePath;

            viewer = new CCDViewer();
            if (viewer.InitializeApplication(this))
            {
                CreatePngSave();
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
                //viewer.MotionEnabled = true;
                timer1.Enabled = true;
            }
            this.tso_config = tso_config;
        }

        private void CreatePngSave()
        {
            CreatePngSaveItem("system.tdcgsav.png");
        }

        void CreatePngSaveItem(string file)
        {
            viewer.LoadAnyFile(Path.Combine(save_path, file), true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.FrameMoveDerived();
            viewer.Render();
        }

        private void cbLimitRotation_CheckedChanged(object sender, EventArgs e)
        {
            viewer.LimitRotationEnabled = cbLimitRotation.Checked;
        }

        private void cbFloor_CheckedChanged(object sender, EventArgs e)
        {
            viewer.FloorEnabled = cbFloor.Checked;
        }
    }
}
