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
    public partial class TAHInfoDialog : Form
    {
        public TAHInfoDialog()
        {
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
        }

        public int tahVersion
        {
            get
            {
                int retval = 1;
                try
                {
                    retval = int.Parse(textBoxTahVersion.Text);
                }
                catch (Exception)
                {
                }
                return retval;
            }
            set { textBoxTahVersion.Text = value.ToString(); }
        }

        public string tahSource
        {
            get { return textBoxTahSource.Text; }
            set { textBoxTahSource.Text = value; }
        }
    }
}
