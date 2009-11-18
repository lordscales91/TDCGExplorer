using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace TDCGExplorer
{
    public class ArcsTahEntry
    {
        public int id;
        public string path;
        public string shortname;
        public int exist;
        public int version;
        public DateTime datetime;
    }
    public class ArcsTahFilesEntry
    {
        public int id;
        public int tahid;
        public int tahentry;
        public string path;
        public int hash;
        public int length;

        public string GetDisplayPath()
        {
            if(path==""){
                return tahentry.ToString("d8")+"_"+hash.ToString("x8");
            }else{
                return path;
            }
        }
    }
    public class ArcsZipArcEntry
    {
        public int id;
        public string path;
        public string code;
        public int exist;
        public DateTime datetime; 

        public string GetDisplayPath()
        {
            // アノテーションが入力されているならそれを表示する.
            if (TDCGExplorer.AnnDB.annotation.ContainsKey(code) == true)
            {
                return TDCGExplorer.AnnDB.annotation[code];
            }
            // リネームされていない場合で.
            if (Path.GetFileNameWithoutExtension(path) == code)
            {
                // arcsnamesに登録がある場合
                if (TDCGExplorer.Arcsnames.ContainsKey(code) == true)
                {
                    ArcsNamesEntry arc = TDCGExplorer.Arcsnames[code];
                    return arc.code + " " + arc.summary + " <" + arc.origname + ":" + arc.location + ">";
                }
            }
            // そうでなければ元のファイル名を返す.
            return Path.GetFileName(path);
        }
    }
    public class ArcsZipTahEntry
    {
        public int id;
        public int zipid;
        public string path;
        public string shortname;
        public int version;
    }
    public class ArcsCollisionRecord
    {
        public int fromTahID;
        public int toTahID;
        public int fromFilesID;
        public int toFilesID;
    }
    public class ArcsDatabase : IDisposable
    {
        private SQLiteConnection cnn;

        public string GetArcsDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath());
        }

        public string GetArcsDatabaseName()
        {
            return Path.Combine(GetArcsDatabasePath(), "arcs.db");
        }

        public ArcsDatabase()
        {
            Directory.CreateDirectory(GetArcsDatabasePath());
            cnn = new SQLiteConnection("Data Source=" + GetArcsDatabaseName());
            cnn.Open();
        }

        public SQLiteConnection MakeClone()
        {
            return (SQLiteConnection)cnn.Clone();
        }

        public ArcsDatabase(ArcsDatabase cloneFrom)
        {
            cnn = cloneFrom.MakeClone();
        }

        public void Dispose()
        {
            if (cnn != null)
            {
                cnn.Close();
                cnn.Dispose();
                cnn = null;
            }
        }
        //TAHテーブル
        public void CreateTahDatabase()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE TahEntry (ID INTEGER PRIMARY KEY, PATH TEXT,SHORTNAME TEXT, EXIST INTEGER, TAHVERSION INTEGER,DATETIME TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        //ファイルテーブル
        public void CreateFilesDatabase()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE FilesEntry (ID INTEGER PRIMARY KEY, TAHID INTEGER, TAHENTRY INTEGER, PATH TEXT, HASH INTEGER,LENGTH INTEGER)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        // ZIPテーブル
        public void CreateZipDatabase()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE ZipEntry (ID INTEGER PRIMARY KEY, PATH TEXT, CODE TEXT,EXIST INTEGER,DATETIME TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        //インストール済みzipテーブル.
        public void CreateInstalledZipTable()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE InstalledZipEntry (ID INTEGER PRIMARY KEY, ZIPID INTEGER)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //ZIP中のTAHテーブル
        public void CreateZipTahDatabase()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE ZipTahEntry (ID INTEGER PRIMARY KEY, PATH TEXT,SHORTNAME TEXT, TAHVERSION INTEGER,ZIPID INTEGER)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        //ZIP中のTAHファイルテーブル
        public void CreateZipTahFilesDatabase()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE ZipTahFilesEntry (ID INTEGER PRIMARY KEY, TAHID INTEGER, TAHENTRY INTEGER, PATH TEXT, HASH INTEGER,LENGTH INTEGER)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void CreateInformationTable()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE Information (ID TEXT PRIMARY KEY, VALUE TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // 値を読み出す.
        private string GetInformationValue(string id)
        {
            string value = "";
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "SELECT VALUE FROM Information WHERE ID=@id";
                    cmd.Parameters.AddWithValue("id", id);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            value = reader[0].ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return value;
        }

        // 値を設定する.
        private void SetInformationValue(string id, string value)
        {
            // 値を追加する.
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    // acpathを追加する.
                    cmd.CommandText = "INSERT OR REPLACE INTO Information (ID,VALUE) VALUES(@id,@value)";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("value", value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // informationアクセス
        public string this[string key]
        {
            get { return GetInformationValue(key); }
            set { SetInformationValue(key, value); }
        }

        //トランザクションを開始する.
        public SQLiteTransaction BeginTransaction()
        {
            return cnn.BeginTransaction();
        }

        //インデックスを作成する.
        public void CreateIndex()
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "CREATE INDEX FilesEntry_Hash ON FilesEntry(HASH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX ZipTahFilesEntry_Hash ON ZipTahFilesEntry(HASH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX FilesEntry_Path ON FilesEntry(PATH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX TahEntry_Path ON TahEntry(PATH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX ZipEntry_Path ON ZipEntry(PATH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX ZipTahEntry_Path ON ZipTahEntry(PATH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX TahEntry_SHORTNAME ON TahEntry(SHORTNAME)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX ZipTahEntry_SHORTNAME ON ZipTahEntry(SHORTNAME)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "CREATE INDEX ZipTahFilesEntry_Path ON ZipTahFilesEntry(PATH)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void DropIndex()
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "DROP INDEX FilesEntry_Hash";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX ZipTahFilesEntry_Hash";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX FilesEntry_Path";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX TahEntry_Path";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX ZipEntry_Path";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX ZipTahEntry_Path";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX TahEntry_SHORTNAME";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX ZipTahEntry_SHORTNAME";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    cmd.CommandText = "DROP INDEX ZipTahFilesEntry_Path";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void Vacuum()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "VACUUM";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        // 特定のtahを返す.
        public ArcsTahEntry GetTah(int tahid)
        {
            ArcsTahEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,SHORTNAME,EXIST,TAHVERSION,DATETIME FROM TahEntry WHERE ID=@tahid";
                cmd.Parameters.AddWithValue("tahid", tahid);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsTahEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.shortname = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.version = int.Parse(reader[4].ToString());
                        entry.datetime = DateTime.Parse(reader[5].ToString());
                        break;
                    }
                }
            }
            return entry;
        }
        // 特定のtahを返す.
        public ArcsTahEntry GetTah(string path)
        {
            ArcsTahEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,SHORTNAME,EXIST,TAHVERSION,DATETIME FROM TahEntry WHERE PATH=@path";
                cmd.Parameters.AddWithValue("path", path);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsTahEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.shortname = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.version = int.Parse(reader[4].ToString());
                        entry.datetime = DateTime.Parse(reader[5].ToString());
                        break;
                    }
                }
            }
            return entry;
        }
        // 全てのtahを返す.
        public List<ArcsTahEntry> GetTahs()
        {
            List<ArcsTahEntry> list = new List<ArcsTahEntry>();
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID,PATH,SHORTNAME,EXIST,TAHVERSION,DATETIME FROM TahEntry ORDER BY PATH";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArcsTahEntry entry = new ArcsTahEntry();
                            entry.id = int.Parse(reader[0].ToString());
                            entry.path = reader[1].ToString();
                            entry.shortname = reader[2].ToString();
                            entry.exist = int.Parse(reader[3].ToString());
                            entry.version = int.Parse(reader[4].ToString());
                            entry.datetime = DateTime.Parse(reader[5].ToString());
                            list.Add(entry);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return list;
        }

        // 特定のTAHファイルを削除する.
        public void DeleteTah(int id)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                // tahに含まれるファイルを削除する.
                cmd.CommandText = "DELETE FROM FilesEntry WHERE TAHID = '" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
            // tahを削除する.
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM TahEntry WHERE ID = '" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        private void makeTahTemporaryIndex()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE INDEX DeleteTemp_TAHID ON FilesEntry(TAHID)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void dropTahTemporaryIndex()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DROP INDEX DeleteTemp_TAHID";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // 存在しないエントリを削除する.
        public void DeleteNoExistentTah()
        {
            makeTahTemporaryIndex();
            // tahファイル一覧を取得する.
            List<ArcsTahEntry> tahs = GetTahs();
            foreach (ArcsTahEntry tah in tahs)
            {
                if (tah.exist == 0)
                {
                    TDCGExplorer.SetToolTips("Deleting " + Path.GetFileName(tah.path));
                    DeleteTah(tah.id);
                }
            }
            dropTahTemporaryIndex();
        }

        // 全てのtahの存在フラグを落とす.
        public void UpdateTahExistDown()
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "UPDATE TahEntry SET EXIST='0'";
                cmd.ExecuteNonQuery();
            }
        }

        // 指定したtahの存在フラグを立てる.
        public void UpdateTahExistUp(int id)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "UPDATE TahEntry SET EXIST='1' WHERE ID='" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        // 存在して追加するからexistは常に1
        public int SetTahEntry(ArcsTahEntry entry /*string path, int version*/)
        {
            string id = "";
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO TahEntry (PATH,SHORTNAME,EXIST,TAHVERSION,DATETIME) VALUES(@path, @shortname, '1',@version,@datetime)";
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("shortname", entry.shortname); 
                cmd.Parameters.AddWithValue("version", entry.version.ToString());
                cmd.Parameters.AddWithValue("datetime", entry.datetime.ToString());
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return int.Parse(id);
        }

        // 指定したtahに含まれるファイルを取得する.
        public List<ArcsTahFilesEntry> GetTahFilesPath(int tahid)
        {
            List<ArcsTahFilesEntry> value = new List<ArcsTahFilesEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,TAHID,TAHENTRY,PATH,HASH,LENGTH FROM FilesEntry WHERE TAHID=@tahid";
                cmd.Parameters.AddWithValue("tahid", tahid.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsTahFilesEntry entry = new ArcsTahFilesEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.tahid = int.Parse(reader[1].ToString());
                        entry.tahentry = int.Parse(reader[2].ToString());
                        entry.path = reader[3].ToString();
                        entry.hash = int.Parse(reader[4].ToString());
                        entry.length = int.Parse(reader[5].ToString());
                        value.Add(entry);
                    }
                }
            }
            return value;
        }

        // TAH内部ファイルを格納する.
        public int SetTahFilesPath(ArcsTahFilesEntry entry)
        {
            string id = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO FilesEntry (TAHID,TAHENTRY,PATH,HASH,LENGTH) VALUES(@tahid,@tahentry,@path,@hash,@length )";
                cmd.Parameters.AddWithValue("tahid",entry.tahid);
                cmd.Parameters.AddWithValue("tahentry", entry.tahentry);
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("hash", entry.hash.ToString());
                cmd.Parameters.AddWithValue("length",entry.length);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return int.Parse(id);
        }

        // ZIPエントリを追加する.
        public int SetZipEntry(ArcsZipArcEntry entry)
        {
            string id = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ZipEntry (PATH,CODE,EXIST,DATETIME) VALUES(@path,@code,@exist,@datetime)";
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("code", entry.code);
                cmd.Parameters.AddWithValue("exist", entry.exist);
                cmd.Parameters.AddWithValue("datetime", entry.datetime.ToString());
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return int.Parse(id);
        }

        // ZIPエントリを取得する.
        public List<ArcsZipArcEntry> GetZips()
        {
            List<ArcsZipArcEntry> list = new List<ArcsZipArcEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,CODE,EXIST,DATETIME FROM ZipEntry ORDER BY PATH";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsZipArcEntry entry = new ArcsZipArcEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.code = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.datetime = DateTime.Parse(reader[4].ToString());
                        list.Add(entry);
                    }
                }
            }
            return list;
        }

        // ZIPエントリを取得する.
        public ArcsZipArcEntry GetZip(int zipid)
        {
            ArcsZipArcEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,CODE,EXIST,DATETIME FROM ZipEntry WHERE ID=@zipid";
                cmd.Parameters.AddWithValue("zipid", zipid.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsZipArcEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.code = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.datetime = DateTime.Parse(reader[4].ToString());
                        break;
                    }
                }
            }
            return entry;
        }

        // ZIPエントリを取得する.
        public ArcsZipArcEntry GetZip(string path)
        {
            ArcsZipArcEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,CODE,EXIST,DATETIME FROM ZipEntry WHERE PATH=@path";
                cmd.Parameters.AddWithValue("path", path);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsZipArcEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.code = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.datetime = DateTime.Parse(reader[4].ToString());
                        break;
                    }
                }
            }
            return entry;
        }

        // ZIPエントリを取得する.
        public ArcsZipArcEntry GetZipByCode(string code)
        {
            ArcsZipArcEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,CODE,EXIST,DATETIME FROM ZipEntry WHERE CODE=@code";
                cmd.Parameters.AddWithValue("code", code);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsZipArcEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.code = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        entry.datetime = DateTime.Parse(reader[4].ToString());
                        break;
                    }
                }
            }
            return entry;
        }

