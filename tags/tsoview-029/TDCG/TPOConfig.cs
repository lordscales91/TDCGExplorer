using System;
//using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TDCG
{
    /// <summary>
    /// �̌^���V�s�v�f
    /// </summary>
public class Proportion
{
    /// <summary>
    /// �̌^�X�N���v�g��
    /// </summary>
    public string ClassName { get; set; }
    /// <summary>
    /// �ό`�䗦
    /// </summary>
    public float Ratio { get; set; }
}
/// <summary>
/// �̌^���V�s�������܂��B
/// </summary>
public class TPOConfig
{
    /// <summary>
    /// �̌^���V�s�v�f�̔z��
    /// </summary>
    public Proportion[] Proportions { get; set; }

    /// <summary>
    /// �̌^���V�s�𐶐����܂��B
    /// </summary>
    public TPOConfig()
    {
        this.Proportions = new Proportion[0];
    }

    /// <summary>
    /// �̌^���V�s�������o���܂��B
    /// </summary>
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

    /// <summary>
    /// �̌^���V�s��ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�t�@�C����</param>
    /// <returns>�̌^���V�s</returns>
    public static TPOConfig Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TPOConfig));
        TPOConfig config = serializer.Deserialize(reader) as TPOConfig;
        reader.Close();
        return config;
    }

    /// <summary>
    /// �̌^���V�s��ۑ����܂��B
    /// </summary>
    /// <param name="dest_file">�ۑ��t�@�C����</param>
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
