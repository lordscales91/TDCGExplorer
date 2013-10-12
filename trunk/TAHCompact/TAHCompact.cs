using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace TAHCompact
{
    class TAHEntryInfo
    {
        public TAHEntry entry;
        public TAHFile file;
    }

    public static class TAHCompact
    {
        private const long FileLimit = 0x100000000-655360*2;

        public static void Compaction(string directory, string tahname,uint version)
        {
            GetDirecotories(directory);
            OpenRead();
            MakeHashTable();
            CreateTahFileList();
            CreateTahFiles(tahname,version);
        }

        static Dictionary<string, TAHFile> tahfiles = new Dictionary<string, TAHFile>();
        static Dictionary<uint, TAHEntryInfo> hashentry = new Dictionary<uint, TAHEntryInfo>();
        static Dictionary<uint, string> filemaps = new Dictionary<uint, string>();
        static List<uint> hashtable = new List<uint>();
        static List<string> filenames = new List<string>();
        static List<List<string>> tahfilelists = new List<List<string>>();

        // ディレクトリリストを作る.
        private static void GetDirecotories(string directory)
        {
            string[] shortnames = directory.Split('\\');
            foreach (string shortname in shortnames) if (shortname[0] == '!') return;

            Program.SetRescent(directory);
            string[] files = Directory.GetFiles(directory, "*.tah");
            foreach (string file in files)
            {
                filenames.Add(file);
            }
            string[] directories = Directory.GetDirectories(directory);
            foreach (string dir in directories)
            {
                GetDirecotories(dir);
            }
        }

        // ファイルをオープンする
        private static void OpenRead()
        {
            foreach (string filename in filenames)
            {
                Program.SetRescent(filename);
                FileStream fs = File.OpenRead(filename);
                tahfiles[filename] = new TAHFile(fs);
                tahfiles[filename].LoadEntries();
            }
        }

        // ハッシュテーブルを構築する
        private static void MakeHashTable()
        {
            foreach (string filename in filenames)
            {
                TAHFile tahfile = tahfiles[filename];
                foreach (TAHEntry entry in tahfile.EntrySet.Entries)
                {
                    if (hashentry.ContainsKey(entry.Hash))
                    {
                        string newfilename = GetFileName(entry);

                        if (hashentry[entry.Hash].entry.FileName != null &&
                            entry.FileName != null &&
                            filemaps[entry.Hash].ToLower() != newfilename.ToLower())
                        {
                            Console.WriteLine("Hash collision : " + filemaps[entry.Hash] + "->" + newfilename + " at " + filename);
                        }
                        // より新しいバージョンか？
                        if (hashentry[entry.Hash].file.Header.Version < tahfile.Header.Version)
                        {
                            hashentry[entry.Hash].file = tahfile;
                            hashentry[entry.Hash].entry = entry;
                            filemaps[entry.Hash] = newfilename;
                        }
                    }
                    else
                    {
                        hashentry[entry.Hash] = new TAHEntryInfo();
                        hashentry[entry.Hash].file = tahfile;
                        hashentry[entry.Hash].entry = entry;
                        filemaps[entry.Hash] = GetFileName(entry);
                        hashtable.Add(entry.Hash);
                    }
                }
            }
        }

        // ファイル名を取得する.
        private static string GetFileName(TAHEntry entry)
        {
            string filename;
            filename = entry.FileName;

            if (filename == null) filename = "00000000" + "_" + entry.Hash.ToString("x8");
            return filename;
        }

        // TAHファイルを作成する
        private static void CreateTahFileList()
        {
            // ファイル名の一覧を作る.
            List<string> filenamelist = new List<string>();
            foreach(uint hash in hashtable)
            {
                filenamelist.Add(filemaps[hash]);
            }

            // 4GBずつリストを分割する
            List<string> tahfilelist = new List<string>();
            tahfilelists.Add(tahfilelist);
            long filesize = 0;
            foreach (string file in filenamelist)
            {
                TAHEntryInfo info = GetEntry(file);
                filesize = filesize + info.entry.Length;
                if (filesize > FileLimit)
                {
                    filesize = info.entry.Length;
                    tahfilelist=new List<string>();
                    tahfilelists.Add(tahfilelist);
                }
                tahfilelist.Add(file);
            }
        }

        private static void CreateTahFiles(string tahname,uint version)
        {
            string basefilename = Path.GetFileNameWithoutExtension(tahname);

            int count=0;
            foreach (List<string> tahfilelist in tahfilelists)
            {
                if(tahfilelist.Count>0){
                    string destname;
                    if (count == 0) destname = tahname;
                    else
                    {
                        tahname = Path.Combine(Path.GetDirectoryName(tahname), basefilename + "-" + count.ToString() + ".tah");
                    }
                    count++;
                    TAHWriter writer = new TAHWriter();
                    writer.Version = version;
                    foreach (string file in tahfilelist)
                    {
                        writer.Add(file);
                    }
                    Console.WriteLine("tah file " + tahname);
                    using (FileStream fs = File.OpenWrite(tahname))
                    {
                        int filecount = 1, filemax = tahfilelist.Count;
                        // データ取得delegate
                        writer.RawData += delegate(string filename)
                        {
                            TAHRawData rawdata = new TAHRawData();
                            TAHEntryInfo fileinfo = GetEntry(filename);
                            TAHFile tahfile = fileinfo.file;
                            uint length;
                            rawdata.data = TAHUtil.ReadRawEntryData(tahfile.Reader, fileinfo.entry, out length);
                            rawdata.length = length;
                            //Console.Write(tahname + " " + filecount++.ToString() + "/" + filemax.ToString() + "\r");
                            return rawdata;
                        };
                        writer.RawWrite(fs);
                        Console.Write("\n");
                    }
                }
            }
        }

        private static TAHEntryInfo GetEntry(string filename)
        {
            uint hash = 0;

            // ファイル名からハッシュ値を求める.
            if (Path.GetDirectoryName(filename) == "")
            {
                // dddddddd_xxxxxxxx.eee形式をハッシュ値に戻す
                if (filename.Length >= 17)
                {
                    string hashcode = filename.Substring(9, 8);
                    try
                    {
                        // 16進数からハッシュ値に.
                        hash = UInt32.Parse(hashcode, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch (Exception)
                    {
                        // 判んない時は適当につける.
                        hash = TAHUtil.CalcHash(filename);
                    }
                }
                else
                {
                    hash = TAHUtil.CalcHash(filename);
                }
            }
            else
            {
                hash = TAHUtil.CalcHash(filename);
            }
            return hashentry[hash];
        }
    }
}
