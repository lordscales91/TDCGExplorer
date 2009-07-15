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
        private string tahdbpath;

        public TAHEditor(string dbpath, GenericTahInfo info)
        {
            tahdbpath = dbpath.ToLower();

            InitializeComponent();

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

        protected override void Dispose(bool disposing)
        {
            if (database != null)
            {
                database.Dispose();
                database = null;
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
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemTAHInfoEdit,
            this.toolStripMenuItemSaveSelectFile,
            this.toolStripMenuItemEditIdentify,
            this.toolStripMenuItemEditCategory,
            this.toolStripMenuItemSaveTAHFile,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(291, 136);
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
            // toolStripMenuItemSaveTAHFile
            // 
            this.toolStripMenuItemSaveTAHFile.Name = "toolStripMenuItemSaveTAHFile";
            this.toolStripMenuItemSaveTAHFile.Size = new System.Drawing.Size(290, 22);
            this.toolStripMenuItemSaveTAHFile.Text = "TAHファイルを保存する";
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
                            using (Stream stream = File.OpenWrite(destfile))
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
                            string newpath = entry[0].ToString() + newname + entry[1].ToString().Substring(newname.Length);
                            // ファイル名を書き換える
                            TAHLocalDbEntry tahentry = database.GetEntry(orgpath);
                            database.DeleteEntry(orgpath);
                            tahentry.path=newpath;
                            database.AddContent(tahentry);
                            Debug.WriteLine(newpath);
                            // TBNなら中身も書き換える
                            if(Path.GetExtension(orgpath).ToLower()==".tbn"){
                                TAHLocalDBDataEntry dataentry = database.GetData(tahentry.dataid);
                                string orgtsoepath = TDCGTbnUtil.GetTsoName(dataentry.data);
                                string[] pathelement = orgtsoepath.Split('/');
                                string tsopath = "";
                                for(int i=0;i<(pathelement.Length-1);i++)
                                    tsopath+=pathelement[i]+"/";
                                string newtsopath = tsopath + newname + pathelement[pathelement.Length - 1].Substring(newname.Length);
                                TDCGTbnUtil.SetTsoName(dataentry.data,newtsopath);
                                database.UpdateData(dataentry);
                                Debug.WriteLine("tbn:" + orgtsoepath + ">" + TDCGTbnUtil.GetTsoName(dataentry.data));
                            }
                            // データグリッドを更新する.
                            string[] newitem = { entry[0].ToString(), newname + entry[1].ToString().Substring(newname.Length), entry[2].ToString() };
                            row.ItemArray = newitem;
                        }
                    }
                }
            }
        }

        // 選択しているファイルの名前を変更する
        private void toolStripMenuItemEditIdentify_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            ReplaceDialog dialog = new ReplaceDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ReplaceFileName(dialog.textTo);
            }
        }

        // 選択しているファイルのカテゴリを変更する
        private void toolStripMenuItemEditCategory_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;

            MessageBox.Show("次のバージョンでやるから待って", "エラー", MessageBoxButtons.OK);
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
            File.Delete(destfilename);
            int count = 1;
            using (Stream stream = File.OpenWrite(destfilename))
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
            }
            TDCGExplorer.TDCGExplorer.SetToolTips("梱包完了");

        }

        // TAHの情報を変更する.
        private void toolStripMenuItemTAHInfoEdit_Click(object sender, EventArgs e)
        {
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
