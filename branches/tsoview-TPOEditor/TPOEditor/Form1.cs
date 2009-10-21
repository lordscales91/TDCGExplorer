﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;
using CSScriptLibrary;

namespace TPOEditor
{
    public partial class Form1 : Form
    {
        Viewer viewer = null;
        List<IProportion> pro_list = new List<IProportion>();
        TPOFileList tpo_list = new TPOFileList();
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public string GetProportionPath()
        {
            return Path.Combine(Application.StartupPath, @"Proportion");
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(1024, 800);
            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, -44.0f);
                timer1.Enabled = true;
            }

            string[] script_files = Directory.GetFiles(GetProportionPath(), "*.cs");
            foreach (string script_file in script_files)
            {
                string class_name = "TDCG.Proportion." + Path.GetFileNameWithoutExtension(script_file);
                var script = CSScript.Load(script_file).CreateInstance(class_name).AlignToInterface<IProportion>();
                pro_list.Add(script);
            }
            tpo_list.SetProportionList(pro_list);
            tpoFileBindingSource.DataSource = tpo_list.files;

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Tmo = fig.Tmo;
            }
        }

        private void ClearTpoRatios()
        {
            for (int i = 0; i < tpo_list.Count; i++)
            {
                TPOFile tpo = tpo_list[i];
                tpo.Ratio = 0.0f;
            }
        }

        private void SetTpoRatio(TPOFile tpo)
        {
            ClearTpoRatios();
            tpo.Ratio = 1.0f;
        }

        private void gvPortions_SelectionChanged(object sender, EventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = tpo_list[tpofile_row];

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                SetTpoRatio(tpo);
                tpo_list.Transform(0);
                fig.UpdateBoneMatrices(true);
            }
            
            tpoNodeBindingSource.DataSource = tpo.nodes;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            viewer.FrameMove();
            viewer.Render();
        }

        private void gvTPONodes_SelectionChanged(object sender, EventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = tpo_list[tpofile_row];

            int tponode_row = tpoNodeBindingSource.Position;
            TPONode tponode = tpo.nodes[tponode_row];

            tpoCommandBindingSource.DataSource = tponode.commands;
        }
    }
}
