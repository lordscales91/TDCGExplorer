
// TDCGExplorer Framework by Konoa.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Data.SQLite;

namespace TDCGExplorer
{
    public class TDCGExplorer
    {
        private static SystemDatabase systemDatabase;
        private static ArcsDatabase arcsDatabase;
        private static AnnotationDB annotationDatabase;
        private static ArcNamesDictionary arcNames;
        private static MainForm form;

        public static volatile bool fThreadRun = false;
        private static volatile string toolTipsMessage = "";
        private static volatile object lockObject = new Object();

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Debug.WriteLine("SystemStart");

            systemDatabase = new SystemDatabase();
            arcsDatabase = new ArcsDatabase();
            arcNames = new ArcNamesDictionary();
            annotationDatabase = new AnnotationDB();

            TAHEntry.ReadExternalFileList();
            arcNames.Init();

            SetToolTips("Copyright © 2009 3DCG Craftsmen's Guild.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new MainForm());

            arcsDatabase.Dispose();
            systemDatabase.Dispose();
        }

        public static string GetAppDataPath()
        {
            return "TechArts3D\\TDCG\\TDCGExplorer";
        }

        public static void SetToolTips(string message)
        {
            lock(lockObject){
                toolTipsMessage = String.Copy(message);
            }
        }

        public static string GetToolTips()
        {
            string retval;
            lock (lockObject)
            {
                retval = String.Copy(toolTipsMessage);
            }
            return retval;
        }

        // システムデータベースの取得.
        public static SystemDatabase GetSystemDatabase()
        {
            return systemDatabase;
        }

        // arcsデータベースの取得.
        public static ArcsDatabase GetArcsDatabase()
        {
            return arcsDatabase;
        }

        // 注訳DBの取得.
        public static AnnotationDB GetAnnoDatabase()
        {
            return annotationDatabase;
        }

        // formの取得.
        public static MainForm GetMainForm()
        {
            return form;
        }

        public static bool DownloadArcNamesZipFromServer()
        {
            return arcNames.DownloadArcNamesZipFromServer();
        }

        public static void GetArcNamesZipInfo()
        {
            arcNames.GetArcNamesZipInfo();
        }

        public static Dictionary<string, ArcsNamesEntry> getArcsnames()
        {
            return arcNames.entry;
        }

        // データベース生成.
        public static void CreateNewArcsDatabase()
        {
            // 二重起動防止.
            if (fThreadRun == true) return;
            CreateArcsDatabaseThread cdb = new CreateArcsDatabaseThread();
            Thread thread = new Thread(new ThreadStart(cdb.Run));
            fThreadRun = true;
            thread.Start();
        }
        // システムデータベースの編集.
        public static void EditSystemDatabase()
        {
            if (fThreadRun == true) return;
            EditSystemDatabase edit = new EditSystemDatabase();
            edit.textArcPath = GetSystemDatabase().arcs_path;
            edit.textZipPath = GetSystemDatabase().zips_path;
            edit.textModDbUrl = GetSystemDatabase().moddb_url;
            edit.textZipRegexp = GetSystemDatabase().zip_regexp;
            edit.textArcnamesServer = GetSystemDatabase().arcnames_server;
            edit.textWorkPath = GetSystemDatabase().work_path;
            edit.lookupmodref = GetSystemDatabase().modrefpage_enable=="true";
            edit.textModRegexp = GetSystemDatabase().directaccess_signature;
            
            if (edit.ShowDialog() == DialogResult.OK)
            {
                // ダイアログに設定されたパラメータを ~/TDCG/TDCDEXplorer/system.dbに書き出す.
                GetSystemDatabase().arcs_path = edit.textArcPath;
                GetSystemDatabase().zips_path = edit.textZipPath;
                GetSystemDatabase().moddb_url = edit.textModDbUrl;
                GetSystemDatabase().zip_regexp = edit.textZipRegexp;
                GetSystemDatabase().arcnames_server = edit.textArcnamesServer;
                GetSystemDatabase().work_path = edit.textWorkPath;
                if (edit.lookupmodref == true) GetSystemDatabase().modrefpage_enable = "true";
                else GetSystemDatabase().modrefpage_enable = "false";
                GetSystemDatabase().directaccess_signature = edit.textModRegexp;
            }
        }

