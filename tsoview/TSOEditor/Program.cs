using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 form1 = new Form1())
            using (Form2 form2 = new Form2())
            {
                form1.viewer.FigureEvent += delegate(object sender, EventArgs e)
                {
                    Figure fig;
                    if (form1.viewer.TryGetFigure(out fig))
                    {
                        form2.AssignTSOFiles(fig);
                    }
                };
                form2.OpenTextureEvent += delegate(object sender, EventArgs e)
                {
                    form1.viewer.OpenTexture(form2.GetSelectedTexture());
                    form1.Invalidate();
                };
                form1.InitializeApplication(args);
                form1.Show();
                form2.Show();
                Application.Run(form1);
            }
        }
    }
}
