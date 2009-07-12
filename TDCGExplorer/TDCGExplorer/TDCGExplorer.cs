
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
        private static SystemDatabase systemDatabase;
        private static ArcsDatabase arcsDatabase;
        private static AnnotationDB annotationDatabase;
        private static ArcNamesDictionary arcNames;
        private static TagNamesDictionary tagNames;
        private static MainForm form;
        private static Byte[] defaultTMO;
        private static string lastAccessFile = null;

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
            tagNames = new TagNamesDictionary();
            annotationDatabase = new AnnotationDB();

            TAHEntry.ReadExternalFileList();
            arcNames.Init();
            tagNames.Init();

            ResetDefaultPose();

            SetToolTips("Copyright © 2009 3DCG Craftsmen's Guild.");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(form = new MainForm());
            }
            catch (Exception ex)
            {
                if (ex.Message != null && ex.Message == "A generic error occurred in GDI+.")
                {
                    MessageBox.Show("大変申し訳ありません。\n\n" +
                                     "プログラムはWindowsのバグによって終了しました。\n" +
                                     "デバッグ情報を" + GetAppDataPath() + "に保存します。\n"+
                                     "このエラーはMicrosoftにお問い合わせ下さい。",
                                     "Windowsサブシステムでエラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("大変申し訳ありません。\n\n" +
                                     "プログラムは予期せぬ例外によって終了しました。\n" +
                                     "デバッグ情報を" + GetAppDataPath() + "に保存します。",
                                     "深刻なエラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                string savepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath());
                savepath = Path.Combine(savepath, "デバッグ情報.txt");
                File.Delete(savepath);
                using (Stream stream = File.OpenWrite(savepath))
                {
                    StreamWriter writer = new StreamWriter(stream);

                    if (lastAccessFile != null)
                    {
                        writer.WriteLine("最後にアクセスしたファイル:");
                        writer.WriteLine(lastAccessFile);
                    }
                    if (ex.Message != null)
                    {
                        writer.WriteLine("Message:");
                        writer.WriteLine(ex.Message);
                    }
                    if (ex.Source != null)
                    {
                        writer.WriteLine("Source:");
                        writer.WriteLine(ex.Source);
                    }
                    if (ex.HelpLink != null)
                    {
                        writer.WriteLine("HelpLink:");
                        writer.WriteLine(ex.HelpLink);
                    }
                    if (ex.InnerException != null)
                    {
                        writer.WriteLine("InnerException:");
                        writer.WriteLine(ex.InnerException);
                    }
                    if (ex.StackTrace != null)
                    {
                        writer.WriteLine("StackTrace:");
                        writer.WriteLine(ex.StackTrace);
                    }
                    if (ex.TargetSite != null)
                    {
                        writer.WriteLine("TargetSite:");
                        writer.WriteLine(ex.TargetSite);
                    }
                    if (ex.Data != null)
                    {
                        writer.WriteLine("Data:");
                        writer.WriteLine(ex.Data);
                    }
                    writer.Close();
                    stream.Close();
                }
            }

            arcsDatabase.Dispose();
            systemDatabase.Dispose();
        }

        public static string SetLastAccessFile
        {
            set { lastAccessFile = value; }
        }

        private static Byte[] LoadTMO(string path)
        {
            //TDCGExplorer.SetLastAccessFile = path;
            FileStream fs = File.OpenRead(path);
            BinaryReader reader = new BinaryReader(fs, System.Text.Encoding.Default);
            return reader.ReadBytes((int)fs.Length);
        }

        public static void ResetDefaultPose()
        {
            defaultTMO = LoadTMO("default.tmo");
        }

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
            }
        }

        public static void MakeArcsTreeView(TreeView tvTree)
        {
            ArcsDatabase db = ArcsDB;
            FilesTreeNode arcs = new FilesTreeNode(SystemDB.arcs_path);
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
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
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
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
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
            ArcsDatabase db = ArcsDB;
            CollisionTahNode arcs = new CollisionTahNode(SystemDB.arcs_path);
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
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
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
                    ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(),entry.id);
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
                            ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(), entry.id);
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
                            ZipTreeNode subnode = new ZipTreeNode(entry.GetDisplayPath(), entry.id);
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

            SavefileNode savenode = new SavefileNode(savedir,savedir);
            tvTree.Nodes.Add(savenode);
            // tahを展開する.
            foreach (string dir in directories)
            {
                if (dir == savedir) continue;

                SavefileNode node = new SavefileNode(Path.GetFileName(dir), dir);
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
                    SavefileNode currentNode = null;
                    SavefileNode parentNode = savenode;
                    int count = 1;
                    foreach (string sublevel in toplevel)
                    {
                        currentNode = null;
                        foreach (SavefileNode nodes in parentNode.Nodes)
                        {
                            if (nodes.Text == sublevel)
                            {
                                currentNode = nodes;
                                break;
                            }
                        }
                        if (currentNode == null)
                        {
                            currentNode = new SavefileNode(sublevel,dir);//parentNode.Nodes.Add(sublevel);
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

        // データベースがビルド済みならツリーを展開する.
        public static void IfReadyDbDisplayArcsDB()
        {
            if (SystemDB.database_build != "") //DisplayArcsDB(tvTree);
                MainFormWindow.DisplayDB();
        }

        public static bool InstallZipFile(TahGenTreeNode sender)
        {
            ZipTreeNode zipNode = (ZipTreeNode)sender;
            ArcsZipArcEntry zipentry = ArcsDB.GetZip(zipNode.Entry);
            string zipsource = Path.Combine(TDCGExplorer.SystemDB.zips_path, zipentry.path);
            string destpath = SystemDB.work_path;
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

        public static void TAHDecrypt(GenTahInfo entry)
        {
            string destpath = Path.Combine(SystemDB.work_path, Path.GetFileNameWithoutExtension(entry.shortname));
            TAHStream stream = new TAHStream(entry, null);
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
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    File.Delete(filename);
                    byte[] data = TAHUtil.ReadEntryData(tah.Reader, ent);
                    using (Stream writefile = File.OpenWrite(filename))
                    {
                        writefile.Write(data, 0, data.Length);
                        writefile.Flush();
                        writefile.Close();
                    }
                    id++;
                }
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
                string arcpath = TDCGExplorer.SystemDB.arcs_path;
                string zippath = TDCGExplorer.SystemDB.zips_path;
#if false
                // クローンを作る.
                ArcsDatabase arcs = new ArcsDatabase(TDCGExplorer.GetArcsDatabase());
#else
                // クローンだとかえって動作がおかしい.
                ArcsDatabase arcs = TDCGExplorer.ArcsDB;
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

                    arcs.Vacuum();

                    TDCGExplorer.MainFormWindow.asyncDisplayFromArcs(); // 表示更新.

                    TDCGExplorer.fThreadRun = false;
                    TDCGExplorer.SetToolTips("Database build complete");
                    TDCGExplorer.SystemDB.database_build = "yes";
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
