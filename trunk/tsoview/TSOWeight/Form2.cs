using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TSOWeight
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        bool pressed = false;

        // This method handles the mouse down event for all the controls on the form.  
        // When a control has captured the mouse
        // the control's name will be output on label1.
        private void Control_MouseDown(System.Object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            Control control = (Control)sender;
            pressed = true;
            if (control.Capture)
                Console.WriteLine(control.Name + " has captured the mouse");
            else
                Console.WriteLine(control.Name + " has not captured the mouse");
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            if (pressed)
                Console.WriteLine(control.Name + string.Format(" x:{0} y:{1}", e.X, e.Y));
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            pressed = false;
            if (control.Capture)
                Console.WriteLine(control.Name + " has captured the mouse");
            else
                Console.WriteLine(control.Name + " has not captured the mouse");
        }
    }
}
