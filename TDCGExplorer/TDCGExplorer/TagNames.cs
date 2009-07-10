using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;
using System.IO;
using System.Text.RegularExpressions;

namespace TDCGExplorer
{
    public class TagNamesEntry
    {
        public string tag;
        public List<string> code = new List<string>();
    }

    class TagNamesDictionary
    {
        private Dictionary<string, TagNamesEntry> TagNames = new Dictionary<string, TagNamesEntry>();

        public void Init()
        {
            if (TDCGExplorer.SystemDB.modrefserver_alwaysenable == "true")
            {
                if (DownloadTagNamesZipFromServer() == false) return;
            }
            else
            {
                // tagname.zipをダウンロードする.
                if (File.Exists(zipTagLocalName()) == false)
                {
                    if (DownloadTagNamesZipFromServer() == false) return;
                }
            }
            GetTagNamesZipInfo();
        }

        public string zipTagLocalName()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath());
            return Path.Combine(path, "tagnames.zip");
        }

        // tagnames.zipをサーバからダウンロードする.
        public bool DownloadTagNamesZipFromServer()
        {
            bool status = false;
            try
            {
                string uri = TDCGExplorer.SystemDB.tagnames_server;
                string localfile = zipTagLocalName();

                // サーバからダウンロードする.
                HttpUtil.DownloadFile(uri, localfile);
                status = true;
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
            return status;
        }

        // arcsnames.zipを解凍、展開する.
        public void GetTagNamesZipInfo()
        {
            try
            {
                string localfile = zipTagLocalName();
                string arcname = "tmp/tagnames.txt";

                // ZIPファイルを展開する.
                IArchive arc = new ZipArchive();
                arc.Open(localfile);
                if (arc == null) return;

                foreach (IArchiveEntry entry in arc)
                {
                    if (entry.FileName == arcname)
                    {
                        TagNames.Clear();
                        using (MemoryStream ms = new MemoryStream((int)entry.Size))
                        {
                            arc.Extract(entry, ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            StreamReader reader = new StreamReader(ms, System.Text.Encoding.GetEncoding("Shift_JIS"));
                            Regex regCsv = new System.Text.RegularExpressions.Regex("\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*,\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*", RegexOptions.None);

                            // CSVファイルを展開してarcsNamesを構築する.
                            for (; ; )
                            {
                                string line = reader.ReadLine();
                                if (line == null) break;
                                Match m = regCsv.Match(line);
                                //TagNamesEntry tagentry = new TagNamesEntry();
                                string tag=null, code=null;
                                while (m.Success)
                                {
                                    for (int index = 1; index < 5; index++)
                                    {
                                        string field = m.Groups[index].Value;
                                        if (field == "\\N") field = "";
                                        field = field.Trim();
                                        if (field.StartsWith("\"") && field.EndsWith("\""))
                                        {
                                            field = field.Substring(1, field.Length - 2);
                                            field = field.Replace("\"\"", "\"");
                                        }
                                        switch (index)
                                        {
                                            case 1:
                                                tag = field;
                                                break;
                                            case 2:
                                                code = field;
                                                break;
                                        }
                                    }
                                    m = m.NextMatch();
                                }
                                if (TagNames.ContainsKey(tag) == false)
                                {
                                    TagNamesEntry codeentry = new TagNamesEntry();
                                    codeentry.tag = tag;
                                    TagNames[tag] = codeentry;
                                }
                                TagNames[tag].code.Add(code);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
        }

        public Dictionary<string, TagNamesEntry> entry
        {
            get { return TagNames; }
            set { }
        }
    }
}
