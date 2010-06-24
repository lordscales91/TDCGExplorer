using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using TDCGExplorer;

namespace System.Windows.Forms
{
    public class MODRefPage : Control
    {
        private WebBrowser webBrowser;
        private ArcsZipArcEntry zipEntry;

        public MODRefPage(int zipid)
        {
            TDCGExplorer.TDCGExplorer.SetToolTips("Databaseに問い合わせ中...");

            zipEntry = TDCGExplorer.TDCGExplorer.ArcsDB.GetZip(zipid);
            InitializeComponent();

            MODRefPageThread workerObject = new MODRefPageThread(zipEntry,this);
            Thread workerThread = new Thread(workerObject.DoWorkerThread);

            webBrowser.DocumentText =
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"" +
                "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
                "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" lang=\"ja\">" +
                "<head>" +
                "  <meta http-equiv=\"content-type\" content=\"text/html;charset=Shift_JIS\" />" +
                "  <title>3DCG mods reference</title>" +
                "  <link href=\"" + TDCGExplorer.TDCGExplorer.SystemDB.moddb_url + "/stylesheets/application.css\" media=\"screen\" rel=\"stylesheet\" type=\"text/css\" />" +
                "</head>" +
                "<body>" +
                "<div id=\"wrapper\">" +
                "  <div id=\"top-menu\">" +
                "    <div id=\"account\">" +
                "      <a href=\"/rails/\">Home</a>" +
                "      <a href=\"/rails/session/new\">Login</a>" +
                "    </div>" +
                "  </div>" +
                "  <div id=\"header\">" +
                "    <h1>3DCG mods reference</h1>" +
                "    <div id=\"main-menu\">" +
                "      <ul>" +
                "        <li><a href=\"/rails/arcs\" class=\"selected\">" + TextResource.Archive + "</a></li>" +
                "        <li><a href=\"/rails/tahs\">tah</a></li>" +
                "        <li><a href=\"/rails/tsos\">tso</a></li>" +
                "        <li><a href=\"/rails/tags\">" + TextResource.Tag + "</a></li>" +
                "      </ul>" +
                "    </div>" +
                "  </div>" +
                "  <div id=\"main\" class=\"nosidebar\">" +
                "    <div id=\"content\">" +
                "    <h2>" +
                "      <b>Loading....</b>" +
                "    </h2>" +
                "    </div>" +
                "  </div>" +
                "</body>" +
                "</html>";

            workerThread.Start();

            Text = zipEntry.GetDisplayPath();
        }

        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(50, 150);
            this.webBrowser.TabIndex = 0;
            // 
            // MODRefPage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.webBrowser);
            this.Resize += new System.EventHandler(this.MODRefPage_Resize);
            this.MouseEnter += new System.EventHandler(this.MODRefPage_MouseEnter);
            this.ResumeLayout(false);

        }

        private void MODRefPage_Resize(object sender, EventArgs e)
        {
            webBrowser.Size = Size;
        }

        private void MODRefPage_MouseEnter(object sender, EventArgs e)
        {
            webBrowser.Focus();
        }

        // invokeの為のdelegate
        private delegate void displayFromArcsHander(string text);
#if false
        // 非同期で呼び出されるメソッド
        private void asyncDlgDisplayFromArcs(string text)
        {
            webBrowser.DocumentText = text;
        }
#endif
        // 非同期で呼び出されるメソッド
        private void asyncDlgDisplayFromArcs(string text)
        {
            if (text != "")
            {
                webBrowser.Url = new Uri(text);
                TDCGExplorer.TDCGExplorer.SetToolTips(text);
            }
            else
            {
                this.webBrowser.DocumentText =
                    "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"" +
                    "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
                    "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" lang=\"ja\">" +
                    "<head>" +
                    "  <meta http-equiv=\"content-type\" content=\"text/html;charset=Shift_JIS\" />" +
                    "  <title>3DCG mods reference</title>" +
                    "  <link href=\"" + TDCGExplorer.TDCGExplorer.SystemDB.moddb_url + "/stylesheets/application.css\" media=\"screen\" rel=\"stylesheet\" type=\"text/css\" />" +
                    "</head>" +
                    "<body>" +
                    "<div id=\"wrapper\">" +
                    "  <div id=\"top-menu\">" +
                    "    <div id=\"account\">" +
                    "      <a href=\"/rails/\">Home</a>" +
                    "      <a href=\"/rails/session/new\">Login</a>" +
                    "    </div>" +
                    "  </div>" +
                    "  <div id=\"header\">" +
                    "    <h1>3DCG mods reference</h1>" +
                    "    <div id=\"main-menu\">" +
                    "      <ul>" +
                    "        <li><a href=\"/rails/arcs\" class=\"selected\">" + TextResource.Archive + "</a></li>" +
                    "        <li><a href=\"/rails/tahs\">tah</a></li>" +
                    "        <li><a href=\"/rails/tsos\">tso</a></li>" +
                    "        <li><a href=\"/rails/tags\">" + TextResource.Tag + "</a></li>" +
                    "      </ul>" +
                    "    </div>" +
                    "  </div>" +
                    "  <div id=\"main\" class=\"nosidebar\">" +
                    "    <div id=\"content\">" +
                    "    <h2>" +
                    "      <b>" + TextResource.ArchiveNotFound + "</b>" +
                    "    </h2>" +
                    "    </div>" +
                    "  </div>" +
                    "</body>" +
                    "</html>";
            }
        }

