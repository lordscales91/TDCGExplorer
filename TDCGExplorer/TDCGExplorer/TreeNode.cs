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
        public virtual void DoLookupServer()
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
            TDCGExplorer.GetMainForm().ListBoxMainView.Items.Clear();
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
            TDCGExplorer.GetMainForm().ListBoxMainView.Items.Clear();
            //セレクトされたときにSQLに問い合わせる.
            List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(zipid);
            foreach (ArcsZipTahEntry file in files)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbZipFileItem(file));
            }
        }
        // MOD Refサーバに問い合わせる.
        public override void DoLookupServer()
        {
            TabPage lastTab = TDCGExplorer.GetMainForm().TabControlMainView.SelectedTab;
            MODRefPage modprefpage = new MODRefPage(zipid);
            TDCGExplorer.GetMainForm().TabControlMainView.Controls.Add(modprefpage);
            TDCGExplorer.GetMainForm().TabControlMainView.SelectTab(TDCGExplorer.GetMainForm().TabControlMainView.Controls.Count - 1);
            if (lastTab != null) lastTab.Dispose();
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
            TDCGExplorer.GetMainForm().ListBoxMainView.Items.Clear();
            foreach (CollisionItem file in entries)
            {
                TDCGExplorer.GetMainForm().ListBoxMainView.Items.Add(new LbCollisionItem(file));
            }
        }
    }

}
