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
    class FindBaseModPage : Control
    {
        private bool firstdata = true;
        private DataGridView dataGridView;
        List<MissingEntryInformation> missings = new List<MissingEntryInformation>();
    
        public FindBaseModPage()
        {
            InitializeComponent();

            DataTable data = new DataTable();
            data.Columns.Add("前提TAHが無いTAH名", Type.GetType("System.String"));
            data.Columns.Add("不明なベースTBN名", Type.GetType("System.String"));
            data.Columns.Add("検索された前提アーカイブファイル", Type.GetType("System.String"));

            dataGridView.DataSource = data;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;

            Text = "前提TAH検索";

            // データの探索はバックグラウンドスレッドで非同期実行する.
            // 二重起動防止.
            FindMissingModThread fdb = new FindMissingModThread();
            fdb.control = this;
            Thread thread = new Thread(new ThreadStart(fdb.Run));
            thread.Start();
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
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(240, 150);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // FindBaseModPage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        // invokeの為のdelegate
        private delegate void AddItemFromBGThread(MissingEntryInformation entry);

        // 非同期で呼び出されるメソッド
        private void AddItem(MissingEntryInformation entry)
        {
            missings.Add(entry);
            DataTable data = dataGridView.DataSource as DataTable;
            if (data != null)
            {
                DataRow row = data.NewRow();
                string zipfile="";
                if (entry.zipfiles == null)
                {
                    zipfile = "アーカイブが見つかりません";
                }
                else
                {
                    bool firstzip = true;
                    foreach (string file in entry.zipfiles)
                    {
                        if (firstzip == false)
                        {
                            zipfile += "\r\n";
                        }
                        else
                        {
                            firstzip = false;
                        }
                        zipfile += file;
                    }
                }
                string[] content = { Path.GetFileName(entry.path), entry.basetbn, zipfile };
                row.ItemArray = content;
                data.Rows.Add(row);
            }
            if (firstdata)
            {
                firstdata = false;
                dataGridView.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
        }

        // 非同期でツリー表示を更新する.
        public void asyncDisplayFromArcs(MissingEntryInformation entry)
        {
            Invoke(new AddItemFromBGThread(AddItem),entry);
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

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView.CurrentCell.RowIndex;
            if (index >= 0)
            {
                // TAHを取得する.
                ArcsTahEntry tah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(missings[index].id);
                TDCGExplorer.TDCGExplorer.SelectArcsTreeNode(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, tah.path));
            }
        }
    }

    public class MissingEntryInformation
    {
        public int id;
        public string path;
        public string basetbn;
        public List<string> zipfiles;
    }

    // 探索スレッド.
    public class FindMissingModThread
    {
        internal FindBaseModPage control;
        public void Run()
        {
            TDCGExplorer.TDCGExplorer.IncBusy();
            ArcsDatabase arcDB = TDCGExplorer.TDCGExplorer.ArcsDB;
            // TAHを列挙する.
            List<ArcsTahEntry> tahs = arcDB.GetTahs();
            foreach (ArcsTahEntry tah in tahs)
            {
                Dictionary<string, ArcsTahEntry> tbnmap = new Dictionary<string, ArcsTahEntry>();
                List<ArcsTahFilesEntry> files = arcDB.GetTahFilesPath(tah.id);
                foreach (ArcsTahFilesEntry file in files)
                {
                    try
                    {
                        TDCGExplorer.TDCGExplorer.SetToolTips("検索中:" + tah.shortname);
                        if (file.path.ToLower().StartsWith("script/items/") || file.path.ToLower().StartsWith("script/backgrounds/"))
                        {
                            // TBNファイルか?
                            if (file.path.ToLower().EndsWith(".tbn") == true)
                            {
                                // N765BODY_A00.TBN
                                // 1234567890123456
                                string directory = Path.GetDirectoryName(file.path).ToLower().Replace('\\', '/');
                                string fullname = Path.GetFileNameWithoutExtension(file.path).ToLower();
                                string basename = fullname.Substring(0, 10);
                                string colbase = directory + "/" + basename + "00.tbn";

                                if (tbnmap.ContainsKey(colbase) == true) continue; // 既に該当tbnの情報を見つけている.
                                // baseとなるtbnそのものだった場合.
                                if (file.path.ToLower() == colbase)
                                {
                                    tbnmap.Add(colbase, tah);
                                    continue;
                                }
                                // カテゴリ先頭のtbnファイルを検索する.
                                List<ArcsTahFilesEntry> tbns = arcDB.GetTahFilesEntry(TDCGExplorer.TAHUtil.CalcHash(colbase));
                                if (tbns.Count == 0)
                                {
                                    // 該当するtbnの00番が見つからない.
                                    MissingEntryInformation entry = new MissingEntryInformation();
                                    entry.id = tah.id;
                                    entry.path = String.Copy(tah.path);
                                    entry.basetbn = String.Copy(colbase);
                                    entry.zipfiles = null;
                                    // 該当tbnを含むzipを検索する.
                                    HashSet<string> zipname = new HashSet<string>();
                                    List<ArcsTahFilesEntry> zipfiles = arcDB.GetZipTahFilesEntries(TDCGExplorer.TAHUtil.CalcHash(colbase));
                                    foreach (ArcsTahFilesEntry zipfile in zipfiles)
                                    {
                                        ArcsZipTahEntry ziptah = arcDB.GetZipTah(zipfile.tahid);
                                        ArcsZipArcEntry zip = arcDB.GetZip(ziptah.zipid);
                                        zipname.Add(zip.GetDisplayPath());
                                    }
                                    // 発見したZIPの数だけ報告する.0
                                    if (zipname.Count == 0)
                                    {
                                        control.asyncDisplayFromArcs(entry);
                                    }
                                    else
                                    {
                                        entry.zipfiles = new List<string>();
                                        foreach (string zip in zipname)
                                        {
                                            entry.zipfiles.Add(String.Copy(zip));
                                        }
                                        control.asyncDisplayFromArcs(entry);
                                    }
                                }
                                tbnmap.Add(colbase, tah);
                            }
                        }
                    }
                    // おかしなファイル名でindex違反が起きるかもしれない.
                    catch (Exception)
                    {
                    }
                }
            }
            TDCGExplorer.TDCGExplorer.SetToolTips("検索完了");
            TDCGExplorer.TDCGExplorer.DecBusy();
        }
    }
}
