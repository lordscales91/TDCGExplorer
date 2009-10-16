using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;
using TDCG;

namespace TSOText
{
    public partial class Form1 : Form
    {
        Viewer viewer;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        System.Drawing.Font font = SystemFonts.DefaultFont;

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(1024, 800);

            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, -44.0f);
                viewer.Camera.Move(0, +100, -150);
                viewer.SwitchMotionEnabled();
                timer1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            Bitmap bmp = new Bitmap(16*8, 16);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawString(tbWord.Text, font, Brushes.Black, 0, 0);

            viewer.Positions.Clear();
            Color pixel;
            SizeF size = g.MeasureString(tbWord.Text, font);
            if (size.Width > bmp.Width)
                size.Width = bmp.Width;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16*8; x++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.A == 0xff)
                    {
                        viewer.Positions.Add(new Vector3(8.0f * (x - size.Width/2) - 2.0f * (y % 2), 0.0f, 8.0f * (y - size.Height/2)));
                    }
                }
            }

            g.Dispose();
            bmp.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
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

        private void btnFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                font = fontDialog1.Font;
            }
        }

        private void tbWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x0d)
            {
                UpdatePositions();
                e.Handled = true;
            }
        }
    }
}
