using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDCGExplorer
{
    class CollisionTahPage : TabPage
    {
        private ListBox listBox;
        private SplitContainer splitContainer;
        private WebBrowser webBrowser;
        CollisionItem collisionEntry;

        public CollisionTahPage(CollisionItem argCollisionEntry)
        {
            InitializeComponent();
            collisionEntry = argCollisionEntry;
            Text = Path.GetFileName(collisionEntry.tah.path);

            foreach (ArcsCollisionRecord col in collisionEntry.entries)
            {
                ArcsDatabase db = TDCGExplorer.GetArcsDatabase();
                // 衝突した先のtahを取得する.
                ArcsTahEntry to = db.GetTah(col.toTahID);
                // 既に同じ名前で追加していないか調べる.
                ArcsTahFilesEntry fromfile = db.GetTahFilesEntry(col.fromFilesID);
                ArcsTahFilesEntry tofile = db.GetTahFilesEntry(col.toFilesID);
                listBox.Items.Add(fromfile.GetDisplayPath().ToLower() + " → " + tofile.GetDisplayPath().ToLower() + " [ "+to.shortname+" ]");
            }
        }

        private void InitializeComponent()
        {
            this.listBox = new System.Windows.Forms.ListBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(300, 43);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer.Size = new System.Drawing.Size(150, 100);
            this.splitContainer.TabIndex = 0;
            this.splitContainer.SplitterDistance = 24;
            // 
            // webBrowser
            // 
            this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(250, 250);
            this.webBrowser.TabIndex = 0;
            // 
            // CollisionTahPage
            // 
            this.Controls.Add(this.splitContainer);
            this.Resize += new System.EventHandler(this.CollisionTahPage_Resize);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index >= 0)
            {
                ArcsCollisionRecord col = collisionEntry.entries[index];

                ArcsDatabase db = TDCGExplorer.GetArcsDatabase();
                // 衝突した先のtahを取得する.
                ArcsTahEntry from = db.GetTah(col.fromTahID);
                ArcsTahEntry to = db.GetTah(col.toTahID);
                // 既に同じ名前で追加していないか調べる.
                ArcsTahFilesEntry fromfile = db.GetTahFilesEntry(col.fromFilesID);
                ArcsTahFilesEntry tofile = db.GetTahFilesEntry(col.toFilesID);

                string text =
                    @"<p>" +
                    @"<h2> Collision from : " + from.shortname + "</h2>" +
                    @"<adress>" + "path : " + Path.GetDirectoryName(from.path) + "</adress>" +
                    @"<h3> Collision to : " + to.shortname + "</h3>" +
                    @"<adress>" + "path : " + Path.GetDirectoryName(to.path) + "</adress>" +
                    @"<pre>" + fromfile.GetDisplayPath().ToLower() + " → " + tofile.GetDisplayPath().ToLower() + "</pre>" +
                    @"<pre>" + "hash code : " + tofile.hash.ToString("x8") + "</pre>";

                webBrowser.DocumentText = text;
            }
        }

        private void CollisionTahPage_Resize(object sender, EventArgs e)
        {
            splitContainer.Size = ClientSize;
            listBox.Size = splitContainer.Panel1.ClientSize;
            webBrowser.Size = splitContainer.Panel2.ClientSize;
        }
    }
}
