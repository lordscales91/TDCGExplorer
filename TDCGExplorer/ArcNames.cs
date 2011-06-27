using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using ArchiveLib;
using System.IO;
using System.Text.RegularExpressions;

namespace TDCGExplorer
{
    public class ArcsNamesEntry
    {
        public string code;
        public string location;
        public string summary;
        public string origname;
    }

    class ArcNamesDictionary
    {
        private Dictionary<string, ArcsNamesEntry> arcsNames = new Dictionary<string,ArcsNamesEntry>();

        public void Init()
        {
            if (TDCGExplorer.SystemDB.modrefserver_alwaysenable == "true")
            {
                // 毎回とりにいく.
                if (DownloadArcNamesZipFromServer() == false) return;
            }
            else
            {
                // arcsname.zipをダウンロードする.
                if (File.Exists(zipArcLocalName()) == false)
                {
                    if (DownloadArcNamesZipFromServer() == false) return;
                }
            }
            GetArcNamesZipInfo();
        }

        public string zipArcLocalName()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), TDCGExplorer.GetAppDataPath());
            return Path.Combine(path,"arcsnames.zip");
        }

        // arcsnames.zipをサーバからダウンロードする.
        public bool DownloadArcNamesZipFromServer()
        {
            bool status = false;
            try
            {
                string uri = TDCGExplorer.SystemDB.arcnames_server;
                string localfile = zipArcLocalName();

                // サーバからダウンロードする.
                HttpUtil.DownloadFile(uri, localfile);
                status = true;
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error DownloadArcNamesZipFromServer : " + e.Message);
            }
            return status;
        }

        // arcsnames.zipを解凍、展開する.
        public void GetArcNamesZipInfo()
        {
            try
            {
                string localfile = zipArcLocalName();
                string arcname = "tmp/arcnames.txt";

                // ZIPファイルを展開する.
                IArchive arc = new ZipArchive();
                arc.Open(localfile);
                if (arc == null) return;

                foreach (IArchiveEntry entry in arc)
                {
                    if (entry.FileName == arcname)
                    {
                        arcsNames.Clear();
                        using (MemoryStream ms = new MemoryStream((int)entry.Size))
                        {
                            arc.Extract(entry, ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            StreamReader reader = new StreamReader(ms, System.Text.Encoding.GetEncoding("Shift_JIS"));
                            Regex regCsv = new System.Text.RegularExpressions.Regex("\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*,\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*,\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*,\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*", RegexOptions.None);

                            // CSVファイルを展開してarcsNamesを構築する.
                            for (; ; )
                            {
                                string line = reader.ReadLine();
                                if (line == null) break;
                                Match m = regCsv.Match(line);
                                ArcsNamesEntry arcentry = new ArcsNamesEntry();
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
                                                arcentry.code = field;
                                                break;
                                            case 2:
                                                arcentry.location = field;
                                                break;
                                            case 3:
                                                arcentry.summary = field;
                                                break;
                                            case 4:
                                                arcentry.origname = field;
                                                break;
                                        }
                                    }
                                    m = m.NextMatch();
                                }
                                if (arcsNames.ContainsKey(arcentry.code) == false)
                                    arcsNames[arcentry.code] = arcentry;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error GetArcNamesZipInfo : " + e.Message);
            }
        }

        public Dictionary<string, ArcsNamesEntry> entry
        {
            get { return arcsNames; }
            set { }
        }
    }
}
