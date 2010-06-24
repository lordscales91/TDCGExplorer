using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDCGExplorer
{
    [XmlRoot("relationship")]
    public class Relationship
    {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("from-id")]
        public int FromId { get; set; }
        [XmlElement("to-id")]
        public int ToId { get; set; }
        [XmlElement("kind")]
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
