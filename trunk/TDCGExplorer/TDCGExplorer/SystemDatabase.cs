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

        Dictionary<string,string> cache = new Dictionary<string,string>();

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
            if (cnn != null)
            {
                cnn.Close();
                cnn.Dispose();
                cnn = null;
            }
        }

        // システムデータベースのパスを求める.
        // システムデータベースはTDCGディレクトリ下のTDCGExplorer/system.db
        public string GetSystemDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath());
        }

        public string GetSystemDatabaseName()
        {
            return Path.Combine(GetSystemDatabasePath(), "system.db");
        }


        // システムデータベースから値を読み出す.
        public string GetSqlValue(string id,string defval)
        {
            // キャッシュに値が存在するならそれを返す.
            if (cache.ContainsKey(id) == true)
                return cache[id];

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
                    // キャッシュを更新する.
                    cache[id] = value;
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
            database_version = database_version;
            window_rectangle = window_rectangle;
            splitter_distance = splitter_distance;
            arcnames_server = arcnames_server;
            tagnames_server = tagnames_server;
            work_path = work_path;
            modrefserver_alwaysenable = modrefserver_alwaysenable;
            zippage_behavior = zippage_behavior;
            directaccess_signature = directaccess_signature;
            initialize_camera = initialize_camera;
            cameracenter = cameracenter;
            translateto = translateto;
            tahpath = tahpath;
            collisionchecklebel = collisionchecklebel;
            findziplevel = findziplevel;
            delete_tahcache = delete_tahcache;
        }

        // arcpathの取得・設定.
        public string arcs_path {
            get { return GetSqlValue("arcpath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TechArts3D\\３Ｄカスタム少女\\arcs")); }
            set { SetSqlValue("arcpath", value); }
        }
        // zipフォルダのトップディレクトリ
        public string zips_path {
            get { return GetSqlValue("zippath", Path.Combine(GetSystemDatabasePath(), "archive")); }
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
        // データベースバージョン.
        public string database_version
        {
            get { return GetSqlValue("database-version", ""); }
            set { SetSqlValue("database-version", value); }
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
        // tagnames.zip取得先
        public string tagnames_server
        {
            get { return GetSqlValue("tagnames_server", "http://3dcustom.ath.cx/text/tagnames.zip"); }
            set { SetSqlValue("tagnames_server", value); }
        }
        // 作業フォルダのトップディレクトリ
        public string work_path
        {
            get { return GetSqlValue("workpath", Path.Combine(GetSystemDatabasePath(), "work")); }
            set { SetSqlValue("workpath", value); }
        }
        // MODREFサーバページの表示ON/OFF
        public string modrefserver_alwaysenable
        {
            get { return GetSqlValue("modrefserver_alwaysenable", "false"); }
            set { SetSqlValue("modrefserver_alwaysenable", value); }
        }
        // DirectAccessArchiveクラス起動条件
        public string directaccess_signature
        {
            get { return GetSqlValue("directaccess_signature", "(^TA[0-9]{4})|(^TA3CH[0-9]{4})|(^TA3DC[0-9]{4})|(^TAC[0-9]{4})|(^TAC[0-9]{5})|(^XPC[0-9]{5})|(^mod[0-9]{4})"); }
            set { SetSqlValue("directaccess_signature", value); }
        }
        // ZIPページを開いた時の動作を指定する (none:なにもしない server:サーバにアクセス image:画像表示 text:テキスト表示)
        public string zippage_behavior
        {
            get { return GetSqlValue("zippage_behavior", "none"); }
            set { SetSqlValue("zippage_behavior", value); }
        }
        // セーブファイルのあるフォルダを指定する.
        public string savefile_directory
        {
            get { return GetSqlValue("savefile_directory", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TechArts3D\\TDCG")); }
            set { SetSqlValue("savefile_directory", value); }
        }
        // カメラ位置をリセットするか?
        public bool initialize_camera
        {
            get { return GetSqlValue("initialize_camera", "false") == "true"; }
            set
            {
                if (value == true) SetSqlValue("initialize_camera", "true");
                else SetSqlValue("initialize_camera", "false");
            }
        }
        // カメラセンターボーン
        public string cameracenter
        {
            get { return GetSqlValue("cameracenter", "W_Hips"); }
            set { SetSqlValue("cameracenter", value); }
        }
        // 初期視点
        public string translateto
        {
            get { return GetSqlValue("translateto", "face_oya"); }
            set { SetSqlValue("translateto", value); }
        }
        // TAH作業フォルダのトップディレクトリ
        public string tahpath
        {
            get { return GetSqlValue("tahpath", Path.Combine(GetSystemDatabasePath(), "taheditor")); }
            set { SetSqlValue("tahpath", value); }
        }
        // 衝突チェックレベル
        public string collisionchecklebel
        {
            get { return GetSqlValue("collisionchecklebel", Path.Combine(GetSystemDatabasePath(), "collision")); }
            set { SetSqlValue("collisionchecklebel", value); }
        }
        // zip探索優先レベル設定
        public bool findziplevel
        {
            get { return GetSqlValue("findziplevel", "false") == "true"; }
            set
            {
                if (value == true) SetSqlValue("findziplevel", "true");
                else SetSqlValue("findziplevel", "false");
            }
        }
        // セーブファイル名の固定
        public bool delete_tahcache
        {
            get { return GetSqlValue("delete_tahcache", "false") == "true"; }
            set
            {
                if (value == true) SetSqlValue("delete_tahcache", "true");
                else SetSqlValue("delete_tahcache", "false");
            }
        }

    }
}