        // 非同期でツリー表示を更新する.
        public void asyncDisplayFromArcs(string text)
        {
            try
            {
                Invoke(new displayFromArcsHander(asyncDlgDisplayFromArcs), text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
#if false
    public class MODRefPageThread
    {
        MODRefPage page;
        ArcsZipArcEntry zipEntry;

        public MODRefPageThread(ArcsZipArcEntry zipentry,MODRefPage self)
        {
            zipEntry = zipentry;
            page = self;
        }

        public void DoWorkerThread()
        {
            string moddb = TDCGExplorer.GetSystemDatabase().moddb_url;
            string relurl;
            ArcRels relationships;

            string msg = "<html><body>";

            int count = 0;

            //            Cursor.Current = Cursors.WaitCursor;
            try
            {

                relurl = moddb + "arcs/code/" + zipEntry.code + "/rels.xml";
                TDCGExplorer.SetToolTips(relurl);
                relationships = ArcRels.Load(relurl);

                Arc thisarc = Arc.Load(moddb + "arcs/code/" + zipEntry.code + ".xml");

                msg += "<h2><a href=" + moddb + "arcs/" + thisarc.Id + ">" + "MOD Archive code:" + zipEntry.code + "</a></h2>";

                if (relationships != null)
                {
                    if (relationships.Relationships != null)
                    {
                        foreach (Relationship relation in relationships.Relationships)
                        {
                            string arcurl = moddb + "arcs/" + relation.ToId.ToString() + ".xml";
                            string[] kindstr = { "0", "同一内容", "新版", "前提" };
                            try
                            {
                                Arc arc = Arc.Load(arcurl);
                                if (arc != null)
                                {
                                    msg += "<pre>";
                                    msg += "属性:" + kindstr[relation.Kind] + "<br/>";
                                    msg += "MODコード名:" + arc.Code + "<br/>";
                                    msg += "サマリー:" + arc.Summary + "<br/>";
                                    msg += "元ファイル名:" + arc.Origname + "<br/>";
                                    msg += "拡張子:" + arc.Extname + "<br/>";
                                    msg += "所在:" + arc.Location + "<br/>";
                                    msg += "</pre>";
                                    count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }

                relurl = moddb + "arcs/code/" + zipEntry.code + "/revs.xml";
                relationships = ArcRels.Load(relurl);
                if (relationships != null)
                {
                    if (relationships.Relationships != null)
                    {
                        foreach (Relationship relation in relationships.Relationships)
                        {
                            if (relation.Kind == 1) continue;
                            string arcurl = moddb + "arcs/" + relation.FromId.ToString() + ".xml";
                            string[] kindstr = { "0", "1", "旧版", "提供" };
                            try
                            {
                                Arc arc = Arc.Load(arcurl);
                                if (arc != null)
                                {
                                    msg += "<pre>";
                                    msg += "属性:" + kindstr[relation.Kind] + "<br/>";
                                    msg += "MODコード名:" + arc.Code + "<br/>";
                                    msg += "サマリー:" + arc.Summary + "<br/>";
                                    msg += "元ファイル名:" + arc.Origname + "<br/>";
                                    msg += "拡張子:" + arc.Extname + "<br/>";
                                    msg += "所在:" + arc.Location + "<br/>";
                                    msg += "</pre>";
                                    count++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            //            Cursor.Current = Cursors.Default;

            if (count == 0)
            {
                msg += "<p>関連情報がありません</p>";
            }
            msg += "</body></html>";
            page.asyncDisplayFromArcs(msg);
        }
    }
#endif

    public class MODRefPageThread
    {
        MODRefPage page;
        ArcsZipArcEntry zipEntry;

        public MODRefPageThread(ArcsZipArcEntry zipentry, MODRefPage self)
        {
            zipEntry = zipentry;
            page = self;
        }

        public void DoWorkerThread()
        {
            string moddb = TDCGExplorer.TDCGExplorer.SystemDB.moddb_url;
            string url;
            try
            {
                Arc thisarc = Arc.Load(moddb + "arcs/code/" + zipEntry.code + ".xml");
                url = moddb + "arcs/" + thisarc.Id;
                page.asyncDisplayFromArcs(url);

            }
            catch (Exception ex)
            {
                page.asyncDisplayFromArcs("");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
