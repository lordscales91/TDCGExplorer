using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArchiveLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using TDCG;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

//using System.Windows.Forms;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class TAHPageControl : Control
    {
        List<ArcsTahFilesEntry> filesEntries;
        private DataGridView dataGridView;
        //private System.ComponentModel.IContainer components;
        GenTahInfo info;

        public TAHPageControl(GenTahInfo entryinfo, List<ArcsTahFilesEntry> filesentries)
        {
            InitializeComponent();

            info = entryinfo;
            Text = info.shortname;
            filesEntries = filesentries;

            DataTable data = new DataTable();
            data.Columns.Add("ID", Type.GetType("System.String"));
            data.Columns.Add("ファイル名", Type.GetType("System.String"));
            data.Columns.Add("ファイルタイプ", Type.GetType("System.String"));
            data.Columns.Add("ハッシュ値", Type.GetType("System.String"));
            data.Columns.Add("データサイズ", Type.GetType("System.String"));
            foreach (ArcsTahFilesEntry file in filesentries)
            {
                DataRow row = data.NewRow();
                string[] content = { file.tahentry.ToString(), file.GetDisplayPath(), Path.GetExtension(file.path), file.hash.ToString("x8"), file.length.ToString() };
                row.ItemArray = content;
                data.Rows.Add(row);
            }
            dataGridView.DataSource = data;

            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.AllowUserToAddRows = false;

            TDCGExplorer.TDCGExplorer.SetToolTips(info.shortname + " : tsoクリックで単体表示,ctrlキー+tsoクリックで複数表示,tmoでポーズ・アニメーションを設定");
        }

        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // TAHPageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = Size;
        }

        internal char current_row = 'A';

        private void SetCurrentTSOFileName(string filename)
        {
            string basename = Path.GetFileNameWithoutExtension(filename);
            if (basename.Length == 12)
                current_row = basename.ToUpper()[9];
            else
                current_row = 'A';
        }

        private int GetCenterBoneType()
        {
            switch (current_row)
            {
                case 'S'://手首
                case 'X'://腕装備(手甲など)
                case 'Z'://手持ちの小物
                    return 1;
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return 2;
                case 'O'://靴
                    return 3;
                default:
                    return 0;
            }
        }
 
        private string GetCenterBoneName()
        {
            switch (current_row)
            {
                case 'A'://身体
                    return "W_Neck";
                case 'E'://瞳
                case 'D'://頭皮(生え際)
                case 'B'://前髪
                case 'C'://後髪
                case 'U'://アホ毛類
                    return "Kami_Oya";
                case 'Q'://眼鏡
                case 'V'://眼帯
                case 'Y'://リボン
                case 'P'://頭部装備(帽子等)
                    return "face_oya";
                case '3'://イヤリング類
                case '0'://眉毛
                case '2'://ほくろ
                case '1'://八重歯
                    return "Head";
                case 'R'://首輪
                    return "W_Neck";
                case 'F'://ブラ
                case 'J'://上衣(シャツ等)
                case 'T'://背中(羽など)
                case 'L'://上着オプション(エプロン等)
                    return "W_Spine3";
                case 'G'://全身下着・水着
                case 'K'://全身衣装(ナース服等)
                    return "W_Spine1";
                case 'S'://手首
                case 'X'://腕装備(手甲など)
                case 'Z'://手持ちの小物
                    return "W_Hips";//not reached
                case 'H'://パンツ
                case 'M'://下衣(スカート等)
                case 'N'://尻尾
                    return "W_Hips";
                case 'W'://タイツ・ガーター
                case 'I'://靴下
                    return "W_Hips";//not reached
                case 'O'://靴
                    return "W_Hips";//not reached
                default:
                    return "W_Hips";
            }
        }

        private void UpdateCenterPosition(string tsoname)
        {
            Vector3 position;

            TSOFile tso = TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.FigureList[0].TSOList[0];
            Viewer viewer = TDCGExplorer.TDCGExplorer.GetMainForm().Viewer;

            Dictionary<string, TSONode> nodemap = new Dictionary<string, TSONode>();
            foreach (TSONode node in tso.nodes)
            {
                if(nodemap.ContainsKey(node.ShortName)==false)
                    nodemap.Add(node.ShortName, node);
            }


            SetCurrentTSOFileName(tsoname);

            switch (GetCenterBoneType())
            {
                case 1://Hand
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightHand";
                        string boneL = "W_LeftHand";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                case 2://Leg
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightLeg";
                        string boneL = "W_LeftLeg";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                case 3://Foot
                    {
                        TSONode tso_nodeR;
                        TSONode tso_nodeL;
                        string boneR = "W_RightFoot";
                        string boneL = "W_LeftFoot";
                        if (nodemap.TryGetValue(boneR, out tso_nodeR) && nodemap.TryGetValue(boneL, out tso_nodeL))
                        {
                            Matrix mR = tso_nodeR.combined_matrix;
                            Matrix mL = tso_nodeL.combined_matrix;
                            position = new Vector3((mR.M41 + mL.M41) / 2.0f, (mR.M42 + mL.M42) / 2.0f, -(mR.M43 + mL.M43) / 2.0f);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
                default:
                    {
                        TSONode tso_node;
                        string bone = GetCenterBoneName();
                        if (nodemap.TryGetValue(bone, out tso_node))
                        {
                            Matrix m = tso_node.combined_matrix;
                            position = new Vector3(m.M41, m.M42, -m.M43);
                            viewer.Camera.Reset();
                            viewer.Camera.SetCenter(position);
                        }
                    }
                    break;
            }
            
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                System.Windows.Forms.DataGridViewRow dgr = this.dataGridView.CurrentRow;
                System.Data.DataRowView drv = (System.Data.DataRowView)dgr.DataBoundItem;
                System.Data.DataRow dr = (System.Data.DataRow)drv.Row;
                int index = int.Parse(dr.ItemArray[0].ToString());

                if (index >= 0)
                {
                    string ext = Path.GetExtension(filesEntries[index].path).ToLower();
                    if (ext == ".tso" || ext == ".tmo")
                    {
                        using (TAHStream tahstream = new TAHStream(info, filesEntries[index]))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            TDCGExplorer.TDCGExplorer.GetMainForm().makeTSOViwer();
                            if (ext == ".tso")
                            {
                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(tahstream.stream);
                                    TDCGExplorer.TDCGExplorer.GetMainForm().doInitialTmoLoad(); // 初期tmoを読み込む.
                                }
                                else
                                {
                                    TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.ClearFigureList();
                                    TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(tahstream.stream);
                                    TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.LoadTMOFile("default.tmo");
                                    UpdateCenterPosition(Path.GetFileName(filesEntries[index].path).ToUpper());
                                }
                            }
                            else if (ext == ".tmo") TDCGExplorer.TDCGExplorer.GetMainForm().Viewer.LoadTMOFile(tahstream.stream);
                            Cursor.Current = Cursors.Default;
                        }
                    }
#if false
                    else
                    {
                        MessageBox.Show("TAH INFO:\n" + filesEntries[index].id + "," + filesEntries[index].path + "," + filesEntries[index].hash.ToString("x8"), "Not Implemented", MessageBoxButtons.OK);
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured:" + ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }
    }
}
