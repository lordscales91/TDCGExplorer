using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TAHTool
{

public class Proportion
{
    public string ClassName { get; set; }
    public float Ratio { get; set; }
}

public class TPOConfig
{
    public Proportion[] Proportions { get; set; }
    public TPOConfig()
    {
        this.Proportions = new Proportion[0];
    }

    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    public static TPOConfig Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        TPOConfig config = serializer.Deserialize(reader) as TPOConfig;
        reader.Close();
        return config;
    }

    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }
}
}
