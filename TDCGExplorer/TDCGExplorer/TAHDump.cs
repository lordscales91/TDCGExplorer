using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ArchiveLib;
using System.Diagnostics;
using System.Data.SQLite;

namespace TDCGExplorer
{
    public static class TDCGTAHDump
    {
        public static string arcspath;
        public static string zipspath;
        public static string zipcoderegexp;
        public static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public static void ArcsDumpDirEntriesMain(string dir, ArcsDatabase db)
        {
            arcspath = dir;
            if (Directory.Exists(dir) == true)
            {
                // 存在フラグを全て落とす.
                TDCGExplorer.SetToolTips("Setup database");
                db.UpdateTahExistDown();
                ArcsDumpDirEntries(dir, db);
                // 存在しないtahファイルは消去する.
                TDCGExplorer.SetToolTips("Deleting orphan records");
                db.DeleteNoExistentTah();
            }
        }

        public static void ArcsDumpDirEntries(string dir,ArcsDatabase db)
        {
            bool skipflag = false;
            string[] shortnames = dir.Split('\\');
            foreach (string shortname in shortnames) if (shortname[0] == '!') skipflag = true;
            if (skipflag) return; // !で始まるディレクトリはスキップ

            string[] tah_files = Directory.GetFiles(dir, "*.TAH");
            foreach (string file in tah_files)
            {
                // 既にdb上にエントリがあるか調べる.
                //string tahid = db.GetTahID(file.Substring(arcspath.Length + 1));
                ArcsTahEntry tah = db.GetTah(file.Substring(arcspath.Length + 1));
                if (tah != null)
                {
                    // 日付が一致するか?
                    DateTime datetime = File.GetLastWriteTime(file);
                    if (tah.datetime.ToString() == datetime.ToString())
                    {
                        // 該当するエントリの存在フラグを立てる.
                        TDCGExplorer.SetToolTips("Update " + Path.GetFileName(file));
                        db.UpdateTahExistUp(tah.id);
                        continue;
                    }
                    else
                    {
                        // dbから一旦削除する.
                        db.DeleteTah(tah.id);
                    }
                }
                TDCGExplorer.SetLastAccessFile = file;
                using (FileStream source = File.OpenRead(file))
                {
                    ArcsDumpTAHEntries(source, db, file);
                }
            }
            string[] entries = Directory.GetDirectories(dir);
            foreach (string entry in entries)
            {
                ArcsDumpDirEntries(entry,db);
            }
        }

