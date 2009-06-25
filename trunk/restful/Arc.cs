using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("tah")]
public class Tah
{
    [XmlElement("id")]
    public int Id { get; set; }
    [XmlElement("path")]
    public string Path { get; set; }
}

[XmlRoot("arc")]
public class Arc
{
    [XmlElement("id")]
    public int Id { get; set; }
    [XmlElement("code")]
    public string Code { get; set; }
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
    public static void Main(string[] args)
    {
        Arc arc = Arc.Load(@"http://3dcustom.ath.cx/devel/arcs/code/mod0066.xml");
        Console.WriteLine(arc.Id);
        Console.WriteLine(arc.Code);
        foreach (Tah tah in arc.Tahs)
        {
            Console.WriteLine(tah.Id);
            Console.WriteLine(tah.Path);
        }
    }
}
