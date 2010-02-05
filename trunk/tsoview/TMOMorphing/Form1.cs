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
            morphing = new Morphing();

            if (viewer.InitializeApplication(this))
            {
                viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Morph();
                };
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +18.0f, +10.0f);

                timer1.Enabled = true;
            }

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

            Morph();
        }

        private void Morph()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                morphing.Morph(fig.Tmo);
                fig.UpdateBoneMatricesWithoutTMOFrame();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "png files|*.png|tmo files|*.tmo";
                dialog.FilterIndex = 0;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string dest_file = dialog.FileName;
                    string extension = Path.GetExtension(dest_file);
                    if (extension == ".png")
                    {
                        TMOFile tmo = fig.Tmo.GenerateTMOFromTransformationMatrix();
                        SavePoseToFile(tmo, dest_file);
                    }
                    else
                    if (extension == ".tmo")
                    {
                        TMOFile tmo = fig.Tmo.GenerateTMOFromTransformationMatrix();
                        tmo.Save(dest_file);
                    }
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

        public void SavePoseToFile(TMOFile tmo, string dest_file)
        {
            if (tmo.frames != null)
            {
                PNGFile png = CreatePNGFile();
                png.WriteTaOb += delegate(BinaryWriter bw)
                {
                    PNGWriter pw = new PNGWriter(bw);
                    WritePose(pw, tmo);
                };
                Debug.WriteLine("Save File: " + dest_file);
                png.Save(dest_file);
            }
        }

        public PNGFile CreatePNGFile()
        {
            MemoryStream ms = new MemoryStream();
            using (Bitmap bmp = new Bitmap(180, 180, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                Graphics g = Graphics.FromImage(bmp);
                Brush brush = new SolidBrush(Color.FromArgb(0xfb, 0xc6, 0xc6));
                g.FillRectangle(brush, 0, 0, 180, 180);
                Font font = new Font(FontFamily.GenericSerif, 36, FontStyle.Bold);
                g.DrawString("morphing", font, Brushes.Black, 0, 0);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            }
            ms.Seek(0, SeekOrigin.Begin);

            PNGFile png = new PNGFile();
            png.Load(ms);

            return png;
        }

        protected byte[] ReadFloats(string dest_file)
        {
            List<float> floats = new List<float>();
            string line;
            using (StreamReader source = new StreamReader(File.OpenRead(dest_file)))
                while ((line = source.ReadLine()) != null)
                {
                    floats.Add(Single.Parse(line));
                }

            byte[] data = new byte[sizeof(Single) * floats.Count];
            int offset = 0;
            foreach (float flo in floats)
            {
                byte[] buf_flo = BitConverter.GetBytes(flo);
                buf_flo.CopyTo(data, offset);
                offset += buf_flo.Length;
            }
            return data;
        }

        string GetCameraPath()
        {
            return Path.Combine(Application.StartupPath, "Camera.txt");
        }

        string GetLightAPath()
        {
            return Path.Combine(Application.StartupPath, "LightA.txt");
        }

        void WritePose(PNGWriter pw, TMOFile tmo)
        {
            byte[] cami = ReadFloats(GetCameraPath());
            byte[] lgta = ReadFloats(GetLightAPath());

            pw.WriteTDCG();
            pw.WritePOSE();
            pw.WriteCAMI(cami);
            pw.WriteLGTA(lgta);
            pw.WriteFTMO(tmo);
        }
    }
}
