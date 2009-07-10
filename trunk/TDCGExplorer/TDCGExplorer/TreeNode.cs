using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace TDCGExplorer
{
    public class TahGenTreeNode : System.Windows.Forms.TreeNode
    {
        public TahGenTreeNode(string text)
            : base(text)
        {
        }
        public virtual void DoTvTreeSelect()
        {
        }
        public virtual void DoEditAnnotation()
        {
        }
    }

    // ファイル個別情報のファイルツリーノード.
    public class FilesTreeNode : TahGenTreeNode
    {
        private List<ArcsTahEntry> entries = new List<ArcsTahEntry>();

        public FilesTreeNode(string text)
            : base(text)
        {
        }

        public List<ArcsTahEntry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.GetMainForm().ListBoxClear();
            foreach (ArcsTahEntry file in Entries)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbFileItem(file));
            }
        }
    }
    // ArcsZipFileEntryを保持するTreeNode
    public class ZipTreeNode : TahGenTreeNode
    {
        int zipid;

        public ZipTreeNode(string text, int zipid)
            : base(text)
        {
            this.zipid = zipid;
        }

        public int Entry
        {
            get { return zipid; }
            set { zipid = value; }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.GetMainForm().ListBoxClear();
            //セレクトされたときにSQLに問い合わせる.
            List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(zipid);
            foreach (ArcsZipTahEntry file in files)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbZipFileItem(file));
            }
#if false
            // MODREFサーバに問い合わせる.
            if (TDCGExplorer.GetSystemDatabase().modrefserver_alwaysenable == "true")
            {
                TDCGExplorer.GetMainForm().AssignTagPageControl(new MODRefPage(zipid));
            }
#endif
            // ZIPページを開いた時の動作を指定する (none:なにもしない server:サーバにアクセス image:画像表示 text:テキスト表示)
            // public string zippage_behavior
            switch (TDCGExplorer.GetSystemDatabase().zippage_behavior)
            {
                case "server":
                    {
                        TDCGExplorer.GetMainForm().AssignTagPageControl(new MODRefPage(zipid));
                    }
                    return;
                case "image":
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".bmp" || ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".tif")
                        {
                            string savefilpath = entry.path.ToLower();
                            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp")) continue;

                            TDCGExplorer.GetMainForm().AssignTagPageControl(new ImagePageControl(new ZipTahInfo(entry)));
                            return;
                        }
                    }
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".txt" || ext == ".doc" || ext == ".xml")
                        {
                            TDCGExplorer.GetMainForm().AssignTagPageControl(new TextPageControl(new ZipTahInfo(entry)));
                            return;
                        }
                    }
                    break;
                case "text":
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".txt" || ext == ".doc" || ext == ".xml")
                        {
                            TDCGExplorer.GetMainForm().AssignTagPageControl(new TextPageControl(new ZipTahInfo(entry)));
                            return;
                        }
                    }
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".bmp" || ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".tif")
                        {
                            string savefilpath = entry.path.ToLower();
                            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp")) continue;

                            TDCGExplorer.GetMainForm().AssignTagPageControl(new ImagePageControl(new ZipTahInfo(entry)));
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        // アノテーションを入力する.
        public override void DoEditAnnotation()
        {
            AnnotationEdit edit = new AnnotationEdit();
            ArcsZipArcEntry zip = TDCGExplorer.GetArcsDatabase().GetZip(zipid);
            Dictionary<string, string> annon = TDCGExplorer.GetAnnoDatabase().annotation;
            edit.text = zip.GetDisplayPath();
            edit.code = zip.code;
            // エディットがOKなら書き換える.
            if (edit.ShowDialog() == DialogResult.OK)
            {
                TDCGExplorer.GetAnnoDatabase().SetSqlValue(zip.code, edit.text);
                Text = edit.text;
            }
        }
    }

    public class CollisionTahNode : TahGenTreeNode
    {
        List<CollisionItem> entries = new List<CollisionItem>();

        public CollisionTahNode(string text)
            : base(text)
        {
        }

        public List<CollisionItem> Entries
        {
            get { return entries; }
            set { entries = value; }
        }


        public override void DoTvTreeSelect()
        {
            TDCGExplorer.GetMainForm().ListBoxClear();
            foreach (CollisionItem file in entries)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbCollisionItem(file));
            }
        }
    }

    public class SavefileNode : TahGenTreeNode
    {
        List<string> Files = new List<string>();

        public SavefileNode(string text,string directory) : base( text )
        {
            string[] files = Directory.GetFiles(directory, "*.*");
            foreach (string file in files)
            {
                if (file.EndsWith(".tdcgsav.png") || file.EndsWith(".tdcgsav.bmp"))
                {
                    Files.Add(file);
                }
            }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.GetMainForm().ListBoxClear();
            foreach (string file in Files)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbSaveFileItem(file));
            }
        }

        public int Count
        {
            get { return Files.Count; }
        }
    }
}
