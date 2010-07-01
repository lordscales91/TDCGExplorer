using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDCG
{
/// ノード範囲
public class NodesRange
{
    /// <summary>
    /// ルートノード名
    /// </summary>
    public List<string> root_names;

    /// <summary>
    /// ノード範囲を生成します。
    /// </summary>
    public NodesRange()
    {
        root_names = new List<string>();
    }

    /// <summary>
    /// ノード範囲を書き出します。
    /// </summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(NodesRange));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// ノード範囲を保存します。
    /// </summary>
    /// <param name="dest_file">ファイル名</param>
    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(NodesRange));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// ノード範囲を読み込みます。
    /// </summary>
    /// <param name="source_file">ファイル名</param>
    /// <returns></returns>
    public static NodesRange Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(NodesRange));
        NodesRange range = serializer.Deserialize(reader) as NodesRange;
        reader.Close();
        return range;
    }
}
}
