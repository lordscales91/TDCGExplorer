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
        TDCGSaveFileInfo savefile;
        private ContextMenuStrip contextMenuStripSaveData;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemMakeTah;
        private DataGridView dataGridView;
        private List<PNGTsoData> tsoDataList = new List<PNGTsoData>();
        private string filename;

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
                FileStream fs = File.OpenRead(path);
                Byte[] buffer;
                BinaryReader reader = new BinaryReader(fs, System.Text.Encoding.Default);
                buffer = reader.ReadBytes((int)fs.Length);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    BindingStream(ms);
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
            this.toolStripMenuItemMakeTah});
            this.contextMenuStripSaveData.Name = "contextMenuStripSaveData";
            this.contextMenuStripSaveData.Size = new System.Drawing.Size(207, 26);
            this.contextMenuStripSaveData.Click += new System.EventHandler(this.contextMenuStripSaveData_Click);
            // 
            // toolStripMenuItemMakeTah
            // 
            this.toolStripMenuItemMakeTah.Name = "toolStripMenuItemMakeTah";
            this.toolStripMenuItemMakeTah.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemMakeTah.Text = "TAHファイルを作成する";
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
            savefile = new TDCGSaveFileInfo((Stream)ms);

            // ヘビーセーブとして読み込んでみる.
            PNGStream pngstream = new PNGStream();
            ms.Seek(0, SeekOrigin.Begin);
            pngstream.LoadPNGFile(ms);

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
                        // まずarcsからさがしてみる.
                        List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetTahFilesEntry(TAHUtil.CalcHash("data/model/" + partsname.Substring(6) + ".tso"));
                        if (files.Count > 0)
                        {
                            ArcsTahFilesEntry file = null;
                            ArcsTahEntry tah = null;
                            int pastVersion = -1;
                            foreach (ArcsTahFilesEntry subfile in files)
                            {
                                ArcsTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(subfile.tahid);
                                if (subtah.version >= pastVersion)
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
                                partfile[2] = tah.path;
                                if (pngstream.count == 0) // 通常セーブなら
                                {
                                    try
                                    {
                                        DisplayTso(new GenericArcsTahInfo(tah), file,i);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error: " + ex);
                                    }
                                }
                            }
                        }
                    }
                    // arcsに無かったら全zipから探す.
                    if (partfile[2] == "")
                    {
                        List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTahFilesEntries(TAHUtil.CalcHash("data/model/" + partsname.Substring(6) + ".tso"));
                        if (files.Count>0)
                        {
                            ArcsTahFilesEntry file = null;
                            ArcsZipTahEntry tah = null;
                            int pastVersion = -1;
                            foreach (ArcsTahFilesEntry subfile in files)
                            {
                                ArcsZipTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTah(subfile.tahid);
                                if (subtah.version >= pastVersion)
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
                                    partfile[2] = zip.path + "\\" + tah.path;

                                    if (pngstream.count == 0)// 通常セーブなら
                                    { 
                                        try
                                        {
                                            DisplayTso(new GenericZipsTahInfo(tah), file,i);
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("Error: " + ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (TDCGExplorer.TDCGExplorer.SystemDB.findziplevel == true && partfile[2] == "")
                    {
                        // まずarcsからさがしてみる.
                        List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetTahFilesEntry(TAHUtil.CalcHash("data/model/" + partsname.Substring(6) + ".tso"));
                        if (files.Count > 0)
                        {
                            ArcsTahFilesEntry file = null;
                            ArcsTahEntry tah = null;
                            int pastVersion = -1;
                            foreach (ArcsTahFilesEntry subfile in files)
                            {
                                ArcsTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(subfile.tahid);
                                if (subtah.version >= pastVersion)
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
                                partfile[2] = tah.path;
                                if (pngstream.count == 0) // 通常セーブなら
                                {
                                    try
                                    {
                                        DisplayTso(new GenericArcsTahInfo(tah), file,i);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error: " + ex);
                                    }
                                }
                            }
                        }
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
            // TSOを読み込む
            using (GenericTAHStream tahstream = new GenericTAHStream(info, file))
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
                Application.DoEvents();
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

        // セーブデータからTAHを作成する.
        private void contextMenuStripSaveData_Click(object sender, EventArgs e)
        {
            string dbfilename = LBFileTahUtl.GetTahDbPath(filename);
            if (File.Exists(dbfilename))
            {
                MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                return;
            }

            string[] tbnname = {    "script/items/n001body_a00.tbn",
                                    "script/items/n001fhea_b00.tbn",
                                    "script/items/n001bhea_c00.tbn",
                                    "script/items/n001hskn_d00.tbn",
                                    "script/items/n001eyes_e00.tbn",
                                    "script/items/n001bura_f00.tbn",
                                    "script/items/n002scho_g00.tbn",
                                    "script/items/n001pant_h00.tbn",
                                    "script/items/n001hsox_i00.tbn",
                                    "script/items/n003sail_j00.tbn",
                                    "script/items/n001nurs_k00.tbn",
                                    "script/items/n001nksk_l00.tbn",
                                    "script/items/n001sail_m00.tbn",
                                    "script/items/n001tail_n00.tbn",
                                    "script/items/n001rofr_o00.tbn",
                                    "script/items/n001nek1_p00.tbn",
                                    "script/items/n001mega_q00.tbn",
                                    "script/items/n001neck_r00.tbn",
                                    "script/items/n004cffs_s00.tbn",
                                    "script/items/n001wing_t00.tbn",
                                    "script/items/n001ahog_u00.tbn",
                                    "script/items/n001gant_v00.tbn",
                                    "script/items/n001loos_w00.tbn",
                                    "script/items/n001cffs_x00.tbn",
                                    "script/items/n001head_y00.tbn",
                                    "script/items/n001obon_z00.tbn",
                                    "script/items/n001mayu_000.tbn",
                                    "script/items/n001kiba_100.tbn",
                                    "script/items/n001hoku_200.tbn",
                                    "script/items/n001head_300.tbn" };

            string[] newtbnname = { "script/items/N999SAVE_A00.tbn",
                                    "script/items/N999SAVE_B00.tbn",
                                    "script/items/N999SAVE_C00.tbn",
                                    "script/items/N999SAVE_D00.tbn",
                                    "script/items/N999SAVE_E00.tbn",
                                    "script/items/N999SAVE_F00.tbn",
                                    "script/items/N999SAVE_G00.tbn",
                                    "script/items/N999SAVE_H00.tbn",
                                    "script/items/N999SAVE_I00.tbn",
                                    "script/items/N999SAVE_J00.tbn",
                                    "script/items/N999SAVE_K00.tbn",
                                    "script/items/N999SAVE_L00.tbn",
                                    "script/items/N999SAVE_M00.tbn",
                                    "script/items/N999SAVE_N00.tbn",
                                    "script/items/N999SAVE_O00.tbn",
                                    "script/items/N999SAVE_P00.tbn",
                                    "script/items/N999SAVE_Q00.tbn",
                                    "script/items/N999SAVE_R00.tbn",
                                    "script/items/N999SAVE_S00.tbn",
                                    "script/items/N999SAVE_T00.tbn",
                                    "script/items/N999SAVE_U00.tbn",
                                    "script/items/N999SAVE_V00.tbn",
                                    "script/items/N999SAVE_W00.tbn",
                                    "script/items/N999SAVE_X00.tbn",
                                    "script/items/N999SAVE_Y00.tbn",
                                    "script/items/N999SAVE_Z00.tbn",
                                    "script/items/N999SAVE_000.tbn",
                                    "script/items/N999SAVE_100.tbn",
                                    "script/items/N999SAVE_200.tbn",
                                    "script/items/N999SAVE_300.tbn" };

            string[] newtsoname = { "data/model/N999SAVE_A00.tso",
                                    "data/model/N999SAVE_B00.tso",
                                    "data/model/N999SAVE_C00.tso",
                                    "data/model/N999SAVE_D00.tso",
                                    "data/model/N999SAVE_E00.tso",
                                    "data/model/N999SAVE_F00.tso",
                                    "data/model/N999SAVE_G00.tso",
                                    "data/model/N999SAVE_H00.tso",
                                    "data/model/N999SAVE_I00.tso",
                                    "data/model/N999SAVE_J00.tso",
                                    "data/model/N999SAVE_K00.tso",
                                    "data/model/N999SAVE_L00.tso",
                                    "data/model/N999SAVE_M00.tso",
                                    "data/model/N999SAVE_N00.tso",
                                    "data/model/N999SAVE_O00.tso",
                                    "data/model/N999SAVE_P00.tso",
                                    "data/model/N999SAVE_Q00.tso",
                                    "data/model/N999SAVE_R00.tso",
                                    "data/model/N999SAVE_S00.tso",
                                    "data/model/N999SAVE_T00.tso",
                                    "data/model/N999SAVE_U00.tso",
                                    "data/model/N999SAVE_V00.tso",
                                    "data/model/N999SAVE_W00.tso",
                                    "data/model/N999SAVE_X00.tso",
                                    "data/model/N999SAVE_Y00.tso",
                                    "data/model/N999SAVE_Z00.tso",
                                    "data/model/N999SAVE_000.tso",
                                    "data/model/N999SAVE_100.tso",
                                    "data/model/N999SAVE_200.tso",
                                    "data/model/N999SAVE_300.tso" };

            string[] newpsdname = { "data/icon/items/N999SAVE_A00.psd",
                                    "data/icon/items/N999SAVE_B00.psd",
                                    "data/icon/items/N999SAVE_C00.psd",
                                    "data/icon/items/N999SAVE_D00.psd",
                                    "data/icon/items/N999SAVE_E00.psd",
                                    "data/icon/items/N999SAVE_F00.psd",
                                    "data/icon/items/N999SAVE_G00.psd",
                                    "data/icon/items/N999SAVE_H00.psd",
                                    "data/icon/items/N999SAVE_I00.psd",
                                    "data/icon/items/N999SAVE_J00.psd",
                                    "data/icon/items/N999SAVE_K00.psd",
                                    "data/icon/items/N999SAVE_L00.psd",
                                    "data/icon/items/N999SAVE_M00.psd",
                                    "data/icon/items/N999SAVE_N00.psd",
                                    "data/icon/items/N999SAVE_O00.psd",
                                    "data/icon/items/N999SAVE_P00.psd",
                                    "data/icon/items/N999SAVE_Q00.psd",
                                    "data/icon/items/N999SAVE_R00.psd",
                                    "data/icon/items/N999SAVE_S00.psd",
                                    "data/icon/items/N999SAVE_T00.psd",
                                    "data/icon/items/N999SAVE_U00.psd",
                                    "data/icon/items/N999SAVE_V00.psd",
                                    "data/icon/items/N999SAVE_W00.psd",
                                    "data/icon/items/N999SAVE_X00.psd",
                                    "data/icon/items/N999SAVE_Y00.psd",
                                    "data/icon/items/N999SAVE_Z00.psd",
                                    "data/icon/items/N999SAVE_000.psd",
                                    "data/icon/items/N999SAVE_100.psd",
                                    "data/icon/items/N999SAVE_200.psd",
                                    "data/icon/items/N999SAVE_300.psd" };

            // base.tahから手持ちアイテム以外のファイルを読み込む.
            Dictionary<uint, byte[]> tbndata = new Dictionary<uint,byte[]>();
            using (Stream file_stream = File.OpenRead(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, "base.tah")))
            {
                TAHFile tah = new TAHFile(file_stream);
                try
                {
                    tah.LoadEntries();
                    foreach (TAHEntry ent in tah.EntrySet.Entries)
                    {
                        for(int id=0;id<tbnname.Length;id++){
                            if (ent.FileName != null && ent.FileName.ToLower() == tbnname[id])
                            {
                                byte[] content = TAHUtil.ReadEntryData(tah.Reader, ent);
                                TDCGTbnUtil.SetTsoName(content, newtsoname[id]);
                                tbndata.Add((uint)id, content);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("base.tahの読み込みでエラーが発生しました。", "エラー", MessageBoxButtons.OK);
                    Debug.WriteLine("basetah.open.error");
                }
            }
           
            // 手持ちアイテムTBNを読み込む.
            try
            {
                using (Stream stream = File.OpenRead("N001OBON_Z00.tbn"))
                using (MemoryStream memorystream = new MemoryStream())
                {
                    ZipFileUtil.CopyStream(stream, memorystream);
                    byte[] content = memorystream.ToArray();
                    TDCGTbnUtil.SetTsoName(content, newtsoname[25]);
                    tbndata.Add(25, content);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("tbnの読み込みでエラーが発生しました。", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine("tbn.open.error");
            }

            byte[] icondata = null;

            // アイコンを読み込む
            try
            {
                using (Stream stream = File.OpenRead("N999SAVE_A00.PSD"))
                using (MemoryStream memorystream = new MemoryStream())
                {
                    ZipFileUtil.CopyStream(stream, memorystream);
                    icondata = memorystream.ToArray();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("PSDの読み込みでエラーが発生しました。", "エラー", MessageBoxButtons.OK);
                Debug.WriteLine("PSD.open.error");
            }

            // 新規TAHを作成する.
            // 常に新規タブで.
            TAHEditor editor = new TAHEditor(dbfilename, null);
            editor.SetInformation(filename + ".tah", 1);
            Object transaction = editor.BeginTransaction();
            foreach (PNGTsoData data in tsoDataList)
            {
                editor.AddItem(newtbnname[data.tsoID], tbndata[data.tsoID]);
                editor.AddItem(newpsdname[data.tsoID], icondata);
                editor.AddItem(newtsoname[data.tsoID], data.tsodata);
            }
            editor.Commit(transaction);
            TDCGExplorer.TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
        }
    }
}
