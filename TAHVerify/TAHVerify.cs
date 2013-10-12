using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace TAHVerify
{
    class TAHEntryInfo
    {
        public TAHEntry entry;
        public TAHFile file;
    }

    public static class TAHCompact
    {
        public static void Verify(string directory)
        {
            Console.Error.WriteLine("ディレクトリの一覧を作成しています : " + directory);
            GetDirectories(directory);
            Console.Error.WriteLine("エントリの一覧を作成しています : " + directory);
            OpenRead();
            Console.Error.WriteLine("ファイルを検査しています : " + directory);
            MakeHashTable();
        }

        static Dictionary<string, TAHFile> tahfiles = new Dictionary<string, TAHFile>();
        static Dictionary<uint, TAHEntryInfo> hashentry = new Dictionary<uint, TAHEntryInfo>();
        static Dictionary<uint, string> filemaps = new Dictionary<uint, string>();
        static List<uint> hashtable = new List<uint>();
        static List<string> filenames = new List<string>();
        static List<List<string>> tahfilelists = new List<List<string>>();

        // ディレクトリリストを作る.
        private static void GetDirectories(string directory)
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
                GetDirectories(dir);
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
                int id;
                Console.Error.WriteLine("ファイルを検査しています : " + filename);
                TAHFile tahfile = tahfiles[filename];
                id = 0;
                int total = tahfile.EntrySet.Entries.Count;
                Console.Error.WriteLine("展開しています : " + filename + ":" + total.ToString() + "個のオブジェクト");

                foreach (TAHEntry entry in tahfile.EntrySet.Entries)
                {
                    if (hashentry.ContainsKey(entry.Hash))
                    {
                        string newfilename = GetFileName(entry);

                        if (hashentry[entry.Hash].entry.FileName != null &&
                            entry.FileName != null &&
                            filemaps[entry.Hash].ToLower() != newfilename.ToLower())
                        {
                            Console.WriteLine("ハッシュが衝突しています : " + filemaps[entry.Hash] + "->" + newfilename + " at " + filename);
                        }
                        // より新しいバージョンか？
                        if (hashentry[entry.Hash].file.Header.Version < tahfile.Header.Version)
                        {
                            hashentry[entry.Hash].file = tahfile;
                            hashentry[entry.Hash].entry = entry;
                            filemaps[entry.Hash] = newfilename;

                            //Console.WriteLine("バージョン番号が違います : " + filemaps[entry.Hash] + "->" + newfilename + " at " + filename);
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
                    // ファイルを実際にデコードする.

                    Console.Error.Write("展開しています : " + id.ToString() + "/" + total.ToString() + "個のオブジェクト\r");

                    try
                    {
                        TAHContent content = tahfile.LoadContent(tahfile.Reader, entry);
                        string type = ext(content.Data);
                        if (type == "")
                        {
                            Console.WriteLine("未知のデータが含まれています : " + filemaps[entry.Hash] + "(" + id.ToString() + "/" + total.ToString() + ")" + ":" + filename);
                            Console.Error.WriteLine("未知のデータが含まれています : " + filemaps[entry.Hash] + "(" + id.ToString() + "/" + total.ToString() + ")" + ":" + filename);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("展開中に例外が発生しました : " + filemaps[entry.Hash] + "(" + id.ToString() + "/" + total.ToString() + ")" + ":" + filename + "\r\n" + e.Message);
                        Console.Error.WriteLine("展開中に例外が発生しました : " + filemaps[entry.Hash] + "(" + id.ToString() + "/" + total.ToString() + ")" + ":" + filename + "\r\n" + e.Message);
                    }

                    id++;
                }
            }
        }

        public static string ext(byte[] data)
        {
            if (data.Length > 4 && data[0] == 'F' && data[1] == 'O' && data[2] == 'N' && data[3] == 'T') return ".font";
            if (data.Length > 4 && data[0] == '8' && data[1] == 'B' && data[2] == 'P' && data[3] == 'S') return ".psd";
            if (data.Length > 4 && data[0] == 0x89 && data[1] == 'P' && data[2] == 'N' && data[3] == 'G') return ".png";
            if (data.Length > 4 && data[0] == 'T' && data[1] == 'S' && data[2] == 'O' && data[3] == '1') return ".tso";
            if (data.Length > 4 && data[0] == 'T' && data[1] == 'M' && data[2] == 'O' && data[3] == '1') return ".tmo";
            if (data.Length > 4 && data[0] == '/' && data[1] == '*' && data[2] == '*' && data[3] == '*') return ".cgfx";
            if (data.Length > 4 && data[0] == 'B' && data[1] == 'B' && data[2] == 'B' && data[3] == 'B') return ".tbn";
            if (data.Length > 4 && data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S') return ".ogg";
            return "";
        }

        // ファイル名を取得する.
        private static string GetFileName(TAHEntry entry)
        {
            string filename;
            filename = entry.FileName;

            if (filename == null) filename = "00000000" + "_" + entry.Hash.ToString("x8");
            return filename;
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
