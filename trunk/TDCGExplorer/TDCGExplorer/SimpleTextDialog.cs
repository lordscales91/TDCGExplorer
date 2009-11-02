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
    public partial class SimpleTextDialog : Form
    {
        public SimpleTextDialog()
        {
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
        }

        public string dialogtext
        {
            get { return Text; }
            set { Text = value; }
        }

        public string labeltext
        {
            get { return labelText.Text; }
            set { labelText.Text = value; }
        }

        public string textfield
        {
            get { return textBoxFind.Text; }
            set { textBoxFind.Text = value; }
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
