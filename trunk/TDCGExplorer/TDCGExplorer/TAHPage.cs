using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArchiveLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;

//using System.Windows.Forms;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class TAHPageControl : Control
    {
        List<ArcsTahFilesEntry> filesEntries;
        private DataGridView dataGridView;
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemClose;
        private ToolStripMenuItem toolStripMenuItemEditMode;
        GenericTahInfo info;

        public TAHPageControl(GenericTahInfo entryinfo, List<ArcsTahFilesEntry> filesentries)
        {
            InitializeComponent();

            info = entryinfo;
            Text = info.shortname+" version "+entryinfo.version.ToString();
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

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;

            TDCGExplorer.TDCGExplorer.SetToolTips(info.shortname + " : tsoクリックで単体表示,ctrlキー+tsoクリックで複数表示,tmoでポーズ・アニメーションを設定");
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditMode = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEditMode,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(123, 48);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // toolStripMenuItemEditMode
            // 
            this.toolStripMenuItemEditMode.Name = "toolStripMenuItemEditMode";
            this.toolStripMenuItemEditMode.Size = new System.Drawing.Size(122, 22);
            this.toolStripMenuItemEditMode.Text = "編集する";
            this.toolStripMenuItemEditMode.Click += new System.EventHandler(this.toolStripMenuItemEditMode_Click);
            // 
            // TAHPageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = Size;
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
                        using (GenericTAHStream tahstream = new GenericTAHStream(info, filesEntries[index]))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                            if (ext == ".tso")
                            {
                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                                }
                                else
                                {
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                                    // カメラをセンター位置に.
                                    TSOCameraAutoCenter camera = new TSOCameraAutoCenter(TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer);
                                    camera.UpdateCenterPosition(Path.GetFileName(filesEntries[index].path).ToUpper());
                                    // 次回カメラが必ずリセットされる様にする.
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();
                                }
                            }
                            else if (ext == ".tmo")
                            {
                                TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTMOFile(tahstream.stream);
                                TDCGExplorer.TDCGExplorer.defaultpose = tahstream.stream;
                            }
                            Cursor.Current = Cursors.Default;
                        }
                    }
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

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        // ページを閉じる
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            Parent.Dispose();
        }

        // 編集モードに切り替える
        private void toolStripMenuItemEditMode_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            LBFileTahUtl.OpenTahEditor(info);
        }
    }
}
