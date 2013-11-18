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
using ArchiveLib;

namespace TDCGExplorer
{
    public class TDCGExplorer
    {
        public const string CONST_DBVERSION = "1.00";
        public const string CONST_APPVERSION = "1.11";
        public const string CONST_COPYRIGHT = "Copyright © 2009 3DCG Craftsmen's Guild.";

        private static SystemDatabase systemDatabase;
        private static ArcsDatabase arcsDatabase;
        private static AnnotationDB annotationDatabase;
        private static ArcNamesDictionary arcNames;
        private static TagNamesDictionary tagNames;
        private static MainForm form;
        private static Byte[] defaultTMO;
        //private static bool figureloaded = false;
        private static string lastAccessFile = "(none)";

        private static volatile string toolTipsMessage = "";
        private static volatile object lockObject = new Object();

        public static volatile int BusyCount=0;
        public static void IncBusy()
        {
            BusyCount++;
        }
        public static void DecBusy()
        {
            BusyCount--;
        }
        public static bool BusyTest()
        {
            if (BusyCount > 0)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return true;
            }
            return false;
        }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //スプラッシュウィンドウを表示
            SplashForm.ShowSplash();

            Directory.SetCurrentDirectory(Application.StartupPath);

            systemDatabase = new SystemDatabase();
            arcsDatabase = new ArcsDatabase();
            arcNames = new ArcNamesDictionary();
            tagNames = new TagNamesDictionary();
            annotationDatabase = new AnnotationDB();

            TAHEntry.ReadExternalFileList();
            arcNames.Init();
            tagNames.Init();

            DefaultTMOPng.InitialDefaultTMOPng();

            ResetDefaultPose();

            SetToolTips(CONST_COPYRIGHT);

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new MainForm());

            arcsDatabase.Dispose();
            systemDatabase.Dispose();
        }

        public static void GcCopact()
        {
            GC.Collect();
            Console.WriteLine(GC.GetTotalMemory(false));
        }

        public static string LastAccessFile
        {
            set { lastAccessFile = value; }
            get { return lastAccessFile; }
        }

        private static Byte[] LoadTMO()
        {
            BinaryReader reader = new BinaryReader(DefaultTMOPng.tmo, System.Text.Encoding.Default);
            return reader.ReadBytes((int)DefaultTMOPng.tmo.Length);
        }

        public static void ResetDefaultPose()
        {
            defaultTMO = LoadTMO();
        }
#if false
        public static bool FigureLoad
        {
            get { return figureloaded; }
            set { figureloaded = value; }
        }
