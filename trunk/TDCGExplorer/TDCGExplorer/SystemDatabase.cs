using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace TDCGExplorer
{
    public class SystemDatabase : IDisposable
    {
        private SQLiteConnection cnn;

        // dbを置く場所を準備する.
        public SystemDatabase()
        {
            Directory.CreateDirectory(GetSystemDatabasePath());
            cnn = new SQLiteConnection("Data Source=" + GetSystemDatabaseName());
            cnn.Open();

            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE SystemDB (ID TEXT PRIMARY KEY, VALUE TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            // デフォルト値を設定する.
            setDefault();
        }

        public void Dispose()
        {
            cnn.Close();
        }

        // システムデータベースのパスを求める.
        // システムデータベースはTDCGディレクトリ下のTDCGExplorer/system.db
        public string GetSystemDatabasePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + TDCGExplorer.GetAppDataPath();
        }

        public string GetSystemDatabaseName()
        {
            return GetSystemDatabasePath() + "\\system.db";
        }


        // システムデータベースから値を読み出す.
        public string GetSqlValue(string id,string defval)
        {
            string value = defval;
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "SELECT VALUE FROM SystemDB WHERE ID=@id";
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

        // システムデータベースに値を設定する.
        public void SetSqlValue(string id, string value)
        {
            // 値を追加する.
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    // acpathを追加する.
                    cmd.CommandText = "INSERT OR REPLACE INTO SystemDB (ID,VALUE) VALUES(@id,@value)";
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

        public void setDefault()
        {
            arcs_path = arcs_path;
            zips_path = zips_path;
            moddb_url = moddb_url;
            zip_regexp = zip_regexp;
            database_build = database_build;
            window_rectangle = window_rectangle;
            splitter_distance = splitter_distance;
            arcnames_server = arcnames_server;
            work_path = work_path;
        }

        // arcpathの取得・設定.
        public string arcs_path {
            get { return GetSqlValue("arcpath", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)+"\\TechArts3D\\３Ｄカスタム少女\\arcs"); }
            set { SetSqlValue("arcpath", value); }
        }
        // zipフォルダのトップディレクトリ
        public string zips_path {
            get { return GetSqlValue("zippath", GetSystemDatabasePath() + "\\archive"); }
            set { SetSqlValue("zippath", value); }
        }
        // mod-dbへのurl
        public string moddb_url
        {
            get { return GetSqlValue("moddburl", "http://3dcustom.ath.cx/rails/"); }
            set { SetSqlValue("moddburl", value); }
        }
        // zipファイル名抽出
        public string zip_regexp
        {
            get { return GetSqlValue("zipregexp", "^([a-zA-Z0-9]+)"); }
            set { SetSqlValue("zipregexp", value); }
        }
        // DBビルドフラグ.
        public string database_build
        {
            get { return GetSqlValue("database-build", ""); }
            set { SetSqlValue("database-build", value); }
        }
        // ウインドウ位置
        public string window_rectangle
        {
            get { return GetSqlValue("window_rectangle", "50,50,640,480"); }
            set { SetSqlValue("window_rectangle", value); }
        }
        // 画面レイアウト
        public string splitter_distance
        {
            get { return GetSqlValue("splitter_distance", "100,100,200"); }
            set { SetSqlValue("splitter_distance", value); }
        }
        // arcnames.zip取得先
        public string arcnames_server
        {
            get { return GetSqlValue("arcnames_server", "http://3dcustom.ath.cx/text/arcnames.zip"); }
            set { SetSqlValue("arcnames_server", value); }
        }
        // 作業フォルダのトップディレクトリ
        public string work_path
        {
            get { return GetSqlValue("workpath", GetSystemDatabasePath() + "\\work"); }
            set { SetSqlValue("workpath", value); }
        }
    }
}
