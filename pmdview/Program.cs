using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace pmdview
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 form1 = new Form1())
            {
                // Show our form and initialize our graphics engine
                form1.Show();
                form1.InitializeGraphics(args);
                Application.Run(form1);
            }
        }
    }
}
