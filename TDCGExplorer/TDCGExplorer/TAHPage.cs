using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArchiveLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Drawing;

//using System.Windows.Forms;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class TAHPageControl : Control
    {
        List<ArcsTahFilesEntry> filesEntries;
        private DataGridView dataGridView;
        private ContextMenuStrip contextMenuStrip;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem toolStripMenuItemClose;
        private ToolStripMenuItem toolStripMenuItemEditMode;
        private ToolStripMenuItem toolStripMenuItemSaveFile;
        GenericTahInfo info;

        public TAHPageControl(GenericTahInfo entryinfo, List<ArcsTahFilesEntry> filesentries)
        {
            InitializeComponent();

            info = entryinfo;
            Text = info.shortname+" version "+entryinfo.version.ToString();
            filesEntries = filesentries;

            DataTable data = new DataTable();
            data.Columns.Add("ID", Type.GetType("System.String"));
            data.Columns.Add(TextResource.Filename, Type.GetType("System.String"));
            data.Columns.Add(TextResource.FileType, Type.GetType("System.String"));
            data.Columns.Add(TextResource.HashCode, Type.GetType("System.String"));
            data.Columns.Add(TextResource.FileSize, Type.GetType("System.String"));
            data.Columns.Add(TextResource.Category, Type.GetType("System.String"));
            foreach (ArcsTahFilesEntry file in filesentries)
            {
                DataRow row = data.NewRow();
                string[] content = { file.tahentry.ToString(), file.GetDisplayPath(), Path.GetExtension(file.path), file.hash.ToString("x8"), file.length.ToString(), TDCGTbnUtil.GetCategoryText(file.GetDisplayPath()) };
                row.ItemArray = content;
                data.Rows.Add(row);
            }
            dataGridView.DataSource = data;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;

            LoadPsdFile(0);

            TDCGExplorer.TDCGExplorer.SetToolTips(info.shortname + " : " + TextResource.TSOMessage);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEditMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveFile = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEditMode,
            this.toolStripMenuItemSaveFile,
            this.toolStripMenuItemClose});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(207, 70);
            // 
            // toolStripMenuItemEditMode
            // 
            this.toolStripMenuItemEditMode.Name = "toolStripMenuItemEditMode";
            this.toolStripMenuItemEditMode.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemEditMode.Text = TextResource.EditTahFile;
            this.toolStripMenuItemEditMode.Click += new System.EventHandler(this.toolStripMenuItemEditMode_Click);
            // 
            // toolStripMenuItemSaveFile
            // 
            this.toolStripMenuItemSaveFile.Name = "toolStripMenuItemSaveFile";
            this.toolStripMenuItemSaveFile.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemSaveFile.Text = TextResource.ExtractTahFile;
            this.toolStripMenuItemSaveFile.Click += new System.EventHandler(this.toolStripMenuItemSaveFile_Click);
            // 
            // toolStripMenuItemClose
            // 
            this.toolStripMenuItemClose.Name = "toolStripMenuItemClose";
            this.toolStripMenuItemClose.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemClose.Text = TextResource.Close;
            this.toolStripMenuItemClose.Click += new System.EventHandler(this.toolStripMenuItemClose_Click);
            // 
            // TAHPageControl
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

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                System.Windows.Forms.DataGridViewRow dgr = this.dataGridView.CurrentRow;
                System.Data.DataRowView drv = (System.Data.DataRowView)dgr.DataBoundItem;
                System.Data.DataRow dr = (System.Data.DataRow)drv.Row;
                int index = int.Parse(dr.ItemArray[0].ToString());

                if (index >= 0)
                {
                    string ext = Path.GetExtension(filesEntries[index].path).ToLower();
                    if (ext == ".tso" || ext == ".tmo")
                    {
                        using (GenericTAHStream tahstream = new GenericTAHStream(info, filesEntries[index]))
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                            if (ext == ".tso")
                            {
                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                    if (TDCGExplorer.TDCGExplorer.SystemDB.loadinitialpose) TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                                }
                                else
                                {
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                    if (TDCGExplorer.TDCGExplorer.SystemDB.loadinitialpose) TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.
                                    // カメラをセンター位置に.
                                    TSOCameraAutoCenter camera = new TSOCameraAutoCenter(TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer);
                                    camera.UpdateCenterPosition(Path.GetFileName(filesEntries[index].path).ToUpper());
                                    // 次回カメラが必ずリセットされる様にする.
                                    TDCGExplorer.TDCGExplorer.MainFormWindow.setNeedCameraReset();
                                }
                            }
                            else if (ext == ".tmo")
                            {
                                TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTMOFile(tahstream.stream);
                                TDCGExplorer.TDCGExplorer.defaultpose = tahstream.stream;
                            }
                            Cursor.Current = Cursors.Default;
                            //TDCGExplorer.TDCGExplorer.FigureLoad = false;
                        }
                    }
                    LoadPsdFile(index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(TextResource.Error + ":" + ex.Message, TextResource.Error, MessageBoxButtons.OK);
            }
        }

        private void LoadPsdFile(int index)
        {
            try
            {
                string ext = Path.GetExtension(filesEntries[index].path).ToLower();
                string psdfilename = filesEntries[index].path;
                string psdpath = "data/icon/items/";
                if (Path.GetDirectoryName(filesEntries[index].path).ToLower() == "script\\backgrounds")
                    psdpath = "data/icon/backgrounds/";
                if (ext != ".psd")
                {
                    string fname = Path.GetFileNameWithoutExtension(psdfilename);
                    psdfilename = psdpath + fname + ".psd";
                }
                psdfilename = psdfilename.ToLower();
                foreach (ArcsTahFilesEntry fentry in filesEntries)
                {
                    if (fentry.path.ToLower() == psdfilename)
                    {
                        using (GenericTAHStream tahstream = new GenericTAHStream(info, fentry))
                        {
                            PSDFile psd = new PSDFile();
                            psd.Load(tahstream.stream);
#if false
                            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = psd.Bitmap;
                            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = psd.Bitmap.Width;
                            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = psd.Bitmap.Height;
#else
                            TDCGExplorer.TDCGExplorer.MainFormWindow.SetBitmap(psd.Bitmap);
#endif
                        }
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
            NoImageIcon();
        }

        private void NoImageIcon()
        {
            Bitmap noimage = new Bitmap("noimage.jpg");
#if false
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Image = noimage;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Width = noimage.Width;
            TDCGExplorer.TDCGExplorer.MainFormWindow.PictureBox.Height = noimage.Height;
#else
            TDCGExplorer.TDCGExplorer.MainFormWindow.SetBitmap(noimage);
#endif
        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        // ページを閉じる
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            Parent.Dispose();
        }

        // 編集モードに切り替える
        private void toolStripMenuItemEditMode_Click(object sender, EventArgs e)
        {
            if (TDCGExplorer.TDCGExplorer.BusyTest()) return;
            LBFileTahUtl.OpenTahEditor(info);
        }

        private void toolStripMenuItemSaveFile_Click(object sender, EventArgs e)
        {
            TDCGExplorer.TDCGExplorer.TAHDecrypt(new GenericZipsTahInfo(info));
        }
    }
}
