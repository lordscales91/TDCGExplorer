using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
    public static void Main(string[] args)
    {
        ArcRels rels = ArcRels.Load(@"http://3dcustom.ath.cx/devel/arcs/code/mod0066/rels.xml");
        foreach (Relationship rel in rels.Relationships)
        {
            Console.WriteLine(rel.Id);
            Console.WriteLine(rel.FromId);
            Console.WriteLine(rel.ToId);
            Console.WriteLine(rel.Kind);
        }
    }
}
