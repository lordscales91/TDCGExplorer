using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace TDCGExplorer
{
    public class TAHLocalDbEntry
    {
        public string path;
        public int dataid;
    }
    public class TAHLocalDBDataEntry
    {
        public int dataid;
        public byte[] data;
    }
    // TAHコンテナ
    public class TAHLocalDB : IDisposable
    {
        string filepath = null;
        private SQLiteConnection cnn = null;

        public TAHLocalDB()
        {
        }

        public void Dispose()
        {
            if (cnn!=null)
            {
                cnn.Close();
                cnn.Dispose();
                cnn = null;
            }
        }

        // データベースをオープンする.
        public void Open(string path)
        {
            filepath = path;

            if (cnn != null) cnn.Close();

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            cnn = new SQLiteConnection("Data Source=" + filepath);
            cnn.Open();

            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE Entry (PATH TEXT PRIMARY KEY, DATAID INTEGER)";
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
                    cmd.CommandText = "CREATE TABLE Data (DATAID INTEGER PRIMARY KEY, DATA BLOB)";
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
                    cmd.CommandText = "CREATE TABLE Information (ID TEXT PRIMARY KEY, VALUE TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }

        public int AddData(TAHLocalDBDataEntry entry)
        {
            int id=-1;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                // acpathを追加する.
                cmd.CommandText = "INSERT INTO Data (DATA) VALUES(@data)";
                cmd.Parameters.AddWithValue("data", entry.data);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid()";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = Int16.Parse(reader[0].ToString());
                        break;
                    }
                }
            }
            return id;
        }

        // データを格納する.
        public void AddContent(TAHLocalDbEntry entry)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                // acpathを追加する.
                cmd.CommandText = "INSERT INTO Entry (PATH,DATAID) VALUES(@path,@id)";
                cmd.Parameters.AddWithValue("path", entry.path);
                cmd.Parameters.AddWithValue("id", entry.dataid);
                cmd.ExecuteNonQuery();
            }
        }

        // データ一覧を取得する.
        public List<string> GetDirectory()
        {
            List<string> list = new List<string>();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT PATH FROM Entry ORDER BY PATH";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }
            }
            return list;
        }

        // エントリを取得する
        public TAHLocalDbEntry GetEntry(string path)
        {
            TAHLocalDbEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT PATH,DATAID FROM Entry WHERE PATH=@path";
                cmd.Parameters.AddWithValue("path", path);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new TAHLocalDbEntry();
                        entry.path = reader[0].ToString();
                        entry.dataid = int.Parse(reader[1].ToString());
                        break;
                    }
                }
            }
            return entry;
        }
        //                    cmd.CommandText = "CREATE TABLE Data (DATAID PRIMARY KEY, DATA BLOB)";

        // データを取得する
        public TAHLocalDBDataEntry GetData(int id)
        {
            TAHLocalDBDataEntry entry = null;
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT DATAID,DATA FROM Data WHERE DATAID=@id";
                cmd.Parameters.AddWithValue("id", id.ToString());
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entry = new TAHLocalDBDataEntry();
                        entry.dataid = int.Parse(reader[0].ToString());
                        entry.data = (byte[]) reader[1];
                        break;
                    }
                }
            }
            return entry;
        }

        // データを更新するする
        public void UpdateData(TAHLocalDBDataEntry entry)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "UPDATE Data SET DATA=@data WHERE DATAID=@id";
                cmd.Parameters.AddWithValue("id", entry.dataid.ToString());
                cmd.Parameters.AddWithValue("data", entry.data);
                cmd.ExecuteNonQuery();
            }
        }


        // エントリを削除する.
        public void DeleteEntry(string path)
        {
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                // acpathを追加する.
                cmd.CommandText = "DELETE FROM Entry WHERE PATH=@path";
                cmd.Parameters.AddWithValue("path", path);
                cmd.ExecuteNonQuery();
            }
        }

        // データを削除する.

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

    }
}
