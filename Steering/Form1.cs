using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Steering
{
    public partial class Form1 : Form
    {
        GameWorld world;
        Stopwatch stopwatch;

        public Form1()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
            ClientSize = new Size(400, 400);
            InitializeComponent();
            world = new GameWorld();
            stopwatch = new Stopwatch();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            double time_elapsed = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            stopwatch.Start();
            world.Update(time_elapsed);

            world.Render(e.Graphics);
            
            Invalidate();
        }
    }
}
