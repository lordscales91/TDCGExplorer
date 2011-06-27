using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;
using System.Windows.Forms;
using System.IO;

namespace TDCGExplorer
{
    public class DirectAccessArchive : IArchive, IEnumerable<IArchiveEntry>, IDisposable
    {
        internal string basepath;
        internal List<string> fileList;

        public DirectAccessArchive()
        {
            fileList = new List<string>();
            basepath = "";
        }

        // ディレクトリ一覧を取得する.
        public void Open(string filename)
        {
            fileList.Clear();
            basepath = filename;
            ListUpFiles(basepath);
        }

        private void ListUpFiles(string directory)
        {
            string[] zip_files = Directory.GetFiles(directory, "*.*");
            foreach (string file in zip_files)
            {
                fileList.Add(file.Substring(basepath.Length + 1));
            }
            string[] entries = Directory.GetDirectories(directory);
            foreach (string entry in entries)
            {
                ListUpFiles(entry);
            }
        }

        // 使わない
        public string FileName
        {
            get
            {
                MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
                return ""; 
            }
            set
            {
                MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
            }
        }

        // 使わない
        public string Password
        {
            get 
            {
                MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
                return ""; 
            }
            set 
            {
                MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
            }
        }

        // 使わない
        public IArchiveEntry[] GetAllEntries()
        {
            MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
            return null;
        }
        // 使う
        IEnumerator<IArchiveEntry> IEnumerable<IArchiveEntry>.GetEnumerator()
        {
            return new DirectAccessArchiveEnumerator(this);
        }

        // 誰が使う?
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DirectAccessArchiveEnumerator(this);
        }

        // 使う.
        public void Extract(ArchiveLib.IArchiveEntry entry, System.IO.Stream output)
        {
            TDCGExplorer.LastAccessFile = entry.FileName;
            Stream input = File.OpenRead(Path.Combine(basepath, entry.FileName));

            // streamにコピーする.
            byte[] buf = new byte[1024];
            int len;
            while ((len = input.Read(buf, 0, buf.Length)) > 0)
            {
                output.Write(buf, 0, len);
            }
        }

        // 使わない
        public void Extract(ArchiveLib.IArchiveEntry entry, string filename)
        {
            MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
        }

        // 使わない
        public void Extract(ArchiveLib.IArchiveEntry entry, byte[] buffer)
        {
            MessageBox.Show("Not Implemented", "Error", MessageBoxButtons.OK);
        }

        // 使う
        public void Dispose()
        {
            fileList.Clear();
            fileList = null;
        }
    }

    public class DirectAccessArchiveEntry : IArchiveEntry
    {
        long size;
        string filename;

        public long Size { get { return size; } }
        public bool IsDirectory { get { return false; } }
        public string FileName { get { return filename; } }
        public long CompressedSize { get { return size; } }
        public override string ToString() { return filename; }
        public DirectAccessArchiveEntry(string basepath,string localpath)
        {
            FileInfo fi = new System.IO.FileInfo(Path.Combine(basepath,localpath));
            size = fi.Length;
            filename = localpath;
        }
    }

    public class DirectAccessArchiveEnumerator : IEnumerator<IArchiveEntry>
    {
        private DirectAccessArchive arc;
        private int position;

        public DirectAccessArchiveEnumerator(DirectAccessArchive itarc)
        {
            arc = itarc;
            position = -1;
        }

        public object Current
        {
            get
            {
                return (Object) new DirectAccessArchiveEntry(arc.basepath, arc.fileList[position]);
            }
        }

        IArchiveEntry IEnumerator<IArchiveEntry>.Current
        {
            get
            {
                return new DirectAccessArchiveEntry(arc.basepath, arc.fileList[position]);
            }
        }

        public void Reset()
        {
            position = -1;
        }
        public bool MoveNext()
        {
            position++;
            if (position == arc.fileList.Count)
            {
                return false;
            }
            return true;
        }
        public void Dispose()
        {
            //arc.Dispose();
        }
    }
}

