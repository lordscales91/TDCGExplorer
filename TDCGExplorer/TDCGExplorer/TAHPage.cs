using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArchiveLib;
using System.Diagnostics;

namespace TDCGExplorer
{
    class TAHPage : TabPage
    {
        private ListBox listBox;
        List<ArcsTahFilesEntry> filesEntries;
        GenTahInfo info;

        public TAHPage(GenTahInfo entryinfo, List<ArcsTahFilesEntry> filesentries)
        {
            InitializeComponent();

            info = entryinfo;
            Text = info.shortname;
            filesEntries = filesentries;

            foreach (ArcsTahFilesEntry file in filesentries)
            {
                listBox.Items.Add(file.GetDisplayPath());
            }
        }

        private void InitializeComponent()
        {
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(200, 95);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // TAHPage
            // 
            this.Controls.Add(this.listBox);
            this.ResumeLayout(false);

        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = listBox.SelectedIndex;
                if (index >= 0)
                {
                    string ext = Path.GetExtension(filesEntries[index].path).ToLower();
                    if (ext == ".tso" || ext == ".tmo")
                    {
                        ArcsTahFilesEntry tsoInfo = filesEntries[index];
                        // zipファイルの中か?
                        if (info.zipid != -1)
                        {
                            IArchive archive;
                            ArcsZipArcEntry zip = TDCGExplorer.GetArcsDatabase().GetZip(info.zipid);
                            string zippath = TDCGExplorer.GetSystemDatabase().zips_path + "\\" + zip.path;
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
                                            }
                                            tahentry++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string source = TDCGExplorer.GetSystemDatabase().arcs_path + "\\" + info.path;
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
                                        TDCGExplorer.GetMainForm().makeTSOViwer();
                                        if (ext == ".tso")
                                        {
                                            TDCGExplorer.GetMainForm().Viewer.LoadTSOFile(ims);
                                            TDCGExplorer.GetMainForm().doInitialTmoLoad(); // 初期tmoを読み込む.
                                        }
                                        else if (ext == ".tmo") TDCGExplorer.GetMainForm().Viewer.LoadTMOFile(ims);
                                    }
                                }
                                tahentry++;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("TAH INFO:\n" + filesEntries[index].id + "," + filesEntries[index].path + "," + filesEntries[index].hash.ToString("x8"), "Not Implemented", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Not Implemented", "Not Implemented", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured:"+ex.Message, "Error", MessageBoxButtons.OK);
            }
        }
    }
}
