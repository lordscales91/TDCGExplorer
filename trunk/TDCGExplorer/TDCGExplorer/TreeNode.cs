using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public class TahGenTreeNode : System.Windows.Forms.TreeNode
    {
        public TahGenTreeNode(string text)
            : base(text)
        {
        }
        public virtual void DoTvTreeSelect(TahGenTreeNode sender, ListBox mainListBox)
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

        public override void DoTvTreeSelect(TahGenTreeNode sender, ListBox mainListBox)
        {
            mainListBox.Items.Clear();
            foreach (ArcsTahEntry file in Entries)
            {
                mainListBox.Items.Add(new LbFileItem(file));
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

        public override void DoTvTreeSelect(TahGenTreeNode sender, ListBox mainListBox)
        {
            mainListBox.Items.Clear();
            //セレクトされたときにSQLに問い合わせる.
            List<ArcsZipTahEntry> files = TDCGExplorer.GetArcsDatabase().GetZipTahs(zipid);
            foreach (ArcsZipTahEntry file in files)
            {
                mainListBox.Items.Add(new LbZipFileItem(file));
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


        public override void DoTvTreeSelect(TahGenTreeNode sender, ListBox mainListBox)
        {
            mainListBox.Items.Clear();
            foreach (CollisionItem file in entries)
            {
                mainListBox.Items.Add(new LbCollisionItem(file));
            }
        }
    }

}
