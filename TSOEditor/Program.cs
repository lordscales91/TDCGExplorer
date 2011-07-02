using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TDCG;

namespace TSOEditor
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            TSOConfig tso_config;

            string tso_config_file = Path.Combine(Application.StartupPath, @"config.xml");
            if (File.Exists(tso_config_file))
                tso_config = TSOConfig.Load(tso_config_file);
            else
                tso_config = new TSOConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 form1 = new Form1())
            using (Form2 form2 = new Form2())
            using (Form3 form3 = new Form3())
            {
                form2.TopLevel = false;
                form2.Location = new System.Drawing.Point(0, 26);
                form1.Controls.Add(form2);
                form2.BringToFront();
                form2.viewer = form1.viewer;
                form3.viewer = form1.viewer;
                form1.viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Figure fig;
                    if (form1.viewer.TryGetFigure(out fig))
                    {
                        form3.AssignTSOFiles(fig);
                    }
                };
                form2.RotationEvent += delegate(object sender, EventArgs e)
                {
                    form1.Invalidate(false);
                };
                form3.OpenTextureEvent += delegate(object sender, EventArgs e)
                {
                    form1.viewer.OpenTexture(form3.GetSelectedTexture());
                    form1.Invalidate();
                };
                form1.InitializeApplication(tso_config, args);
                form1.Show();
                form2.Show();
                form3.Show();
                Application.Run(form1);
            }
        }
    }
}
