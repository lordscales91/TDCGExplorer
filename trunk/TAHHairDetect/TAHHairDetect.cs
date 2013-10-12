using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace TAHHairDetect
{
    public static class TAHHairDetectMain
    {
        public static void Verify(string directory)
        {
            GetDirectories(directory);
            OpenRead();
            CheckColorTable();
        }

        static Dictionary<string, TAHFile> tahfiles = new Dictionary<string, TAHFile>();
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

        // カラーを処理する
        private static void CheckColorTable()
        {
            foreach (string filename in filenames)
            {
                Dictionary<string, List<string>> coltables = new Dictionary<string,List<string>>();
                TAHFile tahfile = tahfiles[filename];
                int total = tahfile.EntrySet.Entries.Count;

                foreach (TAHEntry entry in tahfile.EntrySet.Entries)
                {
                    string newfilename = GetFileName(entry);

                    string dir = Path.GetDirectoryName(newfilename);
                    string file = Path.GetFileName(newfilename);

                    if (Path.GetExtension(file.ToLower()) == ".tbn")
                    {
                        if (file.Length != 16) continue;
                        //         1234567890123456
                        // file = "NhsaFHEA_B00.tbn"
                        string name = file.ToLower().Substring(1,8);
                        string col = file.ToLower().Substring(9,3);
                        // nameとcolのdictionaryを作る.
                        if (coltables.ContainsKey(name) == true)
                        {
                            coltables[name].Add(col);   
                        }
                        else
                        {
                            List<string> value = new List<string>();
                            value.Add(col);
                            coltables.Add(name, value);
                        }
                     }
                }
                string pastname = "";
                foreach(string name in coltables.Keys)
                {
                    if (coltables[name].Count == 1)
                    {
                        string values = coltables[name][0];
                        string type = values.Substring(0, 1);
                        if (type == "u" || type == "c" || type == "b" || type == "d")
                        {
                            string vcode = values.Substring(1, 2);
                            if (vcode == "00")
                            {
                                if (pastname != filename)
                                {
                                    Console.WriteLine(filename);
                                    pastname = filename;
                                }
                            }
                        }
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
    }
}