        public static void ArcsDumpTAHEntries(Stream source, ArcsDatabase db,string tahname)
        {
            try
            {
                TDCGExplorer.SetToolTips("Processing " + Path.GetFileName(tahname));
                using (TAHFile tah = new TAHFile(source))
                {
                    try
                    {
                        tah.LoadEntries();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error: " + ex);
                        return;
                    }

                    DateTime datetime = File.GetLastWriteTime(tahname);

                    ArcsTahEntry entry = new ArcsTahEntry();
                    entry.path = tahname.Substring(arcspath.Length + 1);
                    entry.shortname = Path.GetFileName(tahname).ToLower();
                    entry.version = (int)tah.Header.Version;
                    entry.id = 0;
                    entry.exist = 1;
                    entry.datetime = datetime;

                    entry.id = db.SetTahEntry(entry);
                    ArcsDumpTahFilesEntries(db, entry, tah);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                return;
            }
        }

        public static void ArcsDumpTahFilesEntries(ArcsDatabase db, ArcsTahEntry entry, TAHFile tah)
        {
            string source = Path.Combine(TDCGExplorer.SystemDB.arcs_path, entry.path);
            int tahentry = 0;
            foreach (TAHEntry ent in tah.EntrySet.Entries)
            {
                if (ent.FileName == null)
                {
                    TDCGExplorer.SetToolTips("Dump " + ent.Hash.ToString("x8") + " file");
                }
                else
                {
                    TDCGExplorer.SetToolTips("Dump " + ent.FileName + " file");
                }
                ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                fileentry.id = 0;
                fileentry.tahid = entry.id;
                fileentry.tahentry = tahentry++;
                fileentry.path = ent.FileName;
                if (entry.path == null) entry.path = "";
                fileentry.hash = (int)ent.Hash;
                fileentry.length = (int)ent.Length;
                db.SetTahFilesPath(fileentry);

            }
        }

        //ファイルシステムをスキャンしてZIP情報を集める.
        public static void ZipsDumpDirEntriesMain(string dir, ArcsDatabase db)
        {
            zipspath = dir;
            if (Directory.Exists(dir) == true)
            {
                zipcoderegexp = TDCGExplorer.SystemDB.zip_regexp;
                // 存在フラグを全て落とす.
                TDCGExplorer.SetToolTips("Setup database");
                db.UpdateZipExistDown();
                ZipsDumpDirEntries(dir, db);
                // 存在しないtahファイルは消去する.
                TDCGExplorer.SetToolTips("Deleting orphan records");
                db.DeleteNoExistentZip();
            }
        }

        public static void ZipsDumpDirEntries(string dir, ArcsDatabase db)
        {
            // ファイルを思わしき者は全部調べて、その中からzip,lzh,rarを抽出する.
            string[] zip_files = Directory.GetFiles(dir, "*.*");
            foreach (string file in zip_files)
            {
                //string zipid = db.GetZipID(file.Substring(zipspath.Length + 1));
                ArcsZipArcEntry zip = db.GetZip(file.Substring(zipspath.Length + 1));
                if (zip != null)
                {
                    DateTime datetime = File.GetLastWriteTime(file);
                    if (zip.datetime.ToString() == datetime.ToString())
                    {
                        // 該当するエントリの存在フラグを立てる.
                        TDCGExplorer.SetToolTips("Update " + Path.GetFileName(file));
                        db.UpdateZipExistUp(zip.id);
                        continue;
                    }
                    else
                    {
                        db.DeleteZip(zip.id);
                    }
                }
                ZipsDumpTAHEntries(db, file);
            }
            string[] entries = Directory.GetDirectories(dir);
            foreach (string entry in entries)
            {
                // 解凍済みMODを処理する.
                string zipname = entry.Substring(zipspath.Length + 1);
                ArcsZipArcEntry zip = db.GetZip(zipname);
                if (zip != null)
                {
                    DateTime datetime = File.GetLastWriteTime(entry);
                    if (zip.datetime.ToString() == datetime.ToString())
                    {
                        // 該当するエントリの存在フラグを立てる.
                        TDCGExplorer.SetToolTips("Update " + Path.GetFileName(zip.path));
                        db.UpdateZipExistUp(zip.id);
                        continue;
                    }
                }

                // MOD名に一致するか調べる.
                char[] separetor = { '\\', '/' };
                string[] sublevel = zipname.Split(separetor);
                string directory = sublevel[sublevel.Length - 1];

                Regex regDirectAccess = new System.Text.RegularExpressions.Regex(TDCGExplorer.SystemDB.directaccess_signature);
                Match m = regDirectAccess.Match(directory);
                if (m.Success)
                {
                    Regex filter = new Regex(zipcoderegexp);
                    Match match = filter.Match(directory);
                    if (match.Success == true)
                    {
                        TDCGExplorer.SetToolTips("Processing " + directory);
                        ArcsZipArcEntry ent = new ArcsZipArcEntry();
                        ent.id = 0;
                        ent.path = zipname;
                        ent.code = match.Groups[1].ToString();
                        ent.exist = 1;
                        ent.datetime = File.GetLastWriteTime(entry);
                        ent.id = db.SetZipEntry(ent);
                        DumpArcEntries(db, entry, new DirectAccessArchive(), ent.id);
                        continue;
                    }
                }

                // 通常のディレクトリスキャン.
                ZipsDumpDirEntries(entry, db);
            }
        }

        public static void ZipsDumpTAHEntries(ArcsDatabase db, string zipname)
        {
            string ext=Path.GetExtension(zipname).ToLower();
            if (ext == ".zip" || ext == ".lzh" || ext == ".rar")
            {
                TDCGExplorer.SetToolTips("Processing " + Path.GetFileName(zipname));
                Regex filter = new Regex(zipcoderegexp);
                Match match = filter.Match(Path.GetFileName(zipname));
                if (match.Success)
                {
                    DateTime datetime = File.GetLastWriteTime(zipname);
                    ArcsZipArcEntry entry = new ArcsZipArcEntry();
                    entry.id = 0;
                    entry.path = zipname.Substring(zipspath.Length + 1);
                    entry.code = match.Groups[1].ToString();
                    entry.exist = 1;
                    entry.datetime = datetime;
                    entry.id = db.SetZipEntry(entry);
                    ZipDumpArcEntries(db, entry);
                }
            }
        }

        public static void ZipDumpArcEntries(ArcsDatabase db,ArcsZipArcEntry entry)
        {
            string ext = Path.GetExtension(entry.path).ToLower();
            switch (ext)
            {
                case ".zip":
                    using (IArchive arc = new ZipArchive())
                    {
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.SystemDB.zips_path, entry.path), arc, entry.id);
                    }
                    break;
                case ".rar":
                    using (IArchive arc = new RarArchive())
                    {
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.SystemDB.zips_path, entry.path), arc, entry.id);
                    }
                    break;
                case ".lzh":
                    using (IArchive arc = new LzhArchive())
                    {
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.SystemDB.zips_path, entry.path), arc, entry.id);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void DumpArcEntries(ArcsDatabase db, string source_file, IArchive arc, int id)
        {
            try
            {
                TDCGExplorer.SetLastAccessFile = source_file;
                arc.Open(source_file);

                foreach (IArchiveEntry entry in arc)
                {
                    // ディレクトリのみの場合はスキップする.
                    if (entry.IsDirectory==true) continue;

                    // TAHファイルなら詳細をダンプする.
                    if (Path.GetExtension(entry.FileName) == ".tah")
                    {
                        using (MemoryStream ms = new MemoryStream((int)entry.Size))
                        {
                            arc.Extract(entry, ms);
                            ms.Seek(0, SeekOrigin.Begin);

                            using (TAHFile tah = new TAHFile(ms))
                            {
                                try
                                {
                                    tah.LoadEntries();
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Error: " + ex);
                                    continue;
                                }

                                ArcsZipTahEntry ziptahentry = new ArcsZipTahEntry();
                                ziptahentry.id = 0;
                                ziptahentry.path = entry.FileName;
                                ziptahentry.shortname = Path.GetFileName(entry.FileName).ToLower();
                                ziptahentry.version = (int)tah.Header.Version;
                                ziptahentry.zipid = id;
                                int tahid = db.SetZipTahEntry(ziptahentry);

                                int tahentry = 0;
                                foreach (TAHEntry ent in tah.EntrySet.Entries)
                                {
                                    if (ent.FileName == null)
                                    {
                                        TDCGExplorer.SetToolTips("Dump " + ent.Hash.ToString("x8") + " file");
                                    }
                                    else
                                    {
                                        TDCGExplorer.SetToolTips("Dump " + Path.GetFileName(ent.FileName) + " file");
                                    }
                                    ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                                    fileentry.id = 0;
                                    fileentry.tahid = tahid;
                                    fileentry.tahentry = tahentry++;
                                    fileentry.path = ent.FileName;
                                    if (fileentry.path == null) fileentry.path = "";
                                    //fileentry.md5sum = "";
                                    fileentry.hash = (int)ent.Hash;
                                    fileentry.length = (int)ent.Length;
                                    db.SetZipTahFilesPath(fileentry);

                                }
                            }
                        }
                    }
                    else
                    {
                        // tahファイル以外はファイル名のみ情報を格納する.
                        ArcsZipTahEntry ziptahentry = new ArcsZipTahEntry();
                        ziptahentry.id = 0;
                        ziptahentry.path = entry.FileName;
                        ziptahentry.version = 0;
                        ziptahentry.zipid = id;
                        int tahid = db.SetZipTahEntry(ziptahentry);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                return;
            }
        }
    }
}
