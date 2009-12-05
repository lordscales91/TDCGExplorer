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

namespace TPOEditor
{
    public partial class Form1 : Form
    {
        Viewer viewer = null;
        string save_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";

        public Form1(TSOConfig tso_config, string[] args)
        {
            InitializeComponent();
            this.ClientSize = tso_config.ClientSize;
            viewer = new Viewer();
            if (viewer.InitializeApplication(this))
            {
                foreach (string arg in args)
                    viewer.LoadAnyFile(arg, true);
                if (viewer.FigureList.Count == 0)
                    viewer.LoadAnyFile(Path.Combine(save_path, "system.tdcgsav.png"), true);
                viewer.Camera.SetTranslation(0.0f, +10.0f, +44.0f);
                timer1.Enabled = true;
            }

            tpoFileBindingSource.DataSource = TPOList.files;
        }

        private void Transform()
        {
            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                fig.TransformProportion(fig.GetFrameIndex());
                fig.UpdateBoneMatrices(true);
            }
        }

        TPOFileList TPOList
        {
            get
            {
                Figure fig;
                if (viewer.TryGetFigure(out fig))
                {
                    return fig.TPOList;
                }
                return null;
            }
        }

        string name_space = "TDCG.Proportion";

        string GetProportionClassName(TPOFile tpo)
        {
            return tpo.ProportionName.Substring(name_space.Length + 1);
        }

        void SaveTpoScript(TPOFile tpo)
        {
            string script_file = Path.Combine(ProportionList.GetProportionPath(), GetProportionClassName(tpo) + ".cs");
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
            sw.WriteLine("//css_reference Microsoft.DirectX.dll;");
            sw.WriteLine("//css_reference Microsoft.DirectX.Direct3D.dll;");
            sw.WriteLine("//css_reference Microsoft.DirectX.Direct3DX.dll;");
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

        private void gvPortions_SelectionChanged(object sender, EventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = TPOList[tpofile_row];

            Figure fig;
            if (viewer.TryGetFigure(out fig))
            {
                fig.TPOList.ClearRatios();
                tpo.Ratio = 1.0f;
            }
            Transform();
            
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
            TPOFile tpo = TPOList[tpofile_row];

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
            TPOFile tpo = TPOList[tpofile_row];

            int tponode_row = tpoNodeBindingSource.Position;
            TPONode tponode = tpo.nodes[tponode_row];

            UpdateTpoNodeFactor(e.RowIndex, tponode);
            tpoCommandBindingSource.ResetBindings(false);

            Transform();
        }

        private void UpdateTpoNodeFactor(int row, TPONode tponode)
        {
            DataGridViewRow gvrow = gvCommands.Rows[row];
            switch (row)
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
        }

        private void cbInverseScaleOnChildren_CheckedChanged(object sender, EventArgs e)
        {
            int tpofile_row = tpoFileBindingSource.Position;
            TPOFile tpo = TPOList[tpofile_row];

            int tponode_row = tpoNodeBindingSource.Position;
            TPONode tponode = tpo.nodes[tponode_row];

            bool inv_scale_on_children;
            tponode.SetScaling(tponode.GetScaling(out inv_scale_on_children), cbInverseScaleOnChildren.Checked);

            tpoCommandBindingSource.ResetBindings(false);

            Transform();
        }

        private void gvCommands_MouseWheel(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = gvCommands.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                IncrementGridViewCell(hit.RowIndex, hit.ColumnIndex, e.Delta);

                int tpofile_row = tpoFileBindingSource.Position;
                TPOFile tpo = TPOList[tpofile_row];

                int tponode_row = tpoNodeBindingSource.Position;
                TPONode tponode = tpo.nodes[tponode_row];

                UpdateTpoNodeFactor(hit.RowIndex, tponode);
                tpoCommandBindingSource.ResetBindings(false);

                Transform();
            }
        }

        private void IncrementGridViewCell(int row, int column, int delta)
        {
            DataGridViewCell gvcell = gvCommands.Rows[row].Cells[column];
            gvcell.Value = GetFloatFromGridViewCell(gvcell) + (delta / 120) * GetMouseWheelPower(row);
        }

        private static float GetMouseWheelPower(int row)
        {
            if (row == 1)
                return 1.0f;
            else
                return 0.1f;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (TPOFile tpo in TPOList.files)
                SaveTpoScript(tpo);
        }
    }
}
