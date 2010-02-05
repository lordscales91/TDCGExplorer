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
        string morphing_path = Path.Combine(Application.StartupPath, @"Х\По");

        Morphing morphing;

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
                viewer.Camera.SetTranslation(0.0f, +18.0f, +10.0f);

                timer1.Enabled = true;
            }

            morphing = new Morphing();
            morphing.Load(morphing_path);

            for (int i = 0; i < morphing.Groups.Count; i++)
            {
                MorphGroup group = morphing.Groups[i];

                MorphSlider slider = new MorphSlider();
                slider.Tag = group;
                slider.GroupName = group.Name;

                List<string> names = new List<string>();
                foreach (Morph morph in group.Items)
                {
                    names.Add(morph.Name);
                }
                slider.SetMorphNames(names);

                slider.Location = new System.Drawing.Point(10, 10 + i * 95);
                slider.ValueChanged += new System.EventHandler(this.slider_ValueChanged);
                this.Controls.Add(slider);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            MorphSlider slider = sender as MorphSlider;
            {
                MorphGroup group = slider.Tag as MorphGroup;
                group.ClearRatios();
                Morph morph = group.FindItemByName(slider.MorphName);
                if (morph != null)
                    morph.Ratio = slider.Ratio;
            }

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                morphing.Morph(fig.Tmo);
                fig.UpdateBoneMatricesWithoutTMOFrame();
            }
        }
    }
}
