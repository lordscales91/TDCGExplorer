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
        private ToolStripMenuItem toolStripMenuItemShowModel;
        private Bitmap savefilebitmap;
        List<LoadTsoInfo> loadtsoinfo = new List<LoadTsoInfo>();
        private PNGHSAVStream pngstream = new PNGHSAVStream();

        private bool fDisplayed = false;
        private MemoryStream savedata = null;

        // zipファイルの中から
        public SaveFilePage(GenericTahInfo tahInfo) : base(tahInfo)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                ExtractFile();
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text+" : 行をダブルクリックするとファイルにジャンプします。");
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
                Text = Path.GetFileName(path);
                TDCGExplorer.TDCGExplorer.LastAccessFile = path;
                using (FileStream fs = File.OpenRead(path))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ZipFileUtil.CopyStream(fs, ms);
                        BindingStream(ms);
                        ms.Close();
                    }
                    fs.Close();
                }
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text + " : 行をダブルクリックするとファイルにジャンプします。");
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Cursor.Current = Cursors.Default;
            filename = Path.GetFileName(path);
        }

        protected override void Dispose(bool disposing)
        {
            if (savedata != null)
            {
                savedata.Close();
                savedata.Dispose();
                savedata = null;
            }
            if (savefilebitmap != null)
            {
                savefilebitmap.Dispose();
                savefilebitmap = null;
            }
            if (savefile != null)
            {
                savefile.Dispose();
                savefile = null;
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStripSaveData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMakeTah = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShowModel = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataGridView.DoubleClick += new System.EventHandler(this.dataGridView_DoubleClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStripSaveData
            // 
            this.contextMenuStripSaveData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMakeTah,
            this.toolStripMenuItemHSave,
            this.toolStripMenuItemShowModel,
            this.toolStripMenuItemClose});
            this.contextMenuStripSaveData.Name = "contextMenuStripSaveData";
            this.contextMenuStripSaveData.Size = new System.Drawing.Size(231, 92);
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
            // toolStripMenuItemShowModel
            // 
            this.toolStripMenuItemShowModel.Name = "toolStripMenuItemShowModel";
            this.toolStripMenuItemShowModel.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItemShowModel.Text = "TSOビューに表示する";
            this.toolStripMenuItemShowModel.Click += new System.EventHandler(this.toolStripMenuItemShowModel_Click);
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

            // セーブデータのコピーを保持する
            savedata = new MemoryStream();
            ZipFileUtil.CopyStream(ms, savedata);

            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = savefilebitmap;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = savefilebitmap.Width;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = savefilebitmap.Height;

            DataTable data = new DataTable();
            data.Columns.Add("パーツ", Type.GetType("System.String"));
            data.Columns.Add("属性", Type.GetType("System.String"));
            data.Columns.Add("TAHファイル", Type.GetType("System.String"));

            for (int i = 0; i < TDCGSaveFileStatic.PARTS_SIZE; ++i)
            {
                string[] partfile = {savefile.GetPartsName(i),savefile.GetPartsFileName(i),""};
                // TAHファイルを検索する
                string partsname = savefile.GetPartsFileName(i);
                if (partsname.StartsWith("items/")==true)
                {
                    if(TDCGExplorer.TDCGExplorer.SystemDB.findziplevel==false){
                        string partname = FindFromArcsTahs(partsname, i);
                        if (partname != "")
                        {
                            partfile[2] = "arcs : " + partname;
                        }
                    }
                    if (partfile[2] == "")
                    {
                        string partname = FindFromZipTahs(partsname, i);
                        if (partname != "")
                        {
                            partfile[2] = "zips : " + partname;
                        }
                    }
                    if (TDCGExplorer.TDCGExplorer.SystemDB.findziplevel == true && partfile[2] == "")
                    {
                        string partname = FindFromArcsTahs(partsname, i);
                        if (partname != "")
                        {
                            partfile[2] = "arcs : " + partname;
                        }
                    }
                }

                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }
            for (int i = 0; i < TDCGSaveFileStatic.SLIDER_SIZE; ++i)
            {
                string[] partfile = {savefile.GetSliderName(i),savefile.GetSliderValue(i),""};
                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }

            dataGridView.DataSource = data;
        }

        private void LoadTso(GenericTahInfo info, ArcsTahFilesEntry file, int id)
        {
            LoadTsoInfo tsoinfo = new LoadTsoInfo(info,file,id);
            loadtsoinfo.Add(tsoinfo);
        }

        private void assembleTsoData()
        {
            foreach (LoadTsoInfo tsoload in loadtsoinfo)
            {
                GenericTahInfo info = tsoload.info;
                ArcsTahFilesEntry file = tsoload.file;
                int id = tsoload.id;
                try
                {
                    // tso名を取得する.
                    string tsoname;
                    using (GenericTAHStream tahstream = new GenericTAHStream(info, file))
                    {
                        using (MemoryStream memorystream = new MemoryStream())
                        {
                            ZipFileUtil.CopyStream(tahstream.stream, memorystream);
                            tsoname = TDCGTbnUtil.GetTsoName(memorystream.ToArray());
                        }
                    }

                    GenericTahInfo tsoinfo = null;
                    ArcsTahFilesEntry tso = null;
                    if (info.zipid < 0)
                    {// Arcsの場合
                        int pastVersion = -1;
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
                    }
                    else
                    {// zipの場合
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
                            using (MemoryStream memorystream = new MemoryStream())
                            {
                                ZipFileUtil.CopyStream(tahstream.stream, memorystream);
                                PNGTsoData tsodata = new PNGTsoData();
                                tsodata.tsoID = (uint)id;
                                tsodata.tsodata = memorystream.ToArray();
                                tsoDataList.Add(tsodata);
                            }
                        }
                    }
                }
                catch (Exception)
                {
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
            TAHEditor editor = null;
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
                    string tahfilename = Path.GetFileNameWithoutExtension(dialog.textfield);
                    if (File.Exists(dbfilename))
                    {
                        MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                        return;
                    }

                    // TSOデータを読み込んで組み立てる.
                    if (fDisplayed == false) DisplayTso();

                    // 常に新規タブで.
                    editor = new TAHEditor(dbfilename, null);
                    editor.SetInformation(tahfilename + ".tah", 1);
                    editor.makeTAHFile(tahfilename, tsoDataList);
                    TDCGExplorer.TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                }
            }
            catch (Exception e)
            {
                TDCGExplorer.TDCGExplorer.SetToolTips(e.Message);
                if (editor != null) editor.Dispose();
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

                // TSOデータを読み込んで組み立てる.
                if (fDisplayed == false) DisplayTso();

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

        private string FindFromArcsTahs(string filename, int id)
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
                }
                if (tah != null)
                {
                    retval = tah.path;
                    LoadTso(new GenericArcsTahInfo(tah), file, id);
                }
            }
            TDCGExplorer.TDCGExplorer.FigureLoad = true;
            return retval;
        }

        private string FindFromZipTahs(string filename,int id)
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
                }
                if (tah != null)
                {
                    ArcsZipArcEntry zip = TDCGExplorer.TDCGExplorer.ArcsDB.GetZip(tah.zipid);
                    if (zip != null)
                    {
                        retval = Path.GetDirectoryName(zip.path)+"\\"+zip.GetDisplayPath() + "\\" + tah.path;
                        LoadTso(new GenericZipsTahInfo(tah), file, id);
                    }
                }
            }
            return retval;
        }

        private void toolStripMenuItemShowModel_Click(object sender, EventArgs e)
        {
            DisplayTso();
        }

        public void DisplayTso()
        {
            if (fDisplayed) return;
            fDisplayed = true;

            // ヘビーセーブとして読み込んでみる.
            pngstream = new PNGHSAVStream();
            savedata.Seek(0, SeekOrigin.Begin);
            pngstream.LoadPNGFile(savedata);

            // ヘビーセーブか?
            if (pngstream.count > 0)
            {
                // ヘビーセーブデータをロードする.
                foreach (PNGTsoData tso in pngstream.get)
                {
                    tsoDataList.Add(tso);
                }
            }
            else
            {
                // tsoロード情報があるならこの時点で組み立てる.
                if (loadtsoinfo.Count != 0) assembleTsoData();
            }

            TDCGExplorer.TDCGExplorer.SetToolTips("描画中...");

            try
            {
                // TSOビューワをリセットする
                TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();

                foreach (PNGTsoData tso in tsoDataList)
                {
                    using (MemoryStream stream = new MemoryStream(tso.tsodata))
                    {
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(stream);
                        TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.FrameMove();
                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.Render();

                        TDCGExplorer.TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.TDCGExplorer.DecBusy();
                    }
                }
            }
            catch (Exception)
            {
            }
            TDCGExplorer.TDCGExplorer.SetToolTips("描画完了...");
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            //DisplayTso();
            foreach (DataGridViewRow viewrow in dataGridView.SelectedRows)
            {
                DataRowView vrow = viewrow.DataBoundItem as DataRowView;
                DataRow row = null;
                if (vrow != null)
                {
                    row = vrow.Row;
                    if (row != null)
                    {
                        Object[] entry = row.ItemArray;
                        if (entry != null)
                        {
                            string pathfield = entry[2].ToString();
                            if (pathfield.StartsWith("arcs : "))
                            {
                                TDCGExplorer.TDCGExplorer.SelectArcsTreeNode(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, pathfield.Substring(7)));
                            }
                            else if (pathfield.StartsWith("zips : "))
                            {
                                // tah名はzippathに含まれていないので、ちょっと工夫が必要.
                                string zipfullpath = pathfield.Substring(7);
                                string zippath = "";
                                // tah名はツリー階層にないので該当ファイルをzipから検索する.
                                List<ArcsZipArcEntry> zips = TDCGExplorer.TDCGExplorer.ArcsDB.GetZips();
                                int len = 0;
                                foreach (ArcsZipArcEntry zip in zips)
                                {
                                    string ziplocation = Path.GetDirectoryName(zip.path) + "\\" + zip.GetDisplayPath();
                                    if (zipfullpath.StartsWith(ziplocation))
                                    {
                                        if (len < ziplocation.Length)
                                        {//より長いものがマッチしたら
                                            len = ziplocation.Length;
                                            zippath = ziplocation;
                                        }
                                    }
                                }
                                TDCGExplorer.TDCGExplorer.SelectZipsTreeNode(TDCGExplorer.TDCGExplorer.SystemDB.zips_path + "\\" + zippath);
                            }
                        }
                    }
                }
            }
        }
    }

    public class LoadTsoInfo
    {
        public GenericTahInfo info;
        public ArcsTahFilesEntry file;
        public int id;
        public LoadTsoInfo(GenericTahInfo itinfo, ArcsTahFilesEntry itfile, int itid)
        {
            info = itinfo;
            file = itfile;
            id = itid;
        }
    }
}