#endif
        public static Stream defaultpose
        {
            get { return new MemoryStream(defaultTMO); }
            set
            {
                try
                {
                    Stream fs = value;
                    BinaryReader reader = new BinaryReader(fs, System.Text.Encoding.Default);
                    fs.Seek(0, SeekOrigin.Begin);
                    Byte[] buffer = reader.ReadBytes((int)fs.Length);
                    defaultTMO = buffer;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
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

        public static SystemDatabase SystemDB
        {
            get { return systemDatabase; }
        }

        public static ArcsDatabase ArcsDB
        {
            get { return arcsDatabase; }
        }

        public static AnnotationDB AnnDB
        {
            get { return annotationDatabase; }
        }

        public static MainForm MainFormWindow
        {
            get { return form; }
        }

        public static bool DownloadArcNamesZipFromServer()
        {
            return arcNames.DownloadArcNamesZipFromServer();
        }

        public static void GetArcNamesZipInfo()
        {
            arcNames.GetArcNamesZipInfo();
        }

        public static bool DownloadTagNamesZipFromServer()
        {
            return tagNames.DownloadTagNamesZipFromServer();
        }

        public static void GetTagNamesZipInfo()
        {
            tagNames.GetTagNamesZipInfo();
        }

        public static Dictionary<string, ArcsNamesEntry> Arcsnames
        {
            get { return arcNames.entry; }
        }

        public static Dictionary<string, TagNamesEntry> Tagnames
        {
            get { return tagNames.entry; }
        }

        // データベース生成.
        public static void CreateNewArcsDatabase()
        {
            // 二重起動防止.
            CreateArcsDatabaseThread cdb = new CreateArcsDatabaseThread();
            Thread thread = new Thread(new ThreadStart(cdb.Run));
            thread.Start();
        }
        // システムデータベースの編集.
        public static void EditSystemDatabase()
        {
            EditSystemDatabase edit = new EditSystemDatabase();
            edit.textArcPath = SystemDB.arcs_path;
            edit.textZipPath = SystemDB.zips_path;
            edit.textModDbUrl = SystemDB.moddb_url;
            edit.textZipRegexp = SystemDB.zip_regexp;
            edit.textArcnamesServer = SystemDB.arcnames_server;
            edit.textWorkPath = SystemDB.work_path;
            edit.lookupmodref = SystemDB.modrefserver_alwaysenable == "true";
            edit.textModRegexp = SystemDB.directaccess_signature;
            edit.textTagnamesServer = SystemDB.tagnames_server;
            edit.uiBehavior = SystemDB.zippage_behavior;
            edit.saveDirectory = SystemDB.savefile_directory;
            edit.initializeCamera = SystemDB.initialize_camera;
            edit.translateBone = SystemDB.translateto;
            edit.centerBone = SystemDB.cameracenter;
            edit.tahEditorPath = SystemDB.tahpath;
            edit.collisionDetectLevel = SystemDB.collisionchecklevel;
            edit.findziplevel = SystemDB.findziplevel;
            //edit.delete_tahcache = SystemDB.delete_tahcache;
            edit.taheditorprevire = SystemDB.taheditorpreview;
            edit.alwaysnewtab = SystemDB.alwaysnewtab;
            edit.tahversioncollision = SystemDB.tahversioncollision;
            edit.explorerzipfolder = SystemDB.explorerzipfolder;
            edit.posedir = SystemDB.posefile_savedirectory;
            edit.arcsvacume = systemDatabase.arcsvacume;
            edit.forcereloadsavedata = SystemDB.forcereloadsavedata;
            edit.Owner = MainFormWindow;
            if (edit.ShowDialog() == DialogResult.OK)
            {
                // ダイアログに設定されたパラメータを ~/TDCG/TDCDEXplorer/system.dbに書き出す.
                SystemDB.arcs_path = edit.textArcPath;
                SystemDB.zips_path = edit.textZipPath;
                SystemDB.moddb_url = edit.textModDbUrl;
                SystemDB.zip_regexp = edit.textZipRegexp;
                SystemDB.arcnames_server = edit.textArcnamesServer;
                SystemDB.work_path = edit.textWorkPath;
                if (edit.lookupmodref == true) SystemDB.modrefserver_alwaysenable = "true";
                else SystemDB.modrefserver_alwaysenable = "false";
                SystemDB.directaccess_signature = edit.textModRegexp;
                SystemDB.tagnames_server = edit.textTagnamesServer;
                SystemDB.zippage_behavior = edit.uiBehavior;
                SystemDB.savefile_directory = edit.saveDirectory;
                SystemDB.initialize_camera = edit.initializeCamera;
                SystemDB.translateto = edit.translateBone;
                SystemDB.cameracenter = edit.centerBone;
                SystemDB.tahpath = edit.tahEditorPath;
                SystemDB.collisionchecklevel = edit.collisionDetectLevel;
                SystemDB.findziplevel = edit.findziplevel;
                //SystemDB.delete_tahcache = edit.delete_tahcache;
                SystemDB.taheditorpreview = edit.taheditorprevire;
                SystemDB.alwaysnewtab = edit.alwaysnewtab;
                SystemDB.tahversioncollision = edit.tahversioncollision;
                SystemDB.explorerzipfolder = edit.explorerzipfolder;
                SystemDB.posefile_savedirectory = edit.posedir;
                SystemDB.arcsvacume = edit.arcsvacume;
                SystemDB.forcereloadsavedata = edit.forcereloadsavedata;
                SystemDB.appversion = CONST_APPVERSION;
                MainFormWindow.DisplayDB();
            }
        }

        public static void FileDelete(string path)
        {
            int timeout = 0;
            bool success = false;
            while (success == false)
            {
                try
                {
                    File.Delete(path);
                    success = true;
                }
                catch (Exception)
                {
                }
                if (success == false)
                {
                    if (timeout++ > 300)
                    {
                        MessageBox.Show(TextResource.FileUsed, TextResource.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        throw (new Exception(TextResource.FileUsed));
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public static void MakeArcsTreeView(TreeView tvTree)
        {
            ArcsDatabase db = ArcsDB;
            GenericFilesTreeNode arcs = new GenericFilesTreeNode(SystemDB.arcs_path);
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
                    GenericFilesTreeNode currentNode = null;
                    GenericFilesTreeNode parentNode = arcs;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (GenericFilesTreeNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new GenericFilesTreeNode(sublevel);
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

        public static void MakeZipsTreeView(TreeView tvTree)
        {
            ArcsDatabase db = ArcsDB;
            TreeNode zips = tvTree.Nodes.Add(SystemDB.zips_path);
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
                    GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(),entry.id);
                    zips.Nodes.Add(subnode);
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
                            currentNode = new GenericFilesTreeNode(sublevel);
                            parentNode.Nodes.Add(currentNode);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // tahエントリを持つsubnodeを作る.
                    GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(),entry.id);
                    parentNode.Nodes.Add(subnode);

                    //インストール済みのZIPは青色に.
                    if (installedZip.ContainsKey(entry.id) == true)
                    {
                        subnode.ForeColor = Color.Blue;
                    }
                }
            }
            zips.Expand();
        }

        public static void MakeCollisionTreeView(TreeView tvTree)
        {
            bool collsiondup = false;
            ArcsDatabase db = ArcsDB;
            GenericCollisionTahNode arcs = new GenericCollisionTahNode(SystemDB.arcs_path);
            tvTree.Nodes.Add(arcs);
            // tahを展開する.
            List<ArcsTahEntry> list = db.GetTahs();
            Dictionary<int, List<ArcsCollisionRecord>> colldomain;
            if (SystemDB.collisionchecklevel == "collision")
            {
                colldomain = db.GetCollisionDomain();
            }
            else
            {
                colldomain = db.GetDuplicateDomain();
                collsiondup = true;
            }
            foreach (ArcsTahEntry entry in list)
            {
                if (colldomain.ContainsKey(entry.id) == false) continue;

                if (systemDatabase.tahversioncollision && collsiondup)
                {
                    bool collsioned = false;
                    // 衝突先のバージョンをチェックする.
                    ArcsTahEntry tah1 = ArcsDB.GetTah(entry.id);
                    foreach(ArcsCollisionRecord to in colldomain[entry.id]){
                        ArcsTahEntry tah2 = ArcsDB.GetTah(to.toTahID);
                        if (tah1.version == tah2.version)
                        {
                            collsioned = true;
                            break;
                        }
                    }
                    // 全部バージョン違いならスキップする.
                    if (collsioned == false) continue;
                }

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
                    GenericCollisionTahNode currentNode = null;
                    GenericCollisionTahNode parentNode = arcs;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (GenericCollisionTahNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new GenericCollisionTahNode(sublevel);//parentNode.Nodes.Add(sublevel);
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

        public static void MakeInstalledArcsTreeView(TreeView tvTree)
        {
            ArcsDatabase db = ArcsDB;
            //GetInstalledZipFiles
            TreeNode zips = tvTree.Nodes.Add(SystemDB.zips_path);
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
                    GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(),entry.id);
                    zips.Nodes.Add(subnode);
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
                            currentNode = new GenericFilesTreeNode(sublevel);
                            parentNode.Nodes.Add(currentNode);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // tahエントリを持つsubnodeを作る.
                    GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(),entry.id);
                    parentNode.Nodes.Add(subnode);
                }
            }
            zips.Expand();
        }

        public static void MakeTagTreeView(TreeView tvTree)
        {
            ArcsDatabase db = ArcsDB;
            // 各種変数
            Dictionary<string, TagNamesEntry> tagList = Tagnames;
            Dictionary<string, List<ArcsZipArcEntry>> zipDictionary = new Dictionary<string,List<ArcsZipArcEntry>>();
            Dictionary<int, int> installedZip = db.GetInstalledZips();

            // codeからの逆引きリストを構築する(1zip毎にSQLを実行すると遅いから)
            List<ArcsZipArcEntry> ziplist = db.GetZips();
            foreach (ArcsZipArcEntry entry in ziplist)
            {
                if (zipDictionary.ContainsKey(entry.code) == false)
                {
                    zipDictionary[entry.code]=new List<ArcsZipArcEntry>();
                }
                zipDictionary[entry.code].Add(entry);
            }

            foreach (string tag in tagList.Keys)
            {
                TreeNode zips = tvTree.Nodes.Add(tag);

                // tahを展開する.
                foreach (string code in tagList[tag].code)
                {
                    // 該当するコードのzipが無い時はスキップする.
                    if (zipDictionary.ContainsKey(code) == false)
                        continue;
                    foreach(ArcsZipArcEntry entry in zipDictionary[code])
                    {
                        char[] separetor = { '\\', '/' };
                        string[] toplevel = entry.path.Split(separetor);
                        // tahエントリを持つsubnodeを作る.
                        if (toplevel.Length == 1)
                        {
                            // tahエントリを持つsubnodeを作る.
                            GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(), entry.id);
                            zips.Nodes.Add(subnode);
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
                            GenericZipTreeNode subnode = new GenericZipTreeNode(entry.GetDisplayPath(), entry.id);
                            parentNode.Nodes.Add(subnode);

                            //インストール済みのZIPは青色に.
                            if (installedZip.ContainsKey(entry.id) == true)
                            {
                                subnode.ForeColor = Color.Blue;
                            }
                        }
                    }
                }
            }
        }

        // サブディレクトリを再帰的に調べる.
        private static void iterSubDirectory(List<string> directories, string directory,string except)
        {
            // 自分自身はスキャンしない.
            if (directory.ToLower() == except.ToLower()) return;
            // ディレクトリを追加する.
            directories.Add(directory);
            string[] entries = Directory.GetDirectories(directory);
            foreach (string entry in entries)
            {
                iterSubDirectory(directories, entry,except);
            }
        }

        public static void MakeSavefileTreeView(TreeView tvTree)
        {
            string savedir = SystemDB.savefile_directory;
            List<string> directories = new List<string>();
            iterSubDirectory(directories, savedir, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath()));

            GenericSavefileTreeNode savenode = new GenericSavefileTreeNode(savedir,savedir);
            tvTree.Nodes.Add(savenode);
            // tahを展開する.
            foreach (string dir in directories)
            {
                if (dir == savedir) continue;

                GenericSavefileTreeNode node = new GenericSavefileTreeNode(Path.GetFileName(dir), dir);
                if (node.Count == 0) continue;

                char[] separetor = { '\\', '/' };
                string entry = dir.Substring(savedir.Length + 1);
                string[] toplevel = entry.Split(separetor);

                // tahエントリを持つsubnodeを作る.
                if (toplevel.Length == 1)
                {
                    savenode.Nodes.Add(node);
                }
                else
                {
                    GenericSavefileTreeNode currentNode = null;
                    GenericSavefileTreeNode parentNode = savenode;
                    int count = 1;
                    string subdir = savedir;
                    foreach (string sublevel in toplevel)
                    {
                        subdir = Path.Combine(subdir, sublevel);
                        currentNode = null;
                        foreach (GenericSavefileTreeNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new GenericSavefileTreeNode(sublevel,subdir);//parentNode.Nodes.Add(sublevel);
                            parentNode.Nodes.Add(currentNode);
                        }
                        parentNode = currentNode;
                        if (++count == toplevel.Length) break; // 末端ノードの一つ前で止める.
                    }
                    // 末端レベルにファイル情報を格納する.
                    currentNode.Nodes.Add(node);
                }
            }
            savenode.Expand();
        }

        public static TreeNode FindNode(TreeNodeCollection nodes,string key)
        {
            foreach (TreeNode node in nodes)
                if (node.FullPath.ToLower() == key.ToLower()) return node;
            foreach (TreeNode node in nodes)
            {
                TreeNode subnode = FindNode(node.Nodes, key);
                if (subnode != null) return subnode;
            }
            return null;
        }

        public static void AddFileTree(string path)
        {
            string diretory = Path.GetDirectoryName(path);
            TreeView sftree = MainFormWindow.SaveFileTreeView;
            GenericSavefileTreeNode node = (GenericSavefileTreeNode)FindNode(sftree.Nodes, diretory);
            if (node != null)
            {
                node.Add(path);
            }
        }

        public static void DeleteFileTree(string path)
        {
            string diretory = Path.GetDirectoryName(path);
            TreeView sftree = MainFormWindow.SaveFileTreeView;
            GenericSavefileTreeNode node = (GenericSavefileTreeNode)FindNode(sftree.Nodes, diretory);
            if (node != null)
            {
                node.Del(path);
            }
        }

        // データベースがビルド済みならツリーを展開する.
        public static void IfReadyDbDisplayArcsDB()
        {
            if (SystemDB.database_build != "") //DisplayArcsDB(tvTree);
                MainFormWindow.DisplayDB();
        }

        public static bool InstallZipFile(GenericTahTreeNode sender)
        {
            GenericZipTreeNode zipNode = (GenericZipTreeNode)sender;
            ArcsZipArcEntry zipentry = ArcsDB.GetZip(zipNode.Entry);
            string zipsource = Path.Combine(TDCGExplorer.SystemDB.zips_path, zipentry.path);
            string destpath = SystemDB.work_path;
            destpath = Path.Combine(destpath, ZipFileUtil.ZipName(zipentry.path));
            
            // 展開に成功したらzipのノードの色を変える.
            if (ZipFileUtil.ExtractZipFile(zipsource, destpath) == true)
            {
                if( SystemDB.explorerzipfolder ) ExplorerSelectPath(destpath);
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
#if false
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
#endif

        private static void FindArcsTreeNode(TreeView view,TreeNode node, string path)
        {
            if (node.FullPath == path)
            {
                TreeNode parent = node.Parent;
                while (parent != null)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }
                MainFormWindow.SelectArcsTreeNode(node);
                return;
            }
            foreach (TreeNode subnode in node.Nodes)
            {
                FindArcsTreeNode(view, subnode, path);
            }
        }

        // 指定されたパスのノードを選択する.
        public static void SelectArcsTreeNode(string path)
        {
            TreeView arcsTree = MainFormWindow.ArcsTreeView;
            if (arcsTree.Nodes[0] != null)
            {
                FindArcsTreeNode(arcsTree,arcsTree.Nodes[0], Path.GetDirectoryName(path) );
            }
        }

        private static void FindZipsTreeNode(TreeView view, TreeNode node, string path)
        {
            if (node.FullPath == path)
            {
                TreeNode parent = node.Parent;
                while (parent != null)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }
                MainFormWindow.SelectZipsTreeNode(node);
                return;
            }
            foreach (TreeNode subnode in node.Nodes)
            {
                FindZipsTreeNode(view, subnode, path);
            }
        }

        // 指定されたパスのノードを選択する.
        public static void SelectZipsTreeNode(string path)
        {
            TreeView zipsTree = MainFormWindow.ZipsTreeView;
            if (zipsTree.Nodes[0] != null)
            {
                FindZipsTreeNode(zipsTree, zipsTree.Nodes[0], path);
            }
        }

        public static void InstallPreferZip(GenericZipTreeNode zipNode)
        {
            ArcsZipArcEntry zipentry = ArcsDB.GetZip(zipNode.Entry);

            // mod REF Serverに問い合わせる.

            string moddb = SystemDB.moddb_url;
            string relurl;
            ArcRels relationships;

            Dictionary<int, int> installedZip = ArcsDB.GetInstalledZips();

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
                                    ArcsZipArcEntry ziparc = TDCGExplorer.ArcsDB.GetZipByCode(arc.Code);
                                    if (ziparc != null)
                                    {
                                        // 既にインストールされている物は展開しない.
                                        if (installedZip.ContainsKey(ziparc.id) == true) continue;

                                        string zipsource = Path.Combine(TDCGExplorer.SystemDB.zips_path, ziparc.path);
                                        string destpath = SystemDB.work_path;
                                        destpath = Path.Combine(destpath, "Required " + zipentry.code);
                                        destpath = Path.Combine(destpath, ZipFileUtil.ZipName(ziparc.path));

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
//                MessageBox.Show(installedcount.ToString() + "個のzipを展開しました。\n" +
//                    misscount.ToString() + "個のzipが見つかりませんでした。", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show(installedcount.ToString() + TextResource.PrefZip1 + "\n" +
                    misscount.ToString() + TextResource.PrefZip2, TextResource.ExtractFailureCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
//                if (installedcount > 0)
//                    MessageBox.Show(installedcount.ToString() + "個のzipを展開しました。", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                else
//                    MessageBox.Show("前提zipは全てインストール済みです", "展開", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                if (installedcount > 0)
                    MessageBox.Show(installedcount.ToString() + TextResource.PrefZip1, TextResource.ExtractSuccessCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show(TextResource.PrefZip3, TextResource.ExtractSuccessCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
            if (installedcount > 0)
            {
                string destpath = SystemDB.work_path;
                destpath = Path.Combine(destpath, "Required " + zipentry.code);
                ExplorerPath(destpath);
            }
        }

        public static void TAHDecrypt(GenericTahInfo entry)
        {
#if false
            string destpath = Path.Combine(SystemDB.tahpath, Path.GetFileNameWithoutExtension(entry.shortname));
            GenericTAHStream stream = new GenericTAHStream(entry, null);
            TAHFile tah = stream.tahfile;
            if (tah != null)
            {
                int id = 0;
                foreach (TAHEntry ent in tah.EntrySet.Entries)
                {
                    string filename;
                    if (ent.FileName == null)
                    {
                        filename = Path.Combine(destpath,id.ToString("d8") + "_" + ent.Hash.ToString("x8"));
                    }
                    else
                    {
                        filename = Path.Combine(destpath, ent.FileName);
                    }
                    SetToolTips("ファイル書き込み中:" + Path.GetFileName(filename));
                    IncBusy();
                    Application.DoEvents();
                    DecBusy();
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                    if (Path.GetExtension(filename) == "") filename += TDCGTbnUtil.ext(data); // ファイル内容から拡張子を推定する
                    File.Delete(filename);
                    using (Stream writefile = File.Create(filename))
                    {
                        writefile.Write(data, 0, data.Length);
                        writefile.Flush();
                        writefile.Close();
                    }
                    id++;
                }
            }
            SetToolTips("ファイル書き込み完了:" + entry.shortname);
#endif

            // tahファイルをダンプしてファイルに保存する.
            Dictionary<UInt32,string> tsohash = new Dictionary<uint,string>();
            GenericTAHStream stream = new GenericTAHStream(entry, null);
            TAHFile tah = stream.tahfile;
            if (tah != null)
            {
                // tbnファイル名から推定されるハッシュ引き表を作る.
                foreach (TAHEntry ent in tah.EntrySet.Entries)
                {
                    if (ent.FileName != null)
                    {
                        if (Path.GetExtension(ent.FileName).ToLower() == ".tbn")
                        {
                            byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                            string tsoname = TDCGTbnUtil.GetTsoName(bytedata);
                            if (tsoname != null)
                            {
                                UInt32 hash = TAHUtil.CalcHash(tsoname);
                                if (tsohash.ContainsKey(hash) == false)
                                {
                                    tsohash[hash] = tsoname;
                                }
                                string psdpath = "data/icon/items/" + Path.GetFileNameWithoutExtension(tsoname) + ".psd";
                                UInt32 psdhash = TAHUtil.CalcHash(psdpath);
                                if (tsohash.ContainsKey(psdhash) == false)
                                {
                                    tsohash[psdhash] = psdpath;
                                }
                            }
                        }
                    }
                }
                string tahdirectory = TDCGExplorer.SystemDB.tahpath;
                string savedirectory = Path.Combine(tahdirectory, Path.GetFileNameWithoutExtension(entry.shortname));
                int id = 0;
                foreach (TAHEntry ent in tah.EntrySet.Entries)
                {
                    TDCGExplorer.SetToolTips(TextResource.TBNAnalyze);
                    TDCGExplorer.IncBusy();
                    Application.DoEvents();
                    TDCGExplorer.DecBusy();

                    byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);

                    // ファイル名を復元する.
                    string filename = ent.FileName;
                    if (filename == null)
                    {
                        if (tsohash.ContainsKey(ent.Hash) == true)
                        {
                            filename = tsohash[ent.Hash];
                        }
                        else
                        {
                            filename = id.ToString("d8") + "_" + ent.Hash.ToString("x8");
                            // 拡張子を推定する.
                            filename += TDCGTbnUtil.ext(bytedata);
                        }
                    }
                    id++;

                    // フルパスを作成する.
                    string destfilename = Path.Combine(savedirectory, filename);

                    // ファイルを保存する.
                    Directory.CreateDirectory(Path.GetDirectoryName(destfilename));
                    File.Delete(destfilename);

                    using (Stream output = File.Create(destfilename))
                    {
                        BinaryWriter writer = new BinaryWriter(output, System.Text.Encoding.Default);
                        writer.Write(bytedata, 0, bytedata.Length);
                        writer.Close();
                        output.Close();

                        TDCGExplorer.SetToolTips(TextResource.SaveFile+":"+entry.shortname+":"+Path.GetFileName(destfilename));
                        TDCGExplorer.IncBusy();
                        Application.DoEvents();
                        TDCGExplorer.DecBusy();
                    }
                }
                ExplorerSelectPath(savedirectory);
                TDCGExplorer.SetToolTips(TextResource.TAHDecryptComplete);
            }
        }

        // ディレクトリの一覧を取得する.
        private static void GetDirectories(List<string> directory,string path)
        {
            string[] files = Directory.GetFiles(path, "*");
            foreach (string file in files)
            {
                directory.Add(file);
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories)
            {
                GetDirectories(directory, dir);
            }
        }

        // TAHdecGUIクローンメイン部分.
        public static void FileDrop(string[] files)
        {
            foreach (string file in files)
            {
                string basename = Path.GetFileNameWithoutExtension(file);
                string fullpath = file;
                string filename = Path.GetFileName(file);

                // セーブファイルか?
                if (fullpath.ToLower().EndsWith("tdcgsav.png") == true || fullpath.ToLower().EndsWith("tdcgsav.bmp")==true)
                {
                    // ファイルが複数の時は新規タブで連続してオープンする.
                    if (files.Length > 1)
                    {
                        TDCGExplorer.MainFormWindow.NewTab();
                    }
                    SaveFilePage control;
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(control=new SaveFilePage(fullpath));
                    control.DisplayTso();
                    continue;
                }

                // ポーズファイルか?
                if (fullpath.ToLower().EndsWith(".tdcgpose.png") == true)
                {
                    // ファイルが複数の時は新規タブで連続してオープンする.
                    if (files.Length > 1)
                    {
                        TDCGExplorer.MainFormWindow.NewTab();
                    }
                    PoseFilePage control;
                    TDCGExplorer.MainFormWindow.AssignTagPageControl(control=new PoseFilePage(fullpath));
                    control.DisplayPose();
                    continue;
                }
#if false
                if (File.Exists(LBFileTahUtl.GetTahDbPath(basename)))
                {
                    MessageBox.Show("既にデータベースファイルがあります。\n" + LBFileTahUtl.GetTahDbPath(basename) + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                    continue;
                }
#endif
                // TAHファイルをドロップされた
                if (Path.GetExtension(filename).ToLower() == ".tah")
                {
                    try
                    {
                        // TAHエディタを開いて、TAHファイルの中身をコピーする.
                        TAHEditor editor = null;
                        try
                        {
                            editor = new TAHEditor(null);
                            Object transaction = editor.BeginTransaction();
                            using (Stream stream = File.OpenRead(fullpath))
                            {
                                using (TAHFile tah = new TAHFile(stream))
                                {
                                    tah.LoadEntries();
                                    // TAHヘッダ情報を複製する.
                                    int index = 0;
                                    foreach (TAHEntry ent in tah.EntrySet.Entries)
                                    {
                                        string tahfile = ent.FileName;
                                        if (tahfile == null)
                                        {
                                            tahfile = index.ToString("d8") + "_" + ent.Hash.ToString("x8");
                                        }
                                        SetToolTips(TextResource.FileReading+":" + tahfile);
                                        byte[] tahdata = TAHUtil.ReadEntryData(tah.Reader, ent);
                                        if (Path.GetExtension(tahfile) == "") tahfile += TDCGTbnUtil.ext(tahdata); // ファイル内容から拡張子を推定する
                                        editor.AddItem(tahfile, tahdata);
                                        IncBusy();
                                        Application.DoEvents();
                                        DecBusy();
                                        index++;
                                    }
                                    editor.SetInformation(filename, (int)tah.Header.Version);
                                }
                            }
                            editor.Commit(transaction);
                            // ファイルが複数の時は新規タブで連続してオープンする.
                            if (files.Length > 1)
                            {
                                TDCGExplorer.MainFormWindow.NewTab();
                            }
                            TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                            editor.SelectAll();
                        }
                        catch (Exception)
                        {
                            if (editor != null) editor.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(TextResource.TAHFileReadError+"\n" + exception.Message, TextResource.Error, MessageBoxButtons.OK);
                    }
                }
                else
                {
                    try
                    {
                        List<string> dir = new List<string>();
                        GetDirectories(dir, fullpath);

                        // TAHエディタを開いて、TAHファイルの中身をコピーする.
                        TAHEditor editor = null;
                        try
                        {
                            editor = new TAHEditor(null);
                            Object transaction = editor.BeginTransaction();
                            foreach (string infile in dir)
                            {
                                using (Stream stream = File.OpenRead(infile))
                                {
                                    string newpath = infile.Substring(fullpath.Length + 1).Replace('\\', '/');
                                    SetToolTips(TextResource.FileReading+":" + newpath);
                                    MemoryStream ms = new MemoryStream();
                                    ZipFileUtil.CopyStream(stream, ms);
                                    byte[] tahdata = ms.ToArray();
                                    editor.AddItem(newpath, tahdata);
                                    IncBusy();
                                    Application.DoEvents();
                                    DecBusy();
                                }
                            }
                            editor.SetInformation(basename + ".tah", 1);
                            editor.Commit(transaction);
                            // ファイルが複数の時は新規タブで連続してオープンする.
                            if (files.Length > 1)
                            {
                                TDCGExplorer.MainFormWindow.NewTab();
                            }
                            TDCGExplorer.MainFormWindow.AssignTagPageControl(editor);
                            editor.SelectAll();
                        }
                        catch (Exception)
                        {
                            if (editor != null) editor.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(TextResource.FileReadError+"\n" + exception.Message, TextResource.Error, MessageBoxButtons.OK);
                    }
                }
            }
        }

        // 指定されたパス以下にあるtahを全部一つのtahエディタで開く.
        public static void TAHCompaction(string startwith)
        {
            if (TDCGExplorer.BusyTest()) return;
            UInt64 size = 0;

            Dictionary<UInt32, uint> versiontable = new Dictionary<uint, uint>();

            TAHEditor editor = null;
            try
            {
                TAHInfoDialog dialog = new TAHInfoDialog();
                dialog.Owner = TDCGExplorer.MainFormWindow;
                dialog.tahVersion = 2;
                dialog.tahSource = Path.GetFileNameWithoutExtension(startwith) + ".tah";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
#if false
                    // 新規TAHを作成する.
                    string dbfilename = LBFileTahUtl.GetTahDbPath(dialog.tahSource);

                    if (File.Exists(dbfilename))
                    {
                        MessageBox.Show("既にデータベースファイルがあります。\n" + dbfilename + "\n削除してから操作してください。", "エラー", MessageBoxButtons.OK);
                        return;
                    }
#endif
                    // 常に新規タブで.
                    editor = new TAHEditor(null);
                    editor.SetInformation(dialog.tahSource, dialog.tahVersion);

                    Object transaction = editor.BeginTransaction();

                    // とりあえずtahを全部getする.
                    List<ArcsTahEntry> list = arcsDatabase.GetTahs();

                    // まずファイルサイズチェック.
                    foreach (ArcsTahEntry entry in list)
                    {
                        // 指定された階層以下なばら
                        if (entry.path.StartsWith(startwith + "\\") == true)
                        {
                            using (Stream file_stream = File.OpenRead(Path.Combine(SystemDB.arcs_path, entry.path)))
                            {
                                // ファイル長を足していく.
                                UInt64 filesize = (UInt64)file_stream.Length;
                                size = size + filesize;
                                if (size >= 0x100000000L) break;
                            }
                        }
                    }
                    if (size >= 0x100000000L)
                    {
                        MessageBox.Show(TextResource.TAHFileLimitError,TextResource.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        throw new Exception("File Limit Over");
                    }

                    // ファイルサイズに問題が無いなら集約
                    foreach (ArcsTahEntry entry in list)
                    {
                        // 指定された階層以下なばら
                        if (entry.path.StartsWith(startwith+"\\") == true)
                        {
                            using (Stream file_stream = File.OpenRead(Path.Combine(SystemDB.arcs_path, entry.path)))
                            {
#if false
                                // ファイル長を足していく.
                                UInt64 filesize = (UInt64) file_stream.Length;
                                size = size + filesize;
                                if (size >= 0x100000000L) break;
#endif
                                TAHFile tah = new TAHFile(file_stream);
                                try
                                {
                                    tah.LoadEntries();

                                    Dictionary<UInt32,string> tsohash = new Dictionary<uint,string>();
                                    if (tah != null)
                                    {
                                        // tbnファイル名から推定されるハッシュ引き表を作る.
                                        foreach (TAHEntry ent in tah.EntrySet.Entries)
                                        {
                                            if (ent.FileName != null)
                                            {
                                                if (Path.GetExtension(ent.FileName).ToLower() == ".tbn")
                                                {
                                                    byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                                                    string tsoname = TDCGTbnUtil.GetTsoName(bytedata);
                                                    if (tsoname != null)
                                                    {
                                                        UInt32 hash = TAHUtil.CalcHash(tsoname);
                                                        if (tsohash.ContainsKey(hash) == false)
                                                        {
                                                            tsohash[hash] = tsoname;
                                                        }
                                                        string psdpath = "data/icon/items/" + Path.GetFileNameWithoutExtension(tsoname) + ".psd";
                                                        UInt32 psdhash = TAHUtil.CalcHash(psdpath);
                                                        if (tsohash.ContainsKey(psdhash) == false)
                                                        {
                                                            tsohash[psdhash] = psdpath;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        string tahdirectory = TDCGExplorer.SystemDB.tahpath;
                                        string savedirectory = Path.Combine(tahdirectory, Path.GetFileNameWithoutExtension(entry.shortname));
                                        int id = 0;
                                        foreach (TAHEntry ent in tah.EntrySet.Entries)
                                        {
                                            // ファイル名を復元する.
                                            string filename = ent.FileName;
                                            if (filename == null)
                                            {
                                                if (tsohash.ContainsKey(ent.Hash) == true)
                                                {
                                                    filename = tsohash[ent.Hash];
                                                }
                                                else
                                                {
                                                    byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);
                                                    filename = id.ToString("d8") + "_" + ent.Hash.ToString("x8");
                                                    // 拡張子を推定する.
                                                    filename += TDCGTbnUtil.ext(bytedata);
                                                }
                                            }
                                            id++;

                                            TDCGExplorer.SetToolTips(TextResource.TAHFileReading + " : " + filename);
                                            TDCGExplorer.IncBusy();
                                            Application.DoEvents();
                                            TDCGExplorer.DecBusy();

                                            // バージョンテーブル未登録ならば登録する
                                            if (versiontable.ContainsKey(ent.Hash) == false)
                                            {
                                                byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);

                                                versiontable.Add(ent.Hash, tah.Header.Version);
                                                editor.AddItem(filename, bytedata);
                                            }
                                            else // 既に登録されている場合、新しい場合のみ更新する
                                            {
                                                if (versiontable[ent.Hash] < tah.Header.Version)
                                                {
                                                    byte[] bytedata = TAHUtil.ReadEntryData(tah.Reader, ent);

                                                    editor.AddItem(filename, bytedata);
                                                    versiontable[ent.Hash] = tah.Header.Version;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
#if false
                    if (size >= 0x100000000L)
                    {
                        MessageBox.Show("読み込んだtahファイルが4GBを超えました。", "展開エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        throw new Exception("File Limit Over");
                    }
#endif
                    editor.Commit(transaction);
                    MainFormWindow.AssignTagPageControl(editor);
                    editor.SelectAll();
                    TDCGExplorer.SetToolTips(TextResource.TAHEncryptComplete);
                }
            }
            catch (Exception)
            {
                if (editor != null) editor.Dispose();
                TDCGExplorer.SetToolTips(TextResource.TAHFileReadError);
            }
        }

        public static void ExplorerPath(string destpath)
        {
            if (Directory.Exists(destpath) == true || File.Exists(destpath)==true)
                System.Diagnostics.Process.Start(@"EXPLORER.EXE", "\"" + destpath + "\"");
        }

        public static void ExplorerSelectPath(string destpath)
        {
            if (Directory.Exists(destpath) == true || File.Exists(destpath) == true)
                System.Diagnostics.Process.Start(@"EXPLORER.EXE", "/SELECT,\"" + destpath + "\"");
        }

        public static string GetDirectXVersion()
        {
            string directXVersion = "Unknown";
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\DirectX");
                directXVersion = key.GetValue("Version").ToString();
            }
            catch(Exception)
            {
            }
            switch(directXVersion){
                case "4.08.01.0810":
                    directXVersion += " (8.1)";
                    break;
                case "4.08.01.0881":
                    directXVersion += " (8.1)";
                    break;
                case "4.08.01.0901":
                    directXVersion += " (8.1a)";
                    break;
                case "4.08.02.0134":
                    directXVersion += " (8.2)";
                    break;
                case "4.09.00.0900":
                    directXVersion += " (9)";
                    break;
                case "4.09.00.0901":
                    directXVersion += " (9a)";
                    break;
                case "4.09.00.0902":
                    directXVersion += " (9b)";
                    break;
                case "4.09.00.0903":
                    directXVersion += " (9c)";
                    break;
                case "4.09.00.0904":
                    directXVersion += " (9c)";
                    break;
                default:
                    break;
            }
            return directXVersion;
        }
    }

    public class CreateArcsDatabaseThread
    {
        public void Run()
        {
            // busyでないなら.
            if (TDCGExplorer.BusyCount == 0)
            {
                TDCGExplorer.IncBusy();
                try
                {
                    string arcpath = TDCGExplorer.SystemDB.arcs_path;
                    string zippath = TDCGExplorer.SystemDB.zips_path;
                    // クローンだとかえって動作がおかしい.
                    ArcsDatabase arcs = TDCGExplorer.ArcsDB;
                    using (SQLiteTransaction transacion = arcs.BeginTransaction())
                    {
                        arcs.CreateInformationTable();
                        arcs.CreateTahDatabase();
                        arcs.CreateFilesDatabase();
                        arcs.CreateZipDatabase();
                        arcs.CreateZipTahDatabase();
                        arcs.CreateZipTahFilesDatabase();
                        arcs.CreateInstalledZipTable();
                        arcs.DropIndex(); // 一旦インデックスを削除する.
                        TDCGTAHDump.ArcsDumpDirEntriesMain(arcpath, arcs);
                        TDCGTAHDump.ZipsDumpDirEntriesMain(zippath, arcs);
                        // インストール済みZIPの表を作成する.
                        TDCGExplorer.SetToolTips("Execute SQL Trsansactions");
                        arcs.CreateIndex(); // インデックスを作成する.
                        arcs.CreateInstalledZips();
                        transacion.Commit();

                        if(TDCGExplorer.SystemDB.arcsvacume) arcs.Vacuum();

                        TDCGExplorer.SetToolTips("Database build complete");
                        TDCGExplorer.SystemDB.database_build = "yes";
                        arcs["version"] = TDCGExplorer.CONST_DBVERSION;
                    }
                }
                catch (Exception e)
                {
                    TDCGExplorer.SetToolTips("Error CreateArcsDatabaseThread : (" + TDCGExplorer.LastAccessFile + ") " + e.Message);
                }
                TDCGExplorer.DecBusy();

                TDCGExplorer.MainFormWindow.asyncDisplayFromArcs(); // 表示更新.
            }
        }
    }
}
