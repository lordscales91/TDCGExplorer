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
        private string tahdbpath;

        public TAHEditor(string dbpath, GenericTahInfo info)
        {
            tahdbpath = dbpath.ToLower();

            InitializeComponent();

            // タイマーを登録する.
            formTimer = new System.EventHandler(MainTimer_Tick);
            TDCGExplorer.TDCGExplorer.MainFormWindow.AddTimer(formTimer);

            database = new TAHLocalDB();
            database.Open(dbpath);

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

                // 必ず削除を指定していた時は常に消す
                if (TDCGExplorer.TDCGExplorer.SystemDB.delete_tahcache == true)
                {
                    File.Delete(tahdbpath);
                }
                else
                {
                    if (MessageBox.Show("作業用データベースファイルを削除しますか？\nこのファイルは次回編集時に再利用できます。\n(初期設定でこの表示をせず常に削除する事が出来ます)", "DBの削除", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        File.Delete(tahdbpath);
                }
            }
            base.Dispose(disposing);
        }

        private DataTable newDataTable()
        {
            DataTable data = new DataTable();
            data.Columns.Add("ディレクトリ", Type.GetType("System.String"));
            data.Columns.Add("ファイル名", Type.GetType("System.String"));
            data.Columns.Add("ファイルタイプ", Type.GetType("System.String"));
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
            GenericTAHStream stream = new GenericTAHStream(info, null);
            TAHFile tah = stream.tahfile;
            if (tah != null)
            {
                using (SQLiteTransaction transaction = database.BeginTransaction())
                {
                    database["version"] = info.version.ToString();
                    database["source"] = info.path;
                    int id = 0;
                    foreach (TAHEntry ent in tah.EntrySet.Entries)
                    {
                        string filename;
                        if (ent.FileName == null)
                        {
                            filename = id.ToString("d8") + "_" + ent.Hash.ToString("x8");
                        }
                        else
                        {
                            filename = ent.FileName;
                        }
                        byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                        TAHLocalDbEntry entry = new TAHLocalDbEntry();

                        TAHLocalDBDataEntry dataentry = new TAHLocalDBDataEntry();
                        dataentry.dataid = 0;
                        dataentry.data = bytedata;
                        entry.path = filename;
                        entry.dataid = database.AddData(dataentry);
                        // データベースにデータを登録する.
                        TDCGExplorer.TDCGExplorer.SetToolTips(info.shortname + " : " + filename + " 展開中");
                        database.AddContent(entry);
                        id++;

                        TDCGExplorer.TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.TDCGExplorer.DecBusy();
                    }
                    transaction.Commit();
                }
                TDCGExplorer.TDCGExplorer.SetToolTips("展開完了");
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
            Text = "編集中 : " + database["source"] + " version " + database["version"];
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
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemTAHInfoEdit,
            this.toolStripMenuItemSaveSelectFile,
            this.toolStripMenuItemEditIdentify,
            this.toolStripMenuItemEditCategory,
            this.toolStripMenuItemChangeColor,
            this.toolStripMenuItemDeleteItem,
            this.toolStripMenuItemSaveTAHFile,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(291, 180);
            // 
            // toolStripMenuItemTAHInfoEdit
            // 
            this.toolStripMenuItemTAHInfoEdit.Name = "toolStripMenuItemTAHInfoEdit";
            this.toolStripMenuItemTAHInfoEdit.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemTAHInfoEdit.Text = "TAH情報を変更する";
            this.toolStripMenuItemTAHInfoEdit.Click += new System.EventHandler(this.toolStripMenuItemTAHInfoEdit_Click);
            // 
            // toolStripMenuItemSaveSelectFile
            // 
            this.toolStripMenuItemSaveSelectFile.Name = "toolStripMenuItemSaveSelectFile";
            this.toolStripMenuItemSaveSelectFile.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSaveSelectFile.Text = "選択したファイルを保存する";
            this.toolStripMenuItemSaveSelectFile.Click += new System.EventHandler(this.toolStripMenuItemSaveSelectFile_Click);
            // 
            // toolStripMenuItemEditIdentify
            // 
            this.toolStripMenuItemEditIdentify.Name = "toolStripMenuItemEditIdentify";
            this.toolStripMenuItemEditIdentify.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemEditIdentify.Text = "選択したファイルの名前を変更する";
            this.toolStripMenuItemEditIdentify.Click += new System.EventHandler(this.toolStripMenuItemEditIdentify_Click);
            // 
            // toolStripMenuItemEditCategory
            // 
            this.toolStripMenuItemEditCategory.Name = "toolStripMenuItemEditCategory";
            this.toolStripMenuItemEditCategory.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemEditCategory.Text = "選択したファイルのカテゴリを変更する";
            this.toolStripMenuItemEditCategory.Click += new System.EventHandler(this.toolStripMenuItemEditCategory_Click);
            // 
            // toolStripMenuItemChangeColor
            // 
            this.toolStripMenuItemChangeColor.Name = "toolStripMenuItemChangeColor";
            this.toolStripMenuItemChangeColor.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemChangeColor.Text = "選択した色番号を変更する";
            this.toolStripMenuItemChangeColor.Click += new System.EventHandler(this.toolStripMenuItemChangeColor_Click);
            // 
            // toolStripMenuItemDeleteItem
            // 
            this.toolStripMenuItemDeleteItem.Name = "toolStripMenuItemDeleteItem";
            this.toolStripMenuItemDeleteItem.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemDeleteItem.Text = "選択したファイルの削除";
            this.toolStripMenuItemDeleteItem.Click += new System.EventHandler(this.toolStripMenuItemDeleteItem_Click);
            // 
            // toolStripMenuItemSaveTAHFile
            // 
            this.toolStripMenuItemSaveTAHFile.Name = "toolStripMenuItemSaveTAHFile";
            this.toolStripMenuItemSaveTAHFile.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSaveTAHFile.Text = "選択したファイルをTAHに梱包する";
            this.toolStripMenuItemSaveTAHFile.Click += new System.EventHandler(this.toolStripMenuItemSaveTAHFile_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemClose.Text = "閉じる";
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
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
            dataGridView.Size = Size;
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
                            string destpath = Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.tahpath, Path.GetFileNameWithoutExtension(database["source"]));
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
                                    // データグリッドを更新する.
                                    string[] newitem = { entry[0].ToString(), newfilename, entry[2].ToString() };
                                    row.ItemArray = newitem;
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
                                    // データグリッドを更新する.
                                    string[] newitem = { entry[0].ToString(), newfilename, entry[2].ToString() };
                                    row.ItemArray = newitem;
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


        // 選択しているファイルの名前を変更する
        private void toolStripMenuItemEditIdentify_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.labeltext = "新しい名前";
            dialog.dialogtext = "ファイル名の変更";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string textto = dialog.textfield;
                if (textto.Length > 16)
                {
                    MessageBox.Show("文字数が長すぎます", "エラー", MessageBoxButtons.OK);
                    return;
                }
                try
                {
                    ReplaceFileName(textto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("エラーが発生しました:"+ex.Message, "エラー", MessageBoxButtons.OK);
                    Debug.WriteLine(ex);
                }
            }
        }

        // ファイル名の置換
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
                                            TDCGTbnUtil.SetTsoName(dataentry.data, newtsopath);
                                            // 属性値を書き換える
                                            TDCGTbnUtil.SetTsoSignature(dataentry.data, type);
                                            database.UpdateData(dataentry);
                                        }
                                    }
                                    // データグリッドを更新する.
                                    string[] newitem = { dir, newfilename, ext };
                                    row.ItemArray = newitem;
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

        // 選択しているファイルのカテゴリを変更する
        private void toolStripMenuItemEditCategory_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleDropDownDialog dialog = new SimpleDropDownDialog();
            foreach (TBNCategoryData type in TDCGTbnUtil.CategoryData)
            {
                dialog.AddList(type.symbol.ToString()+" : "+type.name);//+" 属性値1:"+type.byte1.ToString("x2")+" 属性値2:"+type.byte2.ToString("x2")+" 属性値3:"+type.byte3.ToString("x2") );
            }
            dialog.labeltext = "TBNの属性";
            dialog.dialogtext = "TBNの属性変更";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int index = dialog.selectedIndex;
                if (index >= 0)
                {
                    if (index == 25)
                    {
                        MessageBox.Show("手持ちアイテムに変更する事はできません。", "エラー", MessageBoxButtons.OK);
                    }
                    try
                    {
                        ReplaceCategory(TDCGTbnUtil.CategoryData[index]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("エラーが発生しました:" + ex.Message, "エラー", MessageBoxButtons.OK);
                        Debug.WriteLine(e);
                    }
                }
            }
        }

        // TAHファイルを保存する
        private void toolStripMenuItemSaveTAHFile_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            string destpath = TDCGExplorer.TDCGExplorer.SystemDB.tahpath;
            string destfilename = Path.Combine(destpath,Path.GetFileName(database["source"]));

            TAHWriter tah = new TAHWriter();

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
                            tah.Add(path);
                        }
                    }
                }
            }
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
                    TDCGExplorer.TDCGExplorer.SetToolTips(filename + " 梱包中 ("+count++.ToString()+"/"+tah.Count.ToString()+")");
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
            TDCGExplorer.TDCGExplorer.SetToolTips("梱包完了");

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
                database["source"] = dialog.tahSource;
                // 情報を表示する.
                setText();
                Parent.Text = Text;
            }
        }

        // データを追加する.
        public void AddItem(string path, byte[] bytedata)
        {
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
            database["source"] = filename;
            setText();
        }

        private void toolStripMenuItemChangeColor_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            SimpleTextDialog dialog = new SimpleTextDialog();
            dialog.labeltext = "新しい色番号";
            dialog.dialogtext = "色番号のへ工";
            dialog.Owner = TDCGExplorer.TDCGExplorer.MainFormWindow;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string textto = dialog.textfield;
                if (textto.Length != 2)
                {
                    MessageBox.Show("色番号は二文字で指定して下さい", "エラー", MessageBoxButtons.OK);
                    return;
                }
                try
                {
                    ChangeColorNo(textto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("エラーが発生しました:" + ex.Message, "エラー", MessageBoxButtons.OK);
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
                TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                // 選んだアイテムが１個の時は自動センタリングする.
                if (tsoFileList.Count == 1 && thetsoname != "")
                {
                    // カメラをセンター位置に.
                    TSOCameraAutoCenter camera = new TSOCameraAutoCenter(TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer);
                    camera.UpdateCenterPosition(Path.GetFileName(thetsoname).ToUpper());
                    // 次回カメラが必ずリセットされる様にする.
                    TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();
                }
            }
        }

