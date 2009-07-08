using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

namespace TDCGExplorer
{
    static class HttpUtil
    {
        private static void SetRequestHeaders(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = "Mozilla/5.0 TDCGExplorer/0.0.1";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3");
            request.Headers.Add("Accept-Charset", "Shift_JIS,utf-8;q=0.7,*;q=0.7");
        }

        public static bool DownloadFile(string uri, string localfile)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                SetRequestHeaders(request);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
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
    }
}
