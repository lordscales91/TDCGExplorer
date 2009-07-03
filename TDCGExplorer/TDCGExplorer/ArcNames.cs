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
        // arcsNameの取得.
        //public Dictionary<string, ArcsNamesEntry> getArcsnames()
        //{
        //    return arcsNames;
        //}

        public void Init()
        {
            // arcsname.zipをダウンロードする.
            if (File.Exists(zipArcLocalName()) == false)
            {
                if (DownloadArcNamesZipFromServer() == false) return;
            }
            GetArcNamesZipInfo();
        }

        private void SetRequestHeaders(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Mozilla/5.0 TDCGExplorer/0.0.1";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3");
            request.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7");
        }

        public string zipArcLocalName()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + TDCGExplorer.GetAppDataPath() + "\\arcsname.zip";
        }

        // arcsnames.zipをサーバからダウンロードする.
        public bool DownloadArcNamesZipFromServer()
        {
            bool status = false;
            try
            {
                string uri = TDCGExplorer.GetSystemDatabase().arcnames_server;
                string localfile = zipArcLocalName();

                // サーバからダウンロードする.
                DownloadFile(uri, localfile);
                status = true;
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
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
                                if (arcname.Contains(arcentry.code) == false)
                                    arcsNames[arcentry.code] = arcentry;
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

        public bool DownloadFile(string uri, string localfile)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                SetRequestHeaders(request);
                //Console.WriteLine(request.Address);

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Console.WriteLine(response.Headers);

                Stream dataStream = response.GetResponseStream();
                Stream fileStream = File.OpenWrite(localfile);

                BufferedStream bufferedDataStream = new BufferedStream(dataStream);
                BufferedStream bufferedFileStream = new BufferedStream(fileStream);

                byte[] buf = new byte[1024];
                int len;
                while ((len = bufferedDataStream.Read(buf, 0, buf.Length)) > 0)
                {
                    bufferedFileStream.Write(buf, 0, len);
                }

                bufferedFileStream.Flush();
                bufferedFileStream.Close();
                bufferedDataStream.Close();
                response.Close();

                return true;
            }
            catch (Exception e)
            {
                TDCGExplorer.SetToolTips("Error occured : " + e.Message);
            }
            return false;
        }

        public Dictionary<string, ArcsNamesEntry> entry
        {
            get { return arcsNames; }
            set { }
        }
    }
}
