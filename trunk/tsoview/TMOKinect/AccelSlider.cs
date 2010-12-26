using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOKinect
{
    public partial class AccelSlider : UserControl
    {
        public AccelSlider()
        {
            InitializeComponent();
        }

        float accel = 0.5f;

        public float Accel
        {
            get
            {
                return accel;
            }
            set
            {
                accel = value;
                tbAccel.Value = (int)(accel * 10);
                lbAccel.Text = string.Format("{0:F1}", accel);
            }
        }

        private void tbAccel_ValueChanged(object sender, EventArgs e)
        {
            Accel = tbAccel.Value * 0.1f;
            Invalidate();
        }

        private void AccelSlider_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush brush = new SolidBrush(Color.Gray);
            Pen pen = new Pen(Color.Silver);

            int length = 10;
            float p1 = accel;
            
            float p0 = 0.0f;
            float p2 = 1.0f;
            float dt = 1.0f / length;

            int width = 173;
            int x1 = 16;
            int x2 = x1 + width;
            int y1 = 24;
            int r = 5;
            g.DrawLine(pen, x1, y1, x2, y1);
            g.DrawLine(pen, x1, y1 - (r + 2), x1, y1 + (r + 2));
            g.DrawLine(pen, x2, y1 - (r + 2), x2, y1 + (r + 2));
            for (int i = 0; i < length; i++)
            {
                float t = dt * i;
                float p = t * t * (p2 - 2 * p1 + p0) + t * (2 * p1 - 2 * p0) + p0;
                g.FillEllipse(brush, x1 - r + (int)(p * width), y1 - r, r*2, r*2);
            }
        }
    }
}
