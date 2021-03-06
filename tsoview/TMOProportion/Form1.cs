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
        Viewer viewer = null;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        Dictionary<IProportion, ProportionSlider> slidermap; 

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;
            viewer = new Viewer();

            if (viewer.InitializeApplication(this))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Transform();
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);

                timer1.Enabled = true;
            }

            slidermap = new Dictionary<IProportion, ProportionSlider>();
            {
                int nproportion = 0;
                foreach (IProportion proportion in ProportionList.Instance.items)
                {
                    ProportionSlider slider = new ProportionSlider();
                    slider.ClassName = proportion.ToString();

                    slider.Location = new System.Drawing.Point(0, nproportion * 64);
                    slider.ValueChanged += new System.EventHandler(this.slider_ValueChanged);
                    panel1.Controls.Add(slider);
                    
                    slidermap[proportion] = slider;
                    nproportion++;
                }
            }

            AssignSliderProportion();
        }

        private void AssignSliderProportion()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                foreach (TPOFile tpo in fig.TPOList.files)
                {
                    ProportionSlider slider = slidermap[tpo.Proportion];
                    slider.Tag = tpo;
                    slider.Ratio = tpo.Ratio;
                }
            }
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

            Transform();
        }

        private void Transform()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                fig.TransformTpo(fig.GetFrameIndex());
                fig.UpdateBoneMatrices(true);
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                foreach (string src in (string[])e.Data.GetData(DataFormats.FileDrop))
                    viewer.LoadAnyFile(src, (e.KeyState & 8) == 8);
                AssignSliderProportion();
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

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                config.Proportions = new Proportion[fig.TPOList.Count];
                for (int i = 0; i < fig.TPOList.Count; i++)
                    config.Proportions[i] = new Proportion();

                for (int i = 0; i < fig.TPOList.Count; i++)
                {
                    TPOFile tpo = fig.TPOList[i];
                    Proportion portion = config.Proportions[i];
                    portion.ClassName = tpo.ProportionName;
                    portion.Ratio = tpo.Ratio;
                }
            }
            config.Save(Figure.GetTPOConfigPath());
        }

    }
}
