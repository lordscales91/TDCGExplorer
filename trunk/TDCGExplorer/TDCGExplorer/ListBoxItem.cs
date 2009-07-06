using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TDCGExplorer
{
    public class LbGenItem : Object
    {
        public virtual void DoClick(TabControl tabMainView,bool newTab)
        {
        }
        public TabPage CloseLastSelectObject()
        {
            TabPage tab = TDCGExplorer.GetMainForm().TabControlMainView.SelectedTab;
            return tab;
        }
    }

    public class LbFileItem : LbGenItem
    {
        ArcsTahEntry entry;
        public LbFileItem(ArcsTahEntry argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return Path.GetFileName(entry.path);
        }
        public override void DoClick(TabControl tabMainView, bool newTab)
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    TabPage lastTab = CloseLastSelectObject();
                    TAHPage tahPage = new TAHPage(new TahInfo(entry), TDCGExplorer.GetArcsDatabase().GetTahFilesPath(entry.id));
                    tabMainView.Controls.Add(tahPage);
                    tabMainView.SelectTab(tabMainView.Controls.Count - 1);
                    if (newTab == false && lastTab != null) lastTab.Dispose();
                    break;
            }
        }
    }

    public class LbCollisionItem : LbGenItem
    {
        CollisionItem entry;
        public LbCollisionItem(CollisionItem argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return Path.GetFileName(entry.tah.path);
        }
        public override void DoClick(TabControl tabMainView, bool newTab)
        {
            switch (Path.GetExtension(entry.tah.shortname))
            {
                case ".tah":
                    TabPage lastTab = CloseLastSelectObject();
                    CollisionTahPage tahPage = new CollisionTahPage(entry);
                    tabMainView.Controls.Add(tahPage);
                    tabMainView.SelectTab(tabMainView.Controls.Count - 1);
                    if (newTab == false && lastTab != null) lastTab.Dispose();
                    break;
            }
        }
    }

    public class LbZipFileItem : LbGenItem
    {
        ArcsZipTahEntry entry;
        public LbZipFileItem(ArcsZipTahEntry argentry)
        {
            entry = argentry;
        }
        public override string ToString()
        {
            return entry.path;
        }
        public override void DoClick(TabControl tabMainView, bool newTab)
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.path.ToLower()))
            {
                case ".tah":
                    TabPage lastTab = CloseLastSelectObject();
                    TAHPage tahPage = new TAHPage(new ZipTahInfo(entry), TDCGExplorer.GetArcsDatabase().GetZipTahFilesEntries(entry.id));
                    tabMainView.Controls.Add(tahPage);
                    tabMainView.SelectTab(tabMainView.Controls.Count - 1);
                    if (newTab == false && lastTab != null) lastTab.Dispose();
                    break;
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".gif":
                case ".tif":
                case ".tga":
                    TabPage lastTab1 = CloseLastSelectObject();
                    ImagePage imgPage = new ImagePage(new ZipTahInfo(entry));
                    tabMainView.Controls.Add(imgPage);
                    tabMainView.SelectTab(tabMainView.Controls.Count - 1);
                    if (newTab == false && lastTab1 != null) lastTab1.Dispose();
                    break;
                case ".txt":
                case ".doc":
                case ".xml":
                    TabPage lastTab2 = CloseLastSelectObject();
                    TextPage txtPage = new TextPage(new ZipTahInfo(entry));
                    tabMainView.Controls.Add(txtPage);
                    tabMainView.SelectTab(tabMainView.Controls.Count - 1);
                    if (newTab == false && lastTab2 != null) lastTab2.Dispose();
                    break;
            }
        }
    }
}