#if false
        // ZIPエントリを取得する.
        public string GetZipID(string path)
        {
            string id = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID FROM ZipEntry WHERE path=@path";
                cmd.Parameters.AddWithValue("path", path);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return id;
        }
#endif
        // 全てのzipの存在フラグを落とす.
        public void UpdateZipExistDown()
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "UPDATE ZipEntry SET EXIST='0'";
                cmd.ExecuteNonQuery();
            }
        }

        // 指定したzipの存在フラグを立てる.
        public void UpdateZipExistUp(int id)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "UPDATE ZipEntry SET EXIST='1' WHERE ID='" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        // 特定のZIPファイルを削除する.
        public void DeleteZip(int id)
        {
            // ZIP中のTAHを取得する(TAHIDごとに個別削除).
            List<ArcsZipTahEntry> list = GetZipTahs(id);
            foreach (ArcsZipTahEntry tah in list)
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ZipTahFilesEntry WHERE TAHID = '" + tah.id.ToString() + "'";
                    cmd.ExecuteNonQuery();
                }
            }
            // tahファイルを削除する(ZIPIDで一括削除).
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM ZipTahEntry WHERE ZIPID = '" + id.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
            // EXISTが0のエントリを一括削除する.
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM ZipEntry WHERE ID = '"+id.ToString()+"'";
                cmd.ExecuteNonQuery();
            }
        }

        private void makeZipTemporaryIndex()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE INDEX DeleteTemp_TAHID ON ZipTahFilesEntry(TAHID)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE INDEX DeleteTemp_TAHID2 ON ZipTahEntry(ZIPID)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void dropZipTemporaryIndex()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DROP INDEX DeleteTemp_TAHID";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DROP INDEX DeleteTemp_TAHID2;";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // 存在しないエントリを削除する.
        public void DeleteNoExistentZip()
        {
            makeZipTemporaryIndex();
            // tahファイル一覧を取得する.
            List<ArcsZipArcEntry> zips = GetZips();
            foreach (ArcsZipArcEntry zip in zips)
            {
                if (zip.exist == 0)
                {
                    TDCGExplorer.SetToolTips("Deleting " + Path.GetFileName(zip.path));
                    DeleteZip(zip.id);
                }
            }
            dropZipTemporaryIndex();
        }

        // ZIP中のtahファイルを追加する.       
        public int SetZipTahEntry(ArcsZipTahEntry entry)
        {
            string id = "";
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ZipTahEntry (ZIPID,PATH,SHORTNAME,TAHVERSION) VALUES(@zipid,@path,@shortname,@version)";
                cmd.Parameters.AddWithValue("zipid", entry.zipid);
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("shortname", entry.shortname);
                cmd.Parameters.AddWithValue("version", entry.version.ToString());
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return int.Parse(id);
        }

        // TAH内部ファイルを格納する.
        public int SetZipTahFilesPath(ArcsTahFilesEntry entry)
        {
            string id = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ZipTahFilesEntry (TAHID,TAHENTRY,PATH,HASH,LENGTH) VALUES(@tahid,@tahentry,@path,@hash,@length )";
                cmd.Parameters.AddWithValue("tahid", entry.tahid);
                cmd.Parameters.AddWithValue("tahentry", entry.tahentry);
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("hash", entry.hash.ToString());
                cmd.Parameters.AddWithValue("length", entry.length);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader[0].ToString();
                        break;
                    }
                }
            }
            return int.Parse(id);
        }

        // ZIP中の全てのファイルを返す.
        public List<ArcsZipTahEntry> GetZipTahs(int zipid)
        {
            List<ArcsZipTahEntry> list = new List<ArcsZipTahEntry>();
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID,PATH,SHORTNAME,TAHVERSION,ZIPID FROM ZipTahEntry WHERE ZIPID=@zipid";
                    cmd.Parameters.AddWithValue("zipid", zipid.ToString());
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArcsZipTahEntry entry = new ArcsZipTahEntry();
                            entry.id = int.Parse(reader[0].ToString());
                            entry.path = reader[1].ToString();
                            entry.shortname = reader[2].ToString();
                            entry.version = int.Parse(reader[3].ToString());
                            entry.zipid = int.Parse(reader[4].ToString());
                            list.Add(entry);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return list;
        }


        // ZIP中の指定したTAHを取得する.
        public ArcsZipTahEntry GetZipTah(int tahid)
        {
            ArcsZipTahEntry entry = new ArcsZipTahEntry();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,PATH,SHORTNAME,TAHVERSION,ZIPID FROM ZipTahEntry WHERE ID=@tahid";
                cmd.Parameters.AddWithValue("tahid", tahid.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.shortname = reader[2].ToString();
                        entry.version = int.Parse(reader[3].ToString());
                        entry.zipid = int.Parse(reader[4].ToString());
                        break;
                    }
                }
            }
            return entry;
        }

        // 指定したtahに含まれるファイルを取得する.
        public List<ArcsTahFilesEntry> GetZipTahFilesEntries(int tahid)
        {
            List<ArcsTahFilesEntry> value = new List<ArcsTahFilesEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,TAHID,TAHENTRY,PATH,HASH,LENGTH FROM ZipTahFilesEntry WHERE TAHID=@tahid";
                cmd.Parameters.AddWithValue("tahid", tahid.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsTahFilesEntry entry = new ArcsTahFilesEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.tahid = int.Parse(reader[1].ToString());
                        entry.tahentry = int.Parse(reader[2].ToString());
                        entry.path = reader[3].ToString();
                        entry.hash = int.Parse(reader[4].ToString());
                        entry.length = int.Parse(reader[5].ToString());
                        value.Add(entry);
                    }
                }
            }
            return value;
        }

        // 指定したファイルを取得する.
        public List<ArcsTahFilesEntry> GetZipTahFilesEntries(uint hash)
        {
            List<ArcsTahFilesEntry> list = new List<ArcsTahFilesEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,TAHID,TAHENTRY,PATH,HASH,LENGTH FROM ZipTahFilesEntry WHERE HASH=@hash";
                cmd.Parameters.AddWithValue("hash", (int)hash);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsTahFilesEntry entry = new ArcsTahFilesEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.tahid = int.Parse(reader[1].ToString());
                        entry.tahentry = int.Parse(reader[2].ToString());
                        entry.path = reader[3].ToString();
                        entry.hash = int.Parse(reader[4].ToString());
                        entry.length = int.Parse(reader[5].ToString());
                        list.Add(entry);
                    }
                }
            }
            return list;
        }

        // 指定したIDのファイルを取得する.
        public ArcsTahFilesEntry GetTahFilesEntry(int filesid)
        {
            ArcsTahFilesEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,TAHID,TAHENTRY,PATH,HASH,LENGTH FROM FilesEntry WHERE ID=@filesid";
                cmd.Parameters.AddWithValue("filesid", filesid.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new ArcsTahFilesEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.tahid = int.Parse(reader[1].ToString());
                        entry.tahentry = int.Parse(reader[2].ToString());
                        entry.path = reader[3].ToString();
                        entry.hash = int.Parse(reader[4].ToString());
                        entry.length = int.Parse(reader[5].ToString());
                        break;
                    }
                }
            }
            return entry;
        }

        // 指定したファイル名のtahファイルを取得する.
        public List<ArcsTahFilesEntry> GetTahFilesEntry(uint hash)
        {
            List<ArcsTahFilesEntry> list = new List<ArcsTahFilesEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,TAHID,TAHENTRY,PATH,HASH,LENGTH FROM FilesEntry WHERE HASH=@hash";
                cmd.Parameters.AddWithValue("hash", (int)hash);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsTahFilesEntry entry = null;
                        entry = new ArcsTahFilesEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.tahid = int.Parse(reader[1].ToString());
                        entry.tahentry = int.Parse(reader[2].ToString());
                        entry.path = reader[3].ToString();
                        entry.hash = int.Parse(reader[4].ToString());
                        entry.length = int.Parse(reader[5].ToString());
                        list.Add(entry);
                    }
                }
            }
            return list;
        }


