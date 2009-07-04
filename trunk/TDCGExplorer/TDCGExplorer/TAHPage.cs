using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArchiveLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;

namespace TDCGExplorer
{
    class TAHPage : TabPage
    {
        List<ArcsTahFilesEntry> filesEntries;
        private DataGridView dataGridView;
        private System.ComponentModel.IContainer components;
        GenTahInfo info;

        public TAHPage(GenTahInfo entryinfo, List<ArcsTahFilesEntry> filesentries)
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
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.ReadOnly = true;
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
            this.dataGridView.Size = new System.Drawing.Size(240, 150);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            // 
            // TAHPage
            // 
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    string ext = Path.GetExtension(filesEntries[index].path).ToLower();
                    if (ext == ".tso" || ext == ".tmo")
                    {
#if false
                        ArcsTahFilesEntry tsoInfo = filesEntries[index];
                        // zipファイルの中か?
                        if (info.zipid != -1)
                        {
                            IArchive archive;
                            ArcsZipArcEntry zip = TDCGExplorer.GetArcsDatabase().GetZip(info.zipid);
                            string zippath = Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, zip.path);
                            switch (Path.GetExtension(zip.path).ToLower())
                            {
                                case ".zip":
                                    archive = new ZipArchive();
                                    break;
                                case ".lzh":
                                    archive = new LzhArchive();
                                    break;
                                case ".rar":
                                    archive = new RarArchive();
                                    break;
                                default:
                                    MessageBox.Show("Not Implemented", "Not Implemented", MessageBoxButtons.OK);
                                    return;
                            }
                            archive.Open(zippath);
                            if (archive == null) return;

                            // 
                            foreach (IArchiveEntry entry in archive)
                            {
                                // ディレクトリのみの場合はスキップする.
                                if (entry.IsDirectory == true) continue;
                                // マッチするファイルを見つけた.
                                if (entry.FileName == info.path)
                                {
                                    using (MemoryStream ms = new MemoryStream((int)entry.Size))
                                    {
                                        archive.Extract(entry, ms);
                                        ms.Seek(0, SeekOrigin.Begin);
                                        TAHFile tah = new TAHFile(ms);
                                        tah.LoadEntries();
                                        int tahentry = 0;
                                        foreach (TAHEntry ent in tah.EntrySet.Entries)
                                        {
                                            // 該当ファイルを見つけた.
                                            if (tahentry == tsoInfo.tahentry)
                                            {
                                                byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                                                // 
                                                Cursor.Current = Cursors.WaitCursor;
                                                using (MemoryStream ims = new MemoryStream(data))
                                                {

                                                    TDCGExplorer.GetMainForm().makeTSOViwer();
                                                    if (ext == ".tso")
                                                    {
                                                        TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(ims);
                                                        TDCGExplorer.GetMainForm().doInitialTmoLoad(); // 初期tmoを読み込む.
                                                    }
                                                    else if (ext == ".tmo") TDCGExplorer.GetMainForm().Viewer.LoadTMOFile(ims);
                                                }
                                                Cursor.Current = Cursors.Default;
                                            }
                                            tahentry++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string source = Path.Combine(TDCGExplorer.GetSystemDatabase().arcs_path, info.path);
                            TAHFile tah = new TAHFile(source);
                            tah.LoadEntries();
                            int tahentry = 0;
                            foreach (TAHEntry ent in tah.EntrySet.Entries)
                            {
                                // 該当ファイルを見つけた.
                                if (tahentry == tsoInfo.tahentry)
                                {
                                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                                    // 
                                    using (MemoryStream ims = new MemoryStream(data))
                                    {
                                        Cursor.Current = Cursors.WaitCursor;
                                        TDCGExplorer.GetMainForm().makeTSOViwer();
                                        if (ext == ".tso")
                                        {
                                            TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(ims);
                                            TDCGExplorer.GetMainForm().doInitialTmoLoad(); // 初期tmoを読み込む.
                                        }
                                        else if (ext == ".tmo") TDCGExplorer.GetMainForm().Viewer.LoadTMOFile(ims);
                                        Cursor.Current = Cursors.Default;
                                    }
                                }
                                tahentry++;
                            }
                        }
#endif
                        using (TAHStream tahstream = new TAHStream(info, filesEntries[index]))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            TDCGExplorer.GetMainForm().makeTSOViwer();
                            if (ext == ".tso")
                            {
                                TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(tahstream.stream);
                                TDCGExplorer.GetMainForm().doInitialTmoLoad(); // 初期tmoを読み込む.
                            }
                            else if (ext == ".tmo") TDCGExplorer.GetMainForm().Viewer.LoadTMOFile(tahstream.stream);
                            Cursor.Current = Cursors.Default;
                        }
                    }
                    else
                    {
                        MessageBox.Show("TAH INFO:\n" + filesEntries[index].id + "," + filesEntries[index].path + "," + filesEntries[index].hash.ToString("x8"), "Not Implemented", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured:"+ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = Size;
        }
    }
}
