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
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemMakeTah;
        private ToolStripMenuItem toolStripMenuItemClose;
        internal Dictionary<string, List<string>> tbnfiles = null;
        
        public FindBaseModPage()
        {
            InitializeComponent();

            DataTable data = new DataTable();
            data.Columns.Add(TextResource.StrayTAH, Type.GetType("System.String"));
            data.Columns.Add(TextResource.StrayTBN, Type.GetType("System.String"));
            data.Columns.Add(TextResource.FoundStrayArchive, Type.GetType("System.String"));

            dataGridView.DataSource = data;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;

            Text = TextResource.FoundStrayTAH;

            // データの探索はバックグラウンドスレッドで非同期実行する.
            // 二重起動防止.
            FindMissingModThread fdb = new FindMissingModThread();
            fdb.control = this;
            Thread thread = new Thread(new ThreadStart(fdb.Run));
            thread.Start();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMakeTah = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItemMakeTah,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(219, 48);
            // 
            // toolStripMenuItemMakeTah
            // 
            this.toolStripMenuItemMakeTah.Name = "toolStripMenuItemMakeTah";
            this.toolStripMenuItemMakeTah.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItemMakeTah.Text = TextResource.MakeDummyTAH;
            this.toolStripMenuItemMakeTah.Click += new System.EventHandler(this.toolStripMenuItemMakeTah_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItemClose.Text = TextResource.Close;
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // FindBaseModPage
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
                    zipfile = TextResource.ArchiveNotFound;
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
#if false
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
#endif
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            Parent.Dispose();
        }

        private void toolStripMenuItemMakeTah_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            TAHEditor editor = null;
            try
            {
                SimpleTextDialog dialog = new SimpleTextDialog();
                dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
                dialog.dialogtext = TextResource.SaveTAHFile;
                dialog.labeltext = TextResource.Filename;
                dialog.textfield = "dummy.tah";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
#if false
                    // 新規TAHを作成する.
                    string dbfilename = LBFileTahUtl.GetTahDbPath(dialog.textfield);
                    string tahfilename = Path.GetFileNameWithoutExtension(dialog.textfield);

                    if (File.Exists(dbfilename))
                    {
                        MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                        return;
                    }
#endif
                    // 常に新規タブで.
                    editor = new TAHEditor(null);
                    editor.SetInformation(Path.GetFileNameWithoutExtension(dialog.textfield) + ".tah", 1);

                    // baseがないtahを全て反復する.
                    foreach (MissingEntryInformation missing in missings)
                    {
                        try
                        {
                            List<string> tbns = tbnfiles[missing.basetbn];
                            tbns.Sort(); // 順番を並び替える.
                            string tbnfile = tbns[0]; // 一番若いtbnファイル名を得る.
                            if (tbnfile.StartsWith("script/items/")) // 背景以外のtbnのみ処理する.
                            {
                                // tbnファイルを取得する
                                byte[] tbndata = getTahFile(tbnfile);
                                // data/model/N765BODY_A00.TSO
                                // 012345678901
                                string tsoname = TDCGTbnUtil.GetTsoName(tbndata).Substring(11);
                                string psdpath = "data/icon/items/" + tsoname.Substring(0, 12) + ".psd";
                                // psdファイルを取得する
                                byte[] psddata = getTahFile(psdpath);

                                // 新しい名前を付け替える.
                                //
                                // script/items/N765BODY_A00.tbn
                                // 12345678901234567890123456789
                                string newtbn = tbnfile.Substring(0, 23) + "00.tbn";
                                // data/icon/items/N765BODY_A00.tbn
                                // 12345678901234567890123456789
                                string newpsd = psdpath.Substring(0, 26) + "00.psd";

                                editor.AddItem(newtbn, tbndata);
                                if (psddata != null) editor.AddItem(newpsd, psddata);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    TDCGExplorer.TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                }
            }
            catch (Exception)
            {
                if (editor != null) editor.Dispose();
            }
        }
        // TAHからファイルを読み取る.
        private byte[] getTahFile(string file)
        {
            TDCGExplorer.ArcsDatabase arcDB = TDCGExplorer.TDCGExplorer.ArcsDB;
            string filename = file.ToLower();
            byte[] filedata = null;

            List<ArcsTahFilesEntry> tahs = arcDB.GetTahFilesEntry(TDCGExplorer.TAHUtil.CalcHash(filename));
            foreach (ArcsTahFilesEntry tahfile in tahs)
            {
                if (tahfile.path.ToLower() == filename)
                {
                    ArcsTahEntry arcs = arcDB.GetTah(tahfile.tahid);
                    using (Stream file_stream = File.OpenRead(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, arcs.path)))
                    {
                        TAHFile tah = new TAHFile(file_stream);
                        try
                        {
                            tah.LoadEntries();
                            foreach (TAHEntry ent in tah.EntrySet.Entries)
                            {
                                if (ent.FileName != null && ent.FileName.ToLower() == filename)
                                {
                                    filedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                }
            }
            return filedata;
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
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
        private Dictionary<string, List<string>> tbnfiles = new Dictionary<string, List<string>>();

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
                        TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.Searching + ":" + tah.shortname);
                        if (file.path.ToLower().StartsWith("script/items/") /*|| file.path.ToLower().StartsWith("script/backgrounds/" )*/)
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

                                // <<1.08.1
                                // コード毎のtbnファイル名を全て集める.
                                if (tbnfiles.ContainsKey(colbase) == false)
                                {
                                    tbnfiles.Add(colbase,new List<string>());
                                }
                                tbnfiles[colbase].Add(file.path);
                                // 1.08.1>>

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
                                        if (zipfile.path.ToLower() != colbase) continue; // ハッシュ衝突なら無視する.
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
            // TBN辞書をセットして終了
            control.tbnfiles = tbnfiles;
            TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.SearchComplete);
            TDCGExplorer.TDCGExplorer.DecBusy();
        }
    }
}
