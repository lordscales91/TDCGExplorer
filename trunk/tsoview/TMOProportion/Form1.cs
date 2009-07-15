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
        List<IProportion> pro_list = new List<IProportion>();
        List<TrackBar> bar_list = new List<TrackBar>();

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

            pro_list.Add(new TDCG.Proportion.Zoom());

            foreach (IProportion pro in pro_list)
            {
                TrackBar trackBar = new TrackBar();
                trackBar.Location = new System.Drawing.Point(10, 60 + bar_list.Count * 50);
                trackBar.Maximum = 20;
                trackBar.Name = "ProportionTrackBar" + (bar_list.Count + 1).ToString();
                trackBar.Size = new System.Drawing.Size(262, 45);
                trackBar.TabIndex = bar_list.Count + 1;
                trackBar.ValueChanged += new System.EventHandler(this.proportionTrackBar_ValueChanged);
                this.Controls.Add(trackBar);

                bar_list.Add(trackBar);
            }

            UpdateTpoList();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void proportionTrackBar_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = sender as TrackBar;
            {
                TPOFile tpo = trackBar.Tag as TPOFile;
                if (tpo != null)
                    tpo.Ratio = trackBar.Value * 0.1f;
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
                }
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
