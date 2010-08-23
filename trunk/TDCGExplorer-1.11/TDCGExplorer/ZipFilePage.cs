using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArchiveLib;

namespace TDCGExplorer
{
    class ZipFilePage : TabPage
    {
        GenTahInfo info;

        public ZipFilePage()
        {
            info = null;
        }

        public ZipFilePage(GenTahInfo tahInfo)
        {
            info = tahInfo;
            InitializeComponent();
            Text = Path.GetFileName(tahInfo.path);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ZipFilePage
            // 
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }

        public void ExtractFile()
        {
            if (info.zipid != -1)
            {
                ArcsZipArcEntry zipEntry = TDCGExplorer.GetArcsDatabase().GetZip(info.zipid);
                string zipsource = Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, zipEntry.path);
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
