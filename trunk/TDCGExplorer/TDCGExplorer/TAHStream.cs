using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;
using System.IO;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public class GenericTAHStream : IDisposable
    {
        private IArchive archive = null;
        private MemoryStream ms = null;
        private TAHFile tah = null;
        private MemoryStream ims = null;

        public GenericTAHStream(GenericTahInfo info, ArcsTahFilesEntry tsoInfo)
        {
            // zipファイルの中か?
            if (info.zipid != -1)
            {
                ArcsZipArcEntry zip = TDCGExplorer.ArcsDB.GetZip(info.zipid);
                string zippath = Path.Combine(TDCGExplorer.SystemDB.zips_path, zip.path);
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
                        archive = new DirectAccessArchive();
                        break;

                }
                TDCGExplorer.SetLastAccessFile=zippath;
                archive.Open(zippath);
                if (archive == null)
                {
                    throw new Exception("archiveがnullになりました");
                }

                // 
                foreach (IArchiveEntry entry in archive)
                {
                    // ディレクトリのみの場合はスキップする.
                    if (entry.IsDirectory == true) continue;
                    // マッチするファイルを見つけた.
                    if (entry.FileName == info.path)
                    {
                        ms = new MemoryStream((int)entry.Size);

                        archive.Extract(entry, ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        tah = new TAHFile(ms);
                        tah.LoadEntries();
                        if (tsoInfo == null) return;
                        int tahentry = 0;
                        foreach (TAHEntry ent in tah.EntrySet.Entries)
                        {
                            // 該当ファイルを見つけた.
                            if (tahentry == tsoInfo.tahentry)
                            {
                                byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                                // 
                                Cursor.Current = Cursors.WaitCursor;
                                ims = new MemoryStream(data);
                                return;
                            }
                            tahentry++;
                        }
                    }
                }
            }
            else
            {
                string source = Path.Combine(TDCGExplorer.SystemDB.arcs_path, info.path);
                tah = new TAHFile(source);
                tah.LoadEntries();
                if (tsoInfo == null) return;
                int tahentry = 0;
                foreach (TAHEntry ent in tah.EntrySet.Entries)
                {
                    // 該当ファイルを見つけた.
                    if (tahentry == tsoInfo.tahentry)
                    {
                        byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                        // 
                        ims = new MemoryStream(data);
                        return;
                    }
                    tahentry++;
                }
            }
            throw new Exception("TAH内のファイルが見つかりません");
        }

        public Stream stream
        {
            get
            {
                if (ims == null)
                {
                    throw new Exception("TAHStreamは初期化されませんでした");
                }
                return ims;
            }
            set { }
        }

        public TAHFile tahfile
        {
            get
            {
                if (this.tah == null)
                {
                    throw new Exception("TAHStreamは初期化されませんでした");
                }
                return tah;
            }
            set { }
        }

        // 確実に開放する.
        public void Dispose()
        {
            if (ims != null)
            {
                ims.Dispose();
                ims = null;
            }
            if (tah != null)
            {
                tah.Dispose();
                tah = null;
            }
            if (ms != null)
            {
                ms.Dispose();
                ms = null;
            }
            if (archive != null)
            {
                archive.Dispose();
                archive = null;
            }
        }
    }
}
