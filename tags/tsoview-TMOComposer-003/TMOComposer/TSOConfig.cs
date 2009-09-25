using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TMOComposer
{

public class TSOConfig
{
    public Size ClientSize { get; set; }
    public int RecordStep { get; set; }
    public string SavePath { get; set; }
    public string PosePath { get; set; }
    public string FacePath { get; set; }

    public TSOConfig()
    {
        this.ClientSize = new Size(1024, 768);
        this.RecordStep = 5;
        this.SavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TechArts3D\TDCG";
        this.PosePath = SavePath + @"\pose";
        this.FacePath = PosePath;
    }

    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TSOConfig));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    public static TSOConfig Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TSOConfig));
        TSOConfig config = serializer.Deserialize(reader) as TSOConfig;
        reader.Close();
        return config;
    }
}
}
