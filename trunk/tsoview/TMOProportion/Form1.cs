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

namespace TMOProportion
{
    public partial class Form1 : Form
    {
        internal Viewer viewer = null;
        ProportionList pro_list = new ProportionList();
        TPOFileList tpo_list = new TPOFileList();
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;
            viewer = new Viewer();

            if (viewer.InitializeApplication(this))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    UpdateTpoList();
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                timer1.Enabled = true;
            }

            pro_list.Load();
            tpo_list.SetProportionList(pro_list);
            TPOConfig tpo_config = TPOConfig.Load(GetTPOConfigPath());
            tpo_list.SetRatiosFromConfig(tpo_config);

            for (int i = 0; i < tpo_list.Count; i++)
            {
                TPOFile tpo = tpo_list[i];
                ProportionSlider slider = new ProportionSlider();
                slider.Tag = tpo;
                slider.ClassName = tpo.ProportionName;
                slider.Ratio = tpo.Ratio;

                slider.Location = new System.Drawing.Point(10, 10 + i * 95);
                slider.ValueChanged += new System.EventHandler(this.slider_ValueChanged);
                this.Controls.Add(slider);
            }
            UpdateTpoList();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            ProportionSlider slider = sender as ProportionSlider;
            {
                TPOFile tpo = slider.Tag as TPOFile;
                if (tpo != null)
                    tpo.Ratio = slider.Ratio;
            }

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Transform(fig.GetFrameIndex());
                fig.UpdateBoneMatrices(true);
            }
        }

        private void UpdateTpoList()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Tmo = fig.Tmo;
                tpo_list.Transform(0);
                fig.UpdateBoneMatrices(true);
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    viewer.LoadAnyFile(src, (e.KeyState & 8) == 8);
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.KeyState & 8) == 8)
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveTPOConfig();
        }

        private void SaveTPOConfig()
        {
            TPOConfig config = new TPOConfig();

            config.Proportions = new Proportion[tpo_list.Count];
            for (int i = 0; i < tpo_list.Count; i++)
                config.Proportions[i] = new Proportion();

            for (int i = 0; i < tpo_list.Count; i++)
            {
                TPOFile tpo = tpo_list[i];
                Proportion portion = config.Proportions[i];
                portion.ClassName = tpo.ProportionName;
                portion.Ratio = tpo.Ratio;
            }
            config.Save(GetTPOConfigPath());
        }

    }
}
