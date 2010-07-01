using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace mqoview
{
    public partial class Form1 : Form
    {
        Device device = null;

        public Form1()
        {
            InitializeComponent();
        }

        public void InitializeGraphics()
        {
            PresentParameters pp = new PresentParameters();

            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, pp);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            device.Clear(ClearFlags.Target, Color.CornflowerBlue, 1.0f, 0);
            device.Present();
        }
    }
}
