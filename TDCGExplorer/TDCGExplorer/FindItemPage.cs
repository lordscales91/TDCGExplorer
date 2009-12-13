using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCGExplorer;
using System.Data;
using System.Threading;
using System.IO;

namespace System.Windows.Forms
{
    class FindItemPage : Control
    {
        private bool firstdata = true;
        private DataGridView dataGridView;
        List<FindEntryInformation> finditems = new List<FindEntryInformation>();
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemClose;
        private string keyword;
        private bool flagtahlevelsearch;
        private bool flagzipsearch;

        public FindItemPage(string itKeyword,bool zipsearch)
        {
            DoInitialize(itKeyword,zipsearch, false);
        }

        public FindItemPage(string itKeyword,bool zipsearch,bool tahlevelsearch)
        {
            DoInitialize(itKeyword, zipsearch, tahlevelsearch);
        }

        private void DoInitialize(string itKeyword,bool zipsearch,bool tahlevelsearch)
        {
            keyword = itKeyword;
            flagtahlevelsearch = tahlevelsearch;
            flagzipsearch = zipsearch;
            InitializeComponent();

            DataTable data = new DataTable();
            data.Columns.Add("種類", Type.GetType("System.String"));
            data.Columns.Add("ファイル名", Type.GetType("System.String"));
            data.Columns.Add("ディレクトリ", Type.GetType("System.String"));

            dataGridView.DataSource = data;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;

            Text = "検索 : "+keyword;

            // データの探索はバックグラウンドスレッドで非同期実行する.
            // 二重起動防止.
            FindItemThread fdb = new FindItemThread();
            fdb.control = this;
            fdb.Keyword = keyword;
            fdb.TahLevelSearch = flagtahlevelsearch;
            fdb.ZipSearch = flagzipsearch;
            Thread thread = new Thread(new ThreadStart(fdb.Run));
            thread.Start();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(240, 150);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DoubleClick += new System.EventHandler(this.dataGridView_DoubleClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(111, 26);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // FindItemPage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        // invokeの為のdelegate
        private delegate void AddItemFromBGThread(FindEntryInformation entry);

        // 非同期で呼び出されるメソッド
        private void AddItem(FindEntryInformation entry)
        {
            string[] catstr={"arcs","zips"};
            finditems.Add(entry);
            DataTable data = dataGridView.DataSource as DataTable;
            if (data != null)
            {
                DataRow row = data.NewRow();
                string[] content = { catstr[entry.category], entry.file, entry.path };
                row.ItemArray = content;
                data.Rows.Add(row);
            }
            if (firstdata)
            {
                firstdata = false;
#if false
                dataGridView.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
#endif
            }
        }

        // 非同期でツリー表示を更新する.
        public void asyncDisplayFromArcs(FindEntryInformation entry)
        {
            Invoke(new AddItemFromBGThread(AddItem), entry);
        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = ClientSize;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
#if false
        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView.CurrentCell.RowIndex;
            if (index >= 0)
            {
                switch (finditems[index].category)
                {
                    case 0:
                        TDCGExplorer.TDCGExplorer.SelectArcsTreeNode(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, finditems[index].path));
                        break;
                    case 1:
                        TDCGExplorer.TDCGExplorer.SelectZipsTreeNode(TDCGExplorer.TDCGExplorer.SystemDB.zips_path+"\\"+finditems[index].path);
                        break;
                    default:
                        break;
                }
            }
        }
#endif
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            Parent.Dispose();
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            int index = dataGridView.CurrentCell.RowIndex;
            if (index >= 0)
            {
                switch (finditems[index].category)
                {
                    case 0:
                        TDCGExplorer.TDCGExplorer.SelectArcsTreeNode(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, finditems[index].path));
                        break;
                    case 1:
                        TDCGExplorer.TDCGExplorer.SelectZipsTreeNode(TDCGExplorer.TDCGExplorer.SystemDB.zips_path + "\\" + finditems[index].path);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public class FindEntryInformation
    {
        public int category;
        public string file;
        public string path;
    }

    // 探索スレッド.
    public class FindItemThread
    {
        internal FindItemPage control = null;
        internal string keyword = "";

        private static bool HasString(string target, string word)
        {
            if (word == "")
                return false;
            if (target.IndexOf(word) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        private bool flagtahlevelsearch = false;

        public bool TahLevelSearch
        {
            get { return flagtahlevelsearch; }
            set { flagtahlevelsearch = value; }
        }

        private bool flagzipsearch = false;

        public bool ZipSearch
        {
            get { return flagzipsearch; }
            set { flagzipsearch = value; }
        }

        public void Run()
        {
            TDCGExplorer.TDCGExplorer.IncBusy();
            ArcsDatabase arcDB = TDCGExplorer.TDCGExplorer.ArcsDB;
            // TAHを列挙する.
            List<ArcsTahEntry> tahs = arcDB.GetTahs();
            foreach (ArcsTahEntry tah in tahs)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips("検索中:" + tah.shortname);
                if (HasString(tah.path, keyword))
                {
                    FindEntryInformation entry = new FindEntryInformation();
                    entry.category = 0; // arcs
                    entry.path = String.Copy(tah.path);
                    entry.file = Path.GetFileName(tah.path);
                    control.asyncDisplayFromArcs(entry);
                }
            }

            if (flagtahlevelsearch)
            {
                List<ArcsTahFilesEntry> files = arcDB.GetTahFilesPathHasString(keyword);
                foreach (ArcsTahFilesEntry file in files)
                {
                    ArcsTahEntry tah = arcDB.GetTah(file.tahid);
                    FindEntryInformation entry = new FindEntryInformation();
                    entry.category = 0; // arcs
                    entry.path = String.Copy(tah.path);
                    entry.file = String.Copy(file.GetDisplayPath());
                    control.asyncDisplayFromArcs(entry);
                }
            }

            if (flagzipsearch)
            {
                List<ArcsZipArcEntry> zips = arcDB.GetZips();
                foreach (ArcsZipArcEntry zip in zips)
                {
                    TDCGExplorer.TDCGExplorer.SetToolTips("検索中:" + zip.GetDisplayPath());
                    if (HasString(zip.GetDisplayPath(), keyword))
                    {
                        FindEntryInformation entry = new FindEntryInformation();
                        entry.category = 1; //zips
                        entry.path = String.Copy(Path.GetDirectoryName(zip.path) + "\\" + zip.GetDisplayPath());
                        entry.file = zip.GetDisplayPath();
                        control.asyncDisplayFromArcs(entry);
                    }
                }
                //  LIKE文で総当たりする.
                List<ArcsZipTahEntry> ziptahs = arcDB.GetZipTahsHasString(keyword);
                foreach (ArcsZipTahEntry tah in ziptahs)
                {
                    ArcsZipArcEntry subzip = arcDB.GetZip(tah.zipid);
                    FindEntryInformation entry = new FindEntryInformation();
                    entry.category = 1; //zips
                    entry.path = String.Copy(Path.GetDirectoryName(subzip.path) + "\\" + subzip.GetDisplayPath());
                    entry.file = tah.shortname;
                    control.asyncDisplayFromArcs(entry);
                }
            }

            if (flagzipsearch && flagtahlevelsearch)
            {
                List<ArcsTahFilesEntry> files = arcDB.GetTahFilesPathHasString(keyword);
                foreach (ArcsTahFilesEntry file in files)
                {
                    ArcsZipTahEntry tah = arcDB.GetZipTah(file.tahid);
                    ArcsZipArcEntry zip = arcDB.GetZip(tah.zipid);
                    FindEntryInformation entry = new FindEntryInformation();
                    entry.category = 1; //zips
                    entry.path = String.Copy(zip.path + "\\" + zip.GetDisplayPath());
                    entry.file = String.Copy(file.GetDisplayPath());
                    control.asyncDisplayFromArcs(entry);
                }
            }

            // TBN辞書をセットして終了
            TDCGExplorer.TDCGExplorer.SetToolTips("検索完了。行をダブルクリックすると、そのファイルにジャンプします。");
            TDCGExplorer.TDCGExplorer.DecBusy();
        }
    }
}
