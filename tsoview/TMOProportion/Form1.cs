using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSScriptLibrary;
using TDCG;

namespace TMOProportion
{
    public partial class Form1 : Form
    {
        internal Viewer viewer = null;
        List<IProportion> pro_list = new List<IProportion>();
        List<ProportionSlider> bar_list = new List<ProportionSlider>();

        public string GetProportionPath()
        {
            return Path.Combine(Application.StartupPath, @"Proportion");
        }

        public string GetTPOConfigPath()
        {
            return Path.Combine(Application.StartupPath, @"TPOConfig.xml");
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            viewer = new Viewer();

            if (viewer.InitializeApplication(this, true))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    UpdateTpoList();
                };
                timer1.Enabled = true;
            }

            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }

            TPOConfig config = TPOConfig.Load(GetTPOConfigPath());
            Dictionary<string, Proportion> portion_map = new Dictionary<string, Proportion>();
            foreach (Proportion portion in config.Proportions)
                portion_map[portion.ClassName] = portion;

            foreach (IProportion pro in pro_list)
            {
                ProportionSlider slider = new ProportionSlider();
                string class_name = pro.ToString();
                {
                    Proportion portion;
                    if (portion_map.TryGetValue(class_name, out portion))
                        slider.Ratio = portion.Ratio;
                }
                slider.label.Text = class_name;
                slider.Location = new System.Drawing.Point(10, 10 + bar_list.Count * 95);
                slider.ValueChanged += new System.EventHandler(this.slider_ValueChanged);
                this.Controls.Add(slider);

                bar_list.Add(slider);
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

        TPOFileList tpo_list = new TPOFileList();
    
        private void UpdateTpoList()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Tmo = fig.Tmo;
                tpo_list.SetProportionList(pro_list);

                for (int i = 0; i < tpo_list.Count; i++)
                {
                    TPOFile tpo = tpo_list[i];
                    bar_list[i].Tag = tpo;
                    tpo.Ratio = bar_list[i].Ratio;
                }

                tpo_list.Transform(fig.GetFrameIndex());
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

    }
}