        public static void MakeArcsTreeView(ArcsDatabase db, TreeView tvTree)
        {
            FilesTreeNode arcs = new FilesTreeNode(GetSystemDatabase().arcs_path);
            tvTree.Nodes.Add(arcs);
            // tahを展開する.
            List<ArcsTahEntry> list = db.GetTahs();
            foreach (ArcsTahEntry entry in list)
            {
                char[] separetor = { '\\', '/' };
                string[] toplevel = entry.path.Split(separetor);

                // tahエントリを持つsubnodeを作る.
                if (toplevel.Length == 1)
                {
                    arcs.Entries.Add(entry); // ファイルエントリを追加するだけ.
                }
                else
                {
                    FilesTreeNode currentNode = null;
                    FilesTreeNode parentNode = arcs;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (FilesTreeNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new FilesTreeNode(sublevel);//parentNode.Nodes.Add(sublevel);
                            parentNode.Nodes.Add(currentNode);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // 末端レベルにファイル情報を格納する.
                    currentNode.Entries.Add(entry);
                }
            }
            arcs.Expand();
        }

        public static void MakeZipsTreeView(ArcsDatabase db, TreeView tvTree)
        {
            TreeNode zips = tvTree.Nodes.Add(GetSystemDatabase().zips_path);
            Dictionary<int, int> installedZip = db.GetInstalledZips();
            // tahを展開する.
            List<ArcsZipArcEntry> list = db.GetZips();
            foreach (ArcsZipArcEntry entry in list)
            {
                char[] separetor = { '\\', '/' };
                string[] toplevel = entry.path.Split(separetor);
                // tahエントリを持つsubnodeを作る.
                if (toplevel.Length == 1)
                {
                    // tahエントリを持つsubnodeを作る.
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
                    zips.Nodes.Add(subnode);
                    //List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(entry.id);
                    //subnode.Entries=files;
                    //subnode.Entry = entry.id;
                }
                else
                {
                    TreeNode currentNode;
                    TreeNode parentNode = zips;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (TreeNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = parentNode.Nodes.Add(sublevel);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // tahエントリを持つsubnodeを作る.
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
                    parentNode.Nodes.Add(subnode);
                    //List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(entry.id);
                    //subnode.Entries = files;
                    //subnode.Entry = entry.id;

                    //インストール済みのZIPは青色に.
                    if (installedZip.ContainsKey(entry.id) == true)
                    {
                        subnode.ForeColor = Color.Blue;
                    }
                }
            }
            zips.Expand();
        }

        public static void MakeCollisionTreeView(ArcsDatabase db, TreeView tvTree)
        {
            CollisionTahNode arcs = new CollisionTahNode(GetSystemDatabase().arcs_path);
            tvTree.Nodes.Add(arcs);
            // tahを展開する.
            List<ArcsTahEntry> list = db.GetTahs();
            Dictionary<int, List<ArcsCollisionRecord>> colldomain = db.GetCollisionDomain();
            foreach (ArcsTahEntry entry in list)
            {
                if (colldomain.ContainsKey(entry.id) == false) continue;

                char[] separetor = { '\\', '/' };
                string[] toplevel = entry.path.Split(separetor);

                // tahエントリを持つsubnodeを作る.
                if (toplevel.Length == 1)
                {
                    CollisionItem item = new CollisionItem();
                    item.tah = entry;
                    item.entries = colldomain[entry.id];
                    arcs.Entries.Add(item); // ファイルエントリを追加するだけ.
                }
                else
                {
                    CollisionTahNode currentNode = null;
                    CollisionTahNode parentNode = arcs;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (CollisionTahNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new CollisionTahNode(sublevel);//parentNode.Nodes.Add(sublevel);
                            parentNode.Nodes.Add(currentNode);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // 末端レベルにファイル情報を格納する.
                    CollisionItem item = new CollisionItem();
                    item.tah = entry;
                    item.entries = colldomain[entry.id];
                    currentNode.Entries.Add(item);
                }
            }
            arcs.Expand();
        }

        // TODO:ArcsTahEntryをArcsZipArcEntryにする
        public static void MakeInstalledArcsTreeView(ArcsDatabase db, TreeView tvTree)
        {
            //GetInstalledZipFiles
            TreeNode zips = tvTree.Nodes.Add(GetSystemDatabase().zips_path);
            // tahを展開する.
            List<ArcsZipArcEntry> list = db.GetInstalledZipFiles();
            foreach (ArcsZipArcEntry entry in list)
            {
                char[] separetor = { '\\', '/' };
                string[] toplevel = entry.path.Split(separetor);
                // tahエントリを持つsubnodeを作る.
                if (toplevel.Length == 1)
                {
                    // tahエントリを持つsubnodeを作る.
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
                    zips.Nodes.Add(subnode);
                    //List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(entry.id);
                    //subnode.Entries=files;
                    //subnode.Entry = entry.id;
                }
                else
                {
                    TreeNode currentNode;
                    TreeNode parentNode = zips;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (TreeNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = parentNode.Nodes.Add(sublevel);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // tahエントリを持つsubnodeを作る.
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
                    parentNode.Nodes.Add(subnode);
                    //List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(entry.id);
                    //subnode.Entries = files;
                    //subnode.Entry = entry.id;
                }
            }
            zips.Expand();
        }
#if dalse
        // arcs.dbをツリーに展開する(完全なツリー階層を展開すると非現実的に遅いので妥協).
        public static void DisplayArcsDB(TreeView tvTree)
        {
            try
            {
                ArcsDatabase db = GetArcsDatabase();

                // 全ノードを消去する.
                //tvTree.Nodes.Clear();
                GetMainForm().ClearTreeBox();
                MakeArcsTreeView(db, tvTree);
                MakeZipsTreeView(db, tvTree);
                MakeCollisionTreeView(db, tvTree);
                MakeInstalledArcsTreeView(db, tvTree);
            }
            catch (Exception e)
            {
                TDCGExplorer.fThreadRun = false;
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
        }
#endif

        // データベースがビルド済みならツリーを展開する.
        public static void IfReadyDbDisplayArcsDB()
        {
            if (GetSystemDatabase().database_build != "") //DisplayArcsDB(tvTree);
                GetMainForm().DisplayDB();
        }

        public static bool InstallZipFile(TahGenTreeNode sender)
        {
            ZipTreeNode zipNode = (ZipTreeNode)sender;
            ArcsZipArcEntry zipentry = GetArcsDatabase().GetZip(zipNode.Entry);
            string zipsource = Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, zipentry.path);
            string destpath = GetSystemDatabase().work_path;
            if (arcNames.entry.ContainsKey(zipentry.code) == true)
            {
                // サマリ文字列を自動的に追加しておく.
                destpath = Path.Combine(destpath, zipentry.code) + " " + arcNames.entry[zipentry.code].summary;
            }
            else
            {
                destpath = Path.Combine(destpath, zipentry.code);
            }
            // 展開に成功したらzipのノードの色を変える.
            if (ZipFileUtil.ExtractZipFile(zipsource, destpath) == true)
            {
                sender.ForeColor = Color.Magenta;
                return true;
            }
            return false;
        }

        private static bool HasString(string target, string word)
        {
            if (word == "")
                return false;
            if (target.IndexOf(word) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void FindTreeNode(TreeNode node, string key)
        {
            if (HasString(node.Text.ToLower(), key.ToLower()) == true)
            {
                node.BackColor = Color.LawnGreen;
                TreeNode parent = node.Parent;
                while (parent != null)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }
            }
            else
            {
                node.BackColor = Color.Transparent;
            }
            foreach (TreeNode subnode in node.Nodes)
            {
                FindTreeNode(subnode, key);
            }
        }

        public static void InstallPreferZip(ZipTreeNode zipNode)
        {
            ArcsZipArcEntry zipentry =  GetArcsDatabase().GetZip(zipNode.Entry);

            // mod REF Serverに問い合わせる.

            string moddb = GetSystemDatabase().moddb_url;
            string relurl;
            ArcRels relationships;

            Dictionary<int, int> installedZip = GetArcsDatabase().GetInstalledZips();

            int misscount = 0, installedcount = 0;

            relurl = moddb + "arcs/code/" + zipentry.code + "/rels.xml";
            TDCGExplorer.SetToolTips(relurl);
            relationships = ArcRels.Load(relurl);
            if (relationships != null)
            {
                if (relationships.Relationships != null)
                {
                    foreach (Relationship relation in relationships.Relationships)
                    {
                        string arcurl = moddb + "arcs/" + relation.ToId.ToString() + ".xml";
                        try
                        {
                            // 前提MODを見つけたら
                            if (relation.Kind == 3)
                            {
                                Arc arc = Arc.Load(arcurl);
                                if (arc != null)
                                {
                                    // zipファイルのコードを特定する.
                                    ArcsZipArcEntry ziparc = TDCGExplorer.GetArcsDatabase().GetZipByCode(arc.Code);
                                    if (ziparc != null)
                                    {
                                        // 既にインストールされている物は展開しない.
                                        if (installedZip.ContainsKey(ziparc.id) == true) continue;
                                            
                                        string zipsource = Path.Combine(TDCGExplorer.GetSystemDatabase().zips_path, ziparc.path);
                                        string destpath = GetSystemDatabase().work_path;
                                        if (arcNames.entry.ContainsKey(ziparc.code) == true)
                                        {
                                            // サマリ文字列を自動的に追加しておく.
                                            destpath = Path.Combine(destpath, "Required "+zipentry.code);
                                            destpath=Path.Combine(destpath,ziparc.code) + " " + arcNames.entry[ziparc.code].summary;
                                        }
                                        else
                                        {
                                            destpath = Path.Combine(destpath, "Required " + zipentry.code);
                                            destpath = Path.Combine(destpath, ziparc.code);
                                        }
                                        // 展開に成功したらzipのノードの色を変える.
                                        try
                                        {
                                            if (ZipFileUtil.ExtractZipFile(zipsource, destpath) == true)
                                            {
                                                // カウントを増やす.
                                                installedcount++;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine(ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        misscount++;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
            if (misscount > 0)
            {
                MessageBox.Show(installedcount.ToString() + "個のzipを展開しました。\n" +
                    misscount.ToString() + "個のzipが見つかりませんでした。", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (installedcount > 0)
                    MessageBox.Show(installedcount.ToString() + "個のzipを展開しました。", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("前提zipは全てインストール済みです", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

#if false
        public static void FindNode(string key)
        {
            foreach(TreeNode node in GetMainForm().TabTreeMainView.Nodes)
                FindTreeNode(node, key);
        }
#endif
    }

    public class CreateArcsDatabaseThread
    {
        public void Run()
        {
            try{
                string arcpath = TDCGExplorer.GetSystemDatabase().arcs_path;
                string zippath = TDCGExplorer.GetSystemDatabase().zips_path;
#if false
                // クローンを作る.
                ArcsDatabase arcs = new ArcsDatabase(TDCGExplorer.GetArcsDatabase());
#else
                // クローンだとかえって動作がおかしい.
                ArcsDatabase arcs = TDCGExplorer.GetArcsDatabase();
#endif
                using (SQLiteTransaction transacion = arcs.BeginTransaction())
                {
                    arcs.CreateTahDatabase();
                    arcs.CreateFilesDatabase();
                    arcs.CreateZipDatabase();
                    arcs.CreateZipTahDatabase();
                    arcs.CreateZipTahFilesDatabase();
                    arcs.CreateInstalledZipTable();
                    arcs.DropIndex(); // 一旦インデックスを削除する.
                    TAHDump.ArcsDumpDirEntriesMain(arcpath, arcs);
                    TAHDump.ZipsDumpDirEntriesMain(zippath, arcs);
                    // インストール済みZIPの表を作成する.
                    TDCGExplorer.SetToolTips("Execute SQL Trsansactions");
                    arcs.CreateIndex(); // インデックスを作成する.
                    arcs.CreateInstalledZips();
                    transacion.Commit();

                    TDCGExplorer.GetMainForm().asyncDisplayFromArcs(); // 表示更新.

                    TDCGExplorer.fThreadRun = false;
                    TDCGExplorer.SetToolTips("Database build complete");
                    TDCGExplorer.GetSystemDatabase().database_build = "yes";
                }
            }
            catch(Exception e)
            {
                TDCGExplorer.fThreadRun = false;
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
        }
    }
}
