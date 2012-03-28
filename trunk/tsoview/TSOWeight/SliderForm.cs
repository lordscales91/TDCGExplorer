using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TDCG;

namespace TSOWeight
{
    public partial class SliderForm : Form
    {
        public WeightViewer viewer = null;
        public Form1 form1 = null;
        
        public SliderForm(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        private Figure fig = null;

        /// <summary>
        /// フィギュア情報を削除します。
        /// </summary>
        public void Clear()
        {
            this.fig = null;
        }

        /// <summary>
        /// フィギュアをUIに設定します。
        /// </summary>
        /// <param name="fig">フィギュア</param>
        public void SetFigure(Figure fig)
        {
            this.fig = fig;

            this.tbSlideArm.Value = (int)(fig.slider_matrix.ArmRatio * (float)tbSlideArm.Maximum);
            this.tbSlideLeg.Value = (int)(fig.slider_matrix.LegRatio * (float)tbSlideLeg.Maximum);
            this.tbSlideWaist.Value = (int)(fig.slider_matrix.WaistRatio * (float)tbSlideWaist.Maximum);
            this.tbSlideBust.Value = (int)(fig.slider_matrix.BustRatio * (float)tbSlideBust.Maximum);
            this.tbSlideTall.Value = (int)(fig.slider_matrix.TallRatio * (float)tbSlideTall.Maximum);
            this.tbSlideEye.Value = (int)(fig.slider_matrix.EyeRatio * (float)tbSlideEye.Maximum);
        }

        private void tbSlideArm_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.ArmRatio = tbSlideArm.Value / (float)tbSlideArm.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void tbSlideLeg_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.LegRatio = tbSlideLeg.Value / (float)tbSlideLeg.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void tbSlideWaist_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.WaistRatio = tbSlideWaist.Value / (float)tbSlideWaist.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void tbSlideBust_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.BustRatio = tbSlideBust.Value / (float)tbSlideBust.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void tbSlideTall_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.TallRatio = tbSlideTall.Value / (float)tbSlideTall.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void tbSlideEye_ValueChanged(object sender, EventArgs e)
        {
            if (fig == null)
                return;

            fig.slider_matrix.EyeRatio = tbSlideEye.Value / (float)tbSlideEye.Maximum;
            fig.UpdateBoneMatrices(true);
            form1.Invalidate(false);
        }

        private void SliderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
