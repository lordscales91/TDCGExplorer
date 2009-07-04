using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TDCGExplorer
{
    class MODRefPage : TabPage
    {
        private WebBrowser webBrowser;
        private ArcsZipArcEntry zipEntry;
    
        public MODRefPage(int zipid)
        {
            zipEntry = TDCGExplorer.GetArcsDatabase().GetZip(zipid);
            InitializeComponent();
            Text = zipEntry.GetDisplayPath();

            string moddb = TDCGExplorer.GetSystemDatabase().moddb_url;
            string relurl = moddb+"arcs/code/"+zipEntry.code+"/rels.xml";

            ArcRels relationships = ArcRels.Load(relurl);

            string msg = "<html><body><h2>MOD Archive code:"+zipEntry.code+"</h2>";

            if (relationships != null)
            {
                foreach (Relationship relation in relationships.Relationships)
                {
                    string arcurl = moddb + "arcs/" + relation.ToId.ToString() + ".xml";
                    string[] kindstr = { "新版", "旧版", "前提", "提供" };
                    Arc arc = Arc.Load(arcurl);
                    msg += "<pre>";
                    msg += "属性:" + kindstr[relation.Kind] + "<br/>";
                    msg += "MODコード名:" + arc.Code + "<br/>";
                    msg += "サマリー:" + arc.Summary + "<br/>";
                    msg += "元ファイル名:" + arc.Origname + "<br/>";
                    msg += "拡張子:" + arc.Extname + "<br/>";
                    msg += "所在:" + arc.Location + "<br/>";
                    msg += "</pre>";
                }
            }
            msg += "</body></html>";
            webBrowser.DocumentText = msg;
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
            this.webBrowser.Size = new System.Drawing.Size(250, 250);
            this.webBrowser.TabIndex = 0;
            // 
            // MODRefPage
            // 
            this.Controls.Add(this.webBrowser);
            this.Resize += new System.EventHandler(this.MODRefPage_Resize);
            this.ResumeLayout(false);

        }

        private void MODRefPage_Resize(object sender, EventArgs e)
        {
            webBrowser.Size = Size;
        }
    }

    [XmlRoot("relationship")]
    public class Relationship
    {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("from-id")]
        public int FromId { get; set; }
        [XmlElement("to-id")]
        public int ToId { get; set; }
        [XmlElement("Kind")]
        public int Kind { get; set; }
    }
    [XmlRoot("relationships")]
    public class ArcRels
    {
        public Relationship[] Relationships { get; set; }
        public static ArcRels Load(string source_file)
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
            XmlAttributes attrs = new XmlAttributes();
            XmlElementAttribute attr = new XmlElementAttribute();
            attr.ElementName = "relationship";
            attr.Type = typeof(Relationship);
            attrs.XmlElements.Add(attr);
            attrOverrides.Add(typeof(ArcRels), "Relationships", attrs);
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(ArcRels), attrOverrides);
            ArcRels rels = serializer.Deserialize(reader) as ArcRels;
            reader.Close();
            return rels;
        }
    }

    [XmlRoot("tah")]
    public class Tah
    {
        [XmlElement("arc-id")]
        public int Arcid { get; set; }
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("path")]
        public string Path { get; set; }
        [XmlElement("position")]
        public int Position { get; set; }
    }

    [XmlRoot("arc")]
    public class Arc
    {
        [XmlElement("code")]
        public string Code { get; set; }
        [XmlElement("extname")]
        public string Extname { get; set; }
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("location")]
        public string Location { get; set; }
        [XmlElement("origname")]
        public string Origname { get; set; }
        [XmlElement("summary")]
        public string Summary { get; set; }
        [XmlArray("tahs")]
        [XmlArrayItem("tah", typeof(Tah))]
        public Tah[] Tahs { get; set; }

        public static Arc Load(string source_file)
        {
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(Arc));
            Arc arc = serializer.Deserialize(reader) as Arc;
            reader.Close();
            return arc;
        }
    }
}
