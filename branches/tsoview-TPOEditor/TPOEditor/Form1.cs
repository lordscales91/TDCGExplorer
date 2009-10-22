using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
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
                string class_name = name_space + "." + Path.GetFileNameWithoutExtension(script_file);
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

        string name_space = "TDCG.Proportion";

        string GetProportionClassName(TPOFile tpo)
        {
            return tpo.ProportionName.Substring(name_space.Length + 1);
        }

        void SaveTpoScript(TPOFile tpo)
        {
            string script_file = Path.Combine(GetProportionPath(), GetProportionClassName(tpo) + ".cs");
            using (StreamWriter sw = File.CreateText(script_file))
            {
                DumpTpoScript(tpo, sw);
            }
        }

        string indent = "            ";

        void WriteRotationTpoCommandLine(string axis, float angle, StreamWriter sw)
        {
            sw.Write(indent);
            sw.WriteLine(@"node.Rotate{1}(Geometry.DegreeToRadian({0:F}F));", Geometry.RadianToDegree(angle), axis);
        }

        void WriteTpoCommandLine(TPOCommand command, StreamWriter sw)
        {
            string method_name = null;
            switch (command.Type)
            {
                case TPOCommandType.Scale:
                    method_name = "Scale";
                    break;
                case TPOCommandType.Scale1:
                    method_name = "Scale1";
                    break;
                case TPOCommandType.Rotate:
                    method_name = "Rotate";
                    break;
                case TPOCommandType.Move:
                    method_name = "Move";
                    break;
            }
            switch (command.Type)
            {
                case TPOCommandType.Scale:
                case TPOCommandType.Scale1:
                case TPOCommandType.Move:
                    sw.Write(indent);
                    sw.WriteLine(@"node.{3}({0:F}F, {1:F}F, {2:F}F);", command.X, command.Y, command.Z, method_name);
                    break;
                case TPOCommandType.Rotate:
                    if (command.X != 0.0f)
                        WriteRotationTpoCommandLine("X", command.X, sw);
                    if (command.Y != 0.0f)
                        WriteRotationTpoCommandLine("Y", command.Y, sw);
                    if (command.Z != 0.0f)
                        WriteRotationTpoCommandLine("Z", command.Z, sw);
                    break;
            }
        }

        void DumpTpoScript(TPOFile tpo, StreamWriter sw)
        {
            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.IO;");
            sw.WriteLine("using Microsoft.DirectX;");
            sw.WriteLine("using Microsoft.DirectX.Direct3D;");
            sw.WriteLine("using TDCG;");
            sw.WriteLine("");
            sw.WriteLine("namespace {0}", name_space);
            sw.WriteLine("{");
            sw.WriteLine("    public class {0} : IProportion", GetProportionClassName(tpo));
            sw.WriteLine("    {");
            sw.WriteLine("        Dictionary<string, TPONode> nodes;");
            sw.WriteLine("        public Dictionary<string, TPONode> Nodes { set { nodes = value; }}");
            sw.WriteLine("");
            sw.WriteLine("        public void Execute()");
            sw.WriteLine("        {");

            sw.Write(indent); sw.WriteLine("TPONode node;");
            foreach (TPONode tponode in tpo.nodes)
            {
                bool command_exist = false;
                foreach (TPOCommand command in tponode.commands)
                {
                    if (command.Type != TPOCommandType.Scale0)
                    {
                        command_exist = true;
                        break;
                    }
                }
                if (command_exist)
                {
                    sw.WriteLine("");
                    sw.Write(indent); sw.WriteLine("node = nodes[\"{0}\"];", tponode.ShortName);
                }
                foreach (TPOCommand command in tponode.commands)
                {
                    WriteTpoCommandLine(command, sw);
                }
            }
            sw.WriteLine("        }");
            sw.WriteLine("    }");
            sw.WriteLine("}");
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

            gvCommands.Rows.Clear();
            bool inv_scale_on_children;
            Vector3 scaling = tponode.GetScaling(out inv_scale_on_children);
            gvCommands.Rows.Add(new string[] { "Scale", scaling.X.ToString(), scaling.Y.ToString(), scaling.Z.ToString() });
            cbInverseScaleOnChildren.Checked = inv_scale_on_children;

            Vector3 angle = tponode.GetAngle();
            gvCommands.Rows.Add(new string[] { "Rotate", angle.X.ToString(), angle.Y.ToString(), angle.Z.ToString() });

            Vector3 translation = tponode.GetTranslation();
            gvCommands.Rows.Add(new string[] { "Move", translation.X.ToString(), translation.Y.ToString(), translation.Z.ToString() });
        }

        float GetFloatFromGridViewCell(DataGridViewCell gvcell)
        {
            string str = gvcell.Value.ToString();
            float flo = 0.0f;
            try
            {
                flo = float.Parse(str);
            }
            catch (FormatException)
            {
            }
            return flo;
        }

        Vector3 GetVector3FromGridViewRow(DataGridViewRow gvrow)
        {
            float x = GetFloatFromGridViewCell(gvrow.Cells[1]);
            float y = GetFloatFromGridViewCell(gvrow.Cells[2]);
            float z = GetFloatFromGridViewCell(gvrow.Cells[3]);
            return new Vector3(x, y, z);
        }

        private void gvCommands_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = tpo_list[tpofile_row];

            int tponode_row = tpoNodeBindingSource.Position;
            TPONode tponode = tpo.nodes[tponode_row];

            DataGridViewRow gvrow = gvCommands.Rows[e.RowIndex];
            switch (e.RowIndex)
            {
                case 0:
                    tponode.SetScaling(GetVector3FromGridViewRow(gvrow), cbInverseScaleOnChildren.Checked);
                    break;
                case 1:
                    tponode.SetAngle(GetVector3FromGridViewRow(gvrow));
                    break;
                case 2:
                    tponode.SetTranslation(GetVector3FromGridViewRow(gvrow));
                    break;
            }
            tpoCommandBindingSource.ResetBindings(false);

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Transform(0);
                fig.UpdateBoneMatrices(true);
            }
        }

        private void cbInverseScaleOnChildren_CheckedChanged(object sender, EventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = tpo_list[tpofile_row];

            int tponode_row = tpoNodeBindingSource.Position;
            TPONode tponode = tpo.nodes[tponode_row];

            bool inv_scale_on_children;
            tponode.SetScaling(tponode.GetScaling(out inv_scale_on_children), cbInverseScaleOnChildren.Checked);

            tpoCommandBindingSource.ResetBindings(false);

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                tpo_list.Transform(0);
                fig.UpdateBoneMatrices(true);
            }
        }

        private void gvCommands_MouseWheel(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = gvCommands.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                //Console.WriteLine("col {0} row {1} delta {2}", hit.ColumnIndex, hit.RowIndex, e.Delta);
                DataGridViewCell gvcell = gvCommands.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                float flo = GetFloatFromGridViewCell(gvcell);
                float power;
                if (hit.RowIndex == 1)
                    power = 1.0f;
                else
                    power = 0.1f;
                gvcell.Value = (flo + (e.Delta / 120) * power).ToString();

                int tpofile_row = tpoFileBindingSource.Position;
                TPOFile tpo = tpo_list[tpofile_row];

                int tponode_row = tpoNodeBindingSource.Position;
                TPONode tponode = tpo.nodes[tponode_row];

                DataGridViewRow gvrow = gvCommands.Rows[hit.RowIndex];
                switch (hit.RowIndex)
                {
                    case 0:
                        tponode.SetScaling(GetVector3FromGridViewRow(gvrow), cbInverseScaleOnChildren.Checked);
                        break;
                    case 1:
                        tponode.SetAngle(GetVector3FromGridViewRow(gvrow));
                        break;
                    case 2:
                        tponode.SetTranslation(GetVector3FromGridViewRow(gvrow));
                        break;
                }
                tpoCommandBindingSource.ResetBindings(false);

                Figure fig;
                if (viewer.TryGetFigure(out fig))
                {
                    tpo_list.Transform(0);
                    fig.UpdateBoneMatrices(true);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (TPOFile tpo in tpo_list.files)
                SaveTpoScript(tpo);
        }
    }
}