#if false
        string[] editBeginItemRow;
        // セルの値が変更されようとしている.
        private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // 編集される前の値をコピーする.
            DataGridViewRow viewrow = dataGridView.CurrentRow;
            DataRowView vrow = viewrow.DataBoundItem as DataRowView;
            if (vrow != null)
            {
                DataRow row = vrow.Row;
                if (row != null)
                {
                    Object[] entry = row.ItemArray;
                    if (entry != null)
                    {
                        editBeginItemRow = new string[] { entry[0].ToString(), entry[1].ToString(), entry[2].ToString() };
                    }
                }
            }
        }
#endif
#if false
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (editBeginItemRow != null)
            {
                string newdirectory = editBeginItemRow[0];
                string newfilename = editBeginItemRow[1];
                string newextension = editBeginItemRow[2];
                string key = editBeginItemRow[0] + editBeginItemRow[1];

                Object[] entry = null;

                DataGridViewRow viewrow = dataGridView.CurrentRow;
                DataRowView vrow = viewrow.DataBoundItem as DataRowView;
                DataRow row = null;
                if (vrow != null)
                {
                    row = vrow.Row;
                    if (row != null)
                    {
                        entry = row.ItemArray;

                        if (entry != null)
                        {
                            switch (e.ColumnIndex)
                            {
                                case 0:
                                    newdirectory = entry[0].ToString();
                                    break;
                                case 1:
                                    newfilename = entry[1].ToString();
                                    break;
                                case 2:
                                    newextension = entry[2].ToString();
                                    break;
                            }
                            if (newextension != "")
                            {
                                newfilename = TAHInternalPath.GetFileNameWithoutExtension(newfilename) + "." + newextension;
                            }
                            else
                            {
                                newfilename = TAHInternalPath.GetFileNameWithoutExtension(newfilename);
                            }
                        }

                        // データベースの中身を書き換える.
                        DataTable table = dataGridView.DataSource as DataTable;
                        if (table != null)
                        {
                            string newkey = newdirectory + newfilename;
                            TAHDataBaseEntry checkentry = database.GetEntry(newkey);
                            if (checkentry != null)
                            {
                                // 重複していたら編集をキャンセルする.
                                row.ItemArray = new string[] { editBeginItemRow[0], editBeginItemRow[1], editBeginItemRow[2] };
                                SystemSounds.Exclamation.Play();
                                return;
                            }
                            row.ItemArray = new string[] { newdirectory, newfilename, newextension };
                            TAHDataBaseEntry oldentry = database.GetEntry(key);
                            oldentry.path = newkey;
                            database.DeleteEntry(key);
                            database.AddContent(oldentry);
                        }
                    }
                }
            }
        }
#endif
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
