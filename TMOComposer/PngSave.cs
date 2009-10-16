using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TMOComposer
{
    public class PngSave
    {
        public List<PngSaveItem> items = new List<PngSaveItem>();

        public void Dump(string dest_file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PngSave));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.GetEncoding("Shift_JIS");
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(dest_file, settings);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static PngSave Load(string source_file)
        {
            XmlReader reader = XmlReader.Create(source_file);
            XmlSerializer serializer = new XmlSerializer(typeof(PngSave));
            PngSave program = serializer.Deserialize(reader) as PngSave;
            reader.Close();
            return program;
        }
    }
}