#if false
        // 指定したZIPに含まれるinstall済みtahを列挙する.
        public List<ArcsTahEntry> GetInstalledTAHFiles()
        {
            List<ArcsTahEntry> list = new List<ArcsTahEntry>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT TahEntry.ID, TahEntry.PATH, TahEntry.SHORTNAME, TahEntry.TAHVERSION FROM ZipTahEntry INNER JOIN TahEntry ON TahEntry.SHORTNAME=ZipTahEntry.SHORTNAME";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsTahEntry entry = new ArcsTahEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.shortname = reader[2].ToString();
                        entry.version = int.Parse(reader[3].ToString());
                        list.Add(entry);
                    }
                }
            }
            return list;
        }
#endif
        // インストール済みのZIP一覧を取得する.
        public List<ArcsZipArcEntry> GetInstalledZipFiles()
        {
            List<ArcsZipArcEntry> list = new List<ArcsZipArcEntry>();

            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ZipEntry.ID,ZipEntry.PATH,ZipEntry.CODE,ZipEntry.EXIST FROM ZipEntry INNER JOIN InstalledZipEntry ON ZipEntry.ID=InstalledZipEntry.ZIPID ORDER BY ZipEntry.PATH";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsZipArcEntry entry = new ArcsZipArcEntry();
                        entry.id = int.Parse(reader[0].ToString());
                        entry.path = reader[1].ToString();
                        entry.code = reader[2].ToString();
                        entry.exist = int.Parse(reader[3].ToString());
                        list.Add(entry);
                    }
                }
            }
            return list;
        }

        // インストール済み全部のzip表を作成する.
        public void CreateInstalledZips()
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "DROP TABLE InstalledZipEntry";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                //cmd.CommandText = "CREATE TABLE InstalledZipEntry (ID INTEGER PRIMARY KEY, ZIPID INTEGER)";
                //cmd.ExecuteNonQuery();

                CreateInstalledZipTable();

                cmd.CommandText = "INSERT INTO InstalledZipEntry (ZIPID) SELECT DISTINCT ZipTahEntry.ZIPID FROM ZipTahEntry INNER JOIN TahEntry ON TahEntry.SHORTNAME=ZipTahEntry.SHORTNAME";
                cmd.ExecuteNonQuery();
            }
        }

        // インストール済みZIPのハッシュを返す.
        public Dictionary<int, int> GetInstalledZips()
        {
            Dictionary<int, int> table = new Dictionary<int, int>();

            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ZIPID FROM InstalledZipEntry";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int zipid =  int.Parse(reader[0].ToString());
                        if (table.ContainsKey(zipid) == false) table[zipid] = 1;
                    }
                }
            }
            return table;
        }

        //衝突しているTAHIDを列挙する.
        //SELECT A.TAHID AS TahID1 ,B.TAHID AS TahID2,A.ID AS FilesEntryID1, B.ID AS FilesEntryID2  from filesentry a left join filesentry b on a.hash=b.hash WHERE UPPER(a.path)<>UPPER(b.path);
        public Dictionary<int, List<ArcsCollisionRecord>> GetDuplicateDomain()
        {
            Dictionary<int, List<ArcsCollisionRecord>> table = new Dictionary<int, List<ArcsCollisionRecord>>();

            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT A.TAHID AS TahID1 ,B.TAHID AS TahID2,A.ID AS FilesEntryID1, B.ID AS FilesEntryID2 FROM FilesEntry A LEFT JOIN FilesEntry B ON A.HASH=B.HASH WHERE A.TAHID<>B.TAHID;";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsCollisionRecord entry;
                        entry = new ArcsCollisionRecord();
                        entry.fromTahID = int.Parse(reader[0].ToString());
                        entry.toTahID = int.Parse(reader[1].ToString());
                        entry.fromFilesID = int.Parse(reader[2].ToString());
                        entry.toFilesID = int.Parse(reader[3].ToString());
                        if (table.ContainsKey(entry.fromTahID) == false) table[entry.fromTahID] = new List<ArcsCollisionRecord>();
                        table[entry.fromTahID].Add(entry);
                    }
                }
            }
            return table;
        }

        //衝突しているTAHIDを列挙する.
        //SELECT A.TAHID AS TahID1 ,B.TAHID AS TahID2,A.ID AS FilesEntryID1, B.ID AS FilesEntryID2  from filesentry a left join filesentry b on a.hash=b.hash WHERE UPPER(a.path)<>UPPER(b.path);
        public Dictionary<int, List<ArcsCollisionRecord>> GetCollisionDomain()
        {
            Dictionary<int, List<ArcsCollisionRecord>> table = new Dictionary<int, List<ArcsCollisionRecord>>();

            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT A.TAHID AS TahID1 ,B.TAHID AS TahID2,A.ID AS FilesEntryID1, B.ID AS FilesEntryID2 FROM FilesEntry A LEFT JOIN FilesEntry B ON A.HASH=B.HASH WHERE UPPER(A.PATH)<>UPPER(B.PATH);";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ArcsCollisionRecord entry;
                        entry = new ArcsCollisionRecord();
                        entry.fromTahID = int.Parse(reader[0].ToString());
                        entry.toTahID = int.Parse(reader[1].ToString());
                        entry.fromFilesID = int.Parse(reader[2].ToString());
                        entry.toFilesID = int.Parse(reader[3].ToString());
                        if (table.ContainsKey(entry.fromTahID) == false) table[entry.fromTahID] = new List<ArcsCollisionRecord>();
                        table[entry.fromTahID].Add(entry);
                    }
                }
            }
            return table;
        }

    }
}
