using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using System.Data;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class CollisionTahPageControl : Control
    {
        private SplitContainer splitContainer;
        private WebBrowser webBrowser;
        private DataGridView dataGridView;
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemEditTah;
        private ToolStripMenuItem toolStripMenuItemClose;
        CollisionItem collisionEntry;

        public CollisionTahPageControl(CollisionItem argCollisionEntry)
        {
            InitializeComponent();
            collisionEntry = argCollisionEntry;
            Text = Path.GetFileName(collisionEntry.tah.path);

            TDCGExplorer.TDCGExplorer.SetToolTips(Text);

            DataTable data = new DataTable();
            data.Columns.Add("衝突元", Type.GetType("System.String"));
            data.Columns.Add("衝突先", Type.GetType("System.String"));
            data.Columns.Add("衝突先TAH", Type.GetType("System.String"));
            foreach (ArcsCollisionRecord col in collisionEntry.entries)
            {
                ArcsDatabase db = TDCGExplorer.TDCGExplorer.ArcsDB;
                // 衝突した先のtahを取得する.
                ArcsTahEntry to = db.GetTah(col.toTahID);
                // 既に同じ名前で追加していないか調べる.
                ArcsTahFilesEntry fromfile = db.GetTahFilesEntry(col.fromFilesID);
                ArcsTahFilesEntry tofile = db.GetTahFilesEntry(col.toFilesID);
                DataRow row = data.NewRow();
                string[] content = { fromfile.GetDisplayPath(), tofile.GetDisplayPath().ToLower(), to.shortname };
                row.ItemArray = content;
                data.Rows.Add(row);
            }
            dataGridView.DataSource = data;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;

            selectIndex(0);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEditTah = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dataGridView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer.Size = new System.Drawing.Size(0, 54);
            this.splitContainer.TabIndex = 0;
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
            this.dataGridView.Size = new System.Drawing.Size(0, 150);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEditTah,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(207, 48);
            // 
            // toolStripMenuItemEditTah
            // 
            this.toolStripMenuItemEditTah.Name = "toolStripMenuItemEditTah";
            this.toolStripMenuItemEditTah.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemEditTah.Text = "TAHファイルを編集する";
            this.toolStripMenuItemEditTah.Click += new System.EventHandler(this.toolStripMenuItemEditTah_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(20, 250);
            this.webBrowser.TabIndex = 0;
            // 
            // CollisionTahPageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.splitContainer);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CollisionTahPage_Layout);
            this.Resize += new System.EventHandler(this.CollisionTahPage_Resize);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void CollisionTahPage_Resize(object sender, EventArgs e)
        {
            splitContainer.Size = ClientSize;
            dataGridView.Size = splitContainer.Panel1.ClientSize;
            webBrowser.Size = splitContainer.Panel2.ClientSize;
        }

        private void selectIndex(int index)
        {
            ArcsCollisionRecord col = collisionEntry.entries[index];

            ArcsDatabase db = TDCGExplorer.TDCGExplorer.ArcsDB;
            // 衝突した先のtahを取得する.
            ArcsTahEntry from = db.GetTah(col.fromTahID);
            ArcsTahEntry to = db.GetTah(col.toTahID);
            // 既に同じ名前で追加していないか調べる.
            ArcsTahFilesEntry fromfile = db.GetTahFilesEntry(col.fromFilesID);
            ArcsTahFilesEntry tofile = db.GetTahFilesEntry(col.toFilesID);

            string text =
                @"<p>" +
                @"<h2> 衝突したtah : " + from.shortname + "</h2>" +
                @"<adress>" + "ディレクトリ : " + Path.GetDirectoryName(from.path) + "</adress>" +
                @"<h3> 衝突先 : " + to.shortname + "</h3>" +
                @"<adress>" + "ディレクトリ : " + Path.GetDirectoryName(to.path) + "</adress>" +
                @"<pre>" + fromfile.GetDisplayPath().ToLower() + " → " + tofile.GetDisplayPath().ToLower() + "</pre>" +
                @"<pre>" + "ハッシュコード : " + tofile.hash.ToString("x8") + "</pre>";

            webBrowser.DocumentText = text;

            try
            {
                string filename = fromfile.GetDisplayPath().ToLower();
                if (Path.GetExtension(filename) == ".tso")
                {
                    // TSOを表示する.
                    using (GenericTAHStream tahstream = new GenericTAHStream(new GenericArcsTahInfo(from), fromfile))
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                        if(TDCGExplorer.TDCGExplorer.SystemDB.loadinitialpose) TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                        // カメラをセンター位置に.
                        TSOCameraAutoCenter camera = new TSOCameraAutoCenter(TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer);
                        camera.UpdateCenterPosition(fromfile.path.ToUpper());
                        // 次回カメラが必ずリセットされる様にする.
                        TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();
                        Cursor.Current = Cursors.Default;
                        TDCGExplorer.TDCGExplorer.FigureLoad = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                selectIndex(index);
            }
        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void CollisionTahPage_Layout(object sender, LayoutEventArgs e)
        {
            splitContainer.SplitterDistance = ClientSize.Height/3;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void toolStripMenuItemEditTah_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;

            ArcsCollisionRecord col = collisionEntry.entries[0];
            ArcsDatabase db = TDCGExplorer.TDCGExplorer.ArcsDB;
            ArcsTahEntry from = db.GetTah(col.fromTahID);
            LBFileTahUtl.OpenTahEditor(new GenericArcsTahInfo(from));
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            Parent.Dispose();
        }
    }
}
