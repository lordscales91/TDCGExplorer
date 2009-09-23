using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOComposerConfig
{
    public partial class Form1 : Form
    {
        public TSOConfig config;
        string config_path = @"config.xml";

        public Form1()
        {
            InitializeComponent();
            config = TSOConfig.Load(config_path);
            propertyGrid1.SelectedObject = config;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            config.Save(config_path);
            Close();
        }
    }
}
