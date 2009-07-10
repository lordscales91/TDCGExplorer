using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;

//using System.Windows.Forms;
using TDCGExplorer;

namespace System.Windows.Forms
{
    class ZipFilePageControl : Control
    {
        GenTahInfo info;

        public ZipFilePageControl()
        {
            info = null;
        }

        public ZipFilePageControl(GenTahInfo tahInfo)
        {
            info = tahInfo;
            InitializeComponent();
            Text = Path.GetFileName(tahInfo.path);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ZipFilePageControl
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResumeLayout(false);

        }

        public void ExtractFile()
        {
            if (info.zipid != -1)
            {
                ArcsZipArcEntry zipEntry = TDCGExplorer.TDCGExplorer.ArcsDB.GetZip(info.zipid);
                string zipsource = Path.Combine(TDCGExplorer.TDCGExplorer.SystemDB.zips_path, zipEntry.path);
                switch (Path.GetExtension(zipEntry.path.ToLower()))
                {
                    case ".zip":
                        using (IArchive arc = new ZipArchive())
                        {
                            ExtractFile(arc, zipsource);
                        }
                        break;
                    case ".rar":
                        using (IArchive arc = new RarArchive())
                        {
                            ExtractFile(arc, zipsource);
                        }
                        break;
                    case ".lzh":
                        using (IArchive arc = new LzhArchive())
                        {
                            ExtractFile(arc, zipsource);
                        }
                        break;
                    default:
                        using (IArchive arc = new DirectAccessArchive())
                        {
                            ExtractFile(arc, zipsource);
                        }
                        break;
                }
            }
        }
        private void ExtractFile(IArchive arc, string source_file)
        {
            arc.Open(source_file);
            if (arc == null)
                return;

            foreach (IArchiveEntry entry in arc)
            {
                if (entry.FileName == info.path)
                {
                    using (MemoryStream ms = new MemoryStream((int)entry.Size))
                    {
                        arc.Extract(entry, ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        BindingStream(ms);
                    }
                }
            }
        }
        public virtual void BindingStream(MemoryStream ms)
        {
        }
    }
}
