using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TMOComposerConfig
{
    public partial class Form1 : Form
    {
        public TSOConfig config;
        string config_file = Path.Combine(Application.StartupPath, @"config.xml");

        public Form1()
        {
            InitializeComponent();
            if (File.Exists(config_file))
                config = TSOConfig.Load(config_file);
            else
                config = new TSOConfig();
            propertyGrid1.SelectedObject = config;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            config.Save(config_file);
            Close();
        }
    }
}
