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
        public virtual void DoClick()
        {
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
        public override void DoClick()
        {
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.shortname))
            {
                case ".tah":
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new TAHPageControl(new TahInfo(entry), TDCGExplorer.GetArcsDatabase().GetTahFilesPath(entry.id)));
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
        public override void DoClick()
        {
            switch (Path.GetExtension(entry.tah.shortname))
            {
                case ".tah":
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new CollisionTahPageControl(entry));
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
        public override void DoClick()
        {
            // セーブファイルか?
            string savefilpath = entry.path.ToLower();
            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp"))
            {
                // 既に実行中の時は操作禁止
                if(SaveFilePage.Busy==false)
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new SaveFilePage(new ZipTahInfo(entry)));
                return;
            }
            // TAHファイル内容に関するフォームを追加する.
            switch (Path.GetExtension(entry.path.ToLower()))
            {
                case ".tah":
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new TAHPageControl(new ZipTahInfo(entry), TDCGExplorer.GetArcsDatabase().GetZipTahFilesEntries(entry.id)));
                    break;
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".gif":
                case ".tif":
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new ImagePageControl(new ZipTahInfo(entry)));
                    break;

                case ".txt":
                case ".doc":
                case ".xml":
                    TDCGExplorer.GetMainForm().AssignTagPageControl(new TextPageControl(new ZipTahInfo(entry)));
                    break;
            }
        }
    }
    // セーブファイル専用リストボックスアイテム.
    public class LbSaveFileItem : LbGenItem
    {
        string path;
        public LbSaveFileItem(string itpath)
        {
            path = itpath;
        }
        public override string ToString()
        {
            return Path.GetFileName(path);
        }
        public override void DoClick()
        {
            // 既に実行中の時は操作禁止
            if(SaveFilePage.Busy==false)
                TDCGExplorer.GetMainForm().AssignTagPageControl(new SaveFilePage(path));
        }
    }

}
