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
    public static class TAHDump
    {
        public static string arcspath;
        public static string zipspath;
        public static string zipcoderegexp;
        public static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
#if false
        public static void ArcsDumpTAHEntries(string source_file,ArcsDatabase db)
        {
            // 既にdb上にエントリがあるか調べる.
            string tahid = db.GetTahID(source_file.Substring(arcspath.Length + 1));
            if (tahid != null)
            {
                // 該当するエントリの存在フラグを立てる.
                TDCGExplorer.SetToolTips("Update " + Path.GetFileName(source_file) );
                db.UpdateTahExistUp(tahid.ToString());
            }
            else
            {
                using (FileStream source = File.OpenRead(source_file))
                {
                    ArcsDumpTAHEntries(source, db, source_file);
                }
            }
        }
#endif
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
#if false
                ArcsDumpTAHEntries(file,db);
#endif
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
            TDCGExplorer.SetToolTips("Processing " + Path.GetFileName(tahname));
            TAHFile tah = new TAHFile(source);
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
            ArcsDumpTahFilesEntries(db, entry);
#if false
            // 衝突クラスタを構築する.
            int count = 0;
            foreach (TAHEntry ent in tah.EntrySet.Entries)
            {
                CollisionClusterRecord collision = new CollisionClusterRecord();
                collision.tahid = id;
                if (ent.FileName == null) collision.path = ""; //count.ToString("d8") + "_" + ent.Hash.ToString("x8");
                else collision.path = ent.FileName.ToLower();
                collision.hash = (int)ent.Hash;
                db.SetCollisionRecord(collision);
                count++;
            }
#endif
        }

        public static void ArcsDumpTahFilesEntries(ArcsDatabase db,ArcsTahEntry entry)
        {
            string source = Path.Combine(TDCGExplorer.GetSystemDatabase().arcs_path, entry.path);

            TAHFile tah = new TAHFile(source);
            try
            {
                tah.LoadEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
                return;
            }

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
#if false
                // TSOだけ実体化してmd5sumを計算する.
                if(ent.FileName!=null && Path.GetExtension(ent.FileName).ToLower() == ".tso")
                {
                    ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                    byte[] hash = md5.ComputeHash(data);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                        sb.Append(b.ToString("x2"));
                    fileentry.id = 0;
                    fileentry.tahid = entry.id;
                    fileentry.tahentry = tahentry++;
                    fileentry.path = ent.FileName;
                    if (entry.path == null) entry.path = "";
                    fileentry.md5sum = sb.ToString();
                    fileentry.hash = (int) ent.Hash;
                    fileentry.length = (int)ent.Length;
                    db.SetTahFilesPath(fileentry);
                }
                else
                {

                    ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                    fileentry.id = 0;
                    fileentry.tahid = entry.id;
                    fileentry.tahentry = tahentry++;
                    fileentry.path = ent.FileName;
                    if (entry.path == null) entry.path = "";
                    fileentry.md5sum = "";
                    fileentry.hash = (int)ent.Hash;
                    fileentry.length = (int)ent.Length;
                    db.SetTahFilesPath(fileentry);
                }
#endif
                ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                fileentry.id = 0;
                fileentry.tahid = entry.id;
                fileentry.tahentry = tahentry++;
                fileentry.path = ent.FileName;
                if (entry.path == null) entry.path = "";
//              fileentry.md5sum = "";
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
                zipcoderegexp = TDCGExplorer.GetSystemDatabase().zip_regexp;
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
                        TDCGExplorer.SetToolTips("Update " + zipname.Substring(zipspath.Length + 1));
                        db.UpdateZipExistUp(zip.id);
                        continue;
                    }
                }

                // MOD名に一致するか調べる.
                char[] separetor = { '\\', '/' };
                string[] sublevel = zipname.Split(separetor);
                string directory = sublevel[sublevel.Length - 1];

                Regex regDirectAccess = new System.Text.RegularExpressions.Regex(TDCGExplorer.GetSystemDatabase().directaccess_signature);
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
#if false
        public static void ZipsDumpFileEntries(string source_file, ArcsDatabase db)
        {
            // 既にdb上にエントリがあるか調べる.
           string zipid = db.GetZipID(source_file.Substring(zipspath.Length + 1));
           if (zipid != null)
            {
                // 該当するエントリの存在フラグを立てる.
                TDCGExplorer.SetToolTips("Update " + Path.GetFileName(source_file) );
                db.UpdateZipExistUp(zipid);
            }
            else
            {
                ZipsDumpTAHEntries(db, source_file);
            }
        }
#endif
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
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, entry.path), arc, entry.id);
                    }
                    break;
                case ".rar":
                    using (IArchive arc = new RarArchive())
                    {
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, entry.path), arc, entry.id);
                    }
                    break;
                case ".lzh":
                    using (IArchive arc = new LzhArchive())
                    {
                        DumpArcEntries(db, Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, entry.path), arc, entry.id);
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
                arc.Open(source_file);
                if (arc == null)
                    return;

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

                            TAHFile tah = new TAHFile(ms);
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
#if false
                                // TSOだけ実体化してmd5sumを計算する.
                                if (ent.FileName != null && Path.GetExtension(ent.FileName).ToLower() == ".tso")
                                {
                                    ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                                    byte[] hash = md5.ComputeHash(data);
                                    StringBuilder sb = new StringBuilder();
                                    foreach (byte b in hash)
                                        sb.Append(b.ToString("x2"));
                                    fileentry.id = 0;
                                    fileentry.tahid = tahid;
                                    fileentry.tahentry = tahentry++;
                                    fileentry.path = ent.FileName;
                                    if (fileentry.path == null) fileentry.path = "";
                                    fileentry.md5sum = sb.ToString();
                                    fileentry.hash = (int)ent.Hash;
                                    fileentry.length = (int)ent.Length;
                                    db.SetZipTahFilesPath(fileentry);
                                }
                                else
                                {
                                    ArcsTahFilesEntry fileentry = new ArcsTahFilesEntry();
                                    fileentry.id = 0;
                                    fileentry.tahid = tahid;
                                    fileentry.tahentry = tahentry++;
                                    fileentry.path = ent.FileName;
                                    if (fileentry.path == null) fileentry.path = "";
                                    fileentry.md5sum = "";
                                    fileentry.hash = (int)ent.Hash;
                                    fileentry.length = (int)ent.Length;
                                    db.SetZipTahFilesPath(fileentry);
                                }
#endif
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

#if false
        public static void MakeCollisionMap(ArcsDatabase db)
        {
            // 設計中…未完了

            // 全ての衝突レコードを取得する(重複を含む).
            List<CollisionClusterRecord> collisions = db.GetCollisionRecords();

            Dictionary<int,List<CollisionClusterRecord>> coltable = new Dictionary<int,List<CollisionClusterRecord>>();
            List<int> allhash = new List<int>();

            // 衝突している全てのハッシュをリスト化する.
            foreach (CollisionClusterRecord iter in collisions)
            {
                if(!coltable.ContainsKey(iter.hash)){
                    coltable[iter.hash]=new List<CollisionClusterRecord>();
                    allhash.Add(iter.hash);
                }
                coltable[iter.hash].Add(iter);
            }

            Dictionary<string, List<CollisionClusterRecord>> strtable = new Dictionary<string, List<CollisionClusterRecord>>();
            // パス名に対する衝突表を作る
            foreach (CollisionClusterRecord iter in collisions)
            {
                if (!strtable.ContainsKey(iter.path))
                {
                    strtable[iter.path] = new List<CollisionClusterRecord>();
                }
                strtable[iter.path].Add(iter);
            }
        }
#endif

    }
}
