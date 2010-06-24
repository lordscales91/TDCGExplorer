using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;
using TDCGExplorer;
using System.Data.SQLite;
using System.Media;

namespace System.Windows.Forms
{
    class TAHEditor : Control
    {
        private DataGridView dataGridView;
        private TAHLocalDB database;
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemSaveSelectFile;
        private ToolStripMenuItem toolStripMenuItemClose;
        private ToolStripMenuItem toolStripMenuItemEditIdentify;
        private ToolStripMenuItem toolStripMenuItemEditCategory;
        private ToolStripMenuItem toolStripMenuItemSaveTAHFile;
        private ToolStripMenuItem toolStripMenuItemTAHInfoEdit;
        private ToolStripMenuItem toolStripMenuItemChangeColor;
        private ToolStripMenuItem toolStripMenuItemDeleteItem;
        private EventHandler formTimer;
        private ToolStripMenuItem toolStripMenuItemMakeTBN;
        private ToolStripMenuItem toolStripMenuItemSelectAll;
        private string tahdbpath;

        public TAHEditor(GenericTahInfo info)
        {
            tahdbpath = Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.tahpath,Path.GetRandomFileName()+".db");

            InitializeComponent();

            // タイマーを登録する.
            formTimer = new System.EventHandler(MainTimer_Tick);
            TDCGExplorer.TDCGExplorer.MainFormWindow.AddTimer(formTimer);

            database = new TAHLocalDB();
            database.Open(tahdbpath);

            if (info == null)
            {
                DataTable data = newDataTable();
                List<string> directory = database.GetDirectory();
                foreach (string file in directory)
                {
                    DataRow row = data.NewRow();
                    row.ItemArray = getDataEntity(file);
                    data.Rows.Add(row);
                }
                dataGridView.DataSource = data;
            }
            else
            {
                createfromExistTAH(info);
            }

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;

            setText();
        }

        public Object BeginTransaction()
        {
            return database.BeginTransaction();
        }

        public void Commit(Object transactionobject)
        {
            SQLiteTransaction transaction = transactionobject as SQLiteTransaction;
            if (transaction != null)
            {
                transaction.Commit();
            }
        }

