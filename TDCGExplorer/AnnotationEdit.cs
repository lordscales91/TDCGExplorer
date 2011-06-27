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
    public partial class AnnotationEdit : Form
    {
        public AnnotationEdit()
        {
            InitializeComponent();
        }

        public string code
        {
            get { return labelAnnotationCode.Text; }
            set { labelAnnotationCode.Text = value; }
        }
        public string text
        {
            get { return textBoxAnnotation.Text; }
            set { textBoxAnnotation.Text = value; }
        }
    }
}
