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
    public partial class SimpleDropDownDialog : Form
    {
        public SimpleDropDownDialog()
        {
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
        }

        public string dialogtext
        {
            get { return Text; }
            set { Text = value; }
        }
        public void AddList(string text)
        {
            comboBox.Items.Add(text);
        }
        public int selectedIndex
        {
            get { return comboBox.SelectedIndex; }
        }
        public string labeltext
        {
            get { return labelText.Text; }
            set { labelText.Text = value; }
        }
    }
}