        protected override void Dispose(bool disposing)
        {
            // タイマーを解除する.
            TDCGExplorer.TDCGExplorer.MainFormWindow.DeleteTimer(formTimer);

            if (database != null)
            {
                database.Dispose();
                database = null;
#if false
                // 必ず削除を指定していた時は常に消す
                if (TDCGExplorer.TDCGExplorer.SystemDB.delete_tahcache == true)
                {
#endif
                    try
                    {
                        //File.Delete(tahdbpath);
                        TDCGExplorer.TDCGExplorer.FileDelete(tahdbpath);
                    }
                    catch (Exception)
                    {
                        TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.DBFileDeleteError);
                    }
#if false
                }
#endif
            }
            base.Dispose(disposing);
        }

        private DataTable newDataTable()
        {
            DataTable data = new DataTable();
            data.Columns.Add(TextResource.Directory, Type.GetType("System.String"));
            data.Columns.Add(TextResource.Filename, Type.GetType("System.String"));
            data.Columns.Add(TextResource.FileType, Type.GetType("System.String"));
            return data;
        }

        private string[] getDataEntity(string path)
        {
            string[] val = { TAHInternalPath.GetDirectory(path), TAHInternalPath.GetFileName(path), TAHInternalPath.GetExtension(path) };
            return val;
        }

        public string TahDBPath
        {
            get { return tahdbpath; }
        }

        private void createfromExistTAH(GenericTahInfo info)
        {
            List<string> tsonames = new List<string>();
            GenericTAHStream stream = new GenericTAHStream(info, null);
            TAHFile tah = stream.tahfile;
            if (tah != null)
            {
                using (SQLiteTransaction transaction = database.BeginTransaction())
                {
                    bool hashonly = false;
                    database["version"] = info.version.ToString();
                    database["source"] = Path.GetFileName(info.path);
                    int id = 0;
                    foreach (TAHEntry ent in tah.EntrySet.Entries)
                    {
                        string filename;
                        if (ent.FileName == null)
                        {
                            filename = id.ToString("d8") + "_" + ent.Hash.ToString("x8");
                            hashonly = true;
                        }
                        else
                        {
                            filename = ent.FileName;
                        }
                        byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                        TAHLocalDbEntry entry = new TAHLocalDbEntry();
                        if (Path.GetExtension(filename) == "") filename += TDCGTbnUtil.ext(bytedata); // ファイル内容から拡張子を推定する
                        TAHLocalDBDataEntry dataentry = new TAHLocalDBDataEntry();
                        dataentry.dataid = 0;
                        dataentry.data = bytedata;
                        entry.path = filename;
                        entry.hash = (int) ent.Hash;
                        entry.dataid = database.AddData(dataentry);
                        // データベースにデータを登録する.
                        TDCGExplorer.TDCGExplorer.SetToolTips(info.shortname + " : " + filename + " "+TextResource.Extracting);
                        database.AddContent(entry);
                        id++;

                        if (Path.GetExtension(filename).ToLower() == ".tbn")
                        {
                            string tsoname = TDCGTbnUtil.GetTsoName(bytedata);
                            if (tsoname != null) tsonames.Add(Path.GetFileName(tsoname));
                        }

                        TDCGExplorer.TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.TDCGExplorer.DecBusy();
                    }
                    if (hashonly == true)
                    {
                        // tbnの情報を使ってディレクトリ情報を再構築する.
                        foreach (string tsoname in tsonames)
                        {
                            TAHLocalDbEntry entry;
                            // TSOファイル名を復元する.
                            string tsopath = "data/model/" + tsoname;
                            UInt32 tsohash = TAHUtil.CalcHash(tsopath);
                            entry = database.GetEntryHash((int)tsohash);
                            if (entry != null)
                            {
                                TAHLocalDbEntry newent = new TAHLocalDbEntry();
                                newent.dataid = entry.dataid;
                                newent.hash = entry.hash;
                                newent.path = tsopath;
                                database.DeleteEntry(entry.path);
                                database.AddContent(newent);
                            }
                            // PSDファイル名を復元する.
                            string psdpath = "data/icon/items/" + tsoname.Substring(0, 12) + ".psd";
                            UInt32 psdhash = TAHUtil.CalcHash(psdpath);
                            entry = database.GetEntryHash((int)psdhash);
                            if (entry != null)
                            {
                                TAHLocalDbEntry newent = new TAHLocalDbEntry();
                                newent.dataid = entry.dataid;
                                newent.hash = entry.hash;
                                newent.path = psdpath;
                                database.DeleteEntry(entry.path);
                                database.AddContent(newent);
                            }
                        }
                    }
                    transaction.Commit();
                }
                TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.ExtractSuccess);
            }

            // データセットを作り直す.
            DataTable data = newDataTable();

            List<string> directory = database.GetDirectory();
            foreach (string file in directory)
            {
                DataRow row = data.NewRow();
                row.ItemArray = getDataEntity(file);
                data.Rows.Add(row);
            }
            dataGridView.DataSource = data;
            setText();
        }

        private void setText()
        {
            Text = TextResource.Editing + " : " + database["source"] + " version " + database["version"];
            TDCGExplorer.TDCGExplorer.SetToolTips(Text);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemTAHInfoEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveSelectFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditIdentify = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemChangeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveTAHFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMakeTBN = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectAll,
            this.toolStripMenuItemTAHInfoEdit,
            this.toolStripMenuItemSaveSelectFile,
            this.toolStripMenuItemEditIdentify,
            this.toolStripMenuItemEditCategory,
            this.toolStripMenuItemChangeColor,
            this.toolStripMenuItemDeleteItem,
            this.toolStripMenuItemSaveTAHFile,
            this.toolStripMenuItemMakeTBN,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(291, 224);
            // 
            // toolStripMenuItemTAHInfoEdit
            // 
            this.toolStripMenuItemTAHInfoEdit.Name = "toolStripMenuItemTAHInfoEdit";
            this.toolStripMenuItemTAHInfoEdit.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemTAHInfoEdit.Text = TextResource.TAHEditorChangeTAHInformation;
            this.toolStripMenuItemTAHInfoEdit.Click += new System.EventHandler(this.toolStripMenuItemTAHInfoEdit_Click);
            // 
            // toolStripMenuItemSaveSelectFile
            // 
            this.toolStripMenuItemSaveSelectFile.Name = "toolStripMenuItemSaveSelectFile";
            this.toolStripMenuItemSaveSelectFile.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSaveSelectFile.Text = TextResource.TAHEditorSaveSelectedFile;
            this.toolStripMenuItemSaveSelectFile.Click += new System.EventHandler(this.toolStripMenuItemSaveSelectFile_Click);
            // 
            // toolStripMenuItemEditIdentify
            // 
            this.toolStripMenuItemEditIdentify.Name = "toolStripMenuItemEditIdentify";
            this.toolStripMenuItemEditIdentify.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemEditIdentify.Text = TextResource.TAHEditorRename;
            this.toolStripMenuItemEditIdentify.Click += new System.EventHandler(this.toolStripMenuItemEditIdentify_Click);
            // 
            // toolStripMenuItemEditCategory
            // 
            this.toolStripMenuItemEditCategory.Name = "toolStripMenuItemEditCategory";
            this.toolStripMenuItemEditCategory.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemEditCategory.Text = TextResource.TAHEditorChangeCategory;
            this.toolStripMenuItemEditCategory.Click += new System.EventHandler(this.toolStripMenuItemEditCategory_Click);
            // 
            // toolStripMenuItemChangeColor
            // 
            this.toolStripMenuItemChangeColor.Name = "toolStripMenuItemChangeColor";
            this.toolStripMenuItemChangeColor.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemChangeColor.Text = TextResource.TAHEditorChangeColorCode;
            this.toolStripMenuItemChangeColor.Click += new System.EventHandler(this.toolStripMenuItemChangeColor_Click);
            // 
            // toolStripMenuItemDeleteItem
            // 
            this.toolStripMenuItemDeleteItem.Name = "toolStripMenuItemDeleteItem";
            this.toolStripMenuItemDeleteItem.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemDeleteItem.Text = TextResource.TAHEditorFileDelete;
            this.toolStripMenuItemDeleteItem.Click += new System.EventHandler(this.toolStripMenuItemDeleteItem_Click);
            // 
            // toolStripMenuItemSaveTAHFile
            // 
            this.toolStripMenuItemSaveTAHFile.Name = "toolStripMenuItemSaveTAHFile";
            this.toolStripMenuItemSaveTAHFile.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSaveTAHFile.Text = TextResource.TAHEditorComposeTAHFile;
            this.toolStripMenuItemSaveTAHFile.Click += new System.EventHandler(this.toolStripMenuItemSaveTAHFile_Click);
            // 
            // toolStripMenuItemMakeTBN
            // 
            this.toolStripMenuItemMakeTBN.Name = "toolStripMenuItemMakeTBN";
            this.toolStripMenuItemMakeTBN.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemMakeTBN.Text = TextResource.TAHEditorMakeTBN;
            this.toolStripMenuItemMakeTBN.Click += new System.EventHandler(this.toolStripMenuItemMakeTBN_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemClose.Text = TextResource.Close;
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // toolStripMenuItemSelectAll
            // 
            this.toolStripMenuItemSelectAll.Name = "toolStripMenuItemSelectAll";
            this.toolStripMenuItemSelectAll.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSelectAll.Text = TextResource.SelectAll;
            this.toolStripMenuItemSelectAll.Click += new System.EventHandler(this.toolStripMenuItemSelectAll_Click);
            // 
            // TAHEditor
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
            dataGridView.Size = ClientSize;//Size;
        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        // クローズが選択された.
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            Parent.Dispose();
        }

        // 選択しているファイルを保存する.
        private void toolStripMenuItemSaveSelectFile_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            string destpath = Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.tahpath, Path.GetFileNameWithoutExtension(database["source"]));
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
                            string path = entry[0].ToString() + entry[1].ToString();
                            string destfile = Path.Combine(destpath, path);
                            Debug.WriteLine("save to "+destfile);
                            Directory.CreateDirectory(Path.GetDirectoryName(destfile));
                            File.Delete(destfile);
                            using (Stream stream = File.Create(destfile))
                            {
                                BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.Default);
                                TAHLocalDbEntry tahentry = database.GetEntry(path);
                                TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);
                                writer.Write(dataentry.data, 0, dataentry.data.Length);
                                writer.Close();
                                stream.Close();
                            }
                        }
                    }
                }
            }
            TDCGExplorer.TDCGExplorer.ExplorerSelectPath(destpath);
        }

        // ファイル名の置換
        private void ReplaceFileName(string newname)
        {
            using (SQLiteTransaction transaction = database.BeginTransaction())
            {
                try
                {
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
                                    // 新しい名前を作る
                                    string orgpath = entry[0].ToString() + entry[1].ToString();
                                    string newfilename = newname + entry[1].ToString().Substring(newname.Length);
                                    string newpath = entry[0].ToString() + newfilename;
                                    // ファイル名を書き換える
                                    TAHLocalDbEntry tahentry = database.GetEntry(orgpath);
                                    database.DeleteEntry(orgpath);
                                    tahentry.path = newpath;
                                    database.AddContent(tahentry);
                                    Debug.WriteLine(newpath);
                                    // TBNなら中身も書き換える
                                    if (Path.GetExtension(orgpath).ToLower() == ".tbn")
                                    {
                                        TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);
                                        string orgtsoepath = TDCGTbnUtil.GetTsoName(dataentry.data);
                                        if (orgtsoepath != null)
                                        {
                                            string[] pathelement = orgtsoepath.Split('/');
                                            string tsopath = "";
                                            for (int i = 0; i < (pathelement.Length - 1); i++)
                                                tsopath += pathelement[i] + "/";
                                            string newtsopath = tsopath + newname + pathelement[pathelement.Length - 1].Substring(newname.Length);
                                            TDCGTbnUtil.SetTsoName(dataentry.data, newtsopath);
                                            database.UpdateData(dataentry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();

                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // データセットを元に戻す.
                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                    throw ex;
                }
            }
        }

        // ファイル名の置換
        private void ChangeColorNo(string newcolor)
        {
            using (SQLiteTransaction transaction = database.BeginTransaction())
            {
                try
                {
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
                                    // 新しい名前を作る
                                    string orgpath = entry[0].ToString() + entry[1].ToString();
                                    // N765BODY_X00.TSO
                                    // 012345678901
                                    //             1234
                                    string newfilename = entry[1].ToString().Substring(0, 10) + newcolor + entry[1].ToString().Substring(12, 4);
                                    string newpath = entry[0].ToString() + newfilename;
                                    // ファイル名を書き換える
                                    TAHLocalDbEntry tahentry = database.GetEntry(orgpath);
                                    database.DeleteEntry(orgpath);
                                    tahentry.path = newpath;
                                    database.AddContent(tahentry);
                                    Debug.WriteLine(newpath);
                                    // TBNなら中身も書き換える
                                    if (Path.GetExtension(orgpath).ToLower() == ".tbn")
                                    {
                                        TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);
                                        string orgtsoepath = TDCGTbnUtil.GetTsoName(dataentry.data);
                                        if (orgtsoepath != null)
                                        {
                                            string[] pathelement = orgtsoepath.Split('/');
                                            string tsopath = "";
                                            for (int i = 0; i < (pathelement.Length - 1); i++)
                                                tsopath += pathelement[i] + "/";
                                            string newtsopath = tsopath + pathelement[pathelement.Length - 1].Substring(0, 10) + newcolor + pathelement[pathelement.Length - 1].Substring(12, 4);
                                            TDCGTbnUtil.SetTsoName(dataentry.data, newtsopath);
                                            database.UpdateData(dataentry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();

                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // データセットを元に戻す.
                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                    throw ex;
                }
            }
        }


        // 選択しているファイルの名前を変更する
        private void toolStripMenuItemEditIdentify_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.labeltext = TextResource.TAHEditorRenameText;// "新しい名前";
            dialog.dialogtext = TextResource.TAHEditorRenameLabel;// "ファイル名の変更";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string textto = dialog.textfield;
                if (textto.Length > 16)
                {
                    MessageBox.Show(TextResource.TooLongFilename, TextResource.Error, MessageBoxButtons.OK);
                    return;
                }
                try
                {
                    ReplaceFileName(textto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(TextResource.Error + ":" + ex.Message, TextResource.Error, MessageBoxButtons.OK);
                    Debug.WriteLine(ex);
                }
            }
        }

        // カテゴリの置換
        private void ReplaceCategory(TBNCategoryData type)
        {
            using (SQLiteTransaction transaction = database.BeginTransaction())
            {
                try
                {
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
                                    string dir = entry[0].ToString();
                                    string filename = entry[1].ToString();
                                    string ext = entry[2].ToString().ToLower();

                                    string orgpath = dir + filename;
                                    string newfilename = filename.Substring(0, 9) + type.symbol.ToString() + filename.Substring(10, 6);
                                    string newpath = dir + newfilename;

                                    // ファイル名を書き換える
                                    TAHLocalDbEntry tahentry = database.GetEntry(orgpath);
                                    database.DeleteEntry(orgpath);
                                    tahentry.path = newpath;
                                    database.AddContent(tahentry);
                                    Debug.WriteLine(newpath);
                                    // TBNなら中身も書き換える
                                    if (ext == "tbn")
                                    {
                                        TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);
                                        string orgtsoepath = TDCGTbnUtil.GetTsoName(dataentry.data);
                                        if (orgtsoepath != null)
                                        {
                                            string[] pathelement = orgtsoepath.Split('/');
                                            string tsopath = "";
                                            for (int i = 0; i < (pathelement.Length - 1); i++)
                                                tsopath += pathelement[i] + "/";
                                            string oldtsoname = pathelement[pathelement.Length - 1];
                                            string newtsopath = tsopath + oldtsoname.Substring(0, 9) + type.symbol.ToString() + oldtsoname.Substring(10, 6);
                                            // ファイル名を書き換える
                                            byte[]tbn=ReadTbnData(TDCGTbnUtil.typechartotype(type.symbol)); // 適切なtbnを読み込む.
                                            dataentry.data = tbn;
                                            TDCGTbnUtil.SetTsoName(dataentry.data, newtsopath);
                                            database.UpdateData(dataentry);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();

                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // データセットを元に戻す.
                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                    throw ex;
                }
            }
        }

        private byte[] ReadTbnData(int type)
        {
            string[] tbnname = {    "script/items/n001body_a00.tbn",    //00
                                    "script/items/n001fhea_b00.tbn",    //01
                                    "script/items/n001bhea_c00.tbn",    //02
                                    "script/items/n001hskn_d00.tbn",    //03
                                    "script/items/n001eyes_e00.tbn",    //04
                                    "script/items/n001bura_f00.tbn",    //05
                                    "script/items/n002scho_g00.tbn",    //06
                                    "script/items/n001pant_h00.tbn",    //07
                                    "script/items/n001hsox_i00.tbn",    //08
                                    "script/items/n003sail_j00.tbn",    //09
                                    "script/items/n001nurs_k00.tbn",    //10
                                    "script/items/n001nksk_l00.tbn",    //11
                                    "script/items/n001sail_m00.tbn",    //12
                                    "script/items/n001tail_n00.tbn",    //13
                                    "script/items/n001rofr_o00.tbn",    //14
                                    "script/items/n001nek1_p00.tbn",    //15
                                    "script/items/n001mega_q00.tbn",    //16
                                    "script/items/n001neck_r00.tbn",    //17
                                    "script/items/n004cffs_s00.tbn",    //18
                                    "script/items/n001wing_t00.tbn",    //19
                                    "script/items/n001ahog_u00.tbn",    //20
                                    "script/items/n001gant_v00.tbn",    //21
                                    "script/items/n001loos_w00.tbn",    //22
                                    "script/items/n001cffs_x00.tbn",    //23
                                    "script/items/n001head_y00.tbn",    //24
                                    "script/items/n001obon_z00.tbn",    //25
                                    "script/items/n001mayu_000.tbn",    //26
                                    "script/items/n001kiba_100.tbn",    //27
                                    "script/items/n001hoku_200.tbn",    //28
                                    "script/items/n001head_300.tbn" };  //29

            // Zカテゴリはファイルから。それ以外はbase.tahから読み込む.
            if (type==25)
            {
                // 変更先が手持ちの場合
                // tbnファイルを読み込んで書き換える.
                try
                {
                    using (Stream stream = File.OpenRead("N001OBON_Z00.tbn"))
                    using (MemoryStream memorystream = new MemoryStream())
                    {
                        ZipFileUtil.CopyStream(stream, memorystream);
                        byte[] tbn = memorystream.ToArray();
                        return tbn;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(TextResource.TBNReadError, TextResource.Error, MessageBoxButtons.OK);
                    Debug.WriteLine("tbn.open.error");
                }
            }
            else
            {
                // base.tahから手持ちアイテム以外のファイルを読み込む.
                using (Stream file_stream = File.OpenRead(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, "base.tah")))
                {
                    TAHFile tah = new TAHFile(file_stream);
                    try
                    {
                        tah.LoadEntries();
                        foreach (TAHEntry ent in tah.EntrySet.Entries)
                        {
                            for (int id = 0; id < tbnname.Length; id++)
                            {
                                if (ent.FileName != null && ent.FileName.ToLower() == tbnname[type])
                                {
                                    byte[] content = TAHUtil.ReadEntryData(tah.Reader, ent);
                                    return content;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(TextResource.BaseTahReadError, TextResource.Error, MessageBoxButtons.OK);
                        Debug.WriteLine("basetah.open.error");
                    }
                }
            }
            return null;
        }

        // 選択しているファイルのカテゴリを変更する
        private void toolStripMenuItemEditCategory_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleDropDownDialog dialog = new SimpleDropDownDialog();
            foreach (TBNCategoryData type in TDCGTbnUtil.CategoryData)
            {
                dialog.AddList(type.name);//+" 属性値1:"+type.byte1.ToString("x2")+" 属性値2:"+type.byte2.ToString("x2")+" 属性値3:"+type.byte3.ToString("x2") );
            }
            dialog.labeltext = TextResource.TAHEditorTbnCatChangeLabel;// "TBNの属性";
            dialog.dialogtext = TextResource.TAHEditorTbnCatChangeText;// "TBNの属性変更";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int index = dialog.selectedIndex;
                if (index >= 0)
                {
#if false
                    if (index == 25)
                    {
                        MessageBox.Show("手持ちアイテムに変更する事はできません。", "エラー", MessageBoxButtons.OK);
                    }
#endif
                    try
                    {
                        ReplaceCategory(TDCGTbnUtil.CategoryData[index]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(TextResource.Error + ":" + ex.Message, TextResource.Error, MessageBoxButtons.OK);
                        Debug.WriteLine(e);
                    }
                }
            }
        }

        // TAHファイルを保存する
        private void toolStripMenuItemSaveTAHFile_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

//            string destpath = TDCGExplorer.TDCGExplorer.SystemDB.tahpath;
//            string destfilename = Path.Combine(destpath,Path.GetFileName(database["source"]));

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileName(database["source"]);
            dialog.InitialDirectory = TDCGExplorer.TDCGExplorer.SystemDB.tahpath;
            dialog.Filter = TextResource.TAHFileDescription; // "TAHファイル(*.tah)|*.tah";
            dialog.FilterIndex = 0;
            dialog.Title = TextResource.SelectSaveFileName; // "保存先のファイルを選択してください";
            dialog.RestoreDirectory = true;
            dialog.OverwritePrompt = true;
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string destfilename = dialog.FileName;

                TAHWriter tah = new TAHWriter();

                List<String> tahpath = new List<string>();

                // TAHにデータを追加していく.
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
                                string path = entry[0].ToString() + entry[1].ToString();
                                tahpath.Add(path);
                            }
                        }
                    }
                }
                tahpath.Sort();
                foreach (string tahfile in tahpath) tah.Add(tahfile);

                // TAHバージョンを設定する
                tah.Version=uint.Parse(database["version"]);
                // TAHファイルを書き出す.
                Directory.CreateDirectory(Path.GetDirectoryName(destfilename));
                int count = 1;
                File.Delete(destfilename);
                using (Stream stream = File.Create(destfilename))
                {
                    // データ取得delegate
                    tah.Data += delegate(string filename)
                    {
                        TDCGExplorer.TDCGExplorer.SetToolTips(filename + " " + TextResource.Compressing + " (" + count++.ToString() + "/" + tah.Count.ToString() + ")");
                        TAHLocalDbEntry tahentry = database.GetEntry(filename);
                        TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);

                        TDCGExplorer.TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.TDCGExplorer.DecBusy();

                        return dataentry.data;
                    };
                    // データを書き込む.
                    tah.Write(stream);
                    stream.Close();
                }
                TDCGExplorer.TDCGExplorer.SetToolTips(TextResource.CompressComplete);
                TDCGExplorer.TDCGExplorer.ExplorerSelectPath(destfilename);
            }
        }

        // TAHの情報を変更する.
        private void toolStripMenuItemTAHInfoEdit_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            TAHInfoDialog dialog = new TAHInfoDialog();
            dialog.tahVersion = int.Parse(database["version"]);
            dialog.tahSource = database["source"];
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                database["version"] = dialog.tahVersion.ToString();
                database["source"] = Path.GetFileName(dialog.tahSource);
                // 情報を表示する.
                setText();
                Parent.Text = Text;
            }
        }

        // データを追加する.
        public void AddItem(string path, byte[] bytedata)
        {
            // もし既にエントリがある場合はデータを更新する.
            TAHLocalDbEntry past = database.GetEntry(path);
            if (past != null)
            {
                TAHLocalDBDataEntry updatedata = new TAHLocalDBDataEntry();
                updatedata.data = bytedata;
                updatedata.dataid = past.dataid;
                database.UpdateData(updatedata);
                return;
            }

            // データベースにデータを追加する.
            TAHLocalDBDataEntry tahdata = new TAHLocalDBDataEntry();
            tahdata.data = bytedata;
            tahdata.dataid = database.AddData(tahdata);
            TAHLocalDbEntry tahentry = new TAHLocalDbEntry();
            tahentry.dataid = tahdata.dataid;
            tahentry.path = path;
            database.AddContent(tahentry);

            DataTable data = dataGridView.DataSource as DataTable;
            if (data != null)
            {
                DataRow row = data.NewRow();
                row.ItemArray = getDataEntity(path);
                data.Rows.Add(row);
            }
        }

        public void SetInformation(string filename, int version)
        {
            database["version"] = version.ToString();
            database["source"] = Path.GetFileName(filename);
            setText();
        }

        private void toolStripMenuItemChangeColor_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.labeltext = TextResource.TAHEditorNewColorNoLabel; //"新しい色番号";
            dialog.dialogtext = TextResource.TAHEditorNewColorNoText; //"色番号の変更";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string textto = dialog.textfield;
                if (textto.Length != 2)
                {
                    MessageBox.Show(TextResource.TAHEditorNewColorNoError, TextResource.Error, MessageBoxButtons.OK);
                    return;
                }
                try
                {
                    ChangeColorNo(textto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(TextResource.Error + ":" + ex.Message, TextResource.Error, MessageBoxButtons.OK);
                    Debug.WriteLine(ex);
                }
            }
        }

        private void toolStripMenuItemDeleteItem_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            using (SQLiteTransaction transaction = database.BeginTransaction())
            {
                try
                {
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
                                    string dir = entry[0].ToString();
                                    string filename = entry[1].ToString();
                                    string orgpath = dir + filename;
                                    TAHLocalDbEntry tahentry = database.GetEntry(orgpath);
                                    database.DeleteEntry(orgpath);
                                    database.DeleteData(tahentry.dataid);
                                    row.Delete();
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // データセットを元に戻す.
                    DataTable data = newDataTable();
                    List<string> directory = database.GetDirectory();
                    foreach (string file in directory)
                    {
                        DataRow row = data.NewRow();
                        row.ItemArray = getDataEntity(file);
                        data.Rows.Add(row);
                    }
                    dataGridView.DataSource = data;
                    throw ex;
                }
            }
        }

        public void SelectAll()
        {
            dataGridView.SelectAll();
        }

        private bool fChangedSelection = false;
        private int viewChangeTimer = 0;

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.SystemDB.taheditorpreview == true)
            {
                fChangedSelection = true;
                viewChangeTimer = 5;
            }
        }

        // タイマーで待って後から描画する.
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (fChangedSelection == true)
            {
                if (viewChangeTimer-- == 0)
                {
                    DrawSelectedTso();
                    fChangedSelection = false;
                }
            }
        }

        private void DrawSelectedTso()
        {
            HashSet<string> tsoFileList = new HashSet<string>();
            // 表示するTSOの一覧を取得する.
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
                            string dir = entry[0].ToString();
                            string filename = entry[1].ToString();
                            if (entry[2].ToString() == "tso")
                            {
                                tsoFileList.Add(dir + filename);
                            }
                            if (entry[2].ToString() == "tbn")
                            {
                                TAHLocalDbEntry tahentry = database.GetEntry(dir + filename);
                                if (tahentry != null)
                                {
                                    TAHLocalDBDataEntry tahdata = database.GetData(tahentry.dataid);
                                    try
                                    {
                                        string tsoname = TDCGTbnUtil.GetTsoName(tahdata.data);
                                        if(tsoname!=null) tsoFileList.Add(tsoname);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // TSOを描画する。あまりに数が多い時は描画しない.
            if (tsoFileList.Count > 0 && tsoFileList.Count<31)
            {
                string thetsoname = "";
                TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();
                foreach (string tsoname in tsoFileList)
                {
                    TAHLocalDbEntry entry = database.GetEntry(tsoname);
                    if (entry != null)
                    {
                        TAHLocalDBDataEntry data = database.GetData(entry.dataid);
                        using (MemoryStream ms = new MemoryStream(data.data))
                        {
                            TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(ms);
                            thetsoname = tsoname;
                        }
                    }
                }
                if (TDCGExplorer.TDCGExplorer.SystemDB.loadinitialpose) TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                // 選んだアイテムが１個の時は自動センタリングする.
                if (tsoFileList.Count == 1 && thetsoname != "")
                {
                    // カメラをセンター位置に.
                    TSOCameraAutoCenter camera = new TSOCameraAutoCenter(TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer);
                    camera.UpdateCenterPosition(Path.GetFileName(thetsoname).ToUpper());
                    // 次回カメラが必ずリセットされる様にする.
                    TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();
                }
                //TDCGExplorer.TDCGExplorer.FigureLoad = false;
            }
        }

        // 選択されたtsoファイルからpsd,tbnを生成する.
        private void toolStripMenuItemMakeTBN_Click(object sender, EventArgs e)
        {
            HashSet<string> tsoFileList = new HashSet<string>();
            // 表示するTSOの一覧を取得する.
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
                            string filename = entry[1].ToString();
                            if (entry[2].ToString().ToLower() == "tso")
                            {
                                tsoFileList.Add(filename);
                            }
                            else
                            {
                                // tsoファイル以外は無視する.
                                Debug.WriteLine("invalid file format");
                            }
                        }
                    }
                }
            }
            if (tsoFileList.Count == 0)
            {
                MessageBox.Show(TextResource.OnlyTSOFile, TextResource.Error, MessageBoxButtons.OK);
                return;
            }
            TbnSelectForm form = new TbnSelectForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                string tbncat="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123";
                // tbnの選択状態を取得する.
                TDCGTbnCreateInfo info = form.getResult();
                // 選択されたtbnを生成する.
                foreach (string tsoname in tsoFileList)
                {
                    string tsopath = "data/model/" + tsoname;
                    string psdpath = "data/icon/items/" + tsoname.Substring(0, 12) + ".psd";

                    // アイコンがあるなら読み込む.
                    TAHLocalDbEntry iconentry = database.GetEntry(psdpath);
                    TAHLocalDBDataEntry icondata = null;
                    if (iconentry != null)
                    {
                        icondata = database.GetData(iconentry.dataid);
                    }

                    // オリジナルの名前
                    string org_tbnname = tsoname.Substring(0, 12) + ".tbn";
                    string org_psdname = tsoname.Substring(0, 12) + ".psd";

                    // tbnの種類は30個
                    for (int i = 0; i < 30; i++)
                    {
                        // tbn生成フラグがon
                        if (info.tbnFlags[i])
                        {
                            string new_tbnname = "script/items/" + org_tbnname.Substring(0, 9) + tbncat[i] + org_tbnname.Substring(10, 6);

                            // tbnの生成
                            TAHLocalDbEntry tbnentry = database.GetEntry(new_tbnname);
                            if (tbnentry == null) // 既にtbnがある時はスキップする
                            {
                                // tbnデータを作成する.
                                byte[] tbn = ReadTbnData(i); // 適切なtbnを読み込む.
                                TDCGTbnUtil.SetTsoName(tbn, tsopath);
                                AddItem(new_tbnname, tbn);
                            }
                        }
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        // tbn生成フラグがon
                        if (info.tbnFlags[i])
                        {
                            string new_psdname = "data/icon/items/" + org_psdname.Substring(0, 9) + tbncat[i] + org_psdname.Substring(10, 6);

                            // アイコンデータのコピー
                            TAHLocalDbEntry psdentry = database.GetEntry(new_psdname);
                            if (psdentry == null && icondata != null)
                            {
                                AddItem(new_psdname, icondata.data);
                            }
                        }
                    }

                }
            }
        }

        // tahファイル作成ユーティリティ
        public void makeTAHFile(string filename,List<PNGTsoData> tsoDataList)
        {
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
            Dictionary<uint, byte[]> tbndata = new Dictionary<uint, byte[]>();
            using (Stream file_stream = File.OpenRead(Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.arcs_path, "base.tah")))
            {
                TAHFile tah = new TAHFile(file_stream);
                try
                {
                    tah.LoadEntries();
                    foreach (TAHEntry ent in tah.EntrySet.Entries)
                    {
                        for (int id = 0; id < tbnname.Length; id++)
                        {
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
                    MessageBox.Show(TextResource.BaseTahReadError, TextResource.Error, MessageBoxButtons.OK);
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
                MessageBox.Show(TextResource.TBNReadError, TextResource.Error, MessageBoxButtons.OK);
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
                MessageBox.Show(TextResource.PSDFileReadError,TextResource.Error, MessageBoxButtons.OK);
                Debug.WriteLine("PSD.open.error");
            }

            // 新規TAHを作成する.
            // 常に新規タブで.
            SetInformation(filename + ".tah", 1);
            Object transaction = BeginTransaction();
            foreach (PNGTsoData data in tsoDataList)
            {
                AddItem(newtbnname[data.tsoID & 0xff], tbndata[data.tsoID & 0xff]);
                AddItem(newpsdname[data.tsoID & 0xff], icondata);
                AddItem(newtsoname[data.tsoID & 0xff], data.tsodata);
            }
            Commit(transaction);
        }

        private void toolStripMenuItemSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }
    }

    static class TAHInternalPath
    {
        public static string GetDirectory(string path)
        {
            string retval = "";
            string[] dirs = path.Split('/');
            for (int i = 0; i < (dirs.Length - 1); i++)
            {
                retval += dirs[i] + "/";
            }
            return retval;
        }

        public static string GetFileName(string path)
        {
            string[] dirs = path.Split('/');
            string fullname = dirs[dirs.Length - 1];
            return fullname;
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            string fullname = GetFileName(path);
            string[] element = fullname.Split('.');
            return element[0];
        }

        public static string GetExtension(string path)
        {
            string[] dirs = path.Split('/');
            string fullname = dirs[dirs.Length - 1];
            string[] file = fullname.Split('.');
            if (file.Length == 1) return "";
            return file[1];
        }
    }  
}
