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
                case ".tga":
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
}
