using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;
using System.IO;
using System.Diagnostics;

namespace TDCGExplorer
{
    public static class ZipFileUtil
    {
        // ファイルを展開する.
        private static bool ExtractZip(IArchive arc, string srcfile, string destpath)
        {
            try
            {
                TDCGExplorer.SetLastAccessFile = srcfile;
                arc.Open(srcfile);
                if (arc == null) return false;
                foreach (IArchiveEntry entry in arc)
                {
                    if (entry.IsDirectory) continue;
                    if (Path.GetFileName(entry.FileName) == "") continue;
                    using (MemoryStream ms = new MemoryStream((int)entry.Size))
                    {
                        arc.Extract(entry, ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(destpath, entry.FileName)));

                        Stream fileStream = File.OpenWrite(Path.Combine(destpath, entry.FileName));

                        BufferedStream bufferedDataStream = new BufferedStream(ms);
                        BufferedStream bufferedFileStream = new BufferedStream(fileStream);
#if false
                        byte[] buf = new byte[1024];
                        int len;
                        while ((len = bufferedDataStream.Read(buf, 0, buf.Length)) > 0)
                        {
                            bufferedFileStream.Write(buf, 0, len);
                        }
#endif
                        CopyStream(bufferedDataStream, bufferedFileStream);

                        bufferedFileStream.Flush();
                        bufferedFileStream.Close();
                        bufferedDataStream.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }
            return false;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buf = new byte[1024];
            int len;
            while ((len = input.Read(buf, 0, buf.Length)) > 0)
            {
                output.Write(buf, 0, len);
            }
        }

        // ZIPファイルを展開する.
        public static bool ExtractZipFile(string srcfile, string destpath)
        {
            switch (Path.GetExtension(srcfile.ToLower()))
            {
                case ".zip":
                    using (IArchive arc = new ZipArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                case ".rar":
                    using (IArchive arc = new RarArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                case ".lzh":
                    using (IArchive arc = new LzhArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                default:
                    using (IArchive arc = new DirectAccessArchive())
                    {
                        return ExtractZip(arc, srcfile, destpath);
                    }
            }
        }
    }
}
