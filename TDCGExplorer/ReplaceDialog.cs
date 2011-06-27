using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public partial class ReplaceDialog : Form
    {
        public ReplaceDialog()
        {
            InitializeComponent();
        }

        public string textTo
        {
            get { return textBoxToString.Text; }
            set { textBoxToString.Text = value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textTo.Length > 0)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.None;
        }
    }
}
