using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace TDCGExplorer
{
    public class AnnotationDB : IDisposable
    {
        private SQLiteConnection cnn;
        private Dictionary<string, string> annonTable = new Dictionary<string, string>();

        public AnnotationDB()
        {
            Directory.CreateDirectory(GetAnnotationDatabasePath());
            cnn = new SQLiteConnection("Data Source=" + GetAnnotationDatabaseName());
            cnn.Open();

            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE AnnotationDB (ID TEXT PRIMARY KEY, VALUE TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            MakeAnnonTable();
        }

        public void Dispose()
        {
            cnn.Close();
        }

        private string GetAnnotationDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal) , TDCGExplorer.GetAppDataPath());
        }

        private string GetAnnotationDatabaseName()
        {
            return Path.Combine(GetAnnotationDatabasePath(), "annotation.db");
        }

        public string GetSqlValue(string id, string defval)
        {
            string value = defval;
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "SELECT VALUE FROM AnnotationDB WHERE ID=@id";
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

        public void SetSqlValue(string id, string value)
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "INSERT OR REPLACE INTO AnnotationDB (ID,VALUE) VALUES(@id,@value)";
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("value", value);
                    cmd.ExecuteNonQuery();
                }
                annonTable[id] = value;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // アノテーションテーブルを初期化する.
        private void MakeAnnonTable()
        {
            annonTable.Clear();
            using (SQLiteCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID,VALUE FROM AnnotationDB";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader[0].ToString();
                        string value = reader[1].ToString();
                        annonTable[id] = value;
                    }
                }
            }
        }

        public Dictionary<string, string> annotation
        {
            get { return annonTable; }
            set { }
        }

    }
}
