using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDCG
{
/// �m�[�h�͈�
public class NodesRange
{
    /// <summary>
    /// ���[�g�m�[�h��
    /// </summary>
    public List<string> root_names;

    /// <summary>
    /// �m�[�h�͈͂𐶐����܂��B
    /// </summary>
    public NodesRange()
    {
        root_names = new List<string>();
    }

    /// <summary>
    /// �m�[�h�͈͂������o���܂��B
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
    /// �m�[�h�͈͂�ۑ����܂��B
    /// </summary>
    /// <param name="dest_file">�t�@�C����</param>
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
    /// �m�[�h�͈͂�ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�t�@�C����</param>
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
