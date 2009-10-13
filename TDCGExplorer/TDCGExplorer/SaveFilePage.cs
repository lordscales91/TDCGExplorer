using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCGExplorer;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace System.Windows.Forms
{
    class SaveFilePage : ZipFilePageControl
    {
        private TDCGSaveFileInfo savefile;
        private ContextMenuStrip contextMenuStripSaveData;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemMakeTah;
        private DataGridView dataGridView;
        private List<PNGTsoData> tsoDataList = new List<PNGTsoData>();
        private ToolStripMenuItem toolStripMenuItemHSave;
        private string filename;
        private ToolStripMenuItem toolStripMenuItemClose;
        private Bitmap savefilebitmap;

        // zipファイルの中から
        public SaveFilePage(GenericTahInfo tahInfo) : base(tahInfo)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                TDCGExplorer.TDCGExplorer.SetToolTips("データベース検索中...");
                ExtractFile();
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
            filename = Path.GetFileName(tahInfo.path);
        }
        // ファイルから直接読み出す.
        public SaveFilePage(string path)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                TDCGExplorer.TDCGExplorer.SetToolTips("データベース検索中...");
                Text = Path.GetFileName(path);
                TDCGExplorer.TDCGExplorer.SetLastAccessFile = path;
                using (FileStream fs = File.OpenRead(path))
                {
                    Byte[] buffer;
                    BinaryReader reader = new BinaryReader(fs, System.Text.Encoding.Default);
                    buffer = reader.ReadBytes((int)fs.Length);
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        BindingStream(ms);
                    }
                }
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
            filename = Path.GetFileName(path);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStripSaveData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMakeTah = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStripSaveData.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ContextMenuStrip = this.contextMenuStripSaveData;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStripSaveData
            // 
            this.contextMenuStripSaveData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMakeTah,
            this.toolStripMenuItemHSave,
            this.toolStripMenuItemClose});
            this.contextMenuStripSaveData.Name = "contextMenuStripSaveData";
            this.contextMenuStripSaveData.Size = new System.Drawing.Size(231, 70);
            // 
            // toolStripMenuItemMakeTah
            // 
            this.toolStripMenuItemMakeTah.Name = "toolStripMenuItemMakeTah";
            this.toolStripMenuItemMakeTah.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItemMakeTah.Text = "TAHファイルを作成する";
            this.toolStripMenuItemMakeTah.Click += new System.EventHandler(this.toolStripMenuItemMakeTah_Click);
            // 
            // toolStripMenuItemHSave
            // 
            this.toolStripMenuItemHSave.Name = "toolStripMenuItemHSave";
            this.toolStripMenuItemHSave.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItemHSave.Text = "ヘビーセーブ形式で保存する";
            this.toolStripMenuItemHSave.Click += new System.EventHandler(this.toolStripMenuItemHSave_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // SaveFilePage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStripSaveData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        public override void BindingStream(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            savefilebitmap = new Bitmap(ms);
            
            ms.Seek(0, SeekOrigin.Begin);
            savefile = new TDCGSaveFileInfo((Stream)ms);

            // ヘビーセーブとして読み込んでみる.
            PNGHSAVStream pngstream = new PNGHSAVStream();
            ms.Seek(0, SeekOrigin.Begin);
            pngstream.LoadPNGFile(ms);

            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = savefilebitmap;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = savefilebitmap.Width;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = savefilebitmap.Height;

            DataTable data = new DataTable();
            data.Columns.Add("パーツ", Type.GetType("System.String"));
            data.Columns.Add("属性", Type.GetType("System.String"));
            data.Columns.Add("TAHファイル", Type.GetType("System.String"));

            TDCGExplorer.TDCGExplorer.IncBusy();
            Application.DoEvents();
            TDCGExplorer.TDCGExplorer.DecBusy();

            // TSOビューワをリセットする
            TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
            TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();

            // ヘビーセーブか?
            if (pngstream.count > 0)
            {
                // ヘビーセーブデータをロードする.
                foreach (PNGTsoData tso in pngstream.get)
                {
                    try
                    {
                        if (TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer == null) TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(new MemoryStream(tso.tsodata));
                        TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.FrameMove();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.Render();

                        tsoDataList.Add(tso);

                        TDCGExplorer.TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.TDCGExplorer.DecBusy();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error: " + ex);
                    }
                }
            }

            for (int i = 0; i < TDCGSaveFileInfo.PARTS_SIZE; ++i)
            {
                string[] partfile = {savefile.GetPartsName(i),savefile.GetPartsFileName(i),""};
                // TAHファイルを検索する
                string partsname = savefile.GetPartsFileName(i);
                if (partsname.StartsWith("items/")==true)
                {
                    if(TDCGExplorer.TDCGExplorer.SystemDB.findziplevel==false){
                        partfile[2] = FindFromArcsTahs(partsname, i, pngstream.count > 0);
                    }
                    if (partfile[2] == "")
                    {
                        partfile[2] = FindFromZipTahs(partsname, i, pngstream.count > 0);
                    }
                    if (TDCGExplorer.TDCGExplorer.SystemDB.findziplevel == true && partfile[2] == "")
                    {
                        partfile[2] = FindFromArcsTahs(partsname, i, pngstream.count > 0);
                    }
                }

                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }
            for (int i = 0; i < TDCGSaveFileInfo.SLIDER_SIZE; ++i)
            {
                string[] partfile = {savefile.GetSliderName(i),savefile.GetSliderValue(i),""};
                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }

            dataGridView.DataSource = data;
        }

        private void DisplayTso(GenericTahInfo info, ArcsTahFilesEntry file,int id)
        {
            // tso名を取得する.
            string tsoname;
            using (GenericTAHStream tahstream = new GenericTAHStream(info, file))
            {
                using (MemoryStream memorystream = new MemoryStream())
                {
                    tahstream.stream.Seek(0, SeekOrigin.Begin);
                    ZipFileUtil.CopyStream(tahstream.stream, memorystream);
                    tsoname=TDCGTbnUtil.GetTsoName(memorystream.ToArray());
                }
            }

            GenericTahInfo tsoinfo = null;
            ArcsTahFilesEntry tso = null;
            if (info.zipid < 0)
            {// Arcsの場合
                int pastVersion=-1;
                ArcsTahEntry tahinfo = null;
                List<ArcsTahFilesEntry> tsos = TDCGExplorer.TDCGExplorer.ArcsDB.GetTahFilesEntry(TAHUtil.CalcHash(tsoname));
                foreach (ArcsTahFilesEntry subfile in tsos)
                {
                    ArcsTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(subfile.tahid);
                    if (subtah.version > pastVersion)
                    {
                        tso = subfile;
                        tahinfo = subtah;
                        pastVersion = subtah.version;
                    }
                }
                tsoinfo = new GenericArcsTahInfo(tahinfo);
            }else{// zipの場合
                int pastVersion = -1;
                ArcsZipTahEntry tahinfo = null;
                List<ArcsTahFilesEntry> tsos = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTahFilesEntries(TAHUtil.CalcHash(tsoname));
                foreach (ArcsTahFilesEntry subfile in tsos)
                {
                    ArcsZipTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTah(subfile.tahid);
                    if (subtah.version > pastVersion)
                    {
                        tso = subfile;
                        tahinfo = subtah;
                        pastVersion = subtah.version;
                    }
                }
                tsoinfo = new GenericZipsTahInfo(tahinfo);
            }

            if (tsoinfo != null && tso != null)
            {
                // TSOを読み込む
                using (GenericTAHStream tahstream = new GenericTAHStream(tsoinfo, tso))
                {
                    if (TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer == null) TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                    TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad();
                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.FrameMove();
                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.Render();

                    using (MemoryStream memorystream = new MemoryStream())
                    {
                        tahstream.stream.Seek(0, SeekOrigin.Begin);
                        ZipFileUtil.CopyStream(tahstream.stream, memorystream);
                        PNGTsoData tsodata = new PNGTsoData();
                        tsodata.tsoID = (uint)id;
                        tsodata.tsodata = memorystream.ToArray();
                        tsoDataList.Add(tsodata);
                    }

                    TDCGExplorer.TDCGExplorer.IncBusy();
                    Application.DoEvents();
                    TDCGExplorer.TDCGExplorer.DecBusy();
                }
            }
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = Size;
        }

        private void makeTAHFile()
        {
            try
            {
                SimpleTextDialog dialog = new SimpleTextDialog();
                dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
                dialog.dialogtext = "TAH形式の保存";
                dialog.labeltext = "ファイル名";
                dialog.textfield = filename;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 新規TAHを作成する.
                    string dbfilename = LBFileTahUtl.GetTahDbPath(dialog.textfield);
                    if (File.Exists(dbfilename))
                    {
                        MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                        return;
                    }

                    // 常に新規タブで.
                    TAHEditor editor = new TAHEditor(dbfilename, null);
                    editor.SetInformation(filename + ".tah", 1);
                    editor.makeTAHFile(filename, tsoDataList);
                    TDCGExplorer.TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                }
            }
            catch (Exception e)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips(e.Message);
            }
        }

        private void HeavySave()
        {
            try
            {
                // ヘビーセーブ形式で保存する.
                if (savefilebitmap == null) return;
                // まずPNG形式のデータを作る.
                MemoryStream basepng = new MemoryStream();
                savefilebitmap.Save(basepng, System.Drawing.Imaging.ImageFormat.Png);
                // PNGFileクラスにデータを取り込む.
                PNGHSAVStream pngstream = new PNGHSAVStream();
                basepng.Seek(0, SeekOrigin.Begin);
                PNGFile png = pngstream.GetPNG(basepng);
                //TSOデータを設定する.
                foreach (PNGTsoData tsodata in tsoDataList) pngstream.get.Add(tsodata);
                // 保存先を決める.
                string savefile_dir = TDCGExplorer.TDCGExplorer.SystemDB.savefile_directory;
                string savefile_name = Path.GetFileNameWithoutExtension(filename) + ".png";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = filename;
                dialog.InitialDirectory = TDCGExplorer.TDCGExplorer.SystemDB.savefile_directory;
                dialog.Filter = "PNGファイル(*.tdcgsav.png)|*.tdcgsav.png";
                dialog.FilterIndex = 0;
                dialog.Title = "保存先のファイルを選択してください";
                dialog.RestoreDirectory = true;
                dialog.OverwritePrompt = true;
                dialog.CheckPathExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string destpath = dialog.FileName;

                    // 保存先をオープン.
                    File.Delete(destpath);
                    using (Stream output = File.Create(destpath))
                    {
                        // PNGを出力する.
                        pngstream.SavePNGFile(png, output);
                    }
                    // ファイルを追加する.
                    TDCGExplorer.TDCGExplorer.AddFileTree(destpath);
                }
            }
            catch (Exception ex)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips("ファイルセーブエラー:" + ex.Message);
            }
        }

        private void toolStripMenuItemHSave_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;
            HeavySave();
            //TDCGExplorer.TDCGExplorer.MainFormWindow.UpdateSaveFileTree();
        }

        private void toolStripMenuItemMakeTah_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest() == true) return;
            makeTAHFile();
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            Parent.Dispose();
        }

        private string FindFromArcsTahs(string filename, int id, bool heavysave)
        {
            string retval="";

            List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetTahFilesEntry(TAHUtil.CalcHash("script/" + filename + ".tbn"));
            if (files.Count > 0)
            {
                ArcsTahFilesEntry file = null;
                ArcsTahEntry tah = null;
                int pastVersion = -1;
                foreach (ArcsTahFilesEntry subfile in files)
                {
                    ArcsTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(subfile.tahid);
                    if (subtah.version > pastVersion)
                    {
                        file = subfile;
                        tah = subtah;
                        pastVersion = subtah.version;
                    }
                    TDCGExplorer.TDCGExplorer.IncBusy();
                    Application.DoEvents();
                    TDCGExplorer.TDCGExplorer.DecBusy();

                }
                if (tah != null)
                {
                    retval = tah.path;
                    if (heavysave == false) // 通常セーブなら
                    {
                        try
                        {
                            DisplayTso(new GenericArcsTahInfo(tah), file, id);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error: " + ex);
                        }
                    }
                }
            }
            TDCGExplorer.TDCGExplorer.FigureLoad = true;
            return retval;
        }

        private string FindFromZipTahs(string filename,int id,bool heavysave)
        {
            string retval = "";

            List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTahFilesEntries(TAHUtil.CalcHash("script/" + filename + ".tbn"));
            if (files.Count > 0)
            {
                ArcsTahFilesEntry file = null;
                ArcsZipTahEntry tah = null;
                int pastVersion = -1;
                foreach (ArcsTahFilesEntry subfile in files)
                {
                    ArcsZipTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTah(subfile.tahid);
                    if (subtah.version > pastVersion)
                    {
                        file = subfile;
                        tah = subtah;
                        pastVersion = subtah.version;
                    }
                    TDCGExplorer.TDCGExplorer.IncBusy();
                    Application.DoEvents();
                    TDCGExplorer.TDCGExplorer.DecBusy();
                }
                if (tah != null)
                {
                    ArcsZipArcEntry zip = TDCGExplorer.TDCGExplorer.ArcsDB.GetZip(tah.zipid);
                    if (zip != null)
                    {
                        retval = zip.path + "\\" + tah.path;

                        if (heavysave == false)// 通常セーブなら
                        {
                            try
                            {
                                DisplayTso(new GenericZipsTahInfo(tah), file, id);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error: " + ex);
                            }
                        }
                    }
                }
            }
            return retval;
        }
    }
}
