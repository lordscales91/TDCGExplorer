using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDCGExplorer
{
    // TAH情報を抽象化する.
    public class GenTahInfo
    {
        public virtual int id
        {
            get { return -1; }
            set { }
        }
        public virtual int zipid
        {
            get { return -1; }
            set { }
        }
        public virtual string path
        {
            get { return null; }
            set { }
        }
        public virtual string shortname
        {
            get { return null; }
            set { }
        }
        public virtual int version
        {
            get { return -1; }
            set { }
        }
    }

    public class TahInfo : GenTahInfo
    {
        private ArcsTahEntry tahEntry;
        public TahInfo(ArcsTahEntry entry)
        {
            tahEntry = entry;
        }
        public override int id
        {
            get { return tahEntry.id; }
            set { }
        }
        public override string path
        {
            get { return tahEntry.path; }
            set { }
        }
        public override string shortname
        {
            get { return tahEntry.shortname; }
            set { }
        }
        public override int version
        {
            get { return tahEntry.version; }
            set { }
        }
    }
    public class ZipTahInfo : GenTahInfo
    {
        private ArcsZipTahEntry tahEntry;
        public ZipTahInfo(ArcsZipTahEntry entry)
        {
            tahEntry = entry;
        }
        public override int id
        {
            get { return tahEntry.id; }
            set { }
        }
        public override int zipid
        {
            get { return tahEntry.zipid; }
            set { }
        }
        public override string path
        {
            get { return tahEntry.path; }
            set { }
        }
        public override string shortname
        {
            get { return tahEntry.shortname; }
            set { }
        }
        public override int version
        {
            get { return tahEntry.version; }
            set { }
        }
    }
}
